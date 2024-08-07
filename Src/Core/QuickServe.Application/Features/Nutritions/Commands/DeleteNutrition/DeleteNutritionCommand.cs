using MediatR;
using QuickServe.Application.Wrappers;

namespace QuickServe.Application.Features.Nutritions.Commands.DeleteNutrition;

public class DeleteNutritionCommand : IRequest<BaseResult>
{
    public long Id { get; set; }
}