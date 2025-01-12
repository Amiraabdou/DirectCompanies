using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static DirectCompanies.Localization.Localizer;
using System.Globalization;
using System.Reflection.Metadata;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using DirectCompanies.Services;

namespace DirectCompanies.Localization
{
    public class StartingService : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<StartingService> _logger;
        private readonly IServiceProvider _provider;

        static StartingService()
        {
        }

        public StartingService(IConfiguration configuration,
            IServiceProvider provider, ILogger<StartingService> logger,
            IWebHostEnvironment environment
        )
        {
            _configuration = configuration;
            _provider = provider;
            _logger = logger;
            _environment = environment;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Start Load Localizer");
            var path = Path.Combine(_environment.ContentRootPath, "Localization/Files/Localization.csv");
            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = new List<LocalizationRecord>();
                await foreach (var item in csv.GetRecordsAsync<LocalizationRecord>())
                    records.Add(item);
                LoadStrings(records);
            }

            _logger.LogInformation("End Load Localizer");

            using (var scope = _provider.CreateScope())
            {
                _logger.LogInformation("Start Build Schema");
                var dbPath = Path.Combine(_environment.ContentRootPath, "Client", "Database");
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                if (!Directory.Exists(dbPath))
                    Directory.CreateDirectory(dbPath);

                try
                {
                    if (!context.Database.CanConnect())
                        await context.Database.EnsureCreatedAsync(stoppingToken);
                    else
                        await context.Database.MigrateAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                _logger.LogInformation("Schema Built");

            }
        }

    }
}
