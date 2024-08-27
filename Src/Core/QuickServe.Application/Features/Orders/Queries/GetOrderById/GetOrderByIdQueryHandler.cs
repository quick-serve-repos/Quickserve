using System.Collections.Generic;
using QuickServe.Application.Features.ProductTemplates.Queries.GetProductTemplateById;
using QuickServe.Application.Helpers;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Interfaces;
using QuickServe.Application.Wrappers;
using QuickServe.Domain.ProductTemplates.Dtos;
using System.Threading.Tasks;
using System.Threading;
using MediatR;
using QuickServe.Application.Features.ProductTemplates.Queries.GetPagedListProductTemplate;
using QuickServe.Domain.Ingredients.Dtos;
using QuickServe.Domain.Orders.Dtos;
using QuickServe.Domain.Products.Dtos;

namespace QuickServe.Application.Features.Orders.Queries.GetOrderById;

public class GetOrderByIdQueryHandler(IOrderRepository orderRepository) : IRequestHandler<GetOrderByIdQuery, BaseResult<OrderDto>>
{
    public async Task<BaseResult<OrderDto>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(request.Id);
        if (order is null)
        {
            return new BaseResult<OrderDto>(new Error(ErrorCode.NotFound));
        }

        var orderDto = new OrderDto(order);
        var productList = new List<ProDuctsDto>();
        foreach (var item in order.OrderProducts)
        {
            if(item.Product == null) 
                continue;

            var productDto = new ProDuctsDto(item.Product);
            var ingredientList = new List<IngredientDTO>();
            foreach (var obj in item.Product.IngredientProducts)
            {
                if(obj == null) continue;
                ingredientList.Add(new IngredientDTO(obj.Ingredient));
            }

            productDto.Ingredients = ingredientList;
            productList.Add(productDto);
        }

        orderDto.Products = productList;
        return new BaseResult<OrderDto>(orderDto);
    }
}