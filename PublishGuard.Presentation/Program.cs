using PublishGuard.Application;
using PublishGuard.Infrastructure;
using SEO.Publishguard.Application;
using SEO.Publishguard.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "PublishGuard API",
        Version = "v1"
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendCors", policy =>
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

builder.Services.Configure<ValidationRuleOptions>(builder.Configuration.GetSection("ValidationRules"));
builder.Services.AddHttpClient<IArticleExtractor, GoogleDocsArticleExtractor>();
builder.Services.AddScoped<IArticleAnalyzer, ArticleAnalyzer>();
builder.Services.AddScoped<IArticleScorer, DefaultArticleScorer>();
builder.Services.AddScoped<IWordPressPayloadBuilder, WordPressPayloadBuilder>();
builder.Services.AddScoped<IValidationRule, ImageCountValidationRule>();
builder.Services.AddScoped<IValidationRule, ProductLinkValidationRule>();
builder.Services.AddScoped<IValidationRule, H2CountValidationRule>();
builder.Services.AddScoped<IValidationRule, BoldPercentageValidationRule>();
builder.Services.AddScoped<IValidationRule, GoogleDriveImageValidationRule>();

builder.Services.AddHttpClient<IArticleSourceClient, GoogleDocSourceClient>();
builder.Services.AddSingleton<IArticleParser, GoogleDocArticleParser>();
builder.Services.AddSingleton<IQualityChecker, QualityChecker>();
builder.Services.AddTransient<IArticleProcessor, ArticleProcessor>();
builder.Services.AddSingleton<IArticleRepository, InMemoryArticleRepository>();
builder.Services.AddSingleton<IUnitOfWork, InMemoryUnitOfWork>();
builder.Services.AddSingleton<IArticleUploader, WordpressUploadAutomation>();
builder.Services.AddTransient<IArticleService, ArticleService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "PublishGuard API v1");
        options.RoutePrefix = "api";
    });
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseCors("FrontendCors");

app.UseAuthorization();

app.MapControllers();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
