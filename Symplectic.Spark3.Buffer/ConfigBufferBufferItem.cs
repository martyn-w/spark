using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Spark3.Buffer
{
    #region ConfigBufferBufferItem : ConfigurationElement
    public class ConfigBufferBufferItem : ConfigurationElement
    {
        [ConfigurationProperty("select-elements", IsRequired = true)]
        public string SelectElements
        {
            get
            {
                return this["select-elements"] as string;
            }
        }

        [ConfigurationProperty("select-buffered-element", IsRequired = true)]
        public string SelectBufferedElement
        {
            get
            {
                return this["select-buffered-element"] as string;
            }
        }

        [ConfigurationProperty("select-api-relative-uri", IsRequired = true)]
        public string SelectApiRelativeUri
        {
            get
            {
                return this["select-api-relative-uri"] as string;
            }
        }

        [ConfigurationProperty("api-use-modified-since", IsRequired = false, DefaultValue = false)]
        public bool ApiUseModifiedSince
        {
            get
            {
                return (bool)this["api-use-modified-since"];
            }
        }

        [ConfigurationProperty("api-per-page", IsRequired = false, DefaultValue = 100)]
        public int ApiPerPage
        {
            get
            {
                return (int)this["api-per-page"];
            }
        }

        [ConfigurationProperty("select-filename", IsRequired = true)]
        public string SelectFilename
        {
            get
            {
                return this["select-filename"] as string;
            }
        }
    }
    #endregion
}

