namespace Web_AIA.Entities;

/// <summary>
/// Catégorie d'annonce (Immobilier, Véhicules, etc.).
/// </summary>
public class Categorie
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<Annonce> Annonces { get; set; } = new List<Annonce>();
}
