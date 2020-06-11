using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCacheSample
{
    //Singleton Pattern
    public class MyCache
    {
        private static ObjectCache _cache = null;
        static readonly object locker = new object();

        //public static readonly CacheEntryUpdateCallback MyCacheUpdateCallback;

        private MyCache() { }
        

        public static ObjectCache CacheInstance
        {
            get {

                //if (_cache == null)
                //    _cache = MemoryCache.Default;

                //Thread Safe寫法
                if (_cache == null)
                    lock (locker)
                        if (_cache == null)
                            _cache = MemoryCache.Default;
                return _cache;
            }
        }

        //千萬不要這樣做....
        //static CacheItemPolicy _defaultPolicy = new CacheItemPolicy();
        //public static CacheItemPolicy DefaultPolicy
        //{
        //    get
        //    {
        //        _defaultPolicy.AbsoluteExpiration = DateTime.Now.AddSeconds(30d);
        //        return _defaultPolicy;
        //    }
        //}

        public static CacheItemPolicy DefaultPolicy
        {
            get
            {
                CacheItemPolicy _defaultPolicy = new CacheItemPolicy();
                _defaultPolicy.AbsoluteExpiration = DateTime.Now.AddSeconds(30d);
                return _defaultPolicy;
            }
        }

        static readonly Dictionary<string, CacheItemPolicy> Policys = new Dictionary<string, CacheItemPolicy>();
        public static void AddToPolicyStore(string key, CacheItemPolicy cacheItemPolicy)
        {
            Policys.Add(key, cacheItemPolicy);
        }

        public static void GetPolicyFromStore(string Key)
        {
            return Policys[key];
        }

    }
}
