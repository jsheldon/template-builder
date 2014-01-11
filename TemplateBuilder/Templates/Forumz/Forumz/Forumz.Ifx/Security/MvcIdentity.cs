using System.Security.Principal;

namespace Forumz.Ifx.Security
{
    public class MvcIdentity : IIdentity
    {
        public MvcIdentity(int userId)
        {
            UserId = userId;
            IsAuthenticated = true;
        }

        public int UserId { get; set; }

        #region IIdentity Members

        /// <summary>
        ///     Gets the type of authentication used.
        /// </summary>
        /// <returns>
        ///     The type of authentication used to identify the user.
        /// </returns>
        public string AuthenticationType
        {
            get { return "Forms"; }
        }

        /// <summary>
        ///     Gets a value that indicates whether the user has been authenticated.
        /// </summary>
        /// <returns>
        ///     true if the user was authenticated; otherwise, false.
        /// </returns>
        public bool IsAuthenticated { get; internal set; }

        /// <summary>
        ///     Gets the name of the current user.
        /// </summary>
        /// <returns>
        ///     The name of the user on whose behalf the code is running.
        /// </returns>
        public string Name
        {
            get { return UserId.ToString(); }
        }

        #endregion
    }
}