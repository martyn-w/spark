//  Symplectic Spark
//  Copyright 2010 Symplectic Ltd
//  Created by Martyn Whitwell (martyn@symplectic.co.uk)

//  This file is part of Spark.

//  Spark is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.

//  Spark is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.

//  You should have received a copy of the GNU General Public License
//  along with Spark.  If not, see <http://www.gnu.org/licenses/>.

// $URL: https://symplectic-spark.googlecode.com/svn/trunk/Symplectic.Spark.Rebuffer/ConfigBufferSettings.cs $
// $LastChangedDate: 2010-06-09 11:38:27 +0100 (Wed, 09 Jun 2010) $
// $LastChangedRevision: 52 $
// $LastChangedBy: martyn@symplectic.co.uk $


using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Xml;
using System.Xml.Linq;
using Spark3.Buffer;

namespace Spark3.Rebuffer
{

    /*
      
    <buffers>

    <add name="keele-people">
      
        <settings buffer-mode="BufferAllItems" />
        
        <sourceItems select-element="api:object" 
                     api-relative-uri="objects?categories=users&amp;modified-since=2010-03-15T23%3A59%3A00Z&amp;detail=full"
                     api-use-modified-since="false"
                     api-per-page="100" />
     
        <bufferItems select-filename="'people\index.xml'" />
        
        <bufferItem select-element="api:object"
                    select-api-relative-uri="concat('users/',@id,'/relationships?detail=full')"
                    api-use-modified-since="false"
                    api-per-page="100"
                    select-filename="concat('people\',@proprietary-id,'.xml')" />
      </add>
     * 
     * 
    </buffers>
     */



    #region ConfigBufferSettings : ConfigurationSection
    public class ConfigBufferSettings : ConfigurationSection
    {
        public static ConfigBufferSettings Config
        {
            get
            {
                return ConfigurationManager.GetSection("bufferSettings") as ConfigBufferSettings;
            }
        }


        [ConfigurationProperty("buffers", IsRequired = false)]
        public ConfigBuffers Buffers
        {
            get
            {
                return this["buffers"] as ConfigBuffers;
            }
        }
    }
    #endregion

    #region ConfigBuffers : ConfigurationElementCollection
    public class ConfigBuffers : ConfigurationElementCollection
    {

        public ConfigBuffer this[int index]
        {
            get
            {
                return base.BaseGet(index) as ConfigBuffer;
            }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ConfigBuffer();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ConfigBuffer)element).Name;
        }
    }
    #endregion

    #region ConfigBuffer : ConfigurationElement
    public class ConfigBuffer : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get
            {
                return this["name"] as string;
            }
        }
            
        [ConfigurationProperty("settings", IsRequired = true)]
        public ConfigBufferBufferSettings Settings
        {
            get
            {
                return this["settings"] as ConfigBufferBufferSettings;
            }
        }


        [ConfigurationProperty("sourceItems", IsRequired = false)]
        public ConfigBufferSourceItems SourceItems
        {
            get
            {
                return this["sourceItems"] as ConfigBufferSourceItems;
            }
        }

        [ConfigurationProperty("bufferAllItems", IsRequired = false)]
        public ConfigBufferBufferItem BufferAllItems
        {
            get
            {
                return this["bufferAllItems"] as ConfigBufferBufferItem;
            }
        }

        [ConfigurationProperty("bufferItem", IsRequired = false)]
        public ConfigBufferBufferItem BufferItem
        {
            get
            {
                return this["bufferItem"] as ConfigBufferBufferItem;
            }
        }

        [ConfigurationProperty("writeItems", IsRequired = false)]
        public ConfigBufferWriteItems WriteItems
        {
            get
            {
                return this["writeItems"] as ConfigBufferWriteItems;
            }
        }


        //<deleteItems select-filename="concat('person\',@proprietary-id,'.xml')" />
        [ConfigurationProperty("deleteItems", IsRequired = false)]
        public ConfigBufferWriteItems DeleteItems
        {
            get
            {
                return this["deleteItems"] as ConfigBufferWriteItems;
            }
        }
    }
    #endregion

    #region ConfigBufferBufferSettings : ConfigurationElement
    public class ConfigBufferBufferSettings : ConfigurationElement
    {
        public enum enBufferMode
        {
            BufferAllItems, BufferItem, WriteItems, PostProcess, DeleteItems
        }
        
        [ConfigurationProperty("buffer-mode", IsRequired = true)]
        public enBufferMode BufferMode
        {
            get
            {
                return (enBufferMode)this["buffer-mode"];
            }
        }

        [ConfigurationProperty("administration-filename", IsRequired = false)]
        public string AdministrationFilename
        {
            get
            {
                return this["administration-filename"] as string;
            }
        }

        [ConfigurationProperty("input-filename", IsRequired = false)]
        public string InputFilename
        {
            get
            {
                return this["input-filename"] as string;
            }
        }

        [ConfigurationProperty("output-filename", IsRequired = false)]
        public string OutputFilename
        {
            get
            {
                return this["output-filename"] as string;
            }
        }

    }
    #endregion


    #region ConfigBufferSourceItems : ConfigurationElement
    public class ConfigBufferSourceItems : ConfigurationElement
    {
        [ConfigurationProperty("select-elements", IsRequired = true)]
        public string SelectElements
        {
            get
            {
                return this["select-elements"] as string;
            }
        }

        [ConfigurationProperty("api-relative-uri", IsRequired = true)]
        public string ApiRelativeUri
        {
            get
            {
                return this["api-relative-uri"] as string;
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

        [ConfigurationProperty("api-use-paging", IsRequired = false, DefaultValue = true)]
        public bool ApiUsePaging
        {
            get
            {
                return (bool)this["api-use-paging"];
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
    }
    #endregion

}

