using Web_AIA.Entities;
using Web_AIA.Repository;

namespace Web_AIA.Services;

public class CommentaireService : ICommentaireService
{
    private readonly ICommentaireRepository _commentaireRepository;

    public CommentaireService(ICommentaireRepository commentaireRepository)
    {
        _commentaireRepository = commentaireRepository;
    }

    public Task<IEnumerable<Commentaire>> ObtenirParAnnonceAsync(int annonceId) => _commentaireRepository.GetByAnnonceAsync(annonceId);

    public async Task AjouterAsync(Commentaire commentaire)
    {
        await _commentaireRepository.AddAsync(commentaire);
        await _commentaireRepository.SaveChangesAsync();
    }
}
