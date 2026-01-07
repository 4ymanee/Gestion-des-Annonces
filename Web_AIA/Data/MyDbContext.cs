using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Web_AIA.Entities;

namespace Web_AIA.Data;

/// <summary>
/// DbContext principal avec Identity + mapping des entités métier.
/// </summary>
public class MyDbContext : IdentityDbContext<AppUser>
{
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
    {
    }

    public DbSet<Categorie> Categories => Set<Categorie>();
    public DbSet<Annonce> Annonces => Set<Annonce>();
    public DbSet<Commentaire> Commentaires => Set<Commentaire>();
    public DbSet<AnnonceMedia> Medias => Set<AnnonceMedia>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Categorie>(entity =>
        {
            entity.Property(c => c.Nom).IsRequired().HasMaxLength(150);
            entity.Property(c => c.Description).HasMaxLength(500);

            entity.HasData(
                new Categorie { Id = 1, Nom = "Immobilier", Description = "Maisons, appartements, terrains..." },
                new Categorie { Id = 2, Nom = "Véhicules", Description = "Voitures, motos, utilitaires..." },
                new Categorie { Id = 3, Nom = "Électronique", Description = "Téléphones, ordinateurs, consoles..." },
                new Categorie { Id = 4, Nom = "Maison et Jardin", Description = "Mobilier, bricolage, décoration..." },
                new Categorie { Id = 5, Nom = "Mode et Accessoires", Description = "Vêtements, chaussures, bijoux..." },
                new Categorie { Id = 6, Nom = "Sports et Loisirs", Description = "Sport, musique, jeux, livres..." }
            );
        });

        builder.Entity<Annonce>(entity =>
        {
            entity.Property(a => a.Titre).IsRequired().HasMaxLength(200);
            entity.Property(a => a.Description).IsRequired();
            entity.Property(a => a.Ville).IsRequired().HasMaxLength(120);
            entity.Property(a => a.Prix).HasColumnType("decimal(18,2)");
            entity.Property(a => a.DetailsJson).HasColumnType("nvarchar(max)");
            entity.Property(a => a.Etat).HasConversion<string>().IsRequired();
            entity.Property(a => a.ModeTransaction).HasConversion<string>().IsRequired();
            entity.Property(a => a.Statut).HasConversion<string>().IsRequired();

            entity.HasOne(a => a.Categorie)
                  .WithMany(c => c.Annonces)
                  .HasForeignKey(a => a.CategorieId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(a => a.Utilisateur)
                  .WithMany(u => u.Annonces)
                  .HasForeignKey(a => a.UtilisateurId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Commentaire>(entity =>
        {
            entity.Property(c => c.Contenu).IsRequired().HasMaxLength(1000);

            entity.HasOne(c => c.Annonce)
                  .WithMany(a => a.Commentaires)
                  .HasForeignKey(c => c.AnnonceId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(c => c.Utilisateur)
                  .WithMany(u => u.Commentaires)
                  .HasForeignKey(c => c.UtilisateurId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<AnnonceMedia>(entity =>
        {
            entity.Property(m => m.ImageData).IsRequired();
            entity.Property(m => m.ContentType).IsRequired().HasMaxLength(100);
            entity.Property(m => m.FileName).IsRequired().HasMaxLength(300);

            entity.HasOne(m => m.Annonce)
                  .WithMany(a => a.Medias)
                  .HasForeignKey(m => m.AnnonceId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
