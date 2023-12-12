using System.ComponentModel.DataAnnotations;
using DemoMsUser.Utils.Enums;

namespace DemoMsUser.Dtos
{
    public class UserPostDto
    {
        [Required(ErrorMessage = "Id is required!")]
        public string Id { get; set; }

        [Required(ErrorMessage = "Email is required!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Username is required!")]
        public string Username { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Birthday is required!")]
        public DateTime? Birthday { get; set; }

        [Required(ErrorMessage = "Gender is required!")]
        public GenderEnum? Gender { get; set; }
    }
}
