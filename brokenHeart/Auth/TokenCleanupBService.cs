using brokenHeart.Auth.DB;
using brokenHeart.Auth.Entities;

namespace brokenHeart.Auth
{
    public sealed class TokenCleanupBService : BackgroundService
    {
        private IServiceProvider _serviceProvider;

        public TokenCleanupBService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    CleanupTokens();
                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                }
                catch (Exception) { }
            }
        }

        private async void CleanupTokens()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                AuthDbContext context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();

                List<Token> expiredTokens = context
                    .Tokens.Where(x => x.RefreshTokenExpiryTime <= DateTime.UtcNow)
                    .ToList();

                foreach (Token token in expiredTokens)
                {
                    context.Tokens.Remove(token);
                }

                context.SaveChanges();
            }
        }
    }
}
