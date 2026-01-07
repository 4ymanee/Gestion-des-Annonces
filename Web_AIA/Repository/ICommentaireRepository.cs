using Web_AIA.Entities;

namespace Web_AIA.Repository;

public interface ICommentaireRepository : IGenericRepository<Commentaire>
{
    Task<IEnumerable<Commentaire>> GetByAnnonceAsync(int annonceId);
}
