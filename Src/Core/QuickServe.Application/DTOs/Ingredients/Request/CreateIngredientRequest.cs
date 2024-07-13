using FluentValidation;
using Microsoft.AspNetCore.Http;
using QuickServe.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Application.DTOs.Ingredients.Request
{
    public class CreateIngredientRequest
    {
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public int DefaultQuantity { get; set; }
        public int Calo { get; set; }
        public string Description { get; set; } = null!;
        public IFormFile Image { get; set; }
        public int IngredientTypeId { get; set; }
    }
    public class AddUpdateIngredientValidator : AbstractValidator<CreateIngredientRequest>
    {
        public AddUpdateIngredientValidator(ITranslator translator)
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(translator["Tên là bắt buộc."])
                .Length(2, 60).WithMessage(translator["Tên phải từ 2 đến 60 ký tự."]);

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage(translator["Giá phải lớn hơn 0."]);
           
            RuleFor(x => x.DefaultQuantity)
               .GreaterThan(-1).WithMessage(translator["Giá phải lớn hơn -1."]);

            RuleFor(x => x.Calo)
                .GreaterThanOrEqualTo(0).WithMessage(translator["Calo phải lớn hơn hoặc bằng 0."]);

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage(translator["Mô tả là bắt buộc."])
                .MaximumLength(200).WithMessage(translator["Mô tả không được vượt quá 200 ký tự."]);

            RuleFor(x => x.Image)
                .NotNull().WithMessage(translator["Ảnh là bắt buộc."])
                .Must(BeAValidImage).WithMessage(translator["Chỉ các tệp hình ảnh được phép."])
                .Must(BeAValidSize).WithMessage(translator["Kích thước ảnh phải nhỏ hơn 2MB."]);

            RuleFor(x => x.IngredientTypeId)
                .GreaterThan(0).WithMessage(translator["Loại nguyên liệu phải lớn hơn 0."]);
        }

        private bool BeAValidImage(IFormFile file)
        {
            if (file == null) return false;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = System.IO.Path.GetExtension(file.FileName).ToLowerInvariant();

            return allowedExtensions.Contains(extension);
        }

        private bool BeAValidSize(IFormFile file)
        {
            if (file == null) return false;

            const int maxSizeInBytes = 2 * 1024 * 1024; // 2 MB
            return file.Length <= maxSizeInBytes;
        }
    }
}