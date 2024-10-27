using CRE.Data;
using CRE.Interfaces;
using CRE.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using CRE.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddViewOptions(options =>
    {
        options.HtmlHelperOptions.ClientValidationEnabled = true; // Enable client-side validation
    });

// Configure Identity with AppUser and IdentityRole
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    // Optionally configure password and user options here
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configure authentication cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/AppUser/Login";
    options.AccessDeniedPath = "/AppUser/AccessDenied";
});

// Add application services
builder.Services.AddScoped<IChairpersonServices, ChairpersonServices>();
builder.Services.AddScoped<IChiefServices, ChiefServices>();
builder.Services.AddScoped<ICompletionCertificateServices, CompletionCertificateServices>();
builder.Services.AddScoped<ICompletionReportServices, CompletionReportServices>();
builder.Services.AddScoped<ICoProponentServices, CoProponentServices>();
builder.Services.AddScoped<IEthicsApplicationFormsServices, EthicsApplicationFormsServices>();
builder.Services.AddScoped<IEthicsApplicationLogServices, EthicsApplicationLogServices>();
builder.Services.AddScoped<IEthicsApplicationServices, EthicsApplicationServices>();
builder.Services.AddScoped<IEthicsClearanceServices, EthicsClearanceServices>();
builder.Services.AddScoped<IEthicsEvaluationServices, EthicsEvaluationServices>();
builder.Services.AddScoped<IEthicsEvaluatorServices, EthicsEvaluatorServices>();
builder.Services.AddScoped<IEthicsEvaluatorExpertiseServices, EthicsEvaluatorExpertiseServices>();
builder.Services.AddScoped<IExpertiseServices, ExpertiseServices>();
builder.Services.AddScoped<IEthicsFormServices, EthicsFormServices>();
builder.Services.AddScoped<IInitialReviewServices, InitialReviewServices>();
builder.Services.AddScoped<INonFundedResearchInfoServices, NonFundedResearchInfoServices>();
builder.Services.AddScoped<IReceiptInfoServices, ReceiptInfoServices>();
builder.Services.AddScoped<ISecretariatServices, SecretariatServices>();
builder.Services.AddScoped<IAppUserServices, AppUserServices>();

// Register IHttpContextAccessor
builder.Services.AddHttpContextAccessor(); // Add this line

// Configure the database context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Add session services
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Adjust as needed
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // Essential for GDPR compliance
});

var app = builder.Build();

// Seed data if specified
if (args.Length == 1 && args[0].ToLower() == "seeddata")
{
    Seed.SeedDataAsync(app);
}

// Ensure the database is created
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    // Add roles
    string[] roleNames = { "Researcher", "Chief", "Faculty", "Evaluator", "Secretariat", "Chairperson" };

    foreach (var roleName in roleNames)
    {
        // Check if the role exists before creating it
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Use session middleware before authentication
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
