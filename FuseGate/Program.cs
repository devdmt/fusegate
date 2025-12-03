using API.Infrastructure;
using API.Infrastructure.ClaimEngine;
using Sanlam.Configurations;
using Serilog;

Log.Logger = new LoggerConfiguration()
   .WriteTo.Console()
   .CreateBootstrapLogger();

Log.Information("Starting Application...");

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.  
    builder.Host.AddConfigurations();
    builder.Host.UseSerilog((ctx, lc) => lc
        .WriteTo.Console()
        .ReadFrom.Configuration(ctx.Configuration));

    //builder.Services.AddHostedService<TransactionAsync>();  
    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle  
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddInfrastructure(builder.Configuration);

    var app = builder.Build();

    // Configure the HTTP request pipeline.  
    if (app.Environment.IsDevelopment())
    {
        //app.UseSwagger(); // This now resolves to Swashbuckle's UseSwagger method  
        app.UseSwaggerUI();
    }

    app.UseInfrastructure(builder.Environment, builder.Configuration);

    app.MapControllers();
    app.UseSerilogRequestLogging();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception at application");
}

finally
{
    Log.Information("Application Shut down complete");
    Log.CloseAndFlush();
}
