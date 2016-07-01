using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PortfolioStream;

namespace ParserConsoleApp
{
    class Program
    {
        public static void Main()
        {
            //try
            //{
            //    Task.WaitAll(
            //        Task.Factory.StartNew( () => { OrderByTenorThenByPortfolio(); }), 
            //        Task.Factory.StartNew( () => { OrderByPortfolioThenByTenor(); })
            //        );

            //    Console.WriteLine("Please check files 3.txt and 4.txt have been generated");

            //}
            //catch (AggregateException e)
            //{
            //    foreach (Exception t in e.InnerExceptions)
            //    {
            //        Console.WriteLine("\n-------------------------------------------------\n{0}", 
            //            t.ToString());
            //    }
            //    ;
            //}

            Task innerTask1 = Task.Factory.StartNew(async () => { await OrderByTenorThenByPortfolio(); });
            Task innerTask2 = Task.Factory.StartNew(async () => { await OrderByPortfolioThenByTenor(); });

            var task = Task.Factory.ContinueWhenAll(
                new[] { innerTask1, innerTask2 },
                innerTasks =>
                {
                    Task.WaitAll(innerTasks);

                    foreach (var innerTask in innerTasks)
                    {
                        Console.WriteLine("Result: {0}", innerTask.Status);
                    }
                });
            var exceptionTask = task.ContinueWith(
                t => Console.Error.WriteLine("Outer Task exception: {0}", t.Exception),
                TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously);

            try
            {
                task.Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.WriteLine("Done - Outer Task Status: {0}, Exception Task: {1}", task.Status, exceptionTask.Status);
            Console.ReadLine();





        }

        public static async Task OrderByTenorThenByPortfolio()
        {
            var portfolioParser = new PortfolioStream.PortfolioStream<TenorBucket>();
            var algo = new TenorParser();
            var result = await portfolioParser.ReadAsync(algo.ParseFunc,
                                    @"Data.txt",
                                    Encoding.ASCII);

            var orderedBuckets = result.OrderBy(t => t.Tenor).ThenBy(p => p.PortfolioId);

            await portfolioParser.WriteAsync(orderedBuckets,
                @"3.txt",
                Encoding.ASCII,
                b => string.Format("{0},{1},{2}", b.TenorStr, b.PortfolioId, b.Value));
        }

        public static async Task OrderByPortfolioThenByTenor()
        {
            var portfolioParser = new PortfolioStream<TenorBucket>();
            var algo = new TenorParser();
            var result = await portfolioParser.ReadAsync(algo.ParseFunc,
                                    @"Data.txt",
                                    Encoding.ASCII);

            var orderedBuckets = result.OrderBy(p => Convert.ToInt32(p.PortfolioId)).ThenBy(t => t.Tenor);

            await portfolioParser.WriteAsync(orderedBuckets,
                @"4.txt",
                Encoding.ASCII,
                b => string.Format("{0},{1},{2}", b.TenorStr, b.PortfolioId, b.Value));
        }


    }
} // END Program