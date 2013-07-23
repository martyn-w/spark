//  Symplectic Spark
//  Copyright 2013 Symplectic Ltd
//  Created by Martyn Whitwell

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



using System;
using System.Configuration;
using System.Text;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Diagnostics;
using System.Collections;
using Spark3.Buffer;
using System.Collections.Generic;
using System.Security;

using log4net;

//Need this to ensure logging works!
[assembly: log4net.Config.XmlConfigurator(Watch = true)] 

namespace Spark3.Rebuffer
{
    class Program
    {
        #region Logger
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region Configuration Settings
        private static readonly string apiBaseUrl = ConfigurationManager.AppSettings["api-base-url"];
        private static readonly int apiInterval = int.Parse(ConfigurationManager.AppSettings["api-interval"]);
        private static readonly string xslFolder = ConfigurationManager.AppSettings["xsl-folder"];
        private static readonly string groupTopPublicationsXslFile = Path.Combine(xslFolder, "group-top-publications.xsl");
        private static readonly string bufferFolder = ConfigurationManager.AppSettings["buffer-folder"];
        private static readonly string bufferSettingsFile = Path.Combine(bufferFolder, "buffer-settings.xml");
        private static readonly int timeout = int.Parse(ConfigurationManager.AppSettings["timeout"]);
        private static readonly int apiRetryMaxCount = int.Parse(ConfigurationManager.AppSettings["api-retry-max-count"]);
        private static readonly int fileRetryMaxCount = int.Parse(ConfigurationManager.AppSettings["file-retry-max-count"]);
        private static readonly string apiUsername = ConfigurationManager.AppSettings["api-username"];
        private static readonly string apiPassword = ConfigurationManager.AppSettings["api-password"];
        private static readonly bool apiIgnoreCertificate = ConfigurationManager.AppSettings["api-ignore-certificate"] == "true";



            

        #endregion

        private enum Mode { Differential, Full }

        #region namespaceManager
        private static XmlNamespaceManager _namespaceManager;
        public static XNamespace API_NAMESPACE = "http://www.symplectic.co.uk/publications/api";
        public static XNamespace ATOM_NAMESPACE = "http://www.w3.org/2005/Atom";
        public  static XmlNamespaceManager NamespaceManager
        {
            get
            {
                if (_namespaceManager == null)
                {
                    NameTable nameTable = new NameTable();
                    _namespaceManager = new XmlNamespaceManager(nameTable);
                    _namespaceManager.AddNamespace("api", API_NAMESPACE.NamespaceName);
                    _namespaceManager.AddNamespace("atom", ATOM_NAMESPACE.NamespaceName);
                }
                return _namespaceManager;
            }
        }
        #endregion


        static void Main(string[] args)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();            

            Mode mode;
            if (args.Length == 1)
                switch (args[0].ToLower().Trim())
                {
                    case "full":
                        mode = Mode.Full;
                        break;
                    case "differential":
                    default:
                        mode = Mode.Differential;
                        break;
                }
            else
                mode = Mode.Differential;
            
            Logger.InfoFormat("Buffering data");
            Logger.InfoFormat("Buffering mode: {0}", mode);

            Logger.DebugFormat("API:\t{0}", apiBaseUrl);
            Logger.DebugFormat("API Interval:\t{0}", apiInterval);
            Logger.DebugFormat("XSL Folder:\t{0}", xslFolder);
            Logger.DebugFormat("Buffer Folder:\t{0}", bufferFolder);
           // Logger.DebugFormat("Components Folder:\t{0}", componentsFolder);
            Logger.DebugFormat("Timeout:\t{0}", timeout);
            Logger.DebugFormat("API RetryMaxCount:\t{0}", apiRetryMaxCount);
            Logger.DebugFormat("File RetryMaxCount:\t{0}", fileRetryMaxCount);

            Spark3.Buffer.Buffer buffer = new Spark3.Buffer.Buffer(bufferFolder, fileRetryMaxCount, apiBaseUrl, apiInterval, timeout, apiRetryMaxCount, apiUsername, apiPassword, apiIgnoreCertificate);
            int errorCount = 0;

            int count = 0;
            foreach (ConfigBuffer configBuffer in ConfigBufferSettings.Config.Buffers)
            {

                Logger.DebugFormat("Processing ConfigBuffer Element: {0}", configBuffer.Name);

                try
                {
                    //Method to count publications
                    if (configBuffer.Settings.BufferMode == ConfigBufferBufferSettings.enBufferMode.PostProcess)
                    {
                        if (configBuffer.Settings.AdministrationFilename == null || configBuffer.Settings.AdministrationFilename.Length == 0)
                            throw new ConfigurationErrorsException("administration-filename is not set in the PostProcess section");

                        if (configBuffer.Settings.InputFilename == null || configBuffer.Settings.InputFilename.Length == 0)
                            throw new ConfigurationErrorsException("input-filename is not set in the PostProcess section");

                        if (configBuffer.Settings.OutputFilename == null || configBuffer.Settings.OutputFilename.Length == 0)
                            throw new ConfigurationErrorsException("output-filename is not set in the PostProcess section");

                        string administrationFile = Path.Combine(bufferFolder, configBuffer.Settings.AdministrationFilename); // "administration.xml"
                        string inputFile = Path.Combine(bufferFolder, configBuffer.Settings.InputFilename); // "group-person-index-temp.xml"
                        string outputFile = Path.Combine(bufferFolder, configBuffer.Settings.OutputFilename); // "group-person-index.xml"

                        if (!File.Exists(inputFile))
                        {
                            Logger.ErrorFormat("PostProcess input-filename does not exist: {0}", inputFile);
                            throw new FileNotFoundException("PostProcess input-filename does not exist", inputFile);
                        }


                        if (!File.Exists(administrationFile))
                        {
                            Logger.WarnFormat("Administration file {0} does not exist, recreating now", administrationFile);
                            Symplectic.Spark3.Library.AdministrationData adminData = new Symplectic.Spark3.Library.AdministrationData(bufferFolder);
                            adminData.RefreshAdmistrationSettings();
                        }

                        Logger.InfoFormat("Loading {0}", administrationFile);
                        XDocument xdocAdmin = XDocument.Load(administrationFile);

                        Logger.InfoFormat("Loading {0}", inputFile);
                        XDocument xdocGroupPersonIndexTemp = XDocument.Load(inputFile);

                        string[] files = Directory.GetFiles(Path.Combine(bufferFolder, "person"));

                        foreach (string file in files)
                        {
                            XDocument xdoc = XDocument.Load(file);

                            Logger.InfoFormat("Processing {0}", file);

                            XAttribute id = xdoc.Root.Attribute("id");
                            XAttribute bic = xdoc.Root.Attribute("buffered-item-count");

                            if (bic != null && id != null)
                            {
                                Logger.InfoFormat("Count: {0}", bic.Value);


                                foreach (XElement p in xdocAdmin.XPathSelectElements("/items/api:user-group/api:person[@id=" + id.Value + "]", NamespaceManager))
                                {
                                    p.SetAttributeValue(bic.Name, bic.Value);
                                }

                                foreach (XElement o in xdocGroupPersonIndexTemp.XPathSelectElements("/items/api:user-group/api:object[@id=" + id.Value + "]", NamespaceManager))
                                {
                                    o.SetAttributeValue(bic.Name, bic.Value);

                                }
                            }
                            // break;
                        }


                        //now get top X publications for each user-group, store in group-person-index.xml

                        XslCompiledTransform xslt = new XslCompiledTransform();
                        XsltSettings settings = new XsltSettings(true, false);
                        xslt.Load(groupTopPublicationsXslFile, settings, new XmlUrlResolver());


                        files = Directory.GetFiles(Path.Combine(bufferFolder, "group"));
                        foreach (string file in files)
                        {
                            XDocument xdoc = XDocument.Load(file);
                            Logger.InfoFormat("Processing {0}", file);
                            XAttribute id = xdoc.Root.Attribute("id");
                            
                            XsltArgumentList xargs = new XsltArgumentList();
                            xargs.AddParam("group-id", string.Empty, id.Value);
                            xargs.AddParam("mode", string.Empty, "top-10-pubs");

                            StringBuilder output = new StringBuilder();
                            using (StringWriter writer = new StringWriter(output))
                            {
                                try
                                {
                                    xslt.Transform(xdoc.CreateNavigator(), xargs, writer);
                                }
                                catch (XsltException ex)
                                {
                                    throw new Exception("Failed to process XSL: " + groupTopPublicationsXslFile + " Line number:" + ex.LineNumber.ToString() + " position: " + ex.LinePosition.ToString() + " message " + ex.Message, ex);
                                }
                            }

                            foreach (XElement p in xdocGroupPersonIndexTemp.XPathSelectElements("/items/api:user-group[@id=" + id.Value + "]", NamespaceManager))
                            {
                                XElement currentNode = p.Element(XName.Get("group-top-publications", Symplectic.Spark3.Library.XmlUtilities.API_NAMESPACE.NamespaceName));

                                if (currentNode != null)
                                    currentNode.ReplaceWith(XElement.Parse(output.ToString()));
                                else
                                    p.Add(XElement.Parse(output.ToString()));
                            }
                        
                        }

                        Logger.InfoFormat("Saving {0}", administrationFile);
                        xdocAdmin.Save(administrationFile);

                        Logger.InfoFormat("Saving {0}", outputFile);
                        xdocGroupPersonIndexTemp.Save(outputFile); //read from group-person-index-temp.xml; save to group-person-index.xml

                        Logger.InfoFormat("Completed PostProcess");

                    }
                    else
                    {

                        //setup variables
                        DateTime starttime = DateTime.Now;
                        XElement items = new XElement("items");
                        string modifiedSinceKey = configBuffer.Name + "-modified-since";
                        string uri = AppendModifiedSince(configBuffer.SourceItems.ApiRelativeUri, (configBuffer.SourceItems.ApiUseModifiedSince && mode == Mode.Differential), ReadBufferSetting(modifiedSinceKey));
                        int processCount = 0;

                        //load list of items to buffer

                        int sourceItemsCount;

                        if (configBuffer.SourceItems.ApiUsePaging)
                            sourceItemsCount = buffer.ReadPagedData(configBuffer.SourceItems.SelectElements, uri, configBuffer.SourceItems.ApiPerPage, items);
                        else
                            sourceItemsCount = buffer.ReadData(configBuffer.SourceItems.SelectElements, uri, items);

                        Logger.DebugFormat("Loaded {0} {1} items: {2}", configBuffer.Name, uri, sourceItemsCount);

                        switch (configBuffer.Settings.BufferMode)
                        {
                            case ConfigBufferBufferSettings.enBufferMode.BufferAllItems:
                                //Does each item need buffering? if so, call buffer.process()
                                //Buffer the items, into one big file
                                processCount += buffer.BufferAllItems(items, configBuffer.BufferAllItems);
                                Logger.DebugFormat("Buffered {0} {1} records: {2}", configBuffer.Name, uri, processCount);
                                break;

                            case ConfigBufferBufferSettings.enBufferMode.BufferItem:
                                //Does each item need buffering? if so, call buffer.process()
                                //Buffer the items, one at a time
                                processCount += buffer.BufferItems(items, configBuffer.BufferItem);
                                Logger.DebugFormat("Buffered {0} {1} records: {2}", configBuffer.Name, uri, processCount);
                                break;

                            case ConfigBufferBufferSettings.enBufferMode.WriteItems:
                                //Else, just write the items straight to disk
                                //Write the data
                                processCount += buffer.WriteItems(items, configBuffer.WriteItems);
                                Logger.DebugFormat("Buffered {0} {1} records: {2}", configBuffer.Name, uri, processCount);
                                break;

                            case ConfigBufferBufferSettings.enBufferMode.DeleteItems:
                                //Get a list of items to delete and delete them
                                processCount += buffer.DeleteItems(items, configBuffer.DeleteItems);
                                Logger.DebugFormat("Deleted {0} {1} records: {2}", configBuffer.Name, uri, processCount);
                                break;

                        }


                        WriteBufferSetting(modifiedSinceKey, starttime.ToString("o"));
                        count += processCount;
                    }
                }
                catch (Exception ex)
                {
                    Logger.ErrorFormat("Failed to buffer {0}: {1}", configBuffer.Name, ex);
                    errorCount++;
                }
            }

            stopwatch.Stop();

            Logger.InfoFormat("All done, processed {0} objects in {1} seconds ({2} objects/second)", count, (int)stopwatch.Elapsed.TotalSeconds, (int)(count / stopwatch.Elapsed.TotalSeconds));

            if (errorCount > 0)
                Logger.ErrorFormat("There were {0} errors in processing {1} configuration(s). THESE SHOULD BE INVESTIGATED IMMEDIATELY.", errorCount, ConfigBufferSettings.Config.Buffers.Count);

            //Console.Read();
            System.Environment.ExitCode = errorCount;

          //  Console.ReadLine();
        }


        private static XDocument ReadBufferSettings()
        {
            //Create new XmlDocument
            XDocument bufferSettings = null;

            if (File.Exists(bufferSettingsFile))
            {
                try
                {
                    using (FileStream fs = File.Open(bufferSettingsFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (XmlTextReader xtr = new XmlTextReader(fs))
                    {
                        bufferSettings = XDocument.Load(xtr, LoadOptions.None);
                    }
                }
                catch (XmlException exc)
                {
                    Logger.ErrorFormat("{0}: Buffer settings file could not be loaded and has been corrupted: {1}", exc.Message, bufferSettingsFile);
                }
                catch (IOException exc)
                {
                    Logger.WarnFormat("{0}: Buffer settings file could not be read: {1}", exc.Message, bufferSettingsFile);
                }
            }
            else
                Logger.WarnFormat("Buffer settings file does not exist: {0}", bufferSettingsFile);

            //If bufferSettings could not be initialised, create a new one
            if (bufferSettings == null)
            {
                bufferSettings = new XDocument();
                bufferSettings.Add(new XElement("buffer-settings"));
            }

            return bufferSettings;
        }


        private static string AppendModifiedSince(string apiRelativeUri, bool useModifiedSince, string modifiedSince)
        {
            if (useModifiedSince && modifiedSince != null)
            {
                if (apiRelativeUri.Contains("?"))
                    return apiRelativeUri + "&modified-since=" + DateTime.Parse(modifiedSince).ToUniversalTime().ToString("o");
                else
                    return apiRelativeUri + "?modified-since=" + DateTime.Parse(modifiedSince).ToUniversalTime().ToString("o");
            }
            else return apiRelativeUri;
        }

        
        private static string ReadBufferSetting(string key)
        {
            XDocument bufferSettings = ReadBufferSettings();
            XElement setting = bufferSettings.Root.Element(key);
            return setting == null ? null : setting.Value;
        }

        private static bool WriteBufferSetting(string key, string value)
        {
            XDocument bufferSettings = ReadBufferSettings();
            XElement setting = bufferSettings.Root.Element(key);
            bool success = false;

            if (setting != null)
                setting.Value = value;
            else
            {
                setting = new XElement(key, value);
                bufferSettings.Root.Add(setting);
            }

            try
            {
                using (FileStream fs = File.Open(bufferSettingsFile, FileMode.Create, FileAccess.Write, FileShare.Read))
                using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    bufferSettings.Save(sw, SaveOptions.None);
                    success = true;
                }
            }
            catch (IOException exc)
            {
                Logger.WarnFormat("{0}: Buffer settings file could not be writen to: {1}", exc.Message, bufferSettingsFile);
            }

            return success;
        }
    }
}