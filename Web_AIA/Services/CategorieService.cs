using Web_AIA.Entities;
using Web_AIA.Repository;

namespace Web_AIA.Services;

public class CategorieService : ICategorieService
{
    private readonly ICategorieRepository _categorieRepository;

    public CategorieService(ICategorieRepository categorieRepository)
    {
        _categorieRepository = categorieRepository;
    }

    public Task<IEnumerable<Categorie>> ObtenirToutesAsync() => _categorieRepository.GetAllAsync();
}
