using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Xml;

namespace Symplectic.Spark3.Library
{
    public abstract class XmlUtilities : SparkNamespaceManager
    {
        public static XDocument LoadXmlFile(string filepath)
        {
            if (File.Exists(filepath))
            {
                using (FileStream fs = File.Open(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (XmlTextReader xtr = new XmlTextReader(fs))
                {
                    return XDocument.Load(xtr, LoadOptions.None);
                }
            }
            else
                throw new FileNotFoundException("Could not find file", filepath);
        }

        public static void SaveXmlFile(XDocument xdoc, string filepath)
        {
            using (FileStream fs = File.Open(filepath, FileMode.Create, FileAccess.Write, FileShare.Read))
            using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
            {
                xdoc.Save(sw, SaveOptions.None);
            }
        }
    }
}
