using Tarah.API.Models.Domain;

namespace Tarah.API.Repositories
{
    public interface ILocalUsersRepository
    {
        public Task<LocalUser> UserbyId(Guid id);
    }
}
