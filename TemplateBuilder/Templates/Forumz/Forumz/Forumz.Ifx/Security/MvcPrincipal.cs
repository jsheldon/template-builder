using System.Security.Principal;

namespace Forumz.Ifx.Security
{
    public class MvcPrincipal : GenericPrincipal
    {
        public MvcPrincipal(IIdentity identity, UserState userState)
            : base(identity, userState.Roles)
        {
            UserState = userState;
            UserId = userState.UserId;
        }

        public int UserId { get; set; }

        public UserState UserState { get; set; }

        /// <summary>
        ///     Determines whether the current principal belongs to the specified role.
        /// </summary>
        /// <returns>
        ///     true if the current principal is a member of the specified role; otherwise, false.
        /// </returns>
        /// <param name = "role">The name of the role for which to check membership. 
        /// </param>
        public override bool IsInRole(string role)
        {
            if (string.IsNullOrEmpty(role))
                return false;

            switch (role.ToLower())
            {
                case "admin":
                    return UserState.IsAdmin;
                case "moderator":
                    return UserState.IsModerator;
                case "user":
                    return UserState.UserId != 0;
                default:
                    return false;
            }
        }
    }
}