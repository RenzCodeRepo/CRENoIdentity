using CRE.Data;
using CRE.Interfaces;
using CRE.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
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
builder.Services.AddScoped<IInitialReviewServices, InitialReviewServices>();
builder.Services.AddScoped<INonFundedResearchInfoServices, NonFundedResearchInfoServices>();
builder.Services.AddScoped<IReceiptInfoServices, ReceiptInfoServices>();
builder.Services.AddScoped<ISecretariatServices, SecretariatServices>();
builder.Services.AddScoped<IUserServices, UserServices>();


builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

if (args.Length == 1 && args[0].ToLower() == "seeddata")
{
    Seed.SeedData(app);
}
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
