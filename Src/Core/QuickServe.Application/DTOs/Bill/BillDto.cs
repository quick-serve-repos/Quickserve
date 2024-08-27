using System;
using System.Collections.Generic;

namespace QuickServe.Application.DTOs.Bill;

public class BillDto
{
    public string StoreName { get; set; }
    public string StoreAddress { get; set; }
    public DateTime CurrentDate { get; set; }
    public string BillNumber { get; set; } // BillCode từ bảng Order
    public long OrderId { get; set; }
    public List<BillProductDto> Products { get; set; }
    public double TotalPrice { get; set; }
    public string PaymentMethod { get; set; } // Phương thức thanh toán
    
    public int Platform { get; set; }
}