namespace AccountService.Server.Dto
{
    public class ChangePinWithPasswordDto
    {
        public string Password { get; set; } = string.Empty;
        public string NewPin { get; set; } = string.Empty;
    }
}
