using FluentValidation;
using Microsoft.AspNetCore.Http;
using QuickServe.Application.DTOs.Ingredients.Request;
using QuickServe.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Application.DTOs.Nutritions.Request
{
    public class UpdateNutritionImageRequest
    {
        public IFormFile Image { get; set; }
    }
    public class UpdateNutritionImageValidator : AbstractValidator<UpdateNutritionImageRequest>
    {
        public UpdateNutritionImageValidator(ITranslator translator)
        {
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
