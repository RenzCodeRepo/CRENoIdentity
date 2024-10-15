using CRE.Models;

namespace CRE.Extensions
{
    public static class UserExternsions
    {
        public static string FullName(this AppUser user)
        {
            return $"{user.fName} {user.mName} {user.lName}".Trim();
        }
    }
}
