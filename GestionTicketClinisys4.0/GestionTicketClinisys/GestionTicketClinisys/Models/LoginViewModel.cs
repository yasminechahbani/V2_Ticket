using System.ComponentModel.DataAnnotations;

namespace GestionTicketClinisys.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Le nom d'utilisateur est requis")]
        [Display(Name = "Nom d'utilisateur")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le mot de passe est requis")]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Se souvenir de moi")]
        public bool RememberMe { get; set; }
    }

    public class UserProfileViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime LastLogin { get; set; }
        public string? TeamName { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Le mot de passe actuel est requis")]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe actuel")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le nouveau mot de passe est requis")]
        [DataType(DataType.Password)]
        [Display(Name = "Nouveau mot de passe")]
        [StringLength(100, ErrorMessage = "Le {0} doit contenir au moins {2} caractères.", MinimumLength = 6)]
        public string NewPassword { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirmer le nouveau mot de passe")]
        [Compare("NewPassword", ErrorMessage = "Le nouveau mot de passe et sa confirmation ne correspondent pas.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class AuthResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Token { get; set; }
        public UserProfileViewModel? User { get; set; }
    }

    public class ProfileViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom d'utilisateur est requis")]
        [Display(Name = "Nom d'utilisateur")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'email est requis")]
        [EmailAddress(ErrorMessage = "Format d'email invalide")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Prénom")]
        public string? FirstName { get; set; }

        [Display(Name = "Nom")]
        public string? LastName { get; set; }

        [Display(Name = "Téléphone")]
        public string? Phone { get; set; }

        [Display(Name = "Adresse")]
        public string? Address { get; set; }

        [Display(Name = "Date de naissance")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Nationalité")]
        public string? Nationality { get; set; }

        [Display(Name = "Poste")]
        public string? Position { get; set; }

        [Display(Name = "Département")]
        public string? Department { get; set; }

        [Display(Name = "Manager")]
        public string? Manager { get; set; }

        [Display(Name = "Rôle")]
        public string Role { get; set; } = string.Empty;

        [Display(Name = "Équipe")]
        public string? TeamName { get; set; }

        public int? TeamId { get; set; }

        public DateTime? LastLogin { get; set; }

        public bool IsActive { get; set; } = true;

        public string? ProfilePicture { get; set; }

        // Helper properties for display
        public string FullName => $"{FirstName} {LastName}".Trim();
        public string Initials => $"{FirstName?.FirstOrDefault()}{LastName?.FirstOrDefault()}".ToUpper();
    }
}
