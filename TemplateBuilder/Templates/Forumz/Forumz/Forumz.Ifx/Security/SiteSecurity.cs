using System;
using System.Globalization;
using System.Web;
using System.Web.Security;
using Forumz.Common.Crypto;
using Forumz.Common.Web;
using StructureMap;

namespace Forumz.Ifx.Security
{
    public class SiteSecurity
    {
        private const string FORM_COOKIE_NAME = "Forumz";
        private static SiteSecurity m_Instance;
        private readonly IHttpContextProvider m_ContextProvider;
        private readonly IUserManager m_UserManager;
        private ISymmetricCrypto m_SymmetricCrypto;

        public SiteSecurity(IUserManager userManager, IHttpContextProvider contextProvider, ISymmetricCrypto symmetricCrypto)
        {
            m_UserManager = userManager;
            m_ContextProvider = contextProvider;
            m_SymmetricCrypto = symmetricCrypto;
        }

        public static SiteSecurity Instance
        {
            get { return m_Instance ?? (m_Instance = ObjectFactory.GetInstance<SiteSecurity>()); }
        }

        public string CookieSeed
        {
            get { return string.Format("{0}-{1}", m_ContextProvider.Request.UserAgent, m_ContextProvider.Request.UserHostAddress); }
        }

        public void AuthenticateRequest()
        {
            try
            {
                var authTicket = GetTicket();
                if (authTicket == null || string.IsNullOrEmpty(authTicket.UserData))
                    return;

                var userstate = ParseUserState(authTicket.UserData);
                userstate = RefreshUserState(userstate);
                IssueAuthCookie(userstate);
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch (Exception)
            {
                // Cookie failed to decrypt.   Most likely the machineKey changed or something.
            }
        }

        public UserState GetCurrentUserState()
        {
            var u = m_ContextProvider.User as MvcPrincipal; // HttpContext.Current.User as MvcPrincipal;
            if (u == null)
                return null;

            return u.UserState;
        }

        public void IssueAuthCookie(UserState userState)
        {
            var ticket = new FormsAuthenticationTicket(1, userState.UserId.ToString(CultureInfo.InvariantCulture), DateTime.Now, DateTime.Now.AddDays(100), true, userState.ToString());
            SetTicket(ticket);
            var identity = new MvcIdentity(userState.UserId);
            var principal = new MvcPrincipal(identity, userState);
            m_ContextProvider.User = principal;
        }

        public void IssueAuthCookie(IUser user)
        {
            var userState = CreateUserState(user);
            IssueAuthCookie(userState);
        }

        private static UserState CreateUserState(IUser user)
        {
            return new UserState
                   {
                       UserId = user.Id,
                       Email = user.Email,
                       IsAdmin = user.IsAdmin,
                       LastCheck = DateTime.Now
                   };
        }

        private static UserState ParseUserState(string contents)
        {
            var idents = contents.Split(new[]
                                        {
                                            '|'
                                        }, StringSplitOptions.RemoveEmptyEntries);

            DateTime dt;
            if (!DateTime.TryParse(idents[4], out dt))
                dt = DateTime.Now.AddDays(-1);

            bool isAdmin;
            bool isModerator;

            if (!bool.TryParse(idents[2], out isModerator))
                isModerator = false;

            if (!bool.TryParse(idents[3], out isAdmin))
                isAdmin = false;

            var p = new UserState
                    {
                        Email = idents[0],
                        UserId = int.Parse(idents[1]),
                        IsModerator = isModerator,
                        IsAdmin = isAdmin,
                        LastCheck = dt
                    };

            return p;
        }

        private void SetTicket(FormsAuthenticationTicket ticket)
        {
            var ticketString = FormsAuthentication.Encrypt(ticket);
            var ticketValue = m_SymmetricCrypto.Encrypt(ticketString, CookieSeed);

            var cookie = new HttpCookie(FORM_COOKIE_NAME, ticketValue)
                         {
                             Expires = ticket.Expiration
                         };

            m_ContextProvider.Response.Cookies.Add(cookie);
        }

        private FormsAuthenticationTicket GetTicket()
        {
            var cookie = m_ContextProvider.Request.Cookies[FORM_COOKIE_NAME];
            if (cookie == null)
                return null;

            var cookieValue = m_SymmetricCrypto.Decrypt(cookie.Value, CookieSeed);
            return FormsAuthentication.Decrypt(cookieValue);
        }



        private UserState RefreshUserState(UserState userState)
        {
            if (DateTime.Now.Subtract(userState.LastCheck).Minutes < 5)
                return userState;
            var user = m_UserManager.GetUser(userState.UserId);
            return CreateUserState(user);
        }

        public void Signout()
        {
            // User Signout
            FormsAuthentication.SignOut();

            // Cookie Delete
            m_ContextProvider.Response.Cookies.Remove(FORM_COOKIE_NAME);

            // Remove Cache
            m_ContextProvider.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            m_ContextProvider.Response.Cache.SetExpires(DateTime.Now.AddMinutes(-10));
            m_ContextProvider.Response.Cache.SetNoStore();

            // Expire Auth Cookie
            var ticketExpiration = DateTime.Now.AddDays(-7);
            var cookie = new HttpCookie(FORM_COOKIE_NAME)
                         {
                             Expires = ticketExpiration,
                             HttpOnly = true
                         };

            m_ContextProvider.Response.Cookies.Add(cookie);
        }
    }
}