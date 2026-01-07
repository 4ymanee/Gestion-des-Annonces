using Web_AIA.Data;
using Web_AIA.Entities;

namespace Web_AIA.Repository;

public class CategorieRepository : GenericRepository<Categorie>, ICategorieRepository
{
    public CategorieRepository(MyDbContext context) : base(context)
    {
    }
}
