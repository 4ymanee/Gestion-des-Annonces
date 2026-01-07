using Web_AIA.Entities;

namespace Web_AIA.Services;

public interface ICommentaireService
{
    Task<IEnumerable<Commentaire>> ObtenirParAnnonceAsync(int annonceId);
    Task AjouterAsync(Commentaire commentaire);
}
