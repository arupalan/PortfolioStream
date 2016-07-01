using System;
using System.Text.RegularExpressions;

namespace PortfolioStream
{
    public class TenorParser
    {
        const string tenorPattern = @"([1-9]{1,}[ymwd])+";
        private static Regex tenorRegex = new Regex(tenorPattern);

        public Func<string, TenorBucket> ParseFunc = s =>
        {
            if (s.Length == 0 || s.Equals("tenor, portfolioid, value")) return null;
            var arr = s.Split(',');

            var bucket = new TenorBucket {TenorStr = arr[0], PortfolioId = arr[1], Value = Convert.ToDouble(arr[2])};

            MatchCollection allMatches = tenorRegex.Matches(arr[0]);

            if(allMatches.Count == 0) return null;
            var dt = DateTime.Now;
            foreach (Capture capture in allMatches[0].Groups[1].Captures)
            {
                int value = Convert.ToInt32(capture.Value.Substring(0, capture.Value.Length - 1));
                string token = capture.Value.Substring(capture.Value.Length - 1);

                switch (token)
                {
                    case @"y":
                        dt = dt.AddYears(value);
                        break;

                    case @"m":
                        dt = dt.AddMonths(value);
                        break;

                    case @"w":
                        dt = dt.AddDays(value*7);
                        break;

                    case @"d":
                        dt = dt.AddDays(value);
                        break;
                }
            }

            bucket.Tenor = dt - DateTime.Now;
            return bucket;
        };

    }
}
