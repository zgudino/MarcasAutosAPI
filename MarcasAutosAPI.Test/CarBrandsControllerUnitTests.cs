using MarcasAutosAPI.Controllers;
using MarcasAutosAPI.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace MarcasAutosAPI.Test;

public class CarBrandsControllerUnitTests(ApplicationDbContextFactory factory)
    : IClassFixture<ApplicationDbContextFactory>
{
    [Fact]
    public async Task GetCarBrandsAsync_WhenCarBrandsTableIsNotEmpty_ReturnsOkObjectResultStringArray()
    {
        // Arrange
        var db = factory.CreateDbContext();

        db.CarBrands.AddRange(
            new CarBrand { Id = Guid.Parse("62df133e-e1b0-4798-b8f9-7a80438933c5"), Name = "Tesla" },
            new CarBrand { Id = Guid.Parse("725ec2da-9807-4109-bd36-a0cafe595005"), Name = "Toyota" },
            new CarBrand { Id = Guid.Parse("c22d7315-effd-4d5d-abd5-ee0a93628117"), Name = "Suzuki" },
            new CarBrand { Id = Guid.Parse("9e1fe824-7323-433e-8b63-9e60ad7dfd52"), Name = "Mercedes-Benz" },
            new CarBrand { Id = Guid.Parse("6f2e53db-73a8-478c-b9ec-0d66036403ed"), Name = "Porche" }
        );
        db.SaveChanges();

        var controller = new CarBrandsController(db);

        // Act
        var result = await controller.GetCarBrandsAsync();
        var okResult = result.Result as OkObjectResult;

        // Assert
        Assert.NotNull(okResult);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        var actual = Assert.IsType<string[]>(okResult.Value);
        Assert.True(actual.Length == 5);
    }

    [Fact]
    public async Task GetCarBrandsAsync_WhenCarBrandsTableIsEmpty_ReturnsOkObjectResultEmptyStringArray()
    {
        // Arrange
        var db = factory.CreateDbContext();
        var controller = new CarBrandsController(db);

        // Act
        var result = await controller.GetCarBrandsAsync();
        var okResult = result.Result as OkObjectResult;

        // Assert
        Assert.NotNull(okResult);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        var actual = Assert.IsType<string[]>(okResult.Value);
        Assert.True(actual.Length == 0);
    }

    [Fact]
    public async Task GetCarBrandsAsync_WhenCancellationTokenSourceCancelsRequest_ThrowsOperationCanceledException()
    {
        // Arrange
        var db = factory.CreateDbContext();
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        var controller = new CarBrandsController(db);

        // Act
        cancellationTokenSource.Cancel(); // Simula cancelacion
        Task<ActionResult<string[]>> action() => controller.GetCarBrandsAsync(cancellationToken);

        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(action);
    }
}

public class ApplicationDbContextFactory : IDbContextFactory<ApplicationDbContext>, IDisposable
{
    private readonly SqliteConnection _connection;

    public ApplicationDbContextFactory()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();
    }

    public ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;

        var context = new ApplicationDbContext(options);

        // Borrón y cuenta nueva
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        return context;
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}
