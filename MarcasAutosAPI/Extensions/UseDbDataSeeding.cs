using MarcasAutosAPI.Entities;

namespace MarcasAutosAPI;

public static class UseDbDataSeedingExtension
{
    public static IApplicationBuilder UseDbDataSeeding(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (context.Database.EnsureCreated())
        {
            context.CarBrands.AddRange(
                new CarBrand { Id = Guid.Parse("62df133e-e1b0-4798-b8f9-7a80438933c5"), Name = "Tesla" },
                new CarBrand { Id = Guid.Parse("725ec2da-9807-4109-bd36-a0cafe595005"), Name = "Toyota" },
                new CarBrand { Id = Guid.Parse("c22d7315-effd-4d5d-abd5-ee0a93628117"), Name = "Suzuki" },
                new CarBrand { Id = Guid.Parse("9e1fe824-7323-433e-8b63-9e60ad7dfd52"), Name = "Mercedes-Benz" },
                new CarBrand { Id = Guid.Parse("6f2e53db-73a8-478c-b9ec-0d66036403ed"), Name = "Porche" }
            );
            context.SaveChanges();
        }

        return app;
    }
}
