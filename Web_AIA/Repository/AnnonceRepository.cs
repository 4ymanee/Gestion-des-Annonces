using Microsoft.EntityFrameworkCore;
using Web_AIA.Data;
using Web_AIA.Entities;

namespace Web_AIA.Repository;

public class AnnonceRepository : GenericRepository<Annonce>, IAnnonceRepository
{
    public AnnonceRepository(MyDbContext context) : base(context)
    {
    }

    public async Task<Annonce?> GetWithDetailsAsync(int id)
    {
        return await DbSet
            .Include(a => a.Categorie)
            .Include(a => a.Utilisateur)
            .Include(a => a.Medias)
            .Include(a => a.Commentaires)
                .ThenInclude(c => c.Utilisateur)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<Annonce>> SearchAsync(string? motCle, int? categorieId, string? ville, EtatAnnonce? etat)
    {
        IQueryable<Annonce> query = DbSet
            .Include(a => a.Categorie)
            .Include(a => a.Medias);

        if (!string.IsNullOrWhiteSpace(motCle))
        {
            string pattern = motCle.Trim().ToLower();
            query = query.Where(a => a.Titre.ToLower().Contains(pattern) || a.Description.ToLower().Contains(pattern));
        }

        if (categorieId.HasValue)
        {
            query = query.Where(a => a.CategorieId == categorieId.Value);
        }

        if (!string.IsNullOrWhiteSpace(ville))
        {
            string villeLower = ville.Trim().ToLower();
            query = query.Where(a => a.Ville.ToLower() == villeLower);
        }

        if (etat.HasValue)
        {
            query = query.Where(a => a.Etat == etat.Value);
        }

        return await query.OrderByDescending(a => a.DatePublication).ToListAsync();
    }

    public async Task<IEnumerable<Annonce>> GetByUserAsync(string utilisateurId)
    {
        return await DbSet
            .Include(a => a.Categorie)
            .Include(a => a.Medias)
            .Include(a => a.Commentaires)
            .Where(a => a.UtilisateurId == utilisateurId)
            .OrderByDescending(a => a.DatePublication)
            .ToListAsync();
    }

    public async Task<AnnonceMedia?> GetMediaByIdAsync(int mediaId)
    {
        return await Context.Set<AnnonceMedia>().FindAsync(mediaId);
    }
}
