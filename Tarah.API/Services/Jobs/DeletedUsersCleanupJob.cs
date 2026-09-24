using Microsoft.EntityFrameworkCore;
using System;
using Tarah.API.Data;

namespace Tarah.API.Services.Jobs
{
    public class DeletedUsersCleanupJob
    {
        private readonly TarahDbContext _context;

        public DeletedUsersCleanupJob(TarahDbContext context)
        {
            _context = context;
        }

        public async Task ExecuteAsync()
        {
            var cutoff = DateTime.UtcNow.AddHours(-2);

            await _context.DeletedUsers
                .Where(x => x.DeletedAt < cutoff)
                .ExecuteDeleteAsync();
        }
    }
}
