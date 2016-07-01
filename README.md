# PortfolioStream
This is an attempt to showcase functional injection using Func in C# so as to facilitate SOLID SRP and IOC.
Here I attempt to separate business CPU intensive algorithms from other functionalities like ReadWrite to disk.
AsyncReadWrite to disk structurally uses Windows I/O ports without ANY any need for CPU threads. 

You don't really need to have a console app to see if the code works or not. Infact Nunit-tests should suffice. I shall show you the steps 
as to how you can achive the tests right from code even if you have no visual studio installed or anything installed on a plain commodity pc.
Infact I mostly work on mac and I do not have Visual Studio on my windows server.

# Building From Source
1. Move to your local git repository directory or any directory (with git init) in console.

2. Clone repository.

        git clone https://github.com/arupalan/PortfolioStream
        cd PortfolioStream
        
3. Check path of msbuild
        reg.exe query "HKLM\SOFTWARE\Microsoft\MSBuild\ToolsVersions\4.0" /v MSBuildToolsPath
        
4. Now get all nuget packages
        nuget restore PortfolioStream.sln
        
5. Next build the app (Note the path of msbuild above incase its not available to you)
        msbuild PortfolioStream.sln
        
# Testing the app
       cd TestPortfolioStream\bin\Debug
       nunit3-console TestPortfolioStream.dll
       
       You may see errors like below. But don't worry its doing its job as expected .... its just because the data.txt file was not there.
       Overall result: Failed                                            
       Test Count: 4, Passed: 1, Failed: 3, Inconclusive: 0, Skipped: 0  
       Failed Tests - Failures: 0, Errors: 3, Invalid: 0   
       
       after you have copied the data.txt file there you can run the tests again. You will also see the 3.txt and 4.txt
       copy ..\..\Data.txt
       nunit3-console TestPortfolioStream.dll
       
       Test Run Summary                                                     
       Overall result: Passed                                             
       Test Count: 4, Passed: 4, Failed: 0, Inconclusive: 0, Skipped: 0   
       Start time: 2016-07-01 13:11:40Z                                   
       End time: 2016-07-01 13:11:40Z                                   
       Duration: 0.165 seconds                                          
       
## Sample Mock functional injection:
```csharp
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
```

## Sample Mock Exception testing
```c#
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
```
## Test and Coverage
* You can execute with switch -console to see the logs on console
 ![Console Mode](http://www.alanaamy.net/wp-content/uploads/2016/07/Tests.png)

## Outout 3.txt
*  ![Console Mode](http://www.alanaamy.net/wp-content/uploads/2016/07/3.txt)

## Outout 4.txt
*  ![Console Mode](http://www.alanaamy.net/wp-content/uploads/2016/07/4.txt)