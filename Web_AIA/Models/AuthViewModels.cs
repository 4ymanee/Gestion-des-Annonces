using System.ComponentModel.DataAnnotations;

namespace Web_AIA.Models;

public class RegisterViewModel
{
    [Required, Display(Name = "Nom complet")]
    public string NomComplet { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Phone, Display(Name = "Téléphone")]
    public string? Telephone { get; set; }

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), Compare("Password", ErrorMessage = "Les mots de passe ne correspondent pas.")]
    [Display(Name = "Confirmation du mot de passe")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class LoginViewModel
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Se souvenir de moi")]
    public bool RememberMe { get; set; }
}
