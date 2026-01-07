using Microsoft.EntityFrameworkCore;
using Web_AIA.Data;
using Web_AIA.Entities;

namespace Web_AIA.Repository;

public class CommentaireRepository : GenericRepository<Commentaire>, ICommentaireRepository
{
    public CommentaireRepository(MyDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Commentaire>> GetByAnnonceAsync(int annonceId)
    {
        return await DbSet
            .Include(c => c.Utilisateur)
            .Where(c => c.AnnonceId == annonceId)
            .OrderByDescending(c => c.DateCommentaire)
            .ToListAsync();
    }
}
