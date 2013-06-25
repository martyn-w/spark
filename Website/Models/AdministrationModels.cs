using System;
using System.Xml.Linq;
using System.Linq;
using System.IO;
using System.Web.Mvc;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Collections.Specialized;
using System.Collections.Generic;

using Symplectic.Spark3.Library;

namespace Spark3.Models
{

    #region Models
    public class AdministrationModel : AbstractModel
    {
        #region Logger
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        public override string ModelName
        {
            get { return "administration"; }
        }

        public override string BufferFilename
        {
            get
            {
                return Path.Combine(GlobalModel.BufferFolderAbsolute, "administration.xml");
            }
        }

        public string PageTitle
        {
            get
            {
                return "Administration Page";
            }
        }

        public string HeadTitle
        {
            get { return "example-org: " + PageTitle; }
        }

        public override void AfterInit()
        {
            //Refresh the default names and people group-people lists
            AdministrationData adminData = new AdministrationData(GlobalModel.BufferFolderAbsolute);
            adminData.RefreshAdmistrationSettings();
        }

        public bool UpdateAdministrationSettings(NameValueCollection form)
        {
            XDocument xdocAdmin = XDocument.Load(this.BufferFilename);

            //clear hide settings on departments (because the form only returns if hide is selected)
            foreach (XElement p in xdocAdmin.XPathSelectElements("/items/api:user-group", NamespaceManager))
            {
                p.SetAttributeValue("hide", null);
            }

            //clear hide settings on people (because the form only returns if hide is selected)
            foreach (XElement p in xdocAdmin.XPathSelectElements("/items/api:user-group/api:person", NamespaceManager))
            {
                p.SetAttributeValue("hide", null);
            }


            //Now process keys
            foreach (string key in form)
            {
                //get id
                int id;
                if (int.TryParse(key.Split(new char[] { '-' }).Last(), out id))
                {
                    string value = form[key].Trim().Length > 0 ? form[key].Trim() : null;
                    
                    if (value != null)
                        Logger.InfoFormat("{0}\t=\t{1}", key, form[key]);


                    if (key.StartsWith("alternative-name-department-"))
                    {
                        foreach (XElement p in xdocAdmin.XPathSelectElements("/items/api:user-group[@id=" + id.ToString() + "]", NamespaceManager))
                        {
                            p.SetAttributeValue("alternative-name", value);
                        }
                    }
                    
                    if (key.StartsWith("rss-feed-department-"))
                    {
                        foreach (XElement p in xdocAdmin.XPathSelectElements("/items/api:user-group[@id=" + id.ToString() + "]", NamespaceManager))
                        {
                            p.SetAttributeValue("rss-feed", value);
                        }
                    }

                    if (key.StartsWith("hide-department-"))
                    {
                        foreach (XElement p in xdocAdmin.XPathSelectElements("/items/api:user-group[@id=" + id.ToString() + "]", NamespaceManager))
                        {
                            p.SetAttributeValue("hide", value);
                        }
                    }

                    if (key.StartsWith("hide-person-"))
                    {
                        foreach (XElement p in xdocAdmin.XPathSelectElements("/items/api:user-group/api:person[@id=" + id.ToString() + "]", NamespaceManager))
                        {
                            p.SetAttributeValue("hide", value);
                        }
                    }

                }
                else
                {
                    Logger.WarnFormat("Ignoring key {0}", key);
                }
            }

            xdocAdmin.Save(this.BufferFilename);

    
            return true;
        }
    }

    #endregion
}
