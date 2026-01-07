using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web_AIA.Entities;
using Web_AIA.Models;
using Web_AIA.Services;

namespace Web_AIA.Controllers;

[Authorize]
public class AnnoncesController : Controller
{
    private const int MaxPhotos = 5;
    private const int MaxFileSize = 5 * 1024 * 1024; // 5MB

    private readonly IAnnonceService _annonceService;
    private readonly ICategorieService _categorieService;
    private readonly ICommentaireService _commentaireService;
    private readonly UserManager<AppUser> _userManager;

    public AnnoncesController(
        IAnnonceService annonceService,
        ICategorieService categorieService,
        ICommentaireService commentaireService,
        UserManager<AppUser> userManager)
    {
        _annonceService = annonceService;
        _categorieService = categorieService;
        _commentaireService = commentaireService;
        _userManager = userManager;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] AnnonceFiltreViewModel filtre)
    {
        var categories = await _categorieService.ObtenirToutesAsync();
        var annonces = await _annonceService.RechercherAsync(null, null, null, null);
        var resultats = await _annonceService.RechercherAsync(filtre.MotCle, filtre.CategorieId, filtre.Ville, filtre.Etat);

        var annoncesParCategorie = annonces
            .GroupBy(a => a.CategorieId)
            .ToDictionary(g => g.Key, g => g.Count());

        filtre.Categories = categories;
        filtre.CategoriesStats = categories.Select(c => new CategorieStatViewModel
        {
            Id = c.Id,
            Nom = c.Nom,
            Description = string.IsNullOrWhiteSpace(c.Description) ? "" : c.Description,
            NombreAnnonces = annoncesParCategorie.TryGetValue(c.Id, out var total) ? total : 0
        }).ToList();

        filtre.Resultats = resultats.Select(a => new AnnonceListItemViewModel
        {
            Id = a.Id,
            Titre = a.Titre,
            Categorie = a.Categorie?.Nom ?? string.Empty,
            Ville = a.Ville,
            Prix = a.Prix,
            MediaId = a.Medias.FirstOrDefault()?.Id,
            DatePublication = a.DatePublication
        }).ToList();

        return View(filtre);
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var annonce = await _annonceService.ObtenirAsync(id);
        if (annonce == null)
        {
            return NotFound();
        }

        var commentaires = await _commentaireService.ObtenirParAnnonceAsync(id);
        var vm = new AnnonceDetailViewModel
        {
            Annonce = annonce,
            Commentaires = commentaires,
            NouveauCommentaire = new CommentaireCreateViewModel { AnnonceId = id }
        };
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = await _categorieService.ObtenirToutesAsync();
        return View(new AnnonceFormViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(AnnonceFormViewModel model)
    {
        ViewBag.Categories = await _categorieService.ObtenirToutesAsync();

        if (model.Photos?.Count > MaxPhotos)
        {
            ModelState.AddModelError(nameof(model.Photos), $"Vous pouvez ajouter au plus {MaxPhotos} photos.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
            return Challenge();
        }

        var medias = await EnregistrerPhotosAsync(model.Photos);
        var annonce = new Annonce
        {
            Titre = model.Titre,
            Description = model.Description,
            Prix = model.Prix,
            PrixNegociable = model.PrixNegociable,
            Etat = model.Etat,
            Ville = model.Ville,
            ModeTransaction = model.ModeTransaction,
            CategorieId = model.CategorieId,
            UtilisateurId = userId,
            DetailsJson = AnnonceService.ConvertirDetails(model.Details)
        };

        await _annonceService.CreerAsync(annonce, medias);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        ViewBag.Categories = await _categorieService.ObtenirToutesAsync();
        var annonce = await _annonceService.ObtenirAsync(id);
        if (annonce == null)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User);
        if (annonce.UtilisateurId != userId)
        {
            return Forbid();
        }

        var vm = new AnnonceFormViewModel
        {
            Id = annonce.Id,
            Titre = annonce.Titre,
            Description = annonce.Description,
            Prix = annonce.Prix,
            PrixNegociable = annonce.PrixNegociable,
            Etat = annonce.Etat,
            Ville = annonce.Ville,
            ModeTransaction = annonce.ModeTransaction,
            CategorieId = annonce.CategorieId,
            Details = string.IsNullOrWhiteSpace(annonce.DetailsJson)
                ? new Dictionary<string, string?>()
                : System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string?>>(annonce.DetailsJson) ?? new()
        };

        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, AnnonceFormViewModel model)
    {
        ViewBag.Categories = await _categorieService.ObtenirToutesAsync();

        if (model.Photos?.Count > MaxPhotos)
        {
            ModelState.AddModelError(nameof(model.Photos), $"Vous pouvez ajouter au plus {MaxPhotos} photos.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var annonce = await _annonceService.ObtenirAsync(id);
        if (annonce == null)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User);
        if (annonce.UtilisateurId != userId)
        {
            return Forbid();
        }

        annonce.Titre = model.Titre;
        annonce.Description = model.Description;
        annonce.Prix = model.Prix;
        annonce.PrixNegociable = model.PrixNegociable;
        annonce.Etat = model.Etat;
        annonce.Ville = model.Ville;
        annonce.ModeTransaction = model.ModeTransaction;
        annonce.CategorieId = model.CategorieId;
        annonce.DetailsJson = AnnonceService.ConvertirDetails(model.Details);

        var medias = await EnregistrerPhotosAsync(model.Photos);
        await _annonceService.MettreAJourAsync(annonce, medias.Any() ? medias : null);

        return RedirectToAction(nameof(Details), new { id = annonce.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var annonce = await _annonceService.ObtenirAsync(id);
        if (annonce == null)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User);
        if (annonce.UtilisateurId != userId)
        {
            return Forbid();
        }

        await _annonceService.SupprimerAsync(id);
        return RedirectToAction(nameof(Index));
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Commenter(CommentaireCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction(nameof(Details), new { id = model.AnnonceId });
        }

        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId)) return Challenge();

        var annonce = await _annonceService.ObtenirAsync(model.AnnonceId);
        if (annonce == null) return NotFound();

        var commentaire = new Commentaire
        {
            Contenu = model.Contenu,
            AnnonceId = model.AnnonceId,
            UtilisateurId = userId
        };

        await _commentaireService.AjouterAsync(commentaire);
        return RedirectToAction(nameof(Details), new { id = model.AnnonceId });
    }

    /// <summary>
    /// Endpoint pour servir les images stockées en base de données.
    /// </summary>
    [AllowAnonymous]
    [HttpGet]
    [ResponseCache(Duration = 86400)] // Cache 24h
    public async Task<IActionResult> Image(int id)
    {
        var media = await _annonceService.ObtenirMediaAsync(id);
        if (media == null || string.IsNullOrEmpty(media.ImageData))
        {
            return NotFound();
        }

        var imageBytes = Convert.FromBase64String(media.ImageData);
        return File(imageBytes, media.ContentType);
    }

    private async Task<List<AnnonceMedia>> EnregistrerPhotosAsync(IFormFileCollection? photos)
    {
        var medias = new List<AnnonceMedia>();
        if (photos == null || photos.Count == 0)
        {
            return medias;
        }

        foreach (var file in photos.Take(MaxPhotos))
        {
            if (file.Length == 0 || file.Length > MaxFileSize)
            {
                continue; // ignore fichiers vides ou > 5MB
            }

            // Valider le type de fichier
            var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
            if (!allowedTypes.Contains(file.ContentType.ToLower()))
            {
                continue; // ignore fichiers non-images
            }

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var imageData = Convert.ToBase64String(memoryStream.ToArray());

            medias.Add(new AnnonceMedia
            {
                ImageData = imageData,
                ContentType = file.ContentType,
                FileName = file.FileName,
                EstPrincipale = medias.Count == 0 // Première image = principale
            });
        }

        return medias;
    }
}
