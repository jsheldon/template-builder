using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web;

namespace Forumz.Common.Web
{
    public class NoContextProvider : IHttpContextProvider
    {
        #region Fields

        private readonly IDictionary m_Items = new Dictionary<string, object>();

        #endregion

        #region IHttpContextProvider Members

        public IDictionary Items
        {
            get { return m_Items; }
        }

        public HttpRequestBase Request
        {
            get { throw new NotImplementedException(); }
        }

        public HttpResponse Response
        {
            get { throw new NotImplementedException(); }
        }

        public IPrincipal User
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        #endregion
    }
}