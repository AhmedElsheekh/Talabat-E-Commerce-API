using System.ComponentModel.DataAnnotations;

namespace Talabat.APIs.DTOs
{
	public class RegisterDto
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; }
		[Required]
		[RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^\\da-zA-Z]).{8,}$",
			ErrorMessage = "Password Must Contains 1 Upper Case Letter, 1 Lower Case Letter, 1 Digit, 1 Special Character")]
		public string Password { get; set; }
		[Required]
		public string DisplayName { get; set; }
		[Required]
		[Phone]
		public string PhoneNumber { get; set; }
	}
}
