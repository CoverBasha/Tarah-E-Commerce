using AutoMapper;
using Tarah.API.Models.Domain;
using Tarah.API.Models.DTOs;
using Tarah.API.Repositories;

namespace Tarah.API.Services
{
    public class CartsService
    {
        private readonly IProductsRepository productsRepository;
        private readonly ICartsRepository cartsRepository;
        private readonly IMapper mapper;
        private readonly ProfilesService service;
        private readonly CheckoutTransaction checkoutTransaction;

        public CartsService(IProductsRepository productsRepository,ICartsRepository cartsRepository,IMapper mapper, ProfilesService service,
            CheckoutTransaction checkoutTransaction)
        {
            this.productsRepository = productsRepository;
            this.cartsRepository = cartsRepository;
            this.mapper = mapper;
            this.service = service;
            this.checkoutTransaction = checkoutTransaction;
        }

        private async Task<Cart> GetCart(Guid userId)
        {
            var cart = await cartsRepository.GetCart(userId);
            if (cart is null)
            {
                await service.CreateProfiles(userId);
                return await cartsRepository.GetCart(userId);
            }
            return cart;
        }

        private async Task<Cart> GetFullCart(Guid userId)
        {
            var cart = await cartsRepository.GetFullCart(userId);
            if (cart is null)
            {
                await service.CreateProfiles(userId);
                return await cartsRepository.GetFullCart(userId);
            }
            return cart;
        }

        public async Task<ServiceResponse<CartDto>> GetCartDto(Guid userId)
        {
            var cart = await GetFullCart(userId);

            var dto = mapper.Map<CartDto>(cart);
            dto.TotalPrice = dto.Items.Sum(i => i.Product.IsOnSale ? i.Product.PriceAfterSale * i.Quantity : i.Product.Price * i.Quantity);

            return new ServiceResponse<CartDto>
            {
                Status = Status.Success,
                Result = dto
            };
        }

        public async Task<ServiceResponse<bool>> AddToCart(Guid productId, int quantity, Guid userId)
        {
            if (quantity < 1)
                return new ServiceResponse<bool> { Status = Status.Forbidden, Message = "Quantity can't be less than 1" };
            var product = await productsRepository.GetByIdAsync(productId);
            if (product is null)
                return new ServiceResponse<bool> { Status = Status.NotFound, Message = "Product not found" };
            if (product.Stock < quantity)
                return new ServiceResponse<bool> { Status = Status.Forbidden, Message = "Quantity exceeds stock" };

            var cart = await GetCart(userId);
            var item = cart.Items.SingleOrDefault(i => i.ProductId == productId);

            if (item is null)
            {
                await cartsRepository.AddToCart(cart, product, quantity);
                return new ServiceResponse<bool>
                {
                    Status = Status.Success,
                    Result = true,
                    Message = "Product added successfully"
                };
            }

            var success = await cartsRepository.ModifyItemCount(cart, product, item.Quantity + quantity);

            return new ServiceResponse<bool>
            {
                Status = success ? Status.Success : Status.Forbidden,
                Result = success,
                Message = success ? "Product modified successfully" : "Cart not found"
            };
        }

        public async Task<ServiceResponse<bool>> ModifyProductCount(Guid productId, int quantity, Guid userId)
        {
            var product = await productsRepository.GetByIdAsync(productId);

            if (product is null)
                return new ServiceResponse<bool> { Status = Status.NotFound, Message = "Product not found" };
            if (quantity > product.Stock)
                return new ServiceResponse<bool> { Status = Status.Forbidden, Message = "Quantity exceeds stock" };
            if (quantity + product.Stock < 1)
                return new ServiceResponse<bool> { Status = Status.Forbidden, Message = "Quantity can't be less than 1" };

            var cart = await cartsRepository.GetCart(userId);
            var success = await cartsRepository.ModifyItemCount(cart, product, quantity);

            return new ServiceResponse<bool>
            {
                Status = success ? Status.Success : Status.Forbidden,
                Result = success,
                Message = success ? "Item modified" : "Product doesn't exist in your cart"
            };
        }

        public async Task<ServiceResponse<bool>> RemoveFromCart(Guid userId, Guid productId)
        {
            var product = await productsRepository.GetByIdAsync(productId);

            if (product is null)
                return new ServiceResponse<bool> { Status = Status.NotFound, Message = "Product not found" };

            var cart = await GetCart(userId);

            var result = await cartsRepository.RemoveItem(cart, product);

            return new ServiceResponse<bool>
            {
                Status = result ? Status.Success : Status.NotFound,
                Message = result ? "Item deleted" : "Product not in cart",
                Result = result
            };
        }

        public async Task<ServiceResponse<bool>> ClearCart(Guid userId)
        {
            var cart = await GetCart(userId);
            await cartsRepository.ClearCart(cart);

            return new ServiceResponse<bool>
            {
                Status = Status.Success,
                Result = true,
                Message = "Cart cleared"
            };
        }

        public async Task<ServiceResponse<OrderDto>> Checkout(Guid userId)
        {
            Order order;
            try
            {
                order = await checkoutTransaction.Execute(userId);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<OrderDto>
                {
                    Status = Status.Forbidden,
                    Message = ex.Message,
                };
            }

            var orderDto = mapper.Map<OrderDto>(order);

            return new ServiceResponse<OrderDto>
            {
                Status = Status.Success,
                Result = orderDto
            };
        }
    }
}
