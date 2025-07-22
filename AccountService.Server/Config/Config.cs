using Duende.IdentityServer.Models;

namespace AccountService.Server.Config
{
    public static class Config
    {
        // Defines the standard OIDC scopes like "openid", "profile", and "email"
        // These represent identity data about the user that clients can request.
        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>
            {
            new IdentityResources.OpenId(),    // Required for OIDC. Returns the user's unique identifier (sub claim).
            new IdentityResources.Profile(),   // Grants access to user profile claims (name, website, etc.).
            new IdentityResources.Email(),     // Grants access to the user's email and email_verified claims.
            };

        // Defines your APIs. For now, we can leave this empty if you only need user authentication.
        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>();

        // Defines the client applications that are allowed to connect.
        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
            // Add a new client definition for your other service
            new Client
            {
                ClientId = "Portal",
                ClientSecrets = { new Secret("a-strong-and-long-secret-goes-here".Sha256()) },

                // Use the Authorization Code flow, which is standard and secure for web apps.
                AllowedGrantTypes = GrantTypes.Code,

                // The URL where the user is redirected back to after they log in.
                RedirectUris = { "https://my-other-service.com/signin-oidc" },

                // The URL where the user is redirected back to after they log out.
                PostLogoutRedirectUris = { "https://my-other-service.com/signout-callback-oidc" },
                
                // The scopes this client is allowed to request.
                AllowedScopes = { "openid", "profile", "email" }
            }
            };
    }
}
