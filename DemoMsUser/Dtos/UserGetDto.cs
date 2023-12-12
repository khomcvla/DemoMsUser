using DemoMsUser.Utils.Enums;

namespace DemoMsUser.Dtos
{
    public class UserGetShortDto
    {
        public string? Id { get; set; }
        public string? Email { get; set; }

        public string? Username { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }

    public class UserGetDto : UserGetShortDto
    {
        public DateTime? Birthday { get; set; }
        public GenderEnum? Gender { get; set; }
    }
}
