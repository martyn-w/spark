using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.IO;
using System.Web.Hosting;
using System.Xml.Linq;
using System.Text.RegularExpressions;


namespace Symplectic.Spark3.Website.Models
{
    public static class SparkConfig
    {

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

        #endregion
        #endregion

    }
}