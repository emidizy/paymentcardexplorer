using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PaymentCardExplorer;
using Persistence;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Tests.IntegrationTests
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<Startup>
    {
        private HttpClient TestClient = null;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Create a new service provider.
                var serviceProvider = new ServiceCollection()
                    .AddEntityFrameworkInMemoryDatabase()
                    .BuildServiceProvider();

                // Add a database context (AppDbContext) using an in-memory database for testing.
                services.AddDbContext<DatabaseContext>(options =>
                {
                    options.UseInMemoryDatabase("ApplicationDB");
                    options.UseInternalServiceProvider(serviceProvider);
                });


                // Build the service provider.
                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var _dbContext = scopedServices.GetRequiredService<DatabaseContext>();
                    //Ensure database is created
                    _dbContext.Database.EnsureCreated();
                }
            });


        }

        public HttpClient GetTestClient()
        {
            if (TestClient == null)
            {
                TestClient = CreateClient();
                TestClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                TestClient.DefaultRequestHeaders.Add("Authorization", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9");
                base.ConfigureClient(TestClient);
                return TestClient;
            }
            return TestClient;
        }

    }
}
