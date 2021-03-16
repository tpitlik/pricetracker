namespace NlnrPriceDyn.Logic.Common.Models
{
    public class UserRegistrationRequest
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordConfirmation { get; set; }
    }
}
