using FortisService.Core.Exceptions;
using FortisService.Core.Models.Messages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace FortisService.Core.Extensions
{
    public static class DbContextExtensions
    {

        public static async Task<TEntity> GetOrThrowAsync<TEntity>(
            this DbContext databaseContext,
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var result = await databaseContext.Set<TEntity>()
                    .Where(predicate)
                    .FirstOrDefaultAsync(cancellationToken)
                    .ConfigureAwait(false);

            if (result != null)
                return result;

            var message = $"Requested {typeof(TEntity).Name} unique identifier is not found";            

            throw new NotFoundFortisException(message);
        }

        public static async Task<ObjectResponseMessage<TEntity>> CreateAsync<TEntity>(
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
                    throw new ConflictFortisException($"Entity of type, {typeof(TEntity).FullName}, already exists.");
                }
                throw;
            }
        }
    }
}
