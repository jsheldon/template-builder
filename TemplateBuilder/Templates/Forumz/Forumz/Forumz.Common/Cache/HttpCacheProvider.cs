using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Forumz.Common.Cache
{
    public class HttpCacheProvider : ICacheProvider
    {
        private readonly TimeSpan m_DefaultTimeSpan = new TimeSpan(0, 30, 0);

        private static System.Web.Caching.Cache Cache
        {
            get
            {
                if (HttpRuntime.Cache == null)
                    throw new InvalidOperationException("HttpRuntime.Cache is null");

                return HttpRuntime.Cache;
            }
        }

        #region ICacheProvider Members

        public void Add<T>(string key, T obj, TimeSpan cacheDuration) where T : class
        {
            if (obj == null)
                return;

            Cache.Insert(key, obj, null, DateTime.MaxValue, cacheDuration);
        }

        public void Add<T>(string key, T obj) where T : class
        {
            Add(key, obj, m_DefaultTimeSpan);
        }

        public T Get<T>(string key) where T : class
        {
            return Cache[key] as T;
        }

        public IEnumerable<T> GetQuery<T>(IQueryable<T> query, params object[] keyValues)
        {
            return GetQuery(query, m_DefaultTimeSpan, keyValues);
        }

        public IEnumerable<T> GetQuery<T>(IQueryable<T> query, TimeSpan cacheDuration, params object[] keyValues)
        {
            var key = GetKey(query, keyValues);
            var items = Cache.Get(key) as List<T>;
            if (items == null)
            {
                items = query.ToList();
                Cache.Insert(key, items, null, DateTime.MaxValue, cacheDuration);
            }

            return (items);
        }

        public void Remove<T>(IQueryable<T> query) where T : class
        {
            var key = GetKey(query);
            Remove(key);
        }

        public void Remove(string key)
        {
            var obj = Cache[key];
            if (obj == null)
                return;
            Cache.Remove(key);
        }

        #endregion

        private static string GetKey<T>(IQueryable<T> query, params object[] keyValues)
        {
            return string.Concat(query.ToString(), "\n\r", typeof (T).AssemblyQualifiedName, "\n\rKeyValues: ", string.Join(",", keyValues));
        }
    }
}