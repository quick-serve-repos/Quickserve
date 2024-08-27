using MediatR;
using QuickServe.Application.Wrappers;
using QuickServe.Domain.Nutritions.Dtos;

namespace QuickServe.Application.Features.Nutritions.Queries.GetNutritionById;

public class GetNutritionByIdQuery : IRequest<BaseResult<NutritionDTO>>
{
    public long Id { get; set; }
}