using brokenHeart;
using brokenHeart.Auth;
using brokenHeart.DB;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using brokenHeart.Auth.DB;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using brokenHeart.Controllers;
using brokenHeart.JSON;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
                      {
                          policy.WithOrigins("http://localhost:4200")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                      });
});

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.InputFormatters.Insert(0, JPIF.GetJsonPatchInputFormatter());
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.Converters.Add(new ByteArrayConverter());
});

builder.Services.AddDbContext<BrokenDbContext>
    (options => options.UseSqlite(builder.Configuration.GetConnectionString("DataConnection")));
builder.Services.AddDbContext<AuthDbContext>
    (options => options.UseSqlite(builder.Configuration.GetConnectionString("AuthConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
    {
        options.Password.RequireNonAlphanumeric = false;
    }
);
builder.Services.Configure<PasswordHasherOptions>(options =>
{
    options.IterationCount = 300_000;
});

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    }
).AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = false,
            ValidIssuer = builder.Configuration["JWT:ValidIssuer"],

            ValidateAudience = false,
            ValidAudience = builder.Configuration["JWT:ValidAudience"],

            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
        };
    }
);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<CharChangeObservable>();

var app = builder.Build();

app.UseWebSockets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await Constants.ValidateAsync(
    app.Services.CreateScope().ServiceProvider.GetRequiredService<BrokenDbContext>(), 
    app.Services.CreateScope().ServiceProvider.GetRequiredService<AuthDbContext>(), 
    app.Services.CreateScope().ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>());

app.Run();