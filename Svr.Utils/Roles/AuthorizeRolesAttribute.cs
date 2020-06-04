using Microsoft.AspNetCore.Authorization;

namespace Svr.Utils.Roles
{
    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        public AuthorizeRolesAttribute(params string[] roles) : base()
        {
            Roles = string.Join(", ", roles);
        }
    }
}
