using MassTransit;
using Microsoft.EntityFrameworkCore;
using Tarah.API.Data;
using Tarah.API.Models.Domain;
using Tarah.Contracts.Events;

namespace Tarah.API.Services
{
    public class ProfilesService
    {
        private readonly TarahDbContext context;

        public ProfilesService(TarahDbContext context)
        {
            this.context = context;
        }
        public async Task CreateProfiles(Guid id, string username)
        {
            if (context.Customers.Any(c => c.Id == id))
                return;
            if (context.Sellers.Any(c => c.Id == id))
                return;

            var customer = new CustomerProfile
            {
                Id = id,
                Cart = new Cart()
                {
                    CustomerId = id,
                },
            };
            var seller = new SellerProfile
            {
                Id = id,
            };

            await context.Customers.AddAsync(customer);
            await context.Sellers.AddAsync(seller);
            await context.LocalUsers.AddAsync(new LocalUser { Id = id, Username = username });
            await context.SaveChangesAsync();
        }

        public async Task DeleteProfiles(Guid id)
        {
            var customer = await context.Customers.SingleOrDefaultAsync(c => c.Id == id);
            if (customer == null)return;

            var seller = await context.Sellers.SingleOrDefaultAsync(s => s.Id == id);
            if (seller == null) return;

            var localUser = await context.LocalUsers.SingleOrDefaultAsync(u => u.Id == id);

            context.Customers.Remove(customer);
            context.Sellers.Remove(seller);
            context.LocalUsers.Remove(localUser);
            await context.DeletedUsers.AddAsync(new DeletedUser { UserId = id, DeletedAt = DateTime.UtcNow });
            await context.SaveChangesAsync();
        }
    }

    public class UserCreatedConsumer : IConsumer<UserCreated>
    {
        private readonly ProfilesService service;

        public UserCreatedConsumer(ProfilesService service)
        {
            this.service = service;
        }
        public async Task Consume(ConsumeContext<UserCreated> context)
        {
            await service.CreateProfiles(context.Message.UserId, context.Message.Username);
        }
    }

    public class UserDeletedConsumer : IConsumer<UserDeleted>
    {
        private readonly ProfilesService service;

        public UserDeletedConsumer(ProfilesService service)
        {
            this.service = service;
        }
        public async Task Consume(ConsumeContext<UserDeleted> context)
        {
            await service.DeleteProfiles(context.Message.UserId);
        }
    }
}
