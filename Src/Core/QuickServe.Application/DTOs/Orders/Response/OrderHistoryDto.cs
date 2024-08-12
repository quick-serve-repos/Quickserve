using System;
using System.Collections.Generic;
using System.Linq;
using QuickServe.Domain.Orders.Entities;
using QuickServe.Domain.Products.Dtos;

namespace QuickServe.Application.DTOs.Orders.Response;

public class OrderHistoryDto
{
    public string Id { get; set; }
    public double TotalPrice { get; set; }
    public int Status { get; set; }
    public string StoreName { get; set; }
    public DateTime Created { get; set; }
    public string Image { get; set; }

    public OrderHistoryDto(Order order)
    {
        Id = order.Id.ToString();
        TotalPrice = order.Amount;
        Status = order.Status;
        StoreName = order.Store.Name; // Lấy tên cửa hàng từ đối tượng Store
        Created = order.Created;
        Image = order.OrderProducts.FirstOrDefault()?.Product?.ProductTemplate?.ImageUrl; // Lấy hình ảnh từ ProductTemplate
    }
}
