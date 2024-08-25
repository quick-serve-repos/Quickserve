using System.Collections.Generic;

namespace QuickServe.Application.DTOs.Bill;

public class BillProductDto
{
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public List<BillIngredientDto> Ingredients { get; set; }
}