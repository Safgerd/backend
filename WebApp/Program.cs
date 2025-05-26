using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WebApp.Dao;
using WebApp.Dao.DaoInterfaces;
using WebApp.Data;
using WebApp.Service;
using WebApp.Service.Interface;

var builder = WebApplication.CreateBuilder(args);

//dotnet aspnet-codegenerator identity -dc ApplicationDbContext --files "Account.Register;Account.Login"
//dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design
//dotnet tool install -g dotnet-aspnet-codegenerator
//"DefaultConnection": "Data Source=app.db"
//microsoft.aspnetcore.authentication.jwtbearer\8.0.16\
//microsoft.aspnetcore.diagnostics.entityframeworkcore\8.0.16\
//microsoft.aspnetcore.identity.entityframeworkcore\8.0.16\
//microsoft.aspnetcore.identity.ui\8.0.16\
//microsoft.entityframeworkcore.sqlite\8.0.16\
//microsoft.entityframeworkcore.sqlserver\8.0.16\
//microsoft.entityframeworkcore.tools\8.0.16\
//microsoft.visualstudio.web.codegeneration.design\8.0.7\
//swashbuckle.aspnetcore\8.1.2\
//swashbuckle.aspnetcore.swagger\8.1.2\

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
    options.SignIn.RequireConfirmedEmail = false;

    options.Password.RequiredLength = 1;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        } 
    });
});

builder.Services.AddAuthentication().AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opt =>
{
    opt.SaveToken = true;
    opt.TokenValidationParameters = new ()
    {
        ValidateIssuer = true,
        ValidateActor = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = "123456789",
        ValidAudience = "123456789",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("12345678901234567890123456781234567890123456789012345678"))
    };
});

// --- Регистрация DAO ---
builder.Services.AddScoped<IPostDao, PostDao>();
builder.Services.AddScoped<ICommentDao, CommentDao>();

// --- Регистрация Сервисов ---
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<ICommentService, CommentService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();

    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.MapRazorPages();


app.Services.CreateScope().ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>().CreateAsync(new IdentityRole("admin")).Wait();

var a = app.Services.CreateScope().ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>().Roles.ToList().First();

var um = app.Services.CreateScope().ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

var u = um.FindByEmailAsync("2@2.2").Result;
if (u != null && a != null && a.Name != null)
{
    um.AddToRoleAsync(u, a.Name).Wait();
}

app.Run();
