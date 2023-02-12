using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;
using UtilityBot.Domain.Database;
using UtilityBot.Domain.Mappers;
using UtilityBot.Domain.Services.ConfigurationService.Interfaces;
using UtilityBot.Domain.Services.ConfigurationService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<UtilityBotContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<IConfigurationService, ConfigurationService>();

builder.Services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddAutoMapper(typeof(ServerMappingProfile));

var logger = new LoggerConfiguration().MinimumLevel.Warning().WriteTo.File("logs\\log-.txt", rollingInterval: RollingInterval.Hour,
    shared: true, retainedFileCountLimit: 72).CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<UtilityBotContext>();
    context.Database.Migrate();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
