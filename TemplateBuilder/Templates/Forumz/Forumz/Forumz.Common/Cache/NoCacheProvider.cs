using System;
using System.Collections.Generic;
using System.Linq;

namespace Forumz.Common.Cache
{
    public class NoCacheProvider : ICacheProvider
    {
        #region ICacheProvider Members

        public void Add<T>(string key, T obj, TimeSpan cacheDuration) where T : class
        {
        }

        public void Add<T>(string key, T obj) where T : class
        {
        }

        public T Get<T>(string key) where T : class
        {
            return default(T);
        }

        public IEnumerable<T> GetQuery<T>(IQueryable<T> query, params object[] keyValues)
        {
            return query.ToList();
        }

        public IEnumerable<T> GetQuery<T>(IQueryable<T> query, TimeSpan cacheDuration, params object[] keyValues)
        {
            return query.ToList();
        }

        public void Remove<T>(IQueryable<T> obj) where T : class
        {
        }

        public void Remove(string key)
        {
        }

        #endregion
    }
}