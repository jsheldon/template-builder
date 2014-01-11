using System.Collections;
using System.Security.Principal;
using System.Web;

namespace Forumz.Common.Web
{
    public interface IHttpContextProvider
    {
        #region Properties

        IDictionary Items { get; }
        HttpRequestBase Request { get; }
        HttpResponse Response { get; }
        IPrincipal User { get; set; }

        #endregion
    }
}