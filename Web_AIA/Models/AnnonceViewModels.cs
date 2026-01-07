using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Web_AIA.Entities;

namespace Web_AIA.Models;

public class AnnonceFormViewModel
{
    public int? Id { get; set; }

    [Required]
    public string Titre { get; set; } = string.Empty;

    [Required, DataType(DataType.MultilineText)]
    public string Description { get; set; } = string.Empty;

    [Required, Range(0, double.MaxValue)]
    public decimal Prix { get; set; }

    [Display(Name = "Prix à négocier")]
    public bool PrixNegociable { get; set; }

    [Required]
    [Display(Name = "État")]
    public EtatAnnonce Etat { get; set; }

    [Required]
    public string Ville { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Méthode de transaction")]
    public ModeTransaction ModeTransaction { get; set; }

    [Required]
    [Display(Name = "Catégorie")]
    public int CategorieId { get; set; }

    /// <summary>
    /// Champs spécifiques saisis dynamiquement (clé = nom du champ, valeur = saisie).
    /// </summary>
    public Dictionary<string, string?> Details { get; set; } = new();

    [Display(Name = "Photos (max 5MB chacune)")]
    public IFormFileCollection? Photos { get; set; }
}

public class AnnonceListItemViewModel
{
    public int Id { get; set; }
    public string Titre { get; set; } = string.Empty;
    public string Categorie { get; set; } = string.Empty;
    public string Ville { get; set; } = string.Empty;
    public decimal Prix { get; set; }
    public int? MediaId { get; set; }
    public DateTime DatePublication { get; set; }
}

public class AnnonceDetailViewModel
{
    public Annonce? Annonce { get; set; }
    public IEnumerable<Commentaire> Commentaires { get; set; } = Enumerable.Empty<Commentaire>();
    public CommentaireCreateViewModel NouveauCommentaire { get; set; } = new();
}

public class AnnonceFiltreViewModel
{
    public string? MotCle { get; set; }
    public int? CategorieId { get; set; }
    public string? Ville { get; set; }
    public EtatAnnonce? Etat { get; set; }
    public IEnumerable<Categorie> Categories { get; set; } = Enumerable.Empty<Categorie>();
    public IEnumerable<AnnonceListItemViewModel> Resultats { get; set; } = Enumerable.Empty<AnnonceListItemViewModel>();
    public IEnumerable<CategorieStatViewModel> CategoriesStats { get; set; } = Enumerable.Empty<CategorieStatViewModel>();
}

public class CommentaireCreateViewModel
{
    [Required, StringLength(1000)]
    public string Contenu { get; set; } = string.Empty;

    public int AnnonceId { get; set; }
}

public class CategorieStatViewModel
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int NombreAnnonces { get; set; }
}
