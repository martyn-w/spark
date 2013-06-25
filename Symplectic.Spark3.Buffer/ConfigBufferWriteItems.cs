using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Spark3.Buffer
{
    #region ConfigBufferWriteItems : ConfigurationElement
    public class ConfigBufferWriteItems : ConfigurationElement
    {
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

