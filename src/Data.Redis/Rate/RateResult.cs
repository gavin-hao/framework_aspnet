using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.Redis.Rate
{
    public class RateResult
    {
        public string EventSource { get; private set; }
        public double? Score { get; private set; }
        public long? Rank { get; private set; }

        public RateResult(string eventSource, double? score, long? rank)
        {
            this.EventSource = eventSource;
            this.Score = score;
            this.Rank = rank;
        }
    }
}
