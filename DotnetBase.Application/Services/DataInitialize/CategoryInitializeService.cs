using DotnetBase.Domain.Entities;
using DotnetBase.Domain.Entities.Contexts;

namespace DotnetBase.Application.Services.DataInitialize
{
    public class CategoryInitializeService(AppDbContext appDbContext) : IDataInitializeService
    {
        public int Order
        {
            get => 3;
            set
            {

            }
        }

        public async Task RunAsync()
        {
            if (appDbContext.Categories.Any()) return;

            var categories = new List<Category>
            {
                new()
                {
                    Name = "Laptop",
                    Description = "Laptop"
                },
                new()
                {
                    Name = "Mobile phone",
                    Description = "Mobile phone"
                },
                new()
                {
                    Name = "Keyboard",
                    Description = "Keyboard"
                }
            };

            await appDbContext.Categories.AddRangeAsync(categories);

            await appDbContext.SaveChangesAsync();
        }
    }
}
