using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Xml.Linq;
using System.IO;

using Symplectic.Spark3.Library;

namespace Symplectic.Spark3.Website.Models
{
    public abstract class AbstractModel
    {
        
        #region ModelName and ViewName
        //You must overide these in the derived class
        public abstract string ModelName { get; }
        public abstract string ViewName { get; }
        #endregion


        #region Id
        public string Id { get; set; }
        #endregion


        #region BufferXML
        private string _bufferFilename = null;
        public string BufferFilename
        {
            get
            {
                if (this._bufferFilename == null)
                    this._bufferFilename = Path.Combine(SparkConfig.BufferFolderAbsolute, this.ModelName + @"\" + this.Id + ".xml");
                return this._bufferFilename;
            }
        }

        public bool BufferFileExists
        {
            get { return File.Exists(this.BufferFilename); }
        }

        private XDocument _bufferXml = null;
        public virtual XDocument BufferXml
        {
            get
            {
                if (this._bufferXml == null)
                    this._bufferXml = XmlUtilities.LoadXmlFile(this.BufferFilename);
                return this._bufferXml;
            }
            private set { this._bufferXml = value; }
        }
        #endregion


        #region XSL Transforms
        private string _xslFilename = null;
        public virtual string XslFilename
        {
            get
            {
                if (this._xslFilename == null)
                    this._xslFilename = Path.Combine(SparkConfig.XslPathAbsolute, this.ViewName + ".xsl");
                return this._xslFilename;
            }
            set { this._xslFilename = value; }
        }

        public virtual bool XslFileExists
        {
            get { return File.Exists(this.XslFilename); }
        }


        public virtual HtmlString TransformXml()
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

                    xargs.AddParam("application-path", string.Empty, SparkConfig.ApplicationPathVirtual);
                    xargs.AddParam("public-path", string.Empty, SparkConfig.PublicPathVirtual);
                    xargs.AddParam("model", string.Empty, this.ModelName);

                    if (this.Id != null)
                        xargs.AddParam("person-id", string.Empty, this.Id);

                    //AddXArgs(xargs);

                    try
                    {
                        xslt.Transform(BufferXml.CreateNavigator(), xargs, writer);
                    }
                    catch (XsltException ex)
                    {
                        throw new Exception("Failed to process XSL: " + this.XslFilename + " Line number:" + ex.LineNumber.ToString() + " position: " + ex.LinePosition.ToString() + " message " + ex.Message, ex);
                    }
                }
                //return output.ToString();
                return new HtmlString(output.ToString());
            }
            else
            {
                if (!BufferFileExists)
                    return new HtmlString("<!--File " + BufferFilename + " not found-->");
                else
                {
                    if (!XslFileExists)
                        return new HtmlString("File " + XslFilename + " not found");
                    else
                        return null;
                }
            } //return "File not found";//  null;
        }
        #endregion


    }
}