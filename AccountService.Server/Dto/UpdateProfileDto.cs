namespace AccountService.Server.Dto
{
    public class UpdateProfileDto
    {
        public string? Name { get; set; }
        public string? Gender { get; set; }
        public string? Country { get; set; }
        public string? Birthdate { get; set; } // yyyy-MM-dd
        public string? Timezone { get; set; }
    }
}
