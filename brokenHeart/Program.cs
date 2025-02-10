using System.Security.Claims;
using System.Text;
using brokenHeart;
using brokenHeart.Auth;
using brokenHeart.Authentication.DB;
using brokenHeart.DB;
using brokenHeart.JSON;
using brokenHeart.Services.SignalR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
ConfigureBuilder(builder);
var app = builder.Build();
ConfigureApp(app);

await Constants.ValidateAsync(
    app.Services.CreateScope().ServiceProvider.GetRequiredService<BrokenDbContext>(),
    app.Services.CreateScope().ServiceProvider.GetRequiredService<AuthDbContext>()
);

app.Run();

static void ConfigureBuilder(WebApplicationBuilder builder)
{
    AddCors(builder);

    AddControllers(builder);

    builder.Services.AddSignalR();

    AddDatabaseContexts(builder);

    RegisterServices(builder.Services);

    AddAuthentication(builder);

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

static void RegisterServices(IServiceCollection serviceCollection)
{
    serviceCollection.AddHostedService<TokenCleanupBService>();

    brokenHeart.Authentication.ServiceRegistration.RegisterServices(serviceCollection);
    brokenHeart.Database.ServiceRegistration.RegisterServices(serviceCollection);
    brokenHeart.Services.ServiceRegistration.RegisterServices(serviceCollection);
}

static void AddCors(WebApplicationBuilder builder)
{
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy
                .WithOrigins(builder.Configuration["CORS:Origin"])
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
    });
}

static void AddControllers(WebApplicationBuilder builder)
{
    // Add services to the container.
    builder
        .Services.AddControllers(options =>
        {
            options.InputFormatters.Insert(0, JPIF.GetJsonPatchInputFormatter());
        })
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = System
                .Text
                .Json
                .Serialization
                .ReferenceHandler
                .IgnoreCycles;
            options.JsonSerializerOptions.Converters.Add(new ByteArrayConverter());
        });
}

static void AddDatabaseContexts(WebApplicationBuilder builder)
{
    builder.Services.AddDbContext<BrokenDbContext>(options =>
        options.UseSqlite(
            builder.Configuration.GetConnectionString("DataConnection"),
            o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
        )
    );
    builder.Services.AddDbContext<AuthDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("AuthConnection"))
    );
}

static void AddAuthentication(WebApplicationBuilder builder)
{
    builder
        .Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidIssuer = builder.Configuration["JWT:ValidIssuer"],

                ValidateAudience = true,
                ValidAudience = builder.Configuration["JWT:ValidAudience"],

                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["JWT_Secret"])
                )
            };
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var ip = context.Request.HttpContext.Connection.RemoteIpAddress.ToString();
                    var originHeader = context
                        .Request.Headers.SingleOrDefault(x => x.Key == "Origin")
                        .Value.ToString();
                    //If requester is localhost and origin is not [...]4200 (Angular dev url)
                    if ((ip == "127.0.0.1" || ip == "::1") && !originHeader.EndsWith("4200"))
                    {
                        var claims = new[]
                        {
                            new Claim(
                                ClaimTypes.NameIdentifier,
                                "localhost",
                                ClaimValueTypes.String,
                                context.Options.ClaimsIssuer
                            ),
                            new Claim(
                                ClaimTypes.Name,
                                "localhost",
                                ClaimValueTypes.String,
                                context.Options.ClaimsIssuer
                            )
                        };

                        context.Principal = new ClaimsPrincipal(
                            new ClaimsIdentity(claims, context.Scheme.Name)
                        );
                        context.Success();
                        return Task.CompletedTask;
                    }

                    var accessToken = context.Request.Query["access_token"];

                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/signalr")))
                    {
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                }
            };
        });
}

static void ConfigureApp(WebApplication app)
{
    app.UseForwardedHeaders(
        new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        }
    );

    app.UseWebSockets();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseRouting();

    app.UseCors();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapHub<SignalRHub>("/signalr");
    app.MapControllers();
}
