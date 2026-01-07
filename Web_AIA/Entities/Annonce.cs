namespace Web_AIA.Entities;

/// <summary>
/// Annonce avec champs communs et données spécifiques sérialisées en JSON.
/// </summary>
public class Annonce
{
    public int Id { get; set; }
    public string Titre { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Prix { get; set; }
    public bool PrixNegociable { get; set; }
    public EtatAnnonce Etat { get; set; }
    public string Ville { get; set; } = string.Empty;
    public ModeTransaction ModeTransaction { get; set; }
    public DateTime DatePublication { get; set; } = DateTime.UtcNow;
    public StatutAnnonce Statut { get; set; } = StatutAnnonce.Publie;

    public int CategorieId { get; set; }
    public Categorie? Categorie { get; set; }

    public string UtilisateurId { get; set; } = string.Empty;
    public AppUser? Utilisateur { get; set; }

    /// <summary>
    /// Champs spécifiques à la catégorie (structure JSON sérialisée).
    /// </summary>
    public string? DetailsJson { get; set; }

    public ICollection<Commentaire> Commentaires { get; set; } = new List<Commentaire>();
    public ICollection<AnnonceMedia> Medias { get; set; } = new List<AnnonceMedia>();
}
