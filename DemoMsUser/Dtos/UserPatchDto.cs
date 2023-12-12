using System.ComponentModel.DataAnnotations;
using DemoMsUser.Utils.Enums;

namespace DemoMsUser.Dtos
{
    public class UserPatchDto
    {
        public string? Email { get; set; }
        public string? Username { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public DateTime? Birthday { get; set; }
        public GenderEnum Gender { get; set; }
    }

    public class UserPatchDtoAdmin : UserPatchDto
    {
        [Required(ErrorMessage = "Id is required!")]
        public string Id { get; set; }
    }
}
