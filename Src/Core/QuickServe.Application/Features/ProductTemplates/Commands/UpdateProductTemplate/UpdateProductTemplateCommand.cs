using MediatR;
using Microsoft.AspNetCore.Http;
using QuickServe.Application.Wrappers;

namespace QuickServe.Application.Features.ProductTemplates.Commands.UpdateProductTemplate;

public class UpdateProductTemplateCommand : IRequest<BaseResult>
{
    public long Id { get; set; }
    public long CategoryId { get; set; }
    public string Name { get; set; } = null!;
    public string Size { get; set; }
    public string Description { get; set; } = null!;
}