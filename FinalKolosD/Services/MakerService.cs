using Microsoft.Data.SqlClient;
using PJATK-APBD-Kol1-Gr17c-s34002.DTOs;
using PJATK-APBD-Kol1-Gr17c-s34002.Exceptions;

namespace PJATK-APBD-Kol1-Gr17c-s34002.Services;

public class MakerService(IConfiguration configuration) : IMakerService
{
    public async Task<MakerDto> GetMakerByIdAsync(int id)
    {
        MakerDto? m = null;

        await using var conn = new SqlConnection(configuration.GetConnectionString("Default"));
        await using var cmd = new SqlCommand();
        cmd.Connection = conn;
        await conn.OpenAsync();

        cmd.CommandText = "SELECT m.Id as MakerId, m.Name as MakerName, p.Id as ProductId, p.Name as ProductName, " +
                          "p.Description, p.StickerPrice, pt.Id as ProductTypeId, pt.Name as ProductTypeName, " +
                          "v.Code as VendorCode, v.Name as VendorName, vp.Amount, vp.PricePerUnit " +
                          "FROM Makers m " +
                          "LEFT JOIN Products p ON p.MakerId = m.Id " +
                          "LEFT JOIN ProductTypes pt ON pt.Id = p.ProductTypeId " +
                          "LEFT JOIN VendorProducts vp ON vp.ProductId = p.Id " +
                          "LEFT JOIN Vendors v ON v.Code = vp.VendorCode " +
                          "WHERE m.Id = @id";

        cmd.Parameters.AddWithValue("@id", id);

        await using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync())
        {
            if (m == null)
            {
                m = new MakerDto
                {
                    Id = r.GetInt32(0),
                    Name = r.GetString(1),
                    Products = new List<ProductDto>()
                };
            }

            if (r.IsDBNull(2)) continue;

            var pId = r.GetInt32(2);
            var prods = m.Products.ToList();
            var p = prods.FirstOrDefault(x => x.Id == pId);

            if (p == null)
            {
                p = new ProductDto
                {
                    Id = pId,
                    Name = r.GetString(3),
                    Description = r.IsDBNull(4) ? null : r.GetString(4),
                    StrickerPrice = r.GetDecimal(5),
                    ProductType = new ProductTypeDto { Id = r.GetInt32(6), Name = r.GetString(7) },
                    Vendors = new List<VendorDto>()
                };
                prods.Add(p);
                m.Products = prods;
            }

            if (r.IsDBNull(8)) continue;

            var vends = p.Vendors.ToList();
            var vCode = r.GetString(8);
            if (vends.All(x => x.Code != vCode))
            {
                vends.Add(new VendorDto
                {
                    Code = vCode,
                    Name = r.GetString(9),
                    Amount = r.GetInt32(10),
                    PricePerUnit = r.GetDecimal(11)
                });
                p.Vendors = vends;
            }
        }

        if (m == null) throw new NotFoundException($"Maker with ID {id} does not exist.");

        return m;
    }

    public async Task CreateMakerAsync(CreateMakerDto dto)
    {
        await using var conn = new SqlConnection(configuration.GetConnectionString("Default"));
        await using var cmd = new SqlCommand();
        cmd.Connection = conn;
        await conn.OpenAsync();

        var types = new Dictionary<string, int>();
        if (dto.Products != null && dto.Products.Any())
        {
            cmd.CommandText = "SELECT Id FROM ProductTypes WHERE Name = @type";
            foreach (var p in dto.Products)
            {
                if (types.ContainsKey(p.Type)) continue;

                cmd.Parameters.AddWithValue("@type", p.Type);
                var tId = await cmd.ExecuteScalarAsync();
                cmd.Parameters.Clear();

                if (tId == null) throw new NotFoundException($"Product type '{p.Type}' does not exist.");
                types[p.Type] = (int)tId;
            }
        }

        await using var tran = await conn.BeginTransactionAsync();
        cmd.Transaction = (SqlTransaction)tran;

        try
        {
            cmd.CommandText = "INSERT INTO Makers (Name) OUTPUT INSERTED.Id VALUES (@name)";
            cmd.Parameters.AddWithValue("@name", dto.Name);
            var mId = (int)(await cmd.ExecuteScalarAsync())!;
            cmd.Parameters.Clear();

            if (dto.Products != null && dto.Products.Any())
            {
                cmd.CommandText = "INSERT INTO Products (Name, Description, StickerPrice, ProductTypeId, MakerId) " +
                                  "VALUES (@pName, @pDesc, @pPrice, @pTypeId, @mMakerId)";
                foreach (var p in dto.Products)
                {
                    cmd.Parameters.AddWithValue("@pName", p.Name);
                    cmd.Parameters.AddWithValue("@pDesc", string.IsNullOrEmpty(p.Description) ? DBNull.Value : p.Description);
                    cmd.Parameters.AddWithValue("@pPrice", p.StrickerPrice);
                    cmd.Parameters.AddWithValue("@pTypeId", types[p.Type]);
                    cmd.Parameters.AddWithValue("@mMakerId", mId);
                    
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
