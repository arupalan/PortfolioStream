using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using PortfolioStream;

namespace TestPortfolioStream
{
    [TestFixture]
    public class PortfolioStreamUnitTests
    {

        [Test]
        public async Task ReadAsyncThowsExceptionIfDataFileNotExist()
        {
            var portfolioParser = new PortfolioStream<TenorBucket>();
            var algo = new TenorParser();
            Assert.That( async () => await portfolioParser.ReadAsync(algo.ParseFunc,
                                    @"",
                                    Encoding.ASCII),
                                    Throws.TypeOf<FileNotFoundException>());
        }

        [Test]
        public async Task CanReadFileAsyncAndParseELements()
        {
            var portfolioParser = new PortfolioStream<TenorBucket>();
            var algo = new TenorParser();
            var result = await portfolioParser.ReadAsync(algo.ParseFunc, 
                                    @"Data.txt", 
                                    Encoding.ASCII);
            Assert.AreEqual(result.Count,36);
        }

        [Test]
        public async Task CanOrderByTenorThenByPortfolio()
        {
            var portfolioParser = new PortfolioStream<TenorBucket>();
            var algo = new TenorParser();
            var result = await portfolioParser.ReadAsync(algo.ParseFunc,
                                    @"Data.txt",
                                    Encoding.ASCII);
            Assert.AreEqual(result.Count, 36);

            var orderedBuckets = result.OrderBy(t => t.Tenor).ThenBy(p => p.PortfolioId);

            await portfolioParser.WriteAsync(orderedBuckets, 
                @"3.txt",
                Encoding.ASCII,
                b => string.Format("{0},{1},{2}", b.TenorStr, b.PortfolioId, b.Value));
        }

        [Test]
        public async Task CanOrderByPortfolioThenByTenor()
        {
            var portfolioParser = new PortfolioStream<TenorBucket>();
            var algo = new TenorParser();
            var result = await portfolioParser.ReadAsync(algo.ParseFunc,
                                    @"Data.txt",
                                    Encoding.ASCII);
            Assert.AreEqual(result.Count, 36);

            var orderedBuckets = result.OrderBy(p => Convert.ToInt32(p.PortfolioId)).ThenBy(t => t.Tenor);

            await portfolioParser.WriteAsync(orderedBuckets, 
                @"4.txt",
                Encoding.ASCII,
                b => string.Format("{0},{1},{2}", b.TenorStr, b.PortfolioId, b.Value));
        }

    }
}
