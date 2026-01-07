using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Web_AIA.Models;
using Web_AIA.Services;
using Web_AIA.Entities;

namespace Web_AIA.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAnnonceService _annonceService;
        private readonly ICategorieService _categorieService;

        public HomeController(ILogger<HomeController> logger, IAnnonceService annonceService, ICategorieService categorieService)
        {
            _logger = logger;
            _annonceService = annonceService;
            _categorieService = categorieService;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] AnnonceFiltreViewModel filtre)
        {
            var categories = (await _categorieService.ObtenirToutesAsync()).ToList();
            var toutesLesAnnonces = (await _annonceService.RechercherAsync(null, null, null, null)).ToList();
            var annoncesFiltrees = (await _annonceService.RechercherAsync(filtre.MotCle, filtre.CategorieId, filtre.Ville, filtre.Etat))
                .OrderByDescending(a => a.DatePublication)
                .ToList();

            var filtresActifs = !string.IsNullOrWhiteSpace(filtre.MotCle)
                                || filtre.CategorieId.HasValue
                                || !string.IsNullOrWhiteSpace(filtre.Ville)
                                || filtre.Etat.HasValue;

            if (!filtresActifs)
            
            {
                annoncesFiltrees = annoncesFiltrees
                    .OrderByDescending(a => a.DatePublication)
                    .Take(6)
                    .ToList();
            }

            var annoncesParCategorie = toutesLesAnnonces
                .GroupBy(a => a.CategorieId)
                .ToDictionary(g => g.Key, g => g.Count());

            var vm = new AnnonceFiltreViewModel
            {
                MotCle = filtre.MotCle,
                CategorieId = filtre.CategorieId,
                Ville = filtre.Ville,
                Etat = filtre.Etat,
                Categories = categories,
                CategoriesStats = categories.Select(c => new CategorieStatViewModel
                {
                    Id = c.Id,
                    Nom = c.Nom,
                    Description = string.IsNullOrWhiteSpace(c.Description) ? string.Empty : c.Description,
                    NombreAnnonces = annoncesParCategorie.TryGetValue(c.Id, out var total) ? total : 0
                }).ToList(),
                Resultats = annoncesFiltrees.Select(a => new AnnonceListItemViewModel
                {
                    Id = a.Id,
                    Titre = a.Titre,
                    Categorie = a.Categorie?.Nom ?? string.Empty,
                    Ville = a.Ville,
                    Prix = a.Prix,
                    MediaId = a.Medias.FirstOrDefault()?.Id,
                    DatePublication = a.DatePublication
                }).ToList()
            };

            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
