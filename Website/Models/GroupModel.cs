using System;
using System.Xml.Linq;
using System.IO;
using System.Web.Mvc;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Collections.Generic;

namespace Spark3.Models
{

    #region Models
  
    public class GroupModel : AbstractModel
    {
        public override void AfterInit()
        {
            this.Breadcrumbs.Add(new Breadcrumb(GlobalModel.ApplicationPathVirtual + "/departments", "Departments"));
            if (this.GroupId != null)
            {
                this.Breadcrumbs.Add(new Breadcrumb(GlobalModel.ApplicationPathVirtual + "/departments/" + GroupId, PageTitle));
            }
        }

        public override void AddXArgs(XsltArgumentList xargs)
        {
            //Hide the users marked as hidden
            xargs.AddParam("hidden-users", string.Empty, AdministrationXml.CreateNavigator().Select("/items/api:user-group[@id='" + this.GroupId + "']/api:person[@hide='true']", this.NamespaceManager));

            XElement usergroup = AdministrationXml.XPathSelectElement("items/api:user-group[@id='" + this.GroupId + "']", this.NamespaceManager);
            if (usergroup.Attribute("rss-feed") != null)
                xargs.AddParam("rss-feed", string.Empty, usergroup.Attribute("rss-feed").Value);
        }

        public  int[] getHiddenIds()
        {
            List<int> ids = new List<int>();
            foreach (XElement xe in AdministrationXml.XPathSelectElements("items/api:user-group[@id='" + this.GroupId + "']/api:person[@hide='true']", this.NamespaceManager))
            {
                ids.Add(int.Parse(xe.Attribute("id").Value));
            }

            return ids.ToArray();
        }

        public override string ModelName
        {
            get { return "group"; }
        }

        public override string BufferFilename
        {
            get
            {
                return Path.Combine(GlobalModel.BufferFolderAbsolute, "group-person-index.xml");
            }
        }

        public string PageTitle
        {
            get
            {
                if (AdministrationFileExists)
                {
                    XElement usergroup = AdministrationXml.XPathSelectElement("items/api:user-group[@id='" + this.GroupId + "']", this.NamespaceManager);
                    if (usergroup.Attribute("alternative-name") != null)
                        return usergroup.Attribute("alternative-name").Value;
                    else
                        return usergroup.Attribute("name").Value;
                }
                if (BufferFileExists)
                    return BufferXml.XPathSelectElement("items/api:user-group[@id='" + this.GroupId + "']/api:name", this.NamespaceManager).Value;
                else
                    return null;
            }
        }

        public string HeadTitle
        {
            get { return "example-org: " + PageTitle; }
        }

    }

    #endregion

}