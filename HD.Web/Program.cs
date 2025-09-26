using HD.ApplicationCore.Interfaces;
using HD.ApplicationCore.Services;
using HD.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddCors(options => 
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

//creation des instances cnx et UnitOfWork
builder.Services.AddDbContext<DbContext, HelpDeskContext>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<Type>(t => typeof(GenericRepository<>));

//instanciation des services
builder.Services.AddScoped<IServiceComplaint,ServiceComplaint>();
builder.Services.AddScoped<IServiceAuthentication,ServiceAuthentication>();
builder.Services.AddScoped<IServiceAdmin, ServiceAdmin>();
builder.Services.AddScoped<IServiceAgentClaimLog, ServiceAgentClaimLog>();
builder.Services.AddScoped<IServiceFeedback, ServiceFeedback>();
builder.Services.AddScoped<IServiceMessage, ServiceMessage>();
builder.Services.AddScoped<IServiceFeature, ServiceFeature>();
builder.Services.AddScoped<IServiceAgent, ServiceAgent>();
builder.Services.AddScoped<IServiceClient, ServiceClient>();
builder.Services.AddScoped<IServiceComplaintFiles, ServiceComplaintFile>();


//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "HelpDesk API",
        Version = "v1"
    });

//Ajout de la sécurité JWT
options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
{
   Name = "Authorization",
   Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
   Scheme = "Bearer",
   BearerFormat = "JWT",
   In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Entrez 'Bearer {votre_token}' pour vous authentifier"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


// Ajouter l’authentification JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)

    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        opt.JsonSerializerOptions.WriteIndented = true;
    });



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "HelpDesk API v1");
        c.RoutePrefix = string.Empty; // Swagger à la racine : https://localhost:xxxx/
    });
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseCors();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

builder.Services.AddControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
