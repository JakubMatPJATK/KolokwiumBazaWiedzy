using Kolokwium.DTOs;
using Kolokwium.Entities;
using Kolokwium.Exceptions;
using Kolokwium.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Kolokwium.Services;

public class DbService(DatabaseContext ctx) : IDbService
{
    public async Task<IReadOnlyList<ExampleResponseDto>> GetAllExamplesAsync(
        string? name,
        CancellationToken cancellationToken)
    {
        return await ctx.Examples
            .AsNoTracking()
            .Where(e => name == null || e.Name.Contains(name))
            .Select(e => new ExampleResponseDto(e.Id, e.Name))
            .ToListAsync(cancellationToken);
    }

    public async Task<ExampleResponseDto> GetExampleAsync(int id, CancellationToken cancellationToken)
    {
        return await ctx.Examples
            .AsNoTracking()
            .Where(e => e.Id == id)
            .Select(e => new ExampleResponseDto(e.Id, e.Name))
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException($"Example with id {id} not found");
    }

    public async Task AddExampleAsync(ExampleRequest request, CancellationToken cancellationToken)
    {
        if (await ctx.Examples.AnyAsync(
                e => e.Name.Trim().ToLower() == request.Name.Trim().ToLower(),
                cancellationToken))
        {
            throw new ConflictException($"Example with name '{request.Name}' already exists");
        }

        await ctx.Examples.AddAsync(new ExampleEntity { Name = request.Name }, cancellationToken);
        await ctx.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateExampleAsync(int id, ExampleRequest request, CancellationToken cancellationToken)
    {
        if (await ctx.Examples.AnyAsync(
                e => e.Id != id && e.Name.Trim().ToLower() == request.Name.Trim().ToLower(),
                cancellationToken))
        {
            throw new ConflictException($"Example with name '{request.Name}' already exists");
        }

        var entity = await ctx.Examples.FirstOrDefaultAsync(e => e.Id == id, cancellationToken)
            ?? throw new NotFoundException($"Example with id {id} not found");

        entity.Name = request.Name;
        await ctx.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteExampleAsync(int id, CancellationToken cancellationToken)
    {
        await using var transaction = await ctx.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var removedRows = await ctx.Examples
                .Where(e => e.Id == id)
                .ExecuteDeleteAsync(cancellationToken);

            if (removedRows == 0)
                throw new NotFoundException($"Example with id {id} not found");

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
