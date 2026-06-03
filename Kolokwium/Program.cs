using Kolokwium.Infrastructure;
using Kolokwium.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IDbService, DbService>();

builder.Services.AddDbContext<DatabaseContext>(opt =>
{
    var connectionString = builder.Configuration.GetConnectionString("Default")
        ?? builder.Configuration.GetConnectionString("DefaultConnection");

    opt.UseSqlServer(connectionString, sql =>
    {
        var schema = builder.Configuration["DB:DefaultSchema"];
        if (!string.IsNullOrWhiteSpace(schema))
            sql.MigrationsHistoryTable("__EFMigrationsHistory", schema);
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapGet("/", () => Results.Redirect("/swagger"));

app.UseHttpsRedirection();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    await db.Database.MigrateAsync();
}

app.Run();
