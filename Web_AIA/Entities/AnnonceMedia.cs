namespace Web_AIA.Entities;

/// <summary>
/// Média associé à une annonce (photo principale ou supplémentaire).
/// Stocke les images directement en base de données pour éviter le hot reload.
/// </summary>
public class AnnonceMedia
{
    public int Id { get; set; }
    
    public string ImageData { get; set; } = string.Empty;
    
    public string ContentType { get; set; } = string.Empty;
    
    public string FileName { get; set; } = string.Empty;
    
    public bool EstPrincipale { get; set; }
    public int AnnonceId { get; set; }
    public Annonce? Annonce { get; set; }
}
