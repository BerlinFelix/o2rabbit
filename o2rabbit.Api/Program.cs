using o2rabbit.BizLog.Extensions;

namespace o2rabbit.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var conectionstring = builder.Configuration.GetConnectionString("Default") ??
                              throw new NullReferenceException("Default connection string");
        // Add services to the container.
        builder.Services.AddBizLog((o, sp) => { o.ConnectionString = conectionstring; })
            .AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}