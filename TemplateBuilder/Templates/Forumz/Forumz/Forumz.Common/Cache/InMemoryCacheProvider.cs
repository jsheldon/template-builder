using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Forumz.Common.Cache
{
    public class InMemoryCacheProvider : ICacheProvider
    {
        private static ConcurrentDictionary<string, object> m_Dictionary;
        private static InMemoryCacheProvider m_Instance;
        private static readonly object m_Locker = new object();
        private readonly TimeSpan m_DefaultTimeSpan = new TimeSpan(0, 30, 0);

        private InMemoryCacheProvider()
        {
        }

        #region ICacheProvider Members

        public void Add<T>(string key, T obj, TimeSpan cacheDuration) where T : class
        {
            m_Dictionary.TryAdd(key, obj);
        }

        public void Add<T>(string key, T obj) where T : class
        {
            m_Dictionary.TryAdd(key, obj);
        }

        public T Get<T>(string key) where T : class
        {
            return m_Dictionary[key] as T;
        }

        public IEnumerable<T> GetQuery<T>(IQueryable<T> query, params object[] keyValues)
        {
            return GetQuery(query, m_DefaultTimeSpan, keyValues);
        }

        public IEnumerable<T> GetQuery<T>(IQueryable<T> query, TimeSpan cacheDuration, params object[] keyValues)
        {
            var key = GetKey(query, keyValues);
            object obj;
            if (m_Dictionary.TryGetValue(key, out obj))
                return (List<T>) obj;

            var result = query.ToList();
            m_Dictionary.TryAdd(key, result);
            return result;
        }

        public void Remove<T>(IQueryable<T> obj) where T : class
        {
            var key = GetKey(obj);
            Remove(key);
        }

        public void Remove(string key)
        {
            object o;
            m_Dictionary.TryRemove(key, out o);
        }

        #endregion

        public static InMemoryCacheProvider GetInstance()
        {
            lock (m_Locker)
            {
                if (m_Dictionary == null)
                    m_Dictionary = new ConcurrentDictionary<string, object>();

                if (m_Instance == null)
                    m_Instance = new InMemoryCacheProvider();
            }

            return m_Instance;
        }

        private static string GetKey<T>(IQueryable<T> query, params object[] keyValues)
        {
            return string.Concat(query.ToString(), "\n\r", typeof (T).AssemblyQualifiedName, "\n\rKeyValues: ", string.Join(",", keyValues));
        }
    }
}