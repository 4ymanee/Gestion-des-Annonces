using Web_AIA.Entities;

namespace Web_AIA.Services;

public interface IAnnonceService
{
    Task<IEnumerable<Annonce>> RechercherAsync(string? motCle, int? categorieId, string? ville, EtatAnnonce? etat);
    Task<Annonce?> ObtenirAsync(int id);
    Task<IEnumerable<Annonce>> ObtenirParUtilisateurAsync(string utilisateurId);
    Task<Annonce> CreerAsync(Annonce annonce, IEnumerable<AnnonceMedia> medias);
    Task MettreAJourAsync(Annonce annonce, IEnumerable<AnnonceMedia>? medias);
    Task SupprimerAsync(int id);
    Task<AnnonceMedia?> ObtenirMediaAsync(int mediaId);
}
