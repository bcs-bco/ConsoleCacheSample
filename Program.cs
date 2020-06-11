using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ConsoleCacheSample
{
    class Program
    {
        static void Main(string[] args)
        {
            //ObjectCache cache = MemoryCache.Default;
            //CacheItemPolicy policy = new CacheItemPolicy();
            //policy.AbsoluteExpiration = DateTime.Now.AddSeconds(10d);
            //policy.SlidingExpiration = TimeSpan.FromSeconds(3);

            //cache["glenn"] = "ABCDFG";
            //cache.Set("glenn", "glenn.chen", policy);
            //bool isAdd = cache.Add("glenn", "aries.lu", policy);
            //Console.WriteLine($"IsAdd? {isAdd.ToString()}");
            //while (true)
            //{
            //    Thread.Sleep(1000);
            //    Console.WriteLine($"({DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")})\tCache value:\t{cache["glenn"]}");

            //    if (!cache.Contains("glenn")) break;

            //}
            //===========================================================
            var cache = MyCache.CacheInstance;
            var policy = MyCache.DefaultPolicy;
            //MyCache.DefaultPolicy = new CacheItemPolicy();
            policy.AbsoluteExpiration = DateTime.Now.AddSeconds(30d);
            var books = WriteFileCache(policy);
            cache.Set(books, policy);

            //這兩個只能保留一個
            policy.UpdateCallback = (x) => { cache.Remove("books"); Console.WriteLine("===books update==="); };
            //policy.RemovedCallback = (x) => Console.WriteLine("===books removed===");

            while (true)
            {
                var mybooks = (List<XElement>)cache["books"];
                foreach (var book in mybooks)
                {
                    Console.WriteLine($"{book.Attribute("id").Value}\t{book.Element("author").Value}\t{book.Element("name").Value}");
                }
                Thread.Sleep(1000);
                if (!cache.Contains("books")) break;
            }


            Console.WriteLine("Done!!!");
            Console.ReadKey();
        }

        static CacheItem WriteFileCache(CacheItemPolicy policy)
        {
            string rootPath = AppDomain.CurrentDomain.BaseDirectory;
            string dataPath = Path.Combine(rootPath, "Data", "data.xml");
            var books = XDocument.Load(dataPath);

            //實體檔案異動時回收快取
            policy.ChangeMonitors.Add(new HostFileChangeMonitor(new List<string>() { dataPath }));

            //DB檔案異動時回收快取
            //policy.ChangeMonitors.Add(new SqlChangeMonitor(null));

            //繼承實作 ChangeMonitor 類別來建立獨有的監控邏輯
            //policy.ChangeMonitors.Add(new MyChangeMonitor());

            return new CacheItem("books", books.Root.Elements().ToList());

        }


    }

    public class MyChangeMonitor : ChangeMonitor
    {
        public override string UniqueId => throw new NotImplementedException();

        protected override void Dispose(bool disposing)
        {
           
        }
    }
}
