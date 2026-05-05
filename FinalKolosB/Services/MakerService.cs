using Microsoft.Data.SqlClient;
using KolosB.DTOs;
using KolosB.Exceptions;

namespace KolosB.Services;

public class MakerService(IConfiguration configuration) : IMakerService
{
    public async Task<IEnumerable<MakerDto>> GetMakersAsync(string? name)
    {
        var data = new Dictionary<int, MakerDto>();

        await using var sqlConn = new SqlConnection(configuration.GetConnectionString("Default"));
        await using var sqlCmd = new SqlCommand();
        sqlCmd.Connection = sqlConn;
        await sqlConn.OpenAsync();

        sqlCmd.CommandText = "SELECT m.Id as MakerId, m.Name as MakerName, " +
                             "p.Id as ProductId, p.Name as ProductName, p.Description, p.StickerPrice, " +
                             "pt.Id as ProductTypeId, pt.Name as ProductTypeName, " +
                             "v.Code as VendorCode, v.Name as VendorName, vp.Amount, vp.PricePerUnit " +
                             "FROM Makers m " +
                             "LEFT JOIN Products p ON p.MakerId = m.Id " +
                             "LEFT JOIN ProductTypes pt ON pt.Id = p.ProductTypeId " +
                             "LEFT JOIN VendorProducts vp ON vp.ProductId = p.Id " +
                             "LEFT JOIN Vendors v ON v.Code = vp.VendorCode " +
                             "WHERE @name IS NULL OR m.Name LIKE '%' + @name + '%'";

        sqlCmd.Parameters.AddWithValue("@name", string.IsNullOrEmpty(name) ? DBNull.Value : name);

        await using var r = await sqlCmd.ExecuteReaderAsync();
        while (await r.ReadAsync())
        {
            var id = r.GetInt32(0);
            if (!data.ContainsKey(id))
            {
                data.Add(id, new MakerDto
                {
                    Id = id,
                    Name = r.GetString(1),
                    Products = new List<ProductDto>()
                });
            }

            if (r.IsDBNull(2)) continue;

            var products = data[id].Products.ToList();
            var pId = r.GetInt32(2);
            var prod = products.FirstOrDefault(x => x.Id == pId);

            if (prod == null)
            {
                prod = new ProductDto
                {
                    Id = pId,
                    Name = r.GetString(3),
                    Description = r.IsDBNull(4) ? null : r.GetString(4),
                    StrickerPrice = r.GetDecimal(5),
                    ProductType = new ProductTypeDto { Id = r.GetInt32(6), Name = r.GetString(7) },
                    Vendors = new List<VendorDto>()
                };
                products.Add(prod);
                data[id].Products = products;
            }

            if (r.IsDBNull(8)) continue;

            var vendors = prod.Vendors.ToList();
            var vCode = r.GetString(8);
            if (vendors.All(x => x.Code != vCode))
            {
                vendors.Add(new VendorDto
                {
                    Code = vCode,
                    Name = r.GetString(9),
                    Amount = r.GetInt32(10),
                    PricePerUnit = r.GetDecimal(11)
                });
                prod.Vendors = vendors;
            }
        }

        return data.Values;
    }

    public async Task CreateMakerAsync(CreateMakerDto dto)
    {
        await using var sqlConn = new SqlConnection(configuration.GetConnectionString("Default"));
        await using var sqlCmd = new SqlCommand();
        sqlCmd.Connection = sqlConn;
        await sqlConn.OpenAsync();

        var typesDict = new Dictionary<string, int>();
        if (dto.Products != null && dto.Products.Any())
        {
            sqlCmd.CommandText = "SELECT Id FROM ProductTypes WHERE Name = @type";
            foreach (var p in dto.Products)
            {
                if (typesDict.ContainsKey(p.Type)) continue;

                sqlCmd.Parameters.AddWithValue("@type", p.Type);
                var tId = await sqlCmd.ExecuteScalarAsync();
                sqlCmd.Parameters.Clear();

                if (tId == null) throw new NotFoundException($"Product type '{p.Type}' does not exist.");
                typesDict[p.Type] = (int)tId;
            }
        }

        await using var tran = await sqlConn.BeginTransactionAsync();
        sqlCmd.Transaction = (SqlTransaction)tran;

        try
        {
            sqlCmd.CommandText = "INSERT INTO Makers (Name) OUTPUT INSERTED.Id VALUES (@name)";
            sqlCmd.Parameters.AddWithValue("@name", dto.Name);
            var mId = (int)(await sqlCmd.ExecuteScalarAsync())!;
            sqlCmd.Parameters.Clear();

            if (dto.Products != null && dto.Products.Any())
            {
                sqlCmd.CommandText = "INSERT INTO Products (Name, Description, StickerPrice, ProductTypeId, MakerId) " +
                                     "VALUES (@pName, @pDesc, @pPrice, @pTypeId, @mMakerId)";
                foreach (var p in dto.Products)
                {
                    sqlCmd.Parameters.AddWithValue("@pName", p.Name);
                    sqlCmd.Parameters.AddWithValue("@pDesc", string.IsNullOrEmpty(p.Description) ? DBNull.Value : p.Description);
                    sqlCmd.Parameters.AddWithValue("@pPrice", p.StrickerPrice);
                    sqlCmd.Parameters.AddWithValue("@pTypeId", typesDict[p.Type]);
                    sqlCmd.Parameters.AddWithValue("@mMakerId", mId);
                    
                    await sqlCmd.ExecuteNonQueryAsync();
                    sqlCmd.Parameters.Clear();
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
