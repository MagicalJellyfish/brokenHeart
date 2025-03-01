using brokenHeart.Auth;
using brokenHeart.Authentication.Services;
using Microsoft.Extensions.DependencyInjection;

namespace brokenHeart.Authentication
{
    public static class ServiceRegistration
    {
        public static void RegisterServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IPasswordService, PasswordService>();
            serviceCollection.AddScoped<IAuthenticationService, AuthenticationService>();
            serviceCollection.AddScoped<ITokenService, TokenService>();

            serviceCollection.AddHostedService<TokenCleanupBService>();
        }
    }
}
