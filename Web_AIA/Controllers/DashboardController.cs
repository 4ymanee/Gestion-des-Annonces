using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web_AIA.Entities;
using Web_AIA.Models;
using Web_AIA.Services;

namespace Web_AIA.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IAnnonceService _annonceService;
    private readonly UserManager<AppUser> _userManager;

    public DashboardController(IAnnonceService annonceService, UserManager<AppUser> userManager)
    {
        _annonceService = annonceService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
            return Challenge();
        }

        var annonces = await _annonceService.ObtenirParUtilisateurAsync(userId);
        var vm = new DashboardViewModel
        {
            TotalAnnonces = annonces.Count(),
            AnnoncesPubliees = annonces.Count(a => a.Statut == StatutAnnonce.Publie),
            AnnoncesBrouillon = annonces.Count(a => a.Statut == StatutAnnonce.Brouillon),
            CommentairesRecus = annonces.Sum(a => a.Commentaires.Count),
            DernieresAnnonces = annonces.OrderByDescending(a => a.DatePublication)
                .Take(5)
                .Select(a => new AnnonceListItemViewModel
                {
                    Id = a.Id,
                    Titre = a.Titre,
                    Categorie = a.Categorie?.Nom ?? string.Empty,
                    Ville = a.Ville,
                    Prix = a.Prix,
                    MediaId = a.Medias.FirstOrDefault()?.Id,
                    DatePublication = a.DatePublication
                })
        };

        return View(vm);
    }
}
