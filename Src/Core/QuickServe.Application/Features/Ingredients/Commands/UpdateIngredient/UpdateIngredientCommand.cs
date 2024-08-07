using MediatR;
using QuickServe.Application.Wrappers;

namespace QuickServe.Application.Features.Ingredients.Commands.UpdateIngredient;

public class UpdateIngredientCommand : IRequest<BaseResult>
{
    public long Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Calo { get; set; }
    public int DefaultQuantity { get; set; }
    public int QuantityMax { get; set; }
    public string Description { get; set; } 
    public long IngredientTypeId { get; set; }
}