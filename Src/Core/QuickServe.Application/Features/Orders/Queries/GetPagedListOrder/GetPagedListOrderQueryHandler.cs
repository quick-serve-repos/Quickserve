using System.Collections.Generic;
using System.Linq;
using MediatR;
using QuickServe.Application.Features.ProductTemplates.Queries.GetPagedListProductTemplate;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Wrappers;
using QuickServe.Domain.Orders.Dtos;
using QuickServe.Domain.ProductTemplates.Dtos;
using System.Threading.Tasks;
using System.Threading;
using QuickServe.Application.DTOs;
using QuickServe.Domain.Ingredients.Dtos;
using QuickServe.Domain.Products.Dtos;

namespace QuickServe.Application.Features.Orders.Queries.GetPagedListOrder;

public class GetPagedListOrderQueryHandler(IOrderRepository orderRepository) : IRequestHandler<GetPagedListOrderQuery, PagedResponse<OrderDto>>
{
    public async Task<PagedResponse<OrderDto>> Handle(GetPagedListOrderQuery request, CancellationToken cancellationToken)
    {
        var orderList = await orderRepository.GetOrderAsync(request.PageNumber, request.PageSize);
        var orderDtos = new List<OrderDto>();
        if (orderList.Data.Any())
        {
            foreach (var order in orderList.Data)
            {
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
                orderDtos.Add(orderDto);
            }
        }

        var result = new PagenationResponseDto<OrderDto>(orderDtos, request.PageSize);
        
        return new PagedResponse<OrderDto>(result, request);
    }

}