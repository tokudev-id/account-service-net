namespace AccountService.Server.Dto
{
    public class SetPinRequestDto
    {
        public string UserId { get; set; } = default!;
        public string Pin { get; set; } = default!;
    }
}
