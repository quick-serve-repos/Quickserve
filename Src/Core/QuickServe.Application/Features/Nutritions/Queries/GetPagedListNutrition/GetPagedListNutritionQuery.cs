using MediatR;
using QuickServe.Application.Parameters;
using QuickServe.Application.Wrappers;
using QuickServe.Domain.Nutritions.Dtos;

namespace QuickServe.Application.Features.Nutritions.Queries.GetPagedListNutrition;

public class GetPagedListNutritionQuery : PagenationRequestParameter, IRequest<PagedResponse<NutritionDTO>>
{
    public string Name { get; set; }
}