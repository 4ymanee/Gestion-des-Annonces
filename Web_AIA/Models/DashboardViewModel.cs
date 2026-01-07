namespace Web_AIA.Models;

public class DashboardViewModel
{
    public int TotalAnnonces { get; set; }
    public int AnnoncesPubliees { get; set; }
    public int AnnoncesBrouillon { get; set; }
    public int CommentairesRecus { get; set; }
    public IEnumerable<AnnonceListItemViewModel> DernieresAnnonces { get; set; } = Enumerable.Empty<AnnonceListItemViewModel>();
}
