using FluentValidation;
using Microsoft.AspNetCore.Http;
using QuickServe.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Application.DTOs.ProductTemplates
{
    public class CreateProductTemplateRequest
    {
        public long CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public string Size { get; set; }
        public IFormFile Image { get; set; }
        public string Description { get; set; } = null!;
    }

    public class CreateProductTemplateValidator : AbstractValidator<CreateProductTemplateRequest>
    {
        public CreateProductTemplateValidator(ITranslator translator)
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(translator["Tên là bắt buộc."])
                .Length(2, 40).WithMessage(translator["Tên phải từ 2 đến 40 ký tự."]);


            RuleFor(x => x.Description)
                .NotEmpty().WithMessage(translator["Mô tả là bắt buộc."])
                .MaximumLength(200).WithMessage(translator["Mô tả không được vượt quá 200 ký tự."]);

            RuleFor(x => x.Image)
                .NotNull().WithMessage(translator["Ảnh là bắt buộc."])
                .Must(BeAValidImage).WithMessage(translator["Chỉ các tệp ảnh được phép."])
                .Must(BeAValidSize).WithMessage(translator["Kích thước ảnh phải nhỏ hơn 2MB."]);

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage(translator["CategoryId phải lớn hơn 0."]);

            RuleFor(x => x.Size)
                  .NotEmpty().WithMessage(translator["Kích thước là bắt buộc."])
                  .NotNull().WithMessage(translator["Kích thước là bắt buộc."]) 
                  .MaximumLength(10).WithMessage(translator["Kích thước không được vượt quá 10 ký tự."]);
                 
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
