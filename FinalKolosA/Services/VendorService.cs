using Microsoft.Data.SqlClient;
using PJATK-APBD-Kol1-Gr17c-s34002.DTOs;
using PJATK-APBD-Kol1-Gr17c-s34002.Exceptions;

namespace PJATK-APBD-Kol1-Gr17c-s34002.Services;

public class VendorService(IConfiguration configuration) : IVendorService
{
    public async Task<IEnumerable<VendorDto>> GetVendorsAsync(string? name)
    {
        var res = new Dictionary<string, VendorDto>();

        await using var conn = new SqlConnection(configuration.GetConnectionString("Default"));
        await using var cmd = new SqlCommand();
        cmd.Connection = conn;
        await conn.OpenAsync();

        cmd.CommandText = "SELECT v.Code, v.Name as VendorName, p.Id as ProductId, p.Name as ProductName, " +
                          "p.Description, p.StickerPrice, pt.Id as ProductTypeId, pt.Name as ProductTypeName, " +
                          "m.Id as MakerId, m.Name as MakerName, vp.Amount, vp.PricePerUnit " +
                          "FROM Vendors v " +
                          "LEFT JOIN VendorProducts vp ON v.Code = vp.VendorCode " +
                          "LEFT JOIN Products p ON p.Id = vp.ProductId " +
                          "LEFT JOIN ProductTypes pt ON pt.Id = p.ProductTypeId " +
                          "LEFT JOIN Makers m ON m.Id = p.MakerId " +
                          "WHERE @name IS NULL OR v.Name LIKE '%' + @name + '%'";

        cmd.Parameters.AddWithValue("@name", string.IsNullOrEmpty(name) ? DBNull.Value : name);

        await using var dr = await cmd.ExecuteReaderAsync();
        while (await dr.ReadAsync())
        {
            var code = dr.GetString(0);
            if (!res.ContainsKey(code))
            {
                res.Add(code, new VendorDto
                {
                    Code = code,
                    Name = dr.GetString(1),
                    Products = new List<ProductDto>()
                });
            }

            if (dr.IsDBNull(2)) continue;

            var list = res[code].Products.ToList();
            list.Add(new ProductDto
            {
                Id = dr.GetInt32(2),
                Name = dr.GetString(3),
                Description = dr.IsDBNull(4) ? null : dr.GetString(4),
                StrickerPrice = dr.GetDecimal(5),
                ProductType = new ProductTypeDto { Id = dr.GetInt32(6), Name = dr.GetString(7) },
                Maker = new MakerDto { Id = dr.GetInt32(8), Name = dr.GetString(9) },
                VendorOffer = new VendorOfferDto { Amount = dr.GetInt32(10), PricePerUnit = dr.GetDecimal(11) }
            });
            res[code].Products = list;
        }

        return res.Values;
    }

    public async Task CreateVendorAsync(CreateVendorDto dto)
    {
        await using var conn = new SqlConnection(configuration.GetConnectionString("Default"));
        await using var cmd = new SqlCommand();
        cmd.Connection = conn;
        await conn.OpenAsync();

        cmd.CommandText = "SELECT 1 FROM Vendors WHERE Code = @code";
        cmd.Parameters.AddWithValue("@code", dto.Code);
        if (await cmd.ExecuteScalarAsync() is not null)
            throw new BadRequestException($"Vendor with code {dto.Code} already exists");

        cmd.Parameters.Clear();

        if (dto.Products != null && dto.Products.Any())
        {
            cmd.CommandText = "SELECT Id FROM Products WHERE Id = @productId";
            foreach (var p in dto.Products)
            {
                cmd.Parameters.AddWithValue("@productId", p.Id);
                if (await cmd.ExecuteScalarAsync() is null)
                    throw new NotFoundException($"Product with ID {p.Id} does not exist");
                cmd.Parameters.Clear();
            }
        }

        await using var tran = await conn.BeginTransactionAsync();
        cmd.Transaction = (SqlTransaction)tran;

        try
        {
            cmd.CommandText = "INSERT INTO Vendors (Code, Name) VALUES (@code, @name)";
            cmd.Parameters.AddWithValue("@code", dto.Code);
            cmd.Parameters.AddWithValue("@name", dto.Name);
            await cmd.ExecuteNonQueryAsync();
            cmd.Parameters.Clear();

            if (dto.Products != null && dto.Products.Any())
            {
                cmd.CommandText = "INSERT INTO VendorProducts (ProductId, VendorCode, Amount, PricePerUnit) " +
                                  "VALUES (@productId, @code, @amount, @pricePerUnit)";
                foreach (var p in dto.Products)
                {
                    cmd.Parameters.AddWithValue("@code", dto.Code);
                    cmd.Parameters.AddWithValue("@productId", p.Id);
                    cmd.Parameters.AddWithValue("@amount", p.Amount);
                    cmd.Parameters.AddWithValue("@pricePerUnit", p.PricePerUnit);
                    await cmd.ExecuteNonQueryAsync();
                    cmd.Parameters.Clear();
                }
            }

            await tran.CommitAsync();
        }
        catch (Exception)
        {
            await tran.RollbackAsync();
            throw;
        }
    }
}