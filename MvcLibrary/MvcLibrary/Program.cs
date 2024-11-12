using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MvcLibrary.Data;
using MvcLibrary.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<MvcLibraryContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MvcLibraryContext") ?? throw new InvalidOperationException("Connection string 'MvcLibraryContext' not found.")));

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<MvcLibraryContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Configure password policies
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
});

// Add services to the container.
builder.Services.AddControllersWithViews();

var key = Encoding.ASCII.GetBytes("1234567890abcdef1234567890abcdef1234567890abcdef");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "https://localhost:7143",
        ValidAudience = "https://localhost:7143",
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Cookies.ContainsKey("AuthToken"))
            {
                context.Token = context.Request.Cookies["AuthToken"];
            }
            return Task.CompletedTask;
        }
    };
});



builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("LibrarianOnly", policy => policy.RequireRole("Librarian"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseRouting();          // Set up routing for request matching

app.UseStaticFiles();      // Serve static files like images, js, etc.
app.UseHttpsRedirection();

app.UseAuthentication();   // Authenticate the user
app.UseAuthorization();    // Authorize the user

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
