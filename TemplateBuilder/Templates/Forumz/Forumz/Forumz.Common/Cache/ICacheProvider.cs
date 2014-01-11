using System;
using System.Collections.Generic;
using System.Linq;

namespace Forumz.Common.Cache
{
    public interface ICacheProvider
    {
        void Add<T>(string key, T obj, TimeSpan cacheDuration) where T : class;
        void Add<T>(string key, T obj) where T : class;
        T Get<T>(string key) where T : class;
        IEnumerable<T> GetQuery<T>(IQueryable<T> query, params object[] keyValues);
        IEnumerable<T> GetQuery<T>(IQueryable<T> query, TimeSpan cacheDuration, params object[] keyValues);
        void Remove<T>(IQueryable<T> obj) where T : class;
        void Remove(string key);
    }
}