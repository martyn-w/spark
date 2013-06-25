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
    public class PersonListModel : AbstractModel
    {
        public override string ModelName
        {
            get { return "personlist"; }
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
                return "Department People List";
            }
        }

        public string HeadTitle
        {
            get { return "example-org: " + PageTitle; }
        }

        public override void AfterInit()
        {
            this.Breadcrumbs.Add(new Breadcrumb(GlobalModel.ApplicationPathVirtual + "/departments", "Departments"));
        }

    }
    #endregion
}