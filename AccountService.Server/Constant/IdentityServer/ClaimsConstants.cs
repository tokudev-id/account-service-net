using IdentityModel;

namespace AccountService.Server.Constant.IdentityServer
{
    public static class ClaimsConstants
    {
        public const string Subject = "sub";
        public const string Name = JwtClaimTypes.Name;
        public const string Gender = JwtClaimTypes.Gender;
        public const string BirthPlace = "birthplace";
        public const string BirthDate = JwtClaimTypes.BirthDate;
        public const string Country = "country";
        public const string Locale = JwtClaimTypes.Locale;
        public const string Email = JwtClaimTypes.Email;

        public const string DateFormat = "yyyy-MM-dd";
        public const string Picture = JwtClaimTypes.Picture;

        public const string PreferredUserName = JwtClaimTypes.PreferredUserName;
        public const string UserName = "username";
        public const string City = "city";
        public const string NickName = JwtClaimTypes.NickName;
        public const string WalletAddress = "wallet_address";
        public const string Entitlement = "entitlement";
        public const string DiscordUserName = "discord_username";
        public const string TwitterUserName = "twitter_username";
        public const string FacebookName = "facebook_name";
        public const string SteamId = "steam_id";
        public const string SteamUserName = "steam_name";

        public const string PlaytestPhase2 = "playtest_phase2";
        public const string DiscordId = "discord_id";
        public const string TwitterId = "twitter_id";
    }

    public static class ScopeConstants
    {
        public const string PublicProfile = "public_profile";
        public const string Name = JwtClaimTypes.Name;
        public const string Gender = JwtClaimTypes.Gender;
        public const string BirthDate = JwtClaimTypes.BirthDate;
        public const string Location = "location";
        public const string WalletAddress = "wallet_address";
    }

    public static class JwtClaimTypesEx
    {
        public const string Country = "country";
        public const string City = "city";
        public const string Bio = "bio";
    }
}
