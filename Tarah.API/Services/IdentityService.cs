using AutoMapper;
using System.Net;
using System.Net.Http.Headers;
using Tarah.API.Models.DTOs;
using Tarah.API.Repositories;

namespace Tarah.API.Services
{
    public class IdentityService
    {
        private readonly HttpClient client;
        private readonly IProductsRepository productsRepository;
        private readonly IHttpContextAccessor accessor;
        private readonly IMapper mapper;
        private readonly ILocalUsersRepository localUsersRepository;
        const string HOST = "https://localhost:7112";

        public IdentityService(HttpClient client, IProductsRepository productsRepository, IHttpContextAccessor httpContext, IMapper mapper,ILocalUsersRepository localUsersRepository)
        {
            this.client = client;
            this.productsRepository = productsRepository;
            this.accessor = httpContext;
            this.mapper = mapper;
            this.localUsersRepository = localUsersRepository;
        }

        public async Task<ServiceResponse<UserDto>> GetProfile(Guid id, string? requesterId, int pageNumber, int pageSize)
        {
            var isOwner = !string.IsNullOrWhiteSpace(requesterId)
                          && id.ToString() == requesterId;

            if (isOwner)
                return await OwnProfile(id, pageNumber, pageSize);

            return await UserProfile(id, pageNumber, pageSize);
        }

        private async Task<ServiceResponse<UserDto>> OwnProfile(Guid id, int pageNumber, int pageSize)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{HOST}/api/Users/profile");

            var token = accessor.HttpContext?.Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrWhiteSpace(token))
                request.Headers.Authorization = AuthenticationHeaderValue.Parse(token);

            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                    return new ServiceResponse<UserDto>
                    {
                        Status = Status.Unauthorized,
                        Message = "Unauthorized"
                    };
                if (response.StatusCode == HttpStatusCode.NotFound)
                    return new ServiceResponse<UserDto>
                    {
                        Status = Status.NotFound,
                        Message = "Not found"
                    };
                if (response.StatusCode == HttpStatusCode.Forbidden)
                    return new ServiceResponse<UserDto>
                    {
                        Status = Status.Forbidden,
                        Message = "Forbidden"
                    };
            }

            var dto = await response.Content.ReadFromJsonAsync<UserDto>();

            if (dto is null)
                return new ServiceResponse<UserDto>
                {
                    Status = Status.Forbidden,
                    Message = "Invalid response"
                };

            dto.IsFull = true;

            var productsResult = await productsRepository.ProductsByUserAsync(id, pageNumber, pageSize);

            dto.ListProductsDto = new ListProductsDto
            {
                Dtos = mapper.Map<List<ProductDto>>(productsResult.Items),
                Count = productsResult.TotalPages
            };

            return new ServiceResponse<UserDto>
            {
                Status = Status.Success,
                Result = dto
            };
        }

        private async Task<ServiceResponse<UserDto>> UserProfile(Guid id, int pageNumber, int pageSize)
        {
            var user = await localUsersRepository.UserbyId(id);

            if (user == null)
                return new ServiceResponse<UserDto>
                {
                    Status = Status.NotFound,
                    Message = "Not found"
                };

            var dto = new UserDto
            {
                Id = id,
                IsFull = false,
                Username = user.Username,
            };

            var productsResult = await productsRepository.ProductsByUserAsync(id, pageNumber, pageSize);

            dto.ListProductsDto = new ListProductsDto
            {
                Dtos = mapper.Map<List<ProductDto>>(productsResult.Items),
                Count = productsResult.TotalPages
            };

            return new ServiceResponse<UserDto>
            {
                Status = Status.Success,
                Result = dto
            };
        }

    }
}
