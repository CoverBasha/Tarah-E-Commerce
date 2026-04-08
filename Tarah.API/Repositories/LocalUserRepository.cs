using Microsoft.EntityFrameworkCore;
using Tarah.API.Data;
using Tarah.API.Models.Domain;

namespace Tarah.API.Repositories
{
    public class LocalUserRepository : ILocalUsersRepository
    {
        private readonly TarahDbContext context;

        public LocalUserRepository(TarahDbContext context)
        {
            this.context = context;
        }
        public async Task<LocalUser> UserbyId(Guid id)
        {
            return await context.LocalUsers.SingleOrDefaultAsync(u => u.Id == id);
        }
    }
}
