using Data.Entities;
using Core.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Data.Utilities;

public static class CategorySeeder
{
    public static async Task SeedCategories(IServiceProvider services)
    {
        var repo = services.GetRequiredService<IRepository<Category, int>>();

        var existing = await repo.GetAllAsync();
        if (existing.Any()) return; // Exit if data exists

        //Create a couple of root categories
        string[] rootNames = { "Electronics", "Clothing", "Hardware", "Books" };

        for (int i = 0; i < rootNames.Length; i++)
        {
            var root = new Category
            {
                Name = rootNames[i],
                Description = $"Main category for {rootNames[i]}",
                ParentCategoryId = null
            };
            await repo.AddAsync(root);

            //Create a few subcategories for each root
            for (int j = 1; j <= 4; j++)
            {
                await repo.AddAsync(new Category
                {
                    Name = $"{rootNames[i]} Sub {j}",
                    Description = $"Secondary level items for {rootNames[i]}",
                    ParentCategoryId = root.Id
                });
            }
        }
        await repo.SaveChangesAsync();
    }
}
