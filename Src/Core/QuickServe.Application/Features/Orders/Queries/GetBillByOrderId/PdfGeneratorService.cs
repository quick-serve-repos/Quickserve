using System.IO;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using QuickServe.Application.DTOs.Bill;

namespace QuickServe.Application.Features.Orders.Queries.GetBillByOrderId;

public class PdfGeneratorService
{
    //private readonly string _fontPath = @"./DejaVuSans.ttf";  // Path to the font file
    private readonly string _fontPath = @"DejaVuSans.ttf";  // Path to the font file

    public byte[] GenerateBillPdf(BillDto bill)
    {
        using (var memoryStream = new MemoryStream())
        {
            // Initialize PDF writer
            var writer = new PdfWriter(memoryStream);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            // Correct method to create the font with embedding strategy
            PdfFont font = PdfFontFactory.CreateFont(_fontPath, PdfEncodings.IDENTITY_H, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);

            // Set font for the document
            document.SetFont(font);

            // Add store details
            document.Add(new Paragraph("**QUÁN ĂN HUẾ XƯA**")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetBold());
            document.Add(new Paragraph("Địa chỉ: Thành Phố Thủ Đức, Hồ Chí Minh")
                .SetTextAlignment(TextAlignment.CENTER));
            document.Add(new Paragraph(new string('-', 80))
                .SetTextAlignment(TextAlignment.CENTER));

            // Add Bill Information (center aligned)
            document.Add(new Paragraph("HÓA ĐƠN BÁN HÀNG")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetBold());
            document.Add(new Paragraph($"Ngày: {bill.CurrentDate:dd/MM/yyyy - HH:mm:ss}")
                .SetTextAlignment(TextAlignment.CENTER));
            document.Add(new Paragraph($"Số hóa đơn: {bill.BillNumber}")
                .SetTextAlignment(TextAlignment.CENTER));
            document.Add(new Paragraph($"Mã đơn hàng: {bill.OrderId}")
                .SetTextAlignment(TextAlignment.CENTER));
            document.Add(new Paragraph(new string('-', 80))
                .SetTextAlignment(TextAlignment.CENTER));

            // Create a table for the product details (3 columns: Product, SL, Price) without borders
            float[] columnWidths = { 5, 1, 3 }; // Adjust the width ratios for the table
            Table table = new Table(UnitValue.CreatePercentArray(columnWidths)).UseAllAvailableWidth();
            table.SetBorder(Border.NO_BORDER);  // Remove table borders

            // Add header row
            table.AddHeaderCell(new Paragraph("SẢN PHẨM").SetBold().SetBorder(Border.NO_BORDER));
            table.AddHeaderCell(new Paragraph("SL").SetBold().SetTextAlignment(TextAlignment.CENTER).SetBorder(Border.NO_BORDER));
            table.AddHeaderCell(new Paragraph("GIÁ (VND)").SetBold().SetTextAlignment(TextAlignment.RIGHT).SetBorder(Border.NO_BORDER));

            // Add product rows without borders
            foreach (var product in bill.Products)
            {
                table.AddCell(new Paragraph(product.ProductName).SetBorder(Border.NO_BORDER));
                table.AddCell(new Paragraph($"{product.Quantity}").SetTextAlignment(TextAlignment.CENTER).SetBorder(Border.NO_BORDER));
                table.AddCell(new Paragraph($"{product.Price:N0}").SetTextAlignment(TextAlignment.RIGHT).SetBorder(Border.NO_BORDER));

                // Add ingredients indented to the right, and slightly adjust the alignment
                foreach (var ingredient in product.Ingredients)
                {
                    table.AddCell(new Paragraph($"     * {ingredient.IngredientName}").SetBorder(Border.NO_BORDER));
                    table.AddCell(new Paragraph($"{ingredient.Quantity}").SetTextAlignment(TextAlignment.CENTER).SetBorder(Border.NO_BORDER));
                    table.AddCell(new Paragraph($"{ingredient.Price:N0}").SetTextAlignment(TextAlignment.RIGHT).SetBorder(Border.NO_BORDER));
                }
            }

            document.Add(table);

            // Add total price section (centered)
            document.Add(new Paragraph(new string('-', 80))
                .SetTextAlignment(TextAlignment.CENTER));
            document.Add(new Paragraph($"TỔNG TIỀN: {bill.TotalPrice:N0} VND")
                .SetBold()
                .SetTextAlignment(TextAlignment.CENTER));
            document.Add(new Paragraph($"Phương thức thanh toán: {bill.PaymentMethod}")
                .SetTextAlignment(TextAlignment.CENTER));

            // Add footer (centered)
            document.Add(new Paragraph(new string('-', 80))
                .SetTextAlignment(TextAlignment.CENTER));
            document.Add(new Paragraph("CẢM ƠN QUÝ KHÁCH! HẸN GẶP LẠI!")
                .SetTextAlignment(TextAlignment.CENTER));

            // Close document
            document.Close();

            return memoryStream.ToArray();
        }
    }
}
