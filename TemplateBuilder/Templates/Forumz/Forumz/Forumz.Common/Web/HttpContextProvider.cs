using System.Collections;
using System.Security.Principal;
using System.Web;

namespace Forumz.Common.Web
{
    public class HttpContextProvider : IHttpContextProvider
    {
        #region IHttpContextProvider Members

        public IDictionary Items
        {
            get
            {
                if (HttpContext.Current == null)
                    return null;
                return HttpContext.Current.Items;
            }
        }

        public HttpRequestBase Request
        {
            get
            {
                if (HttpContext.Current == null)
                    return null;
                return new HttpRequestWrapper(HttpContext.Current.Request);
            }
        }

        public HttpResponse Response
        {
            get
            {
                if (HttpContext.Current == null)
                    return null;
                return HttpContext.Current.Response;
            }
        }

        public IPrincipal User
        {
            get
            {
                if (HttpContext.Current == null)
                    return null;
                return HttpContext.Current.User;
            }
            set
            {
                if (HttpContext.Current != null)
                    HttpContext.Current.User = value;
            }
        }

        #endregion
    }
}