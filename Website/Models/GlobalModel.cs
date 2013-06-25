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

// $URL: https://symplectic-spark.googlecode.com/svn/trunk/website/App_Code/Global.cs $
// $LastChangedDate: 2010-03-30 14:27:49 +0100 (Tue, 30 Mar 2010) $
// $LastChangedRevision: 24 $
// $LastChangedBy: martyn@symplectic.co.uk $

using System;
using System.Configuration;
using System.IO;
using System.Web.Hosting;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace Spark3.Models
{
    public static class GlobalModel
    {
        //#region Logger
        //private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //#endregion

        //#region Debug
        //public static void ClearSettings()
        //{
        //    //This method clears out all the cached ElementTypes and Components, to be reloaded per request (in debug mode)
        //    _xElementTypes = null;
        //    _elementTypes = null;
        //    _xComponents = null;
        //    _components = null;
        //}
        //#endregion

        //#region ElementTypes
        //private static XElement _xElementTypes = null;
        //public static XElement XElementTypes
        //{
        //    get
        //    {
        //        if (_xElementTypes == null)
        //        {
        //            XElementTypes = XElement.Load(ElementTypesPathAbsolute);
        //        }
        //        return _xElementTypes;
        //    }
        //    private set
        //    {
        //        _xElementTypes = value;
        //    }
        //}

        //private static ElementTypes _elementTypes = null;
        //public static ElementTypes ElementTypes
        //{
        //    get
        //    {
        //        if (_elementTypes == null)
        //            ElementTypes = new ElementTypes(XElementTypes);
        //        return _elementTypes;
        //    }
        //    private set
        //    {
        //        _elementTypes = value;
        //    }
        //}

        //public static ElementType ErrorElementType
        //{
        //    get
        //    {
        //        if (ElementTypes.ContainsKey("error"))
        //            return ElementTypes["error"];
        //        else
        //            throw new Exception("Error ElementType could not be found");
        //    }
        //}
        //#endregion

        //#region Components
        //private static XElement _xComponents = null;
        //public static XElement XComponents
        //{
        //    get
        //    {
        //        if (_xComponents == null)
        //            XComponents = XElement.Load(ComponentsPathAbsolute);
        //        return _xComponents;
        //    }
        //    private set
        //    {
        //        _xComponents = value;
        //    }
        //}

        //private static Components _components = null;
        //public static Components Components
        //{
        //    get
        //    {
        //        if (_components == null)
        //            Components = new Components(XComponents);
        //        return _components;
        //    }
        //    private set
        //    {
        //        _components = value;
        //    }
        //}
        //#endregion

        //#region Templates
        //private static Templates _templates = null;
        //public static Templates Templates
        //{
        //    get
        //    {
        //        //TO DO: Improve efficency of Template Watcher
        //        if (_templates == null)
        //            Templates = new Templates(ElementTypes);
        //        return _templates;
        //    }
        //    private set
        //    {
        //        _templates = value;
        //    }
        //}

        //public static Template ErrorTemplate
        //{
        //    get
        //    {
        //        if (Templates.ContainsKey(ErrorElementType.Index.Name))
        //            return Templates[ErrorElementType.Index.Name];
        //        else
        //            throw new Exception("Error Template could not be found");
        //    }
        //}
        //#endregion

        #region Paths
        #region VirtualPaths
        private static string _applicationPathVirtual = null;
        public static string ApplicationPathVirtual
        {
            get
            {
                if (_applicationPathVirtual == null)
                    _applicationPathVirtual = HostingEnvironment.ApplicationVirtualPath == "/" ? "" : HostingEnvironment.ApplicationVirtualPath;
                return _applicationPathVirtual;
            }
        }

        public static string PublicPathVirtual
        {
            get { return ApplicationPathVirtual + "/" + ConfigurationManager.AppSettings["public-path"]; }
        }

        public static string PublicPathVirtualFilename(string filename)
        {
            return PublicPathVirtual + filename;
        }
        #endregion

        #region AbsolutePaths
        public static string ApplicationPathAbsolute
        {
            get { return HostingEnvironment.ApplicationPhysicalPath; }
        }

        public static string PrivateFolderAbsolute
        {
            get { return Path.Combine(ApplicationPathAbsolute, ConfigurationManager.AppSettings["private-folder"]); }
        }

        public static string BufferFolderAbsolute
        {
            get { return Path.Combine(ApplicationPathAbsolute, ConfigurationManager.AppSettings["buffer-folder"]); }
        }

        public static string XslPathAbsolute
        {
            get { return Path.Combine(PrivateFolderAbsolute, "xsl"); }
        }

        public static string TemplatePathAbsolute
        {
            get { return Path.Combine(PrivateFolderAbsolute, "templates"); }
        }

        public static string ElementTypesPathAbsolute
        {
            get { return Path.Combine(PrivateFolderAbsolute, @"element-types\element-types.xml"); }
        }

        public static string ComponentsPathAbsolute
        {
            get { return Path.Combine(PrivateFolderAbsolute, @"components\components.xml"); }
        }

        public static string IndexDirectory
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["index-directory"]; }
        }

        #endregion
        #endregion

        #region Settings
        public static bool DebugMode
        {
            get { return bool.Parse(ConfigurationManager.AppSettings["debug-mode"]); }
        }

        public static int GetRequestTimeout
        {
            get { return int.Parse(ConfigurationManager.AppSettings["get-request-timeout"]); }
        }

        public static string PerPage
        {
            get { return ConfigurationManager.AppSettings["per-page"]; }
        }

        public static string Version
        {
            get { return File.ReadAllText(Path.Combine(ApplicationPathAbsolute, "version.txt")); }
        }
        #endregion

        //#region FileWatchers
        //private static void ElementTypes_OnChanged(object sender, FileSystemEventArgs e)
        //{
        //    Logger.DebugFormat("File {0} has changed, reloading XElementTypes and ElementTypes", e.FullPath);
        //    XElementTypes = null;
        //    ElementTypes = null;
        //}

        //private static void Components_OnChanged(object sender, FileSystemEventArgs e)
        //{
        //    Logger.DebugFormat("File {0} has changed, reloading XComponents and Components", e.FullPath);
        //    XComponents = null;
        //    Components = null;
        //}

        //private static void Templates_OnChanged(object sender, FileSystemEventArgs e)
        //{
        //    Logger.DebugFormat("File {0} has changed, reloading Templates", e.FullPath);
        //    Templates = null;

        //}
        //#endregion

        #region Regular Expressions
        public static readonly Regex RegexComponent = new Regex(@"\[\[(?<name>[^\]]+)\]\]", RegexOptions.Compiled);
        public static readonly Regex RegexInternalFunction = new Regex(@"\(\((?<name>[^\)]+)\)\)", RegexOptions.Compiled);
        public static readonly Regex RegexRequestSpec = new Regex(@"\{\{(?<name>[^\}]+)\}\}", RegexOptions.Compiled);
        #endregion

        // constructor, to initialise the global constants
        //static Global()
        //{
        //    //Watch XML files and templates in case they are updated on the fly
        //    FileSystemWatcher watchElementTypes = new FileSystemWatcher(Path.GetDirectoryName(ElementTypesPathAbsolute), Path.GetFileName(ElementTypesPathAbsolute));
        //    watchElementTypes.Changed += new FileSystemEventHandler(ElementTypes_OnChanged);
        //    watchElementTypes.NotifyFilter = NotifyFilters.LastWrite;
        //    watchElementTypes.EnableRaisingEvents = true;

        //    FileSystemWatcher watchComponents = new FileSystemWatcher(Path.GetDirectoryName(ComponentsPathAbsolute), Path.GetFileName(ComponentsPathAbsolute));
        //    watchComponents.Changed += new FileSystemEventHandler(Components_OnChanged);
        //    watchComponents.NotifyFilter = NotifyFilters.LastWrite;
        //    watchComponents.EnableRaisingEvents = true;

        //    FileSystemWatcher watchTemplates = new FileSystemWatcher(Global.TemplatePathAbsolute, "*.*");
        //    watchTemplates.Changed += new FileSystemEventHandler(Templates_OnChanged);
        //    watchTemplates.NotifyFilter = NotifyFilters.LastWrite;
        //    watchTemplates.EnableRaisingEvents = true;
        //}

    }
}
