using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;


namespace Symplectic.Spark3.Library
{
    public abstract class SparkNamespaceManager
    {
        #region namespaceManager
        private XmlNamespaceManager _namespaceManager;
        public static XNamespace API_NAMESPACE = "http://www.symplectic.co.uk/publications/api";
        public static XNamespace ATOM_NAMESPACE = "http://www.w3.org/2005/Atom";
        public virtual XmlNamespaceManager NamespaceManager
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
    }
}
