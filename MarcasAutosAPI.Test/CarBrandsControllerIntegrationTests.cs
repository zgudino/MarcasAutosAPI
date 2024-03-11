using System.Net;
using System.Net.Mime;
using System.Text.Json;
using MarcasAutosAPI.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MarcasAutosAPI.Test;

public class CarBrandsControllerIntegrationTests
    : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly WebApplicationFactory<Program> _factory;

    public CarBrandsControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));
                services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(_connection!));

                var serviceProvider = services.BuildServiceProvider();
                using var scope = serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                context.CarBrands.AddRange(
                    new CarBrand { Id = Guid.Parse("62df133e-e1b0-4798-b8f9-7a80438933c5"), Name = "Tesla" },
                    new CarBrand { Id = Guid.Parse("725ec2da-9807-4109-bd36-a0cafe595005"), Name = "Toyota" },
                    new CarBrand { Id = Guid.Parse("c22d7315-effd-4d5d-abd5-ee0a93628117"), Name = "Suzuki" },
                    new CarBrand { Id = Guid.Parse("9e1fe824-7323-433e-8b63-9e60ad7dfd52"), Name = "Mercedes-Benz" },
                    new CarBrand { Id = Guid.Parse("6f2e53db-73a8-478c-b9ec-0d66036403ed"), Name = "Porche" }
                );

                context.SaveChanges();
            });
        });
    }

    public void Dispose() => _connection.Dispose();

    [Fact]
    public async Task GetCarBrandsAsync_WhenCalledWithHttpClient_ReturnsOkObjectResultStringArray()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/carbrands");
        var body = await response.Content.ReadAsStringAsync();
        var payload = JsonSerializer.Deserialize<string[]>(body);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.IsType<string[]>(payload);
        Assert.True(payload.Length == 5);
    }

    [Fact]
    public async Task GetCarBrandsAsync_WhenCalledWithHttpClient_ReturnsApplicationJsonMimeResponse()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/carbrands");
        var mediaType = response.Content.Headers.ContentType?.MediaType;

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.Json, mediaType);
    }
}
