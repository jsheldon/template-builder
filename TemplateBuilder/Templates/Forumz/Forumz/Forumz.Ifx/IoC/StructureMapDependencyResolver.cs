using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using StructureMap;

namespace Forumz.Ifx.IoC
{
    public class StructureMapDependencyResolver : IDependencyResolver
    {
        #region Fields

        private readonly IContainer m_Container;

        #endregion

        #region Constructors

        public StructureMapDependencyResolver(IContainer container)
        {
            m_Container = container;
        }

        #endregion

        #region IDependencyResolver Members

        public object GetService(Type serviceType)
        {
            if (serviceType == null)
                return null;
            try
            {
                if (serviceType.IsAbstract || serviceType.IsInterface)
                    return m_Container.TryGetInstance(serviceType);
                return m_Container.GetInstance(serviceType);
            }
            catch
            {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return m_Container.GetAllInstances<object>().Where(s => s.GetType() == serviceType);
        }

        #endregion
    }
}