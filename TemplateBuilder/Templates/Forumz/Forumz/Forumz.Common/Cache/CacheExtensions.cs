using System.Collections.Generic;
using System.Linq;
using StructureMap;

namespace Forumz.Common.Cache
{
    public static class CacheExtensions
    {
        private static ICacheProvider m_CacheProvider;

        private static ICacheProvider CacheProvider
        {
            get
            {
                if (m_CacheProvider == null)
                    return m_CacheProvider = ObjectFactory.GetInstance<ICacheProvider>();

                return m_CacheProvider;
            }
        }

        public static IEnumerable<T> AsCacheable<T>(this IQueryable<T> query, params object[] keyValues)
        {
            return CacheProvider.GetQuery(query, keyValues);
        }
    }
}