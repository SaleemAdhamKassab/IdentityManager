using System.ComponentModel.DataAnnotations;

namespace IdentityManager.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required, EmailAddress]
        public  string Email { get; set; }
    }
}