namespace Learn.Users.Models.Input
{
    public class CreateUserInput
    {
        public required string Name { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string CountryCode { get; set; }
    }
}
