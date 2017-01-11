﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dongbo.Configuration
{
    /// <summary>
    /// Keep major version compatible
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ConfigurationVersionAttribute : Attribute
    {
        public ConfigurationVersionAttribute()
        {
            majorVersion = 1;
        }

        public ConfigurationVersionAttribute(int majorVersion)
        {
            this.majorVersion = majorVersion;
        }

        private int majorVersion;


        /// <summary>
        /// this is major version!!
        /// </summary>
        public int MajorVersion
        {
            get { return majorVersion; }
            set { majorVersion = value; }
        }
    }
}
