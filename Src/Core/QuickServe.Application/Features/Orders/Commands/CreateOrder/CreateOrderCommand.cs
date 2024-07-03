using FluentValidation;
using System;
using System.Collections.Generic;

namespace QuickServe.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommand
{
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public List<ProductCommand> Products { get; set; }

    public class ProductCommand
    {
        public long ProductTemplateId { get; set; }
        public int Quantity { get; set; }

        public List<IngredientCommand> Ingredients { get; set; }
    }
    public class IngredientCommand
    {
        public long Id { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}

