using System.Text.Json;
using Web_AIA.Entities;
using Web_AIA.Repository;

namespace Web_AIA.Services;

public class AnnonceService : IAnnonceService
{
    private readonly IAnnonceRepository _annonceRepository;

    public AnnonceService(IAnnonceRepository annonceRepository)
    {
        _annonceRepository = annonceRepository;
    }

    public Task<IEnumerable<Annonce>> RechercherAsync(string? motCle, int? categorieId, string? ville, EtatAnnonce? etat)
        => _annonceRepository.SearchAsync(motCle, categorieId, ville, etat);

    public Task<Annonce?> ObtenirAsync(int id) => _annonceRepository.GetWithDetailsAsync(id);

    public async Task<IEnumerable<Annonce>> ObtenirParUtilisateurAsync(string utilisateurId)
    {
        return await _annonceRepository.GetByUserAsync(utilisateurId);
    }

    public async Task<Annonce> CreerAsync(Annonce annonce, IEnumerable<AnnonceMedia> medias)
    {
        annonce.DatePublication = DateTime.UtcNow;
        annonce.Statut = StatutAnnonce.Publie;
        annonce.Medias = medias.ToList();

        await _annonceRepository.AddAsync(annonce);
        await _annonceRepository.SaveChangesAsync();
        return annonce;
    }

    public async Task MettreAJourAsync(Annonce annonce, IEnumerable<AnnonceMedia>? medias)
    {
        if (medias != null && medias.Any())
        {
            annonce.Medias = medias.ToList();
        }

        _annonceRepository.Update(annonce);
        await _annonceRepository.SaveChangesAsync();
    }

    public async Task<AnnonceMedia?> ObtenirMediaAsync(int mediaId)
    {
        return await _annonceRepository.GetMediaByIdAsync(mediaId);
    }

    public async Task SupprimerAsync(int id)
    {
        var annonce = await _annonceRepository.GetByIdAsync(id);
        if (annonce is null)
        {
            return;
        }

        _annonceRepository.Remove(annonce);
        await _annonceRepository.SaveChangesAsync();
    }

    /// <summary>
    /// Sérialise un dictionnaire dynamique en JSON pour DetailsJson.
    /// </summary>
    public static string? ConvertirDetails(Dictionary<string, string?>? details)
    {
        if (details == null || details.Count == 0)
        {
            return null;
        }

        return JsonSerializer.Serialize(details, new JsonSerializerOptions { WriteIndented = false });
    }
}
