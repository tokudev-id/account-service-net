using Microsoft.AspNetCore.Identity;

namespace AccountService.Server.Models
{
    public class ApplicationRole : IdentityRole
    {
        /// <summary>
        /// Initializes a new instance of <see cref="T:Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole" />.
        /// </summary>
        /// <remarks>
        /// The Id property is initialized to from a new GUID string value.
        /// </remarks>
        public ApplicationRole():base() {}

        /// <summary>
        /// Initializes a new instance of <see cref="T:Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole" />.
        /// </summary>
        /// <param name="roleType">The role type.</param>
        /// <remarks>
        /// The Id property is initialized to from a new GUID string value.
        /// </remarks>
        public ApplicationRole(string roleType) : base(roleType)
        {
        }
    }
}
