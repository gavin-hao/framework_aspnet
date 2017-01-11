using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.Redis.Rate
{

    interface IRateContext
    {
        Granularity[] Granularities { get; }
        string Namespace { get; }
        //int DbIndex { get; }
        IKeyFactory KeyFactory { get; }
    }
    internal class RateContext : IRateContext
    {
        public string Namespace { get; private set; }
        public Granularity[] Granularities { get; private set; }
        public IKeyFactory KeyFactory { get; private set; }
        public RateContext(string @namespace, Granularity[] granularities)
        {
            this.Namespace = @namespace;
            this.KeyFactory = new DefaultKeyFactory(this.Namespace);
            this.Granularities = granularities != null && granularities.Length == 0 ? new DefaultGranularityProvider().GetGranularities() : granularities;
        }
    }

}
