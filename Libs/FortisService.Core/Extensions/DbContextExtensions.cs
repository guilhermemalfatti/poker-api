using FortisService.Core.Models.Messages;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FortisService.Core.Extensions
{
    public static class DbContextExtensions
    {
        public static async Task<CreatedResponseMessage<TEntity>> CreateAsync<TEntity>(
            this DbContext databaseContext,
            Expression<Func<TEntity, bool>> predicate,
            TEntity entry,
            CancellationToken cancellationToken)
            where TEntity : class
        {
            try
            {
                await databaseContext.Set<TEntity>().AddAsync(entry, cancellationToken).ConfigureAwait(false);
                await databaseContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                return new CreatedResponseMessage<TEntity>(entry);
            }
            catch (Exception ex) when (ex is ArgumentException || ex is DbUpdateException)
            {
                // ArgumentException or DbUpdateException is thrown if the key was already there.
                if (await databaseContext.Set<TEntity>().AnyAsync(predicate).ConfigureAwait(false))
                {

                    throw new DbUpdateException($"Entity of type, {typeof(TEntity).FullName}, already exists.");
                }

                throw;
            }
        }
    }
}
