using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Web_AIA.Data;
using Web_AIA.Entities;
using Web_AIA.Repository;
using Web_AIA.Services;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<MyDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Repository + Services
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IAnnonceRepository, AnnonceRepository>();
builder.Services.AddScoped<ICategorieRepository, CategorieRepository>();
builder.Services.AddScoped<ICommentaireRepository, CommentaireRepository>();

builder.Services.AddScoped<IAnnonceService, AnnonceService>();
builder.Services.AddScoped<ICategorieService, CategorieService>();
builder.Services.AddScoped<ICommentaireService, CommentaireService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
