using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Serilog;
using WebScraping.Core.Application.DTOs;
using WebScraping.Core.Application.Heplers;
using WebScraping.Core.Application.Mappings;
using WebScraping.Core.Application.Utils;
using WebScraping.Infrastructure.Persistence.DbContexts;
using WebScraping.Infrastructure.Persistence.Models;
using Emuns = WebScraping.Core.Application.Emuns;

namespace WebScraping
{
    internal class Program
    {
        private static ILogger _logger;

        private static async Task Main(string[] args)
        {
            _logger = Logger.CreateLogger().ForContext<Program>();

            await LoadBlackList();
            try
            {
                var amazonTask = Task.Run(() =>
                {
                    //Amazon.Run();
                });

                var theStoreTask = Task.Run(() =>
                {
                    //TheStore.Run();
                });

                var eBayTask = Task.Run(async () =>
                {
                    await Ebay.RunAsync();
                });

                await Task.WhenAll(amazonTask, theStoreTask, eBayTask);
            }
            catch (Exception e)
            {
                _logger.Error(e.ToString());
            }
            finally
            {
                await UpdateItems();
                _logger.Information("Done");
            }
        }

        private static async Task UpdateItems()
        {
            Task markingToInStock = Task.Run(async () =>
            {
                using (var context = new ApplicationDbContext())
                {
                    var ItemListToMarkInStock = await context.Items
                                            .Where(x => x.StatusId != (int)Emuns.Status.InStock &&
                                                   Helper.checkedItemList.Contains(x.Link))
                                            .ToListAsync();

                    Parallel.ForEach(ItemListToMarkInStock, (item) =>
                    {
                        item.StatusId = (int)Emuns.Status.InStock;
                    });
                    await context.SaveChangesAsync();
                }
            });

            Task markingToOutStock = Task.Run(async () =>
            {
                using (var context = new ApplicationDbContext())
                {
                    var ItemListToMarkOutStock = await context.Items
                                            .Where(x => x.StatusId == (int)Emuns.Status.InStock &&
                                                   !Helper.checkedItemList.Contains(x.Link))
                                            .ToListAsync();

                    Parallel.ForEach(ItemListToMarkOutStock, (item) =>
                    {
                        item.StatusId = (int)Emuns.Status.OutStock;
                    });
                    await context.SaveChangesAsync();
                }
            });

            await Task.WhenAll(markingToInStock, markingToOutStock);
            _logger.Information($"{Helper.checkedItemList.Count} Items In Stock");
            Console.Beep(500, 800);
        }

        private static async Task LoadBlackList()
        {
            using (var context = new ApplicationDbContext())
            {
                var mapperConfiguration = new MapperConfiguration(x => x.AddProfile(new AutomapperConfig())).CreateMapper();

                var backList = await context.BlackLists
                    .ProjectTo<BlackListDTO>(mapperConfiguration.ConfigurationProvider)
                    .ToListAsync();

                Helper.linkBlackList = backList;

                _logger.Information("Blacklist loaded");
            }
        }
    }
}