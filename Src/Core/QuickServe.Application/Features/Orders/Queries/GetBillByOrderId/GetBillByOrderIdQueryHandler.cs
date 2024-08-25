using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using MediatR;
using QuickServe.Application.DTOs.Bill;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Wrappers;

namespace QuickServe.Application.Features.Orders.Queries.GetBillByOrderId;

public class GetBillByOrderIdQueryHandler(IOrderRepository orderRepository, IPaymentRepository paymentRepository) : IRequestHandler<GetBillByOrderIdQuery, BaseResult<BillDto>>
{
    public async Task<BaseResult<BillDto>> Handle(GetBillByOrderIdQuery request, CancellationToken cancellationToken)
    {
        // Lấy thông tin đơn hàng
        var order = await orderRepository.GetByIdAsync(request.OrderId);
        if (order == null || order.Status != 2) // Kiểm tra trạng thái đơn hàng đã thanh toán
        {
            return new BaseResult<BillDto>(new Error(ErrorCode.NotFound, "Order not found or not paid"));
        }

        // Lấy thông tin phương thức thanh toán
        var payment = await paymentRepository.GetByOrderIdAsync(request.OrderId);
        // Xử lý logic paymentMethod dựa trên PaymentType
        var paymentMethod = "Unknown";
        if (payment != null)
        {
            paymentMethod = payment.PaymentType switch
            {
                1 => "Tiền mặt",           // PaymentType = 1 => Tiền mặt
                2 => "Thanh toán online",   // PaymentType = 2 => Thanh toán online
                _ => "Unknown"              // Giá trị mặc định nếu không khớp
            };
        }

        // Lấy thông tin cửa hàng
        var store = order.Store;

        // Tạo DTO cho hóa đơn
        var billDto = new BillDto
        {
            StoreName = store.Name,
            StoreAddress = store.Address,
            CurrentDate = DateTime.UtcNow.AddHours(7), // Giờ hiện tại theo UTC+7
            BillNumber = order.BillCode,
            OrderId = order.Id,
            TotalPrice = order.Amount,
            PaymentMethod = paymentMethod,
            Platform = order.Platform,
            Products = new List<BillProductDto>()
        };

        // Duyệt qua sản phẩm trong đơn hàng
        foreach (var orderProduct in order.OrderProducts)
        {
            var productDto = new BillProductDto
            {
                ProductName = orderProduct.Product.Name,
                Quantity = orderProduct.Quantity ?? 0,
                Price = orderProduct.Price,
                Ingredients = new List<BillIngredientDto>()
            };

            // Duyệt qua nguyên liệu của sản phẩm
            foreach (var ingredientProduct in orderProduct.Product.IngredientProducts)
            {
                var ingredientDto = new BillIngredientDto
                {
                    IngredientName = ingredientProduct.Ingredient.Name,
                    Quantity = ingredientProduct.Quantity,
                    Price = ingredientProduct.Ingredient.Price
                };

                productDto.Ingredients.Add(ingredientDto);
            }

            billDto.Products.Add(productDto);
        }

        return new BaseResult<BillDto>(billDto);
    }
}

/*public class GetBillByOrderIdQueryHandler : IRequestHandler<GetBillByOrderIdQuery, BaseResult<BillDto>>
{
    private readonly IOrderRepository orderRepository;
    private readonly IPaymentRepository paymentRepository;

    public GetBillByOrderIdQueryHandler(IOrderRepository orderRepository, IPaymentRepository paymentRepository)
    {
        this.orderRepository = orderRepository;
        this.paymentRepository = paymentRepository;
    }

    public async Task<BaseResult<BillDto>> Handle(GetBillByOrderIdQuery request, CancellationToken cancellationToken)
    {
        // Lấy thông tin đơn hàng
        var order = await orderRepository.GetByIdAsync(request.OrderId);
        if (order == null || order.Status != 2) // Kiểm tra trạng thái đơn hàng đã thanh toán
        {
            return new BaseResult<BillDto>(new Error(ErrorCode.NotFound, "Order not found or not paid"));

        }

        // Lấy thông tin phương thức thanh toán
        var payment = await paymentRepository.GetByOrderIdAsync(request.OrderId);
        var paymentMethod = payment?.PaymentType switch
        {
            1 => "Tiền mặt",
            2 => "Thanh toán online",
            _ => "Unknown"
        };

        // Tạo DTO cho hóa đơn
        var billDto = new BillDto
        {
            StoreName = order.Store.Name,
            StoreAddress = order.Store.Address,
            CurrentDate = DateTime.UtcNow.AddHours(7), // Giờ UTC+7
            BillNumber = order.BillCode,
            OrderId = order.Id,
            TotalPrice = order.Amount,
            PaymentMethod = paymentMethod,
            Platform = order.Platform,
            Products = order.OrderProducts.Select(op => new BillProductDto
            {
                ProductName = op.Product.Name,
                Quantity = op.Quantity ?? 0,
                Price = op.Price,
                Ingredients = op.Product.IngredientProducts.Select(ip => new BillIngredientDto
                {
                    IngredientName = ip.Ingredient.Name,
                    Quantity = ip.Quantity,
                    Price = ip.Ingredient.Price
                }).ToList()
            }).ToList()
        };

        // Xuất PDF từ billDto
        var pdfPath = await CreatePdfAsync(billDto);

        return new BaseResult<BillDto>(billDto);
    }

    private async Task<string> CreatePdfAsync(BillDto billDto)
    {
        var pdfPath = Path.Combine(Directory.GetCurrentDirectory(), $"Bill_{billDto.BillNumber}.pdf");

        using (PdfWriter writer = new PdfWriter(pdfPath))
        using (PdfDocument pdf = new PdfDocument(writer))
        {
            Document document = new Document(pdf);

            // Thêm tiêu đề hóa đơn
            document.Add(new Paragraph(billDto.StoreName).SetTextAlignment(TextAlignment.CENTER).SetBold().SetFontSize(16));
            document.Add(new Paragraph(billDto.StoreAddress).SetTextAlignment(TextAlignment.CENTER).SetFontSize(12));
            document.Add(new Paragraph($"Ngày: {billDto.CurrentDate:dd/MM/yyyy HH:mm:ss}"));
            document.Add(new Paragraph($"Số hóa đơn: {billDto.BillNumber}"));
            document.Add(new Paragraph($"Mã đơn hàng: {billDto.OrderId}"));

            // Thêm bảng sản phẩm
            Table productTable = new Table(3, false);
            productTable.AddHeaderCell("Sản phẩm");
            productTable.AddHeaderCell("Số lượng");
            productTable.AddHeaderCell("Giá (VND)");

            foreach (var product in billDto.Products)
            {
                productTable.AddCell(product.ProductName);
                productTable.AddCell(product.Quantity.ToString());
                productTable.AddCell(product.Price.ToString("N0"));

                foreach (var ingredient in product.Ingredients)
                {
                    productTable.AddCell("  - " + ingredient.IngredientName);
                    productTable.AddCell(ingredient.Quantity.ToString());
                    productTable.AddCell(ingredient.Price.ToString("N0"));
                }
            }

            document.Add(productTable);

            // Thêm tổng tiền và phương thức thanh toán
            document.Add(new Paragraph($"Tổng tiền: {billDto.TotalPrice:N0} VND").SetBold());
            document.Add(new Paragraph($"Phương thức thanh toán: {billDto.PaymentMethod}"));

            document.Close();
        }

        return pdfPath;
    }
}*/
