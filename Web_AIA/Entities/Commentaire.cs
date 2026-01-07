namespace Web_AIA.Entities;

/// <summary>
/// Commentaire rattaché à une annonce.
/// </summary>
public class Commentaire
{
    public int Id { get; set; }
    public string Contenu { get; set; } = string.Empty;
    public DateTime DateCommentaire { get; set; } = DateTime.UtcNow;

    public string UtilisateurId { get; set; } = string.Empty;
    public AppUser? Utilisateur { get; set; }

    public int AnnonceId { get; set; }
    public Annonce? Annonce { get; set; }
}
