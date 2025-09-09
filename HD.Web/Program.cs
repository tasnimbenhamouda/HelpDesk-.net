using HD.ApplicationCore.Interfaces;
using HD.ApplicationCore.Services;
using HD.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

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


var app = builder.Build();

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

app.UseAuthentication();
app.UseAuthorization();

builder.Services.AddControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
