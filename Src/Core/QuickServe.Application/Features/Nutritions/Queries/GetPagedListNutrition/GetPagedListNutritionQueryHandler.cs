using MediatR;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Wrappers;
using System.Threading.Tasks;
using System.Threading;
using QuickServe.Domain.Nutritions.Dtos;

namespace QuickServe.Application.Features.Nutritions.Queries.GetPagedListNutrition;

public class GetPagedListNutritionQueryHandler(INutritionRepository nutritionRepository) : IRequestHandler<GetPagedListNutritionQuery, PagedResponse<NutritionDTO>>
{
    public async Task<PagedResponse<NutritionDTO>> Handle(GetPagedListNutritionQuery request, CancellationToken cancellationToken)
    {
        var result = await nutritionRepository.GetPagedListAsync(request.PageNumber, request.PageSize, request.Name);
        return new PagedResponse<NutritionDTO>(result, request);
    }
}

