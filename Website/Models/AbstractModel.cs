using System;
using System.Xml.Linq;
using System.IO;
//using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using Symplectic.Spark3.Library;


namespace Spark3.Models
{

    public abstract class AbstractModel : XmlUtilities
    {
        //You must overide this in the derived class
        #region ModelName
        public abstract string ModelName { get; }
        #endregion

        public AbstractModel()
        {
            //AfterInit();
        }

        public virtual void AfterInit()
        { }

        public virtual string GroupId
        {
            get;
            set;
        }

        public virtual string PersonId
        {
            get;
            set;
        }

        public virtual void AddXArgs(XsltArgumentList xargs)
        { }
        
        #region buffer

        protected string AdministrationFilename
        {
            get { return Path.Combine(GlobalModel.BufferFolderAbsolute, "administration.xml"); }
        }

        protected bool AdministrationFileExists
        {
            get { return File.Exists(this.AdministrationFilename); }
        }
        
        private XDocument _administrationXml = null;
        protected XDocument AdministrationXml
        {
            get
            {
                if (this._administrationXml == null)
                    this._administrationXml = LoadXmlFile(this.AdministrationFilename);
                return this._administrationXml;
            }
            private set { this._administrationXml = value; }
        }

        public abstract string BufferFilename
        {
            get;
        }


        public virtual bool BufferFileExists
        {
            get { return File.Exists(this.BufferFilename); }
        }

        private XDocument _bufferXml = null;
        public virtual XDocument BufferXml
        {
            get
            {
                if (this._bufferXml == null)
                    this._bufferXml = LoadXmlFile(this.BufferFilename);
                return this._bufferXml;
            }
            private set { this._bufferXml = value; }
        }
        #endregion
                
        #region xsl
        private string _xslFilename = null;
        public virtual string XslFilename
        {
            get
            {
                if (this._xslFilename == null)
                    this._xslFilename = Path.Combine(GlobalModel.XslPathAbsolute, this.ModelName + ".xsl");
                return this._xslFilename;
            }
            set { this._xslFilename = value; }
        }

        public virtual bool XslFileExists
        {
            get { return File.Exists(this.XslFilename); }
        }


        public virtual string TransformXml()
        {
            if (XslFileExists && BufferFileExists)
            {
                StringBuilder output = new StringBuilder();
                using (StringWriter writer = new StringWriter(output))
                {
                    // Load the style sheet.
                    XslCompiledTransform xslt = new XslCompiledTransform();
                    XsltSettings settings = new XsltSettings(true, false);
                    xslt.Load(this.XslFilename, settings, new XmlUrlResolver());
                    XsltArgumentList xargs = new XsltArgumentList();

                    xargs.AddParam("application-path", string.Empty, GlobalModel.ApplicationPathVirtual);
                    xargs.AddParam("public-path", string.Empty, GlobalModel.PublicPathVirtual);
                    xargs.AddParam("model", string.Empty, this.ModelName);

                    if (this.GroupId != null)
                        xargs.AddParam("group-id", string.Empty, GroupId);

                    if (this.PersonId != null)
                        xargs.AddParam("person-id", string.Empty, PersonId);

                    AddXArgs(xargs);

                    try
                    {
                        xslt.Transform(BufferXml.CreateNavigator(), xargs, writer);
                    }
                    catch (XsltException ex)
                    {
                        throw new Exception("Failed to process XSL: " + this.XslFilename + " Line number:" + ex.LineNumber.ToString() + " position: " + ex.LinePosition.ToString() + " message " + ex.Message, ex);
                    }
                }
                return output.ToString();
            }
            else
            {
                if (!BufferFileExists)
                    return "File " + BufferFilename + " not found";
                else
                {
                    if (!XslFileExists)
                        return "File " + XslFilename + " not found";
                    else
                        return null;
                }
            } //return "File not found";//  null;
        }
        #endregion

        #region Breadcrumbs
        private List<Breadcrumb> _breadcrumbs = null;
        public virtual List<Breadcrumb> Breadcrumbs
        {
            get
            {
                if (this._breadcrumbs == null)
                {
                    this._breadcrumbs = new List<Breadcrumb>();
                    this.Breadcrumbs.Add(new Breadcrumb("http://www.example-org.ac.uk/", "Home"));
                }
                return this._breadcrumbs;
            }
            private set { this._breadcrumbs = value; }
        }

        public virtual string BreadcrumbsHtml()
        {
            StringBuilder output = new StringBuilder();
            foreach (Breadcrumb b in Breadcrumbs)
            {
                output.AppendLine(b.ToHtml());
            }
            return output.ToString();
        }
        #endregion

        #region useful properties
        public virtual string LastModifiedWhen
        {
            get { return BufferXml.XPathSelectElement("api:object/@last-modified-when", this.NamespaceManager).Value; }
        }
        
        #endregion

        
    }

}