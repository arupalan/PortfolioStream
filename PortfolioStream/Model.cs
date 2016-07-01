using System;

namespace PortfolioStream
{
    public class TenorBucket
    {
        public string TenorStr { get; set; }
        public TimeSpan Tenor { get; set; }
        public string   PortfolioId { get; set; }
        public double Value { get; set; }
    }
}
