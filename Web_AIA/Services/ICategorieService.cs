using Web_AIA.Entities;

namespace Web_AIA.Services;

public interface ICategorieService
{
    Task<IEnumerable<Categorie>> ObtenirToutesAsync();
}
