using AutoMapper;
using Tarah.API.Models.DTOs;
using Tarah.API.Repositories;

namespace Tarah.API.Services
{
    public class OrdersService
    {
        private readonly IOrdersRepository repository;
        private readonly IMapper mapper;

        public OrdersService(IOrdersRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }
        public async Task<ServiceResponse<List<OrderDto>>> AllOrders(Guid userId, int page, int pageSize)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Min(pageSize, 20);


            var orders = await repository.AllOrders(userId, page, pageSize);

            var dtos = mapper.Map<List<OrderDto>>(orders);

            return new ServiceResponse<List<OrderDto>>()
            {
                Status = Status.Success,
                Result = dtos
            };
        }

        public async Task<ServiceResponse<OrderDto>> OrderById(Guid userId, Guid id)
        {
            var order = await repository.OrderById(userId, id);

            if (order is null)
                return new ServiceResponse<OrderDto>()
                {
                    Status = Status.NotFound,
                    Message = "Order not found"
                };

            var dto = mapper.Map<OrderDto>(order);

            return new ServiceResponse<OrderDto>()
            {
                Status = Status.Success,
                Result = dto,
            };
        }
    }
}
