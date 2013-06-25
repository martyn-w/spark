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

// $URL: https://symplectic-spark.googlecode.com/svn/trunk/Symplectic.Spark.Buffer/Buffer.cs $
// $LastChangedDate: 2010-06-09 11:38:27 +0100 (Wed, 09 Jun 2010) $
// $LastChangedRevision: 52 $
// $LastChangedBy: martyn@symplectic.co.uk $

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;

namespace Spark3.Buffer
{
    public class Buffer : Spark3.ElementsAPIAbstract.ElementsAPIAbstract
    {
        #region Logger
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region Private members
        private string bufferFolder { get; set; }
        private int retryMaxCountFile { get; set; }
        #endregion

        #region Constructors
        public Buffer(string bufferFolder, int fileRetryMaxCount, string apiBaseUri, int apiInterval, int timeoutMillisecondsAPI, int apiRetryMaxCount)
            : base(apiBaseUri, apiInterval, timeoutMillisecondsAPI, apiRetryMaxCount)
        {
            this.bufferFolder = bufferFolder;
            this.retryMaxCountFile = fileRetryMaxCount;
            
            DirectoryInfo dir = new DirectoryInfo(bufferFolder);
            if (!dir.Exists)
            {
                Logger.WarnFormat("Creating directory: {0}", dir.FullName);
                dir.Create();
            }
        }
        #endregion

        #region Public Methods

        public int BufferAllItems(XElement items, ConfigBufferBufferItem configBufferBufferItem)
        {
            //Initialise counters
            int itemCount = 1;
            int totalItemCount = items.Elements().Count();
            int bufferedDataCount = 0;

            string filename = Path.Combine(this.bufferFolder, EvaluateXPath(items.Elements().First(), configBufferBufferItem.SelectFilename)).ToLower();
            


            //OK to use Elements() (rather than XPath) because the elements have already been selected via XPath
            foreach (XElement item in items.Elements())
            {
                string apiRelativeUri = EvaluateXPath(item, configBufferBufferItem.SelectApiRelativeUri);
                Logger.DebugFormat("Processing ({0}/{1}): {2} --> {3}", itemCount, totalItemCount, apiRelativeUri, filename);
                bufferedDataCount += BufferAllPagedData(item, configBufferBufferItem.SelectBufferedElement, configBufferBufferItem.SelectElements, apiRelativeUri, configBufferBufferItem.ApiPerPage);
                itemCount++;
            }

            WriteData(filename, items, 0, this.retryMaxCountFile);
            Logger.DebugFormat("Buffered {0}, {1} records", filename, totalItemCount);

            return bufferedDataCount;
        }

        public int BufferItems(XElement items, ConfigBufferBufferItem configBufferBufferItem)
        {
            //Initialise counters
            int itemCount = 1;
            int totalItemCount = items.Elements().Count();
            int bufferedDataCount = 0;

            //OK to use Elements() (rather than XPath) because the elements have already been selected via XPath
            foreach (XElement item in items.Elements())
            {
                string apiRelativeUri = EvaluateXPath(item, configBufferBufferItem.SelectApiRelativeUri);
                string filename = Path.Combine(this.bufferFolder, EvaluateXPath(item, configBufferBufferItem.SelectFilename)).ToLower();
                Logger.DebugFormat("Processing ({0}/{1}): {2} --> {3}", itemCount, totalItemCount, apiRelativeUri, filename);
                bufferedDataCount += BufferPagedData(item, configBufferBufferItem.SelectBufferedElement, configBufferBufferItem.SelectElements, apiRelativeUri, filename, configBufferBufferItem.ApiPerPage);
                itemCount++;
            }

            return bufferedDataCount;
        }

        public int WriteItems(XElement items, ConfigBufferWriteItems configBufferBufferItems)
        {
            //Initialise counters
            int totalItemCount = items.Elements().Count();

            string filename = Path.Combine(this.bufferFolder, EvaluateXPath(items, configBufferBufferItems.SelectFilename)).ToLower();

            WriteData(filename, items, 0, this.retryMaxCountFile);

            Logger.DebugFormat("Buffered {0}, {1} records", filename, totalItemCount);

            return totalItemCount;
        }
        #endregion

        #region Private Methods
        private string EvaluateXPath(XElement item, string xpath)
        {
            IEnumerable value = (IEnumerable)item.XPathEvaluate(xpath, ElementsAPIAbstract.ElementsAPIAbstract.NAMESPACE_MANAGER);


            //If its just a string
            if (value is string && value != null)
                return (string)value;

            //Now try as an Attribute
            XAttribute attribute = value.OfType<XAttribute>().FirstOrDefault();
            if (attribute != null)
                return attribute.Value;

            //now try as an XElement
            XElement element = value.OfType<XElement>().FirstOrDefault();
            if (element != null)
                return element.Value;

            //Cannot be evaluated - return null
            return null;
        }

        private int BufferAllPagedData(XElement rootElement, string selectBufferedElement, string selectElements,
            string apiRelativeUri, int apiPerPage)
        {
            XElement bufferedElement = rootElement.XPathSelectElement(selectBufferedElement, NAMESPACE_MANAGER);

            rootElement.Add(new XAttribute("buffered-when", DateTime.Now.ToString("o")));

            int count = ReadPagedData(selectElements, apiRelativeUri, apiPerPage, bufferedElement);

            rootElement.Add(new XAttribute("buffered-item-count", count));

            return count;
        }

        private int BufferPagedData(XElement rootElement, string selectBufferedElement, string selectElements, string apiRelativeUri, string filename, int apiPerPage)
        {
            XElement bufferedElement = rootElement.XPathSelectElement(selectBufferedElement, NAMESPACE_MANAGER);

            rootElement.Add(new XAttribute("buffered-when", DateTime.Now.ToString("o")));

            int count = ReadPagedData(selectElements, apiRelativeUri, apiPerPage, bufferedElement);

            rootElement.Add(new XAttribute("buffered-item-count", count));

            WriteData(filename, rootElement, 0, this.retryMaxCountFile);

            Logger.DebugFormat("Buffered {0}, {1} records", filename, count);
            return count;
        }


        private void WriteData(string filename, XElement data, int retryCount, int retryMaxCount)
        {
            //Check if directory needs to be created
            DirectoryInfo dir = Directory.GetParent(filename);
            if (!dir.Exists)
            {
                Logger.WarnFormat("Creating directory: {0}", dir.FullName);
                dir.Create();
            }
            
            //Write data to file
            try
            {
                using (FileStream fs = File.Open(filename, FileMode.Create, FileAccess.Write, FileShare.Read))
                using (XmlTextWriter xw = new XmlTextWriter(fs, Encoding.UTF8))
                {
                    xw.WriteStartDocument();
                    data.WriteTo(xw);
                    xw.WriteEndDocument();
                }
            }
            catch (System.IO.IOException exc)
            {
                if (retryCount < retryMaxCount)
                {
                    Logger.WarnFormat("{0}: Could not write to: {1}. Sleeping and Retrying (count: {2}/{3}).", exc.Message, filename, retryCount, retryMaxCount);
                    Thread.Sleep(500); //Sleep for 500 ms, and then retry.
                    WriteData(filename, data, retryCount + 1, retryMaxCount);
                }
                else
                    throw;
            }
        }
        #endregion
    }
}