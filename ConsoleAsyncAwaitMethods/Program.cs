using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleAsyncAwaitMethods
{
    class Program
    {
        public static void App(Task<string> data)
        {
            Console.WriteLine(data.Result.Length);
        }

        public class Site
        {
            public int Length { get; set; }
            public string Url { get; set; }
            public string Content { get; set; }
        }

        public static async Task<Site> GetSiteAsync(string url)
        {
            Site site = new Site();
            var data = await new HttpClient().GetStringAsync(url);

            await Task.Delay(10000);
            site.Url = url;
            site.Content = data;
            site.Length = data.Length;
            Console.WriteLine("Thread Id: " + Thread.CurrentThread.ManagedThreadId);

            return site;

        }
        public class Status
        {
            public int ThreadID { get; set; }
            public DateTime Date { get; set; }
        }

        // Result used when used asynchronous method in a synchronous method  and returned a value from the synchronous method
        public static string GetData()
        {
            var task = new HttpClient().GetStringAsync("https://www.google.com");

            return task.Result;
        }
        public static string CachedData { get; set; }

        public static Task<string> GetDataAsync()
        {
            if (string.IsNullOrEmpty(CachedData))
            {
                return File.ReadAllTextAsync("File.txt");
            }
            else
            {

                return Task.FromResult(CachedData);
            }
        }
        static async Task Main(string[] args)
        {
            //Console.WriteLine(GetData());
            #region FromResult return an object as asynchronously
            //CachedData = await GetDataAsync();
            //Console.WriteLine(CachedData);
            //CachedData = await GetDataAsync();
            //Console.WriteLine(CachedData);
            #endregion

            #region StartNew like Run method runs the code in another thread, difference from run method gets and returns parameters
            Console.WriteLine("Main Thread: " + Thread.CurrentThread.ManagedThreadId);
            var myTask = Task.Factory.StartNew((obj) =>
            {
                Console.WriteLine("MyTask running");
                var status = obj as Status;
                status.ThreadID = Thread.CurrentThread.ManagedThreadId;
            }, new Status() { Date = DateTime.Now });
            await myTask;
            Status s = myTask.AsyncState as Status;
            Console.WriteLine("LASTCODE");
            Console.WriteLine(s.ThreadID + "-" + s.Date.ToString());
            #endregion


            #region ContinueWith 
            //Console.WriteLine("Hello World!");
            ////var myTask = new HttpClient().GetStringAsync("https://www.google.com").ContinueWith(App);
            //var myTask = new HttpClient().GetStringAsync("https://www.trendyol.com").ContinueWith( async data =>
            //{
            //    Console.WriteLine("1"+ data.Result.Length);
            //    var data1 =  GetDataAsync();
            //    Console.WriteLine("2" + data1.Result);
            //});
            //Console.WriteLine("adım1");
            //await myTask;
            #endregion

            Console.WriteLine("Main Thread: " + Thread.CurrentThread.ManagedThreadId);
            List<string> urls = new List<string>()
            {
                "https://www.google.com",
                "https://www.microsoft.com",
                "https://www.sozcu.com",
                "https://www.amazon.com",
                "https://www.trendyol.com"
            };

            List<Task<Site>> taskList = new List<Task<Site>>();

            urls.ForEach(x =>
            {
                taskList.Add(GetSiteAsync(x));
            });

            #region WhenAll waits all tasks without non blocking current thread
            //kullanım1
            //Console.WriteLine("WhenAll öncesi");
            //var sites = await Task.WhenAll(taskList.ToArray());
            //Console.WriteLine("WhenAll sonrası");
            //sites.ToList().ForEach(x =>
            //{
            //    Console.WriteLine($"{x.Url} length:{x.Length}");
            //});

            //kullanım2
            //var sites = Task.WhenAll(taskList.ToArray());
            //Console.WriteLine("WhenAll sonrası");
            //var data = await sites;
            //data.ToList().ForEach(x =>
            //{
            //    Console.WriteLine($"{x.Url} length:{x.Length}");
            //});
            #endregion


            #region WhenAny  used for get first finished task 
            //var firstData = await Task.WhenAny(taskList);

            //Console.WriteLine($"{firstData.Result.Url} - {firstData.Result.Length}");
            #endregion

            #region WaitAll runs like WhenAll but blocks the current thread
            Console.WriteLine("WaitAll öncesi");
            Task.WaitAll(taskList.ToArray());
            Console.WriteLine("WaitAll sonrası");
            Console.WriteLine($"{taskList.First().Result.Url} - {taskList.First().Result.Length}");
            #endregion

            #region WaitAny returns first index of finished task, blocks the current thread
            //Console.WriteLine("WaitAny öncesi");
            //var first = Task.WaitAny(taskList.ToArray());
            //Console.WriteLine($"{taskList[first].Result.Url} - {taskList[first].Result.Length}");
            #endregion
        }
    }
}
