﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dongbo.Configuration
{
    public class BaseConfig<T> where T : new()
    {
        private static T instance = ConfigManager.GetSection<T>();
        public static T Instance
        {
            get { return instance; }
            set
            {
                instance = value;
                OnConfigChanged();
            }
        }

        public static T GetConfig(EventHandler reloadEventHandler)
        {
            ConfigChanged += reloadEventHandler;
            return Instance;
        }

        public static event EventHandler ConfigChanged;
        public static void OnConfigChanged()
        {
            if (ConfigChanged != null)
                ConfigChanged(Instance, EventArgs.Empty);
        }
    }
}
