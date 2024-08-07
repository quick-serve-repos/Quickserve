using FluentValidation;
using QuickServe.Application.Interfaces;
using System;

namespace QuickServe.Application.Features.Sessions.Commands.CreateSession;

public class CreateSessionCommandValidator : AbstractValidator<CreateSessionCommand>
{
    public CreateSessionCommandValidator(ITranslator translator)
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(translator["Tên là bắt buộc"])
            .NotNull().WithMessage(translator["Tên là bắt buộc"])
            .MaximumLength(40).WithMessage(translator["Tên không được vượt quá 40 ký tự"])
            .Must(name => char.IsUpper(name[0])).WithMessage(translator["Chữ cái đầu tiên của tên phải là chữ in hoa"])
            .WithName(p => translator[nameof(p.Name)]);

        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage(translator["Thời gian bắt đầu là bắt buộc"])
            .Must(BeValidTime).WithMessage(translator["Thời gian bắt đầu không hợp lệ"])
            .WithName(p => translator[nameof(p.StartTime)]);

        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage(translator["Thời gian kết thúc là bắt buộc"])
            .Must(BeValidTime).WithMessage(translator["Thời gian kết thúc không hợp lệ"])
            .WithName(p => translator[nameof(p.EndTime)]);
    }

    // Kiểm tra xem chuỗi thời gian có đúng định dạng không (hh:mm:ss)
    private bool BeValidTime(string time)
    {
        return TimeSpan.TryParse(time, out _);
    }
}