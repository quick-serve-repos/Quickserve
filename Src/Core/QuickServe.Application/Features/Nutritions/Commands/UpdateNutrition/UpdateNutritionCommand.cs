using MediatR;
using Microsoft.AspNetCore.Http;
using QuickServe.Application.Wrappers;

namespace QuickServe.Application.Features.Nutritions.Commands.UpdateNutrition;

public class UpdateNutritionCommand : IRequest<BaseResult>
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Vitamin { get; set; }
    public string HealthValue { get; set; }
}