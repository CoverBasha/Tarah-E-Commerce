using AutoMapper;
using Tarah.API.Models.Domain;
using Tarah.API.Models.DTOs;

namespace Tarah.API.Mappings
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(
                dest => dest.Categories,
                opt => opt.MapFrom(
                    src => src.Categories.Select(ci => ci.Category)
                ));

            CreateMap<Category, CategoryDto>();
            CreateMap<AddProductDto, Product>().ForMember(
                dest => dest.Images,
                opt => opt.Ignore());

            CreateMap<CartItem, ProductDto>().IncludeMembers(src => src.Product);

            CreateMap<Cart, CartDto>();

            CreateMap<Order, OrderDto>();

            CreateMap<OrderItem, OrderItemDto>();

            CreateMap<CartItem, CartItemDto>();
        }
    }
}
