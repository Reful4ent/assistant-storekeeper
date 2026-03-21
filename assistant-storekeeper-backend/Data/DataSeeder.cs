using System.Threading.Tasks;
using assistant_storekeeper_backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace assistant_storekeeper_backend.Data
{
    public static class DataSeeder
    {
        private static readonly string[] WarehouseNames =
        {
            "Северозападный склад г.Санкт-Петербург",
            "Северовосточный склад г.Санкт-Петербург",
            "Юговосточный склад г.Москва"
        };

        private static readonly string[] NomenclatureNames =
        {
            "Ручка",
            "Стяжка",
            "Шуруп",
            "Гвоздь",
            "Болт",
            "Гайка",
            "Шайба"
        };

        public static async Task SeedAsync(ApplicationDbContext context, ILogger? logger = null)
        {
            await SeedWarehousesAsync(context, logger);
            await SeedNomenclaturesAsync(context, logger);
        }

        private static async Task SeedWarehousesAsync(ApplicationDbContext context, ILogger? logger)
        {
            foreach (var name in WarehouseNames)
            {
                var exists = await context.CompanyWarehouses.AnyAsync(w => w.Name == name);
                if (exists)
                {
                    continue;
                }

                context.CompanyWarehouses.Add(new CompanyWarehouse { Name = name });
                logger?.LogInformation("Создан склад: {Name}", name);
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedNomenclaturesAsync(ApplicationDbContext context, ILogger? logger)
        {
            foreach (var name in NomenclatureNames)
            {
                var exists = await context.Nomenclatures.AnyAsync(n => n.Name == name);
                if (exists)
                {
                    continue;
                }

                context.Nomenclatures.Add(new Nomenclature { Name = name });
                logger?.LogInformation("Создана номенклатура: {Name}", name);
            }

            await context.SaveChangesAsync();
        }
    }
}
