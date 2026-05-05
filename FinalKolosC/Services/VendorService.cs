using Microsoft.Data.SqlClient;
using KolosC.DTOs;
using KolosC.Exceptions;

namespace KolosC.Services;

public class VendorService(IConfiguration configuration) : IVendorService
{
    public async Task<VendorDto> GetVendorByCodeAsync(string code)
    {
        VendorDto? v = null;
        
        await using var sqlConnection = new SqlConnection(configuration.GetConnectionString("Default"));
        await using var sqlCommand = new SqlCommand();
        sqlCommand.Connection = sqlConnection;
        await sqlConnection.OpenAsync();
        
        sqlCommand.CommandText = "SELECT v.Code, v.Name as VendorName, p.Id as ProductId, p.Name as ProductName, " +
                                 "p.Description, p.StickerPrice, pt.Id as ProductTypeId, pt.Name as ProductTypeName, " +
                                 "m.Id as MakerId, m.Name as MakerName, vp.Amount, vp.PricePerUnit " +
                                 "FROM Vendors v " +
                                 "LEFT JOIN VendorProducts vp ON v.Code = vp.VendorCode " +
                                 "LEFT JOIN Products p ON p.Id = vp.ProductId " +
                                 "LEFT JOIN ProductTypes pt ON pt.Id = p.ProductTypeId " +
                                 "LEFT JOIN Makers m ON m.Id = p.MakerId " +
                                 "WHERE v.Code = @code";

        sqlCommand.Parameters.AddWithValue("@code", code);

        await using var dr = await sqlCommand.ExecuteReaderAsync();
        while (await dr.ReadAsync())
        {
            if (v == null)
            {
                v = new VendorDto
                {
                    Code = dr.GetString(0),
                    Name = dr.GetString(1),
                    Products = new List<ProductDto>()
                };
            }
            
            if (dr.IsDBNull(2)) continue;

            var products = v.Products.ToList();
            products.Add(new ProductDto
            {
                Id = dr.GetInt32(2),
                Name = dr.GetString(3),
                Description = dr.IsDBNull(4) ? null : dr.GetString(4),
                StrickerPrice = dr.GetDecimal(5),
                ProductType = new ProductTypeDto { Id = dr.GetInt32(6), Name = dr.GetString(7) },
                Maker = new MakerDto { Id = dr.GetInt32(8), Name = dr.GetString(9) },
                VendorOffer = new VendorOfferDto { Amount = dr.GetInt32(10), PricePerUnit = dr.GetDecimal(11) }
            });
            v.Products = products;
        }

        if (v == null) throw new NotFoundException($"Vendor with code {code} does not exist.");

        return v;
    }

    public async Task CreateVendorAsync(CreateVendorDto dto)
    {
        await using var sqlConnection = new SqlConnection(configuration.GetConnectionString("Default"));
        await using var sqlCommand = new SqlCommand();
        sqlCommand.Connection = sqlConnection;
        await sqlConnection.OpenAsync();

        sqlCommand.CommandText = "SELECT 1 FROM Vendors WHERE Code = @code";
        sqlCommand.Parameters.AddWithValue("@code", dto.Code);
        if (await sqlCommand.ExecuteScalarAsync() != null)
            throw new BadRequestException($"Vendor with code {dto.Code} already exists");
        
        sqlCommand.Parameters.Clear();

        if (dto.Products != null && dto.Products.Any())
        {
            sqlCommand.CommandText = "SELECT Id FROM Products WHERE Id = @productId";
            foreach (var p in dto.Products)
            {
                sqlCommand.Parameters.AddWithValue("@productId", p.Id);
                if (await sqlCommand.ExecuteScalarAsync() == null)
                    throw new NotFoundException($"Product with ID {p.Id} does not exist");
                sqlCommand.Parameters.Clear();
            }
        }

        await using var trans = await sqlConnection.BeginTransactionAsync();
        sqlCommand.Transaction = (SqlTransaction)trans;

        try
        {
            sqlCommand.CommandText = "INSERT INTO Vendors (Code, Name) VALUES (@code, @name)";
            sqlCommand.Parameters.AddWithValue("@code", dto.Code);
            sqlCommand.Parameters.AddWithValue("@name", dto.Name);
            await sqlCommand.ExecuteNonQueryAsync();
            sqlCommand.Parameters.Clear();

            if (dto.Products != null && dto.Products.Any())
            {
                sqlCommand.CommandText = "INSERT INTO VendorProducts (ProductId, VendorCode, Amount, PricePerUnit) VALUES (@productId, @code, @amount, @pricePerUnit)";
                foreach (var p in dto.Products)
                {
                    sqlCommand.Parameters.AddWithValue("@code", dto.Code);
                    sqlCommand.Parameters.AddWithValue("@productId", p.Id);
                    sqlCommand.Parameters.AddWithValue("@amount", p.Amount);
                    sqlCommand.Parameters.AddWithValue("@pricePerUnit", p.PricePerUnit);
                    await sqlCommand.ExecuteNonQueryAsync();
                    sqlCommand.Parameters.Clear();
                }
            }

            await trans.CommitAsync();
        }
        catch (Exception)
        {
            await trans.RollbackAsync();
            throw;
        }
    }
}
