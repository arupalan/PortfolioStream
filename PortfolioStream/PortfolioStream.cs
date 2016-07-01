using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioStream
{

    public interface IPortfolioStream<TR>
    {
        Task<List<TR>> ReadAsync(Func<string, TR> parserFunc, string filename, Encoding encoding);
        Task WriteAsync(IEnumerable<TR> tenorBuckets, string filename, Encoding encoding, Func<TR, string> writeformatFunc);
    }

    public class PortfolioStream<TR> : IPortfolioStream<TR> 
    {
        /// <summary>
        /// ReadLineAsync uses Windows I/O ports to await this without ANY CPU threads
        /// </summary>
        /// <param name="parserFunc"></param>
        /// <param name="filename"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public async Task<List<TR>> ReadAsync(Func<string,TR> parserFunc, string filename, Encoding encoding)
        {
            if (!File.Exists(filename)) throw new FileNotFoundException(filename);
            var buckets = new List<TR>();
            using (var reader = new StreamReader(filename, encoding))
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    var bucket = parserFunc(line);
                    if (bucket != null) buckets.Add(bucket);
                }
            }
            return buckets;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="encoding"></param>
        /// <param name="writeformatFunc"></param>
        /// <param name="tenorBuckets"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public async Task WriteAsync(IEnumerable<TR> tenorBuckets,string filename, Encoding encoding, Func<TR, string> writeformatFunc)
        {
            using (var writer = new StreamWriter(filename, false,encoding))
            {
                await writer.WriteLineAsync("tenor, portfolioid, value");
                foreach (var bucket in tenorBuckets)
                {
                    await writer.WriteLineAsync(writeformatFunc(bucket));
                }
            }
        }
    }
}