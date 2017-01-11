﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.Redis.Rate
{
    public interface IGranularityProvider
    {
        Granularity[] GetGranularities();
        bool RegisterGranularity(Granularity granularity);
    }
    public class DefaultGranularityProvider : IGranularityProvider
    {
        private List<Granularity> Granularities { get; set; }

        public DefaultGranularityProvider()
        {
            this.Granularities = new List<Granularity>();
            this.Granularities.Add(Granularity.Second);
            this.Granularities.Add(Granularity.Minute);
            this.Granularities.Add(Granularity.Hour);
            this.Granularities.Add(Granularity.Day);
            this.Granularities.Add(Granularity.AllTime);
        }

        public Granularity[] GetGranularities()
        {
            return Granularities.ToArray();
        }


        public bool RegisterGranularity(Granularity granularity)
        {
            if (granularity.Ttl <= 0 || granularity.Factor <= 0 || string.IsNullOrEmpty(granularity.Name))
            {
                throw new ArgumentException("granularity is not valid");
            }

            if (this.Granularities.Exists(g => g.Factor == granularity.Factor))
            {
                //throw new InvalidOperationException("a granularity with same factor & size is laready registered.");
                return false;
            }

            this.Granularities.Add(granularity);
            return true;
        }
    }
}
