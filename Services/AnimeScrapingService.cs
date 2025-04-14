using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AnimeNowApi.Data;

namespace AnimeNowApi.Services
{
    public class AnimeScrapingService : IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AnimeScrapingService> _logger;
        private Timer? _timer; // 修改為可空

        public AnimeScrapingService(
            IServiceProvider serviceProvider,
            ILogger<AnimeScrapingService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("番劇爬蟲服務已啟動");

            // 每天凌晨運行一次
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromHours(24));

            return Task.CompletedTask;
        }

        private async void DoWork(object? state) // 修改為可空參數
        {
            _logger.LogInformation("番劇爬蟲開始工作");

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AnimeDbContext>();

                // 使用 HttpClient 獲取數據
                using var httpClient = new HttpClient();

                var response = await httpClient.GetAsync("https://acgsecrets.hk/bangumi/202504/");
                var content = await response.Content.ReadAsStringAsync();


                _logger.LogInformation("番劇爬蟲工作完成");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "番劇爬蟲工作出錯");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("番劇爬蟲服務已停止");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}