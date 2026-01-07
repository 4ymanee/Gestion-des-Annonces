using Microsoft.AspNetCore.Identity;

namespace Web_AIA.Entities;

/// <summary>
/// Utilisateur de l'application adossé à Identity.
/// </summary>
public class AppUser : IdentityUser
{
    public string NomComplet { get; set; } = string.Empty;
    public string? Telephone { get; set; }
    public DateTime DateInscription { get; set; } = DateTime.UtcNow;

    public ICollection<Annonce> Annonces { get; set; } = new List<Annonce>();
    public ICollection<Commentaire> Commentaires { get; set; } = new List<Commentaire>();
}
