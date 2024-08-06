using System;
using System.Collections.Generic;
using QuickServe.Domain.Orders.Entities;
using QuickServe.Domain.Products.Dtos;

namespace QuickServe.Application.DTOs.Orders.Response;

public class OrderHistoryDto
{
    public string Id { get; set; }
    public double TotalPrice { get; set; }
    public string BillCode { get; set; }
    public int Status { get; set; }
    public long StoreId { get; set; }
    public DateTime Created { get; set; }


    public OrderHistoryDto(Order order)
    {
        Id = order.Id.ToString();
        TotalPrice = order.Amount;
        BillCode = order.BillCode;
        Status = order.Status;
        StoreId = order.StoreId;
        Created = order.Created;
        
    }
}