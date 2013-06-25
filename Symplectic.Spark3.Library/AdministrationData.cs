using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using System.IO;


namespace Symplectic.Spark3.Library
{
    public class AdministrationData : XmlUtilities
    {
        private string BufferFolderAbsolute { get; set; }
        public AdministrationData(string bufferFolderAbsolute)
        {
            this.BufferFolderAbsolute = bufferFolderAbsolute;
        }

        private void SetAttributeFromElement(XElement sourceElement, XElement destinationElement, string destinationAttribute)
        {
            if (sourceElement != null)
                destinationElement.SetAttributeValue(destinationAttribute, sourceElement.Value);
        }

         
        public void RefreshAdmistrationSettings()
        {
            string filenameGroupPersonIndex = Path.Combine(BufferFolderAbsolute, "group-person-index.xml");
            string filenameBufferSettings = Path.Combine(BufferFolderAbsolute, "buffer-settings.xml");
            string filenameAdministration = Path.Combine(BufferFolderAbsolute, "administration.xml"); ;

            XDocument xdocAdministration;

            if (File.Exists(filenameAdministration))
                xdocAdministration = LoadXmlFile(filenameAdministration);
            else
            {
                xdocAdministration = new XDocument();
                xdocAdministration.Add(new XElement("items"));
            }

            string output = null;

            if (File.Exists(filenameGroupPersonIndex))
            {
                XDocument xdocGroupPersonIndex = LoadXmlFile(filenameGroupPersonIndex);
                foreach (XElement userGroup in xdocGroupPersonIndex.XPathSelectElements("items/api:user-group", this.NamespaceManager))
                {
                    XElement adminUserGroup = xdocAdministration.XPathSelectElement("/items/api:user-group[@id='" + userGroup.Attribute("id").Value + "']", this.NamespaceManager);
                    if (adminUserGroup == null)
                    {
                        adminUserGroup = new XElement(API_NAMESPACE + "user-group");
                        adminUserGroup.Add(userGroup.Attribute("id"));
                        xdocAdministration.Root.Add(adminUserGroup);
                    }
                    
                    adminUserGroup.SetAttributeValue("name", userGroup.Element(API_NAMESPACE + "name").Value);

                    foreach (XElement userObject in userGroup.Elements(API_NAMESPACE + "object"))
                    {
                        XElement adminPerson = adminUserGroup.XPathSelectElement("api:person[@id='" + userObject.Attribute("id").Value + "']", this.NamespaceManager);
                        if (adminPerson == null)
                        {
                            adminPerson = new XElement(API_NAMESPACE + "person", new XAttribute("id", userObject.Attribute("id").Value));
                            adminUserGroup.Add(adminPerson);
                        }

                        //adminPerson.SetAttributeValue("username", userObject.Attribute("username").Value);
                        //adminPerson.SetAttributeValue("proprietary-id", userObject.Attribute("proprietary-id").Value);
                        SetAttributeFromElement(userObject.Element(API_NAMESPACE + "title"), adminPerson, "title");
                        SetAttributeFromElement(userObject.Element(API_NAMESPACE + "initials"), adminPerson, "initials");
                        SetAttributeFromElement(userObject.Element(API_NAMESPACE + "first-name"), adminPerson, "first-name");
                        SetAttributeFromElement(userObject.Element(API_NAMESPACE + "last-name"), adminPerson, "last-name");
                    }
                }

                SaveXmlFile(xdocAdministration, filenameAdministration);
            }
        }
    }
}
