using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Solbakken.Models
{

    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Nåværende passord")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} må være minst {2} bokstaver langt.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nytt passord")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Bekreft nytt passord")]
        [Compare("NewPassword", ErrorMessage = "Det nye passordet og bekreftelsen stemmer ikke overens.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "Brukernavn")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Passord")]
        public string Password { get; set; }

        [Display(Name = "Husk meg?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "Brukernavn")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "E-postadresse")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} må være minst {2} bokstaver langt.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Passord")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Bekreft passord")]
        [Compare("Password", ErrorMessage = "Passord og bekreftet passord stemmer ikke overens.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Hemmelig kodeord")]
        [RegularExpression("^Sandsøya", ErrorMessage = "Hemmlig kodeord ikke riktig.")]
        public string HemmeligOrd { get; set; }
    }
}
