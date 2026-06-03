Baza uczelniana SQL Server:
Należy mieć włączony uczelniany VPN
Adres do bazy: db-mssql
Przykładowy Connection string:
Data Source=db-mssql;Initial Catalog=2019SBD;Integrated Security=True;Trust Server Certificate=True
lub
Data Source=db-mssql;Initial Catalog=2019SBD;Integrated Security=True

# Template kolokwium APBD (.NET 9)

Szkielet wzorowany na [PJATK-APBD-CodeFirst-Test-Example](https://github.com/ashiaraiyashiki/PJATK-APBD-CodeFirst-Test-Example) + **Swagger** do testów.

## Struktura (jak w przykładzie PJATK)

```
Kolokwium/
├── Controllers/          ← try/catch NotFoundException, ConflictException
├── DTOs/                 ← record + [Required]
├── Entities/
├── Exceptions/           ← NotFoundException, ConflictException
├── Infrastructure/       ← DatabaseContext (zamiast Data/)
├── Services/             ← IDbService + DbService (zamiast Repositories/)
├── Migrations/
├── Program.cs
├── Kolokwium.http        ← testy HTTP w Rider/VS
└── Properties/launchSettings.json
```

## Uruchomienie

```powershell
cd template\Kolokwium
dotnet restore
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet run
```

**Swagger:** http://localhost:5180/swagger

## Na kolokwium

1. Skopiuj folder `Kolokwium`, zmień nazwę projektu.
2. Usuń `Example*` (encja, metody w `IDbService`/`DbService`, `ExampleController`).
3. Dodaj encje + Fluent API w `DatabaseContext`.
4. Rozszerz `IDbService` / `DbService` o metody z PDF.
5. Nowy kontroler z tym samym wzorcem `try/catch`.
6. Connection string `Default` w `appsettings.json` (na uczelni – serwer z PDF).
7. Opcjonalnie `DB:DefaultSchema` – schemat SQL jak w przykładzie GitHub.

## Db First (grupa E)

- Bez `Migrations/`
- Zakomentuj `MigrateAsync()` w `Program.cs`
- Encje + `DatabaseContext` mapują tabele z `create.sql`

## Różnice vs przykład GitHub

| GitHub | Ten template |
|--------|----------------|
| OpenAPI | **Swagger** (Swashbuckle) |
| `MigrateAsync` brak przy starcie | `MigrateAsync` w `Program.cs` (wygodnie lokalnie) |
| .NET 10 | **.NET 9** |

Wzorce kodu: `WZORCE.md`


# Wzorce (PJATK + kolokwium)

## Kontroler – obsługa wyjątków

```csharp
[HttpGet("{id:int}")]
public async Task<ActionResult<BookResponse>> GetById(int id, CancellationToken ct)
{
    try
    {
        return Ok(await service.GetBookAsync(id, ct));
    }
    catch (NotFoundException e)
    {
        return NotFound(e.Message);
    }
}

[HttpPost]
public async Task<IActionResult> Create([FromBody] BookRequest req, CancellationToken ct)
{
    try
    {
        await service.AddBookAsync(req, ct);
        return Created();
    }
    catch (ConflictException e)
    {
        return Conflict(e.Message);
    }
}
```

## Serwis – NotFound / Conflict

```csharp
return await ctx.Books
    .Where(e => e.Id == id)
    .Select(e => new BookResponse(...))
    .FirstOrDefaultAsync(ct)
    ?? throw new NotFoundException($"Book with id {id} not found");

if (await ctx.Books.AnyAsync(e => e.Title == request.Title, ct))
    throw new ConflictException($"Book with title {request.Title} already exists");
```

## GET z filtrem opcjonalnym

```csharp
.Where(e => title == null || e.Title.Contains(title))
```

## Transakcja jawna (DELETE / wiele tabel)

```csharp
await using var transaction = await ctx.Database.BeginTransactionAsync(ct);
try
{
    await ctx.Rentals.Where(e => e.BookId == id).ExecuteDeleteAsync(ct);
    var removed = await ctx.Books.Where(e => e.Id == id).ExecuteDeleteAsync(ct);
    if (removed == 0)
        throw new NotFoundException(...);
    await transaction.CommitAsync(ct);
}
catch
{
    await transaction.RollbackAsync(ct);
    throw;
}
```

## Jedna transakcja przez SaveChanges

Wiele operacji na `ctx` + jedno `SaveChangesAsync()` = jedna transakcja (jak POST z gatunkami w przykładzie GitHub).

## DatabaseContext – schemat

```json
"DB": { "DefaultSchema": "twoj-schemat" }
```

```csharp
modelBuilder.HasDefaultSchema(configuration["DB:DefaultSchema"]);
```

## Fluent API

```csharp
modelBuilder.Entity<Enrollment>().HasKey(e => new { e.CourseId, e.StudentId });
modelBuilder.Entity<Order>().Property(o => o.UserId).HasColumnName("Users_UserId");
```

