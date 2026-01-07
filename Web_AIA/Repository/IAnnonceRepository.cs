using Web_AIA.Entities;

namespace Web_AIA.Repository;

public interface IAnnonceRepository : IGenericRepository<Annonce>
{
    Task<Annonce?> GetWithDetailsAsync(int id);
    Task<IEnumerable<Annonce>> SearchAsync(string? motCle, int? categorieId, string? ville, EtatAnnonce? etat);
    Task<IEnumerable<Annonce>> GetByUserAsync(string utilisateurId);
    Task<AnnonceMedia?> GetMediaByIdAsync(int mediaId);
}
