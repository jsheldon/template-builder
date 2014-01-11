using System;
using System.Collections.Generic;

namespace Forumz.Ifx.Security
{
    public class UserState
    {
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsModerator { get; set; }
        public DateTime LastCheck { get; set; }

        public string[] Roles
        {
            get
            {
                var l = new List<string> { "user" };
                if (IsAdmin)
                    l.Add("admin");
                if (IsModerator)
                    l.Add("moderator");
                return l.ToArray();
            }
        }

        public int UserId { get; set; }

        public override string ToString()
        {
            return string.Join("|", Email, UserId, IsModerator, IsAdmin, LastCheck);
        }
    }
}