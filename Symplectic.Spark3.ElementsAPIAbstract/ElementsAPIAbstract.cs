//  Symplectic Spark
//  Copyright 2010 Symplectic Ltd
//  Created by Martyn Whitwell (martyn@symplectic.co.uk)

//  This file is part of Spark.

//  Spark is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.

//  Spark is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.

//  You should have received a copy of the GNU General Public License
//  along with Spark.  If not, see <http://www.gnu.org/licenses/>.

// $URL: https://symplectic-spark.googlecode.com/svn/trunk/Symplectic.Spark.ElementsAPIAbstract/ElementsAPIAbstract.cs $
// $LastChangedDate: 2010-06-04 19:04:48 +0100 (Fri, 04 Jun 2010) $
// $LastChangedRevision: 50 $
// $LastChangedBy: martyn@symplectic.co.uk $

using System;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Collections.Generic;
using System.Threading;

namespace Spark3.ElementsAPIAbstract
{
    public abstract class ElementsAPIAbstract
    {
        #region Logger
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion
        
        #region Constants
        public static XNamespace API_NAMESPACE = "http://www.symplectic.co.uk/publications/api";
        public static XNamespace ATOM_NAMESPACE = "http://www.w3.org/2005/Atom";
        #endregion

        #region NameSpaceManager
        private static XmlNamespaceManager _namespace_manager;
        public static XmlNamespaceManager NAMESPACE_MANAGER
        {
            get
            {
                if (_namespace_manager == null)
                {
                    _namespace_manager = new XmlNamespaceManager(new NameTable());
                    _namespace_manager.AddNamespace("api", API_NAMESPACE.NamespaceName);
                    _namespace_manager.AddNamespace("atom", ATOM_NAMESPACE.NamespaceName);
                }

                return _namespace_manager;
            }
        }
        #endregion

        #region Private members
        private string apiBaseUri { get; set; }
        private int apiInterval { get; set; }
        private int timeoutMilliseconds { get; set; }
        private int retryMaxCount { get; set; }
        #endregion

        #region Delegate definitions
        public delegate void delgProcessDataItem(XElement element);
        #endregion

        #region Constructors
        public ElementsAPIAbstract(string apiBaseUri, int apiInterval, int timeoutMilliseconds, int retryMaxCount)
        {
            this.apiBaseUri = apiBaseUri;
            this.apiInterval = apiInterval;
            this.timeoutMilliseconds = timeoutMilliseconds;
            this.retryMaxCount = retryMaxCount;
        }
        #endregion

        #region Private API Methods
        //Reads data from the API and retries a few times if there are timeout errors etc
        private XElement ReadData(string relativeUrl, int retryMaxCount)
        {
            return ReadData(relativeUrl, 0, retryMaxCount);
        }

        private XElement ReadData(string relativeUrl, int retryCount, int retryMaxCount)
        {

            
            Logger.DebugFormat("Querying URI: {0}", (this.apiBaseUri + relativeUrl));

            WebRequest request = HttpWebRequest.Create(this.apiBaseUri + relativeUrl);
            request.Timeout = this.timeoutMilliseconds;
            try
            {
                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                using (var reader = XmlReader.Create(stream))
                {
                    while (reader.Read() && reader.NodeType != XmlNodeType.Element) ; //read until you get to an element
                    return (XElement)XNode.ReadFrom(reader);
                }
            }
            catch (WebException exc)
            {
                switch (exc.Status)
                {
                    case WebExceptionStatus.ConnectFailure:
                    case WebExceptionStatus.KeepAliveFailure:
                    case WebExceptionStatus.PipelineFailure:
                    case WebExceptionStatus.ConnectionClosed:
                    case WebExceptionStatus.Timeout:
                    case WebExceptionStatus.ProtocolError:
                        if (retryCount < retryMaxCount)
                        {
                            Logger.WarnFormat("{0}: {1}. Timeout: {2}. Sleeping and Retrying (count: {3}/{4}). {5}", exc.Status, exc.Message, timeoutMilliseconds, retryCount, retryMaxCount, request.RequestUri);
                            Thread.Sleep(5000); //Sleep for 5 seconds, and then retry.
                            return ReadData(relativeUrl, retryCount + 1, retryMaxCount);
                        }
                        else
                        {
                            Logger.ErrorFormat("{0}: {1}. Timeout: {2}. Retry limit exceeded (count: {3}/{4}). {5}", exc.Status, exc.Message, timeoutMilliseconds, retryCount, retryMaxCount, request.RequestUri);
                            throw new WebException(string.Format("{0}: {1}. Timeout: {2}. Retry limit exceeded (count: {3}/{4}). {5}", exc.Status, exc.Message, timeoutMilliseconds, retryCount, retryMaxCount, request.RequestUri), exc);
                        }

                    default:
                        Logger.ErrorFormat("{0}: {1}. Not retrying (count: {2}/{3}). {4}", exc.Status, exc.Message, retryCount, retryMaxCount, request.RequestUri);
                        throw new WebException(string.Format("{0}: {1}. Not retrying (count: {2}/{3}). {4}", exc.Status, exc.Message, retryCount, retryMaxCount, request.RequestUri), exc);
                }
            }
        }
        #endregion

        #region Public API methods

        #region ReadData (non-paged)
        public int ReadData(string selectElements, string relativeUri, XElement data)
        {
            delgProcessDataItem process = delegate(XElement element)
            {
                data.Add(element);
            };

            return ReadData(selectElements, relativeUri, process);
        }

        public int ReadData(string selectElements, string relativeUri, delgProcessDataItem process)
        {
            int count = 0;
            //Load XML data
            XElement data = ReadData(relativeUri, retryMaxCount);

            //Pause for a moment to allow API to recover
            Thread.Sleep(apiInterval);

            //Select desired elements via XPath (as its more generalisable)
            foreach (XElement element in data.XPathSelectElements(selectElements, NAMESPACE_MANAGER))
            {
                process(element);
                count++;
            }
            return count;
        }
        #endregion

        #region ReadPagedData
        public int ReadPagedData(string selectElements, string relativeUri, int perPage, XElement data)
        {
            delgProcessDataItem process = delegate(XElement element)
            {
                data.Add(element);
            };

            return ReadPagedData(selectElements, relativeUri, perPage, process);
        }


        public int ReadPagedData(string selectElements, string relativeUri, int perPage, delgProcessDataItem process)
        {
            int thisPage = 0;
            int nextPage = 1; // start here
            int lastPage = 0;
            
            int count = 0;

            //loop through the pages of related object
            do
            {
                //Load XML data
                XElement data = ReadData(string.Format("{0}{1}per-page={2}&page={3}", relativeUri, (relativeUri.Contains("?") ? "&" : "?"), perPage, nextPage), retryMaxCount);

                //Pause for a moment to allow API to recover
                Thread.Sleep(apiInterval);

                //Reset pagination information
                thisPage = 0;
                nextPage = 0;
                lastPage = 0;
                //Get pagination information
                XElement pagination = data.Element(API_NAMESPACE + "pagination");
                foreach (XElement page in pagination.Elements(API_NAMESPACE + "page"))
                {
                    switch (page.Attribute("position").Value)
                    {
                        case "this":
                            thisPage = int.Parse(page.Attribute("number").Value);
                            break;
                        case "next":
                            nextPage = int.Parse(page.Attribute("number").Value);
                            break;
                        case "last":
                            lastPage = int.Parse(page.Attribute("number").Value);
                            break;
                    }
                }

                //Select desired elements via XPath (as its more generalisable)
                foreach (XElement element in data.XPathSelectElements(selectElements, NAMESPACE_MANAGER))
                {
                    process(element);
                    count++;
                }

            } while (nextPage > 0 && thisPage < lastPage && nextPage > thisPage);

            return count;
        }
        #endregion
        #endregion

    }
}
