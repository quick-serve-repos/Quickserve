using FluentValidation;
using Microsoft.AspNetCore.Http;
using QuickServe.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Application.DTOs.Nutritions.Request
{
    public class CreateNutritionRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IFormFile Image { get; set; }
        public string Vitamin { get; set; }
        public string HealthValue { get; set; }

        public class CreateNutritionRequestValidator : AbstractValidator<CreateNutritionRequest>
        {
            public CreateNutritionRequestValidator(ITranslator translator)
            {
                RuleFor(x => x.Name)
                    .NotEmpty().WithMessage(translator["Tên là bắt buộc."])
                    .Must(name => char.IsUpper(name[0])).WithMessage(translator["Chữ cái đầu tiên của tên phải viết hoa"])
                    .Length(2, 40).WithMessage(translator["Tên phải từ 2 đến 40 ký tự."]);
                RuleFor(x => x.Description)
                    .NotEmpty().WithMessage(translator["Mô tả là bắt buộc."])
                    .MaximumLength(100).WithMessage(translator["Mô tả không được vượt quá 100 ký tự."]);
                RuleFor(x => x.Vitamin)
                   .NotEmpty().WithMessage(translator["Viatamin là bắt buộc."])
                   .Length(2, 40).WithMessage(translator["Tên phải từ 2 đến 40 ký tự."]);
                RuleFor(x => x.HealthValue)
                   .NotEmpty().WithMessage(translator["Tên là bắt buộc."])
                   .Length(2, 100).WithMessage(translator["Tên phải từ 2 đến 100 ký tự."]);
                RuleFor(x => x.Image)
                    .NotNull().WithMessage(translator["Ảnh là bắt buộc."])
                    .Must(BeAValidImage).WithMessage(translator["Chỉ các tệp hình ảnh được phép."])
                    .Must(BeAValidSize).WithMessage(translator["Kích thước ảnh phải nhỏ hơn 2MB."]);
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
}
