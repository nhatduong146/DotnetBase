namespace DotnetBase.Infrastructure.Mvc.Filters
{
    public class Policy
    {
        public static readonly string[] Policies =
        {
            ADMIN_ACCESS, USER_ACCESS
        };

        public const string ADMIN_ACCESS = "AdminAccess";
        public const string USER_ACCESS = "UserAccess";
    }
}
