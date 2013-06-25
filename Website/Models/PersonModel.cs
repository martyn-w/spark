using System;
using System.Xml.Linq;
using System.IO;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Collections.Generic;

namespace Spark3.Models
{

    #region Models
    public class PersonModel : AbstractModel
    {
        private string _bufferFilename = null;
        public override string BufferFilename
        {
            get
            {
                if (this._bufferFilename == null)
                    this._bufferFilename = Path.Combine(GlobalModel.BufferFolderAbsolute, this.ModelName + @"\" + this.PersonId + ".xml");
                return this._bufferFilename;
            }
        }


        public override void AfterInit()
        {
            this.Breadcrumbs.Add(new Breadcrumb(GlobalModel.ApplicationPathVirtual + "/departments", "Departments"));

            if (this.GroupId != null)
            {
                this.Breadcrumbs.Add(new Breadcrumb(GlobalModel.ApplicationPathVirtual + "/departments/" + GroupId, DepartmentName));

                if (this.PersonId != null)
                {
                    this.Breadcrumbs.Add(new Breadcrumb(GlobalModel.ApplicationPathVirtual + "/departments/" + GroupId + "/people/" + PersonId.ToLower(), PageTitle));
                }
            }
        }


        private string _departmentName = null;
        private string DepartmentName
        {
            get
            {
                if (_departmentName == null && AdministrationXml != null)
                {
                    XElement usergroup = AdministrationXml.XPathSelectElement("items/api:user-group[@id='" + this.GroupId + "']", this.NamespaceManager);

                    if (usergroup.Attribute("alternative-name") != null)
                        _departmentName = usergroup.Attribute("alternative-name").Value;
                    else
                        _departmentName = usergroup.Attribute("name").Value;
                }
                return _departmentName;
            }
        }

    
        public override string ModelName
        {
            get { return "person"; }
        }

        public string PageTitle
        {
            get
            {
                if (BufferFileExists)
                    return (string)BufferXml.XPathEvaluate("concat(api:object[@category='user']/api:title, ' ', api:object[@category='user']/api:first-name, ' ', api:object[@category='user']/api:last-name)", this.NamespaceManager);
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