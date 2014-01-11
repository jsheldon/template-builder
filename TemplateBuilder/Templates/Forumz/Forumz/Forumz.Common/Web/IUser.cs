namespace Forumz.Common.Web
{
    public interface IUser
    {
        int Id { get; set; }
        string Email { get; set; }
        bool IsAdmin { get; set; }
    }

    public interface IUserManager
    {
        IUser GetUser(int id);
    }
}