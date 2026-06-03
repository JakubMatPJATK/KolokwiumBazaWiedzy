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
