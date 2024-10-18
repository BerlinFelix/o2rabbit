using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using o2rabbit.Migrations.Context;

namespace o2rabbit.Migrations;

class Program
{
    static async Task Main(string[] args)
    {
        await Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services.AddDbContext<DefaultContext>(builder =>
                {
                    builder.UseNpgsql(context.Configuration.GetConnectionString("DefaultConnection"));
                });
            })
            .RunConsoleAsync().ConfigureAwait(false);
    }
}