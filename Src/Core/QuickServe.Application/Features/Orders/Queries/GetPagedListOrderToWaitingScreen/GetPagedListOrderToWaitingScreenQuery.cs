using MediatR;
using QuickServe.Application.DTOs.Orders.Response;
using QuickServe.Application.Parameters;
using QuickServe.Application.Wrappers;
using QuickServe.Domain.Orders.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Application.Features.Orders.Queries.GetPagedListOrderToWaitingScreen
{
    public class GetPagedListOrderToWaitingScreenQuery : PagenationRequestParameter, IRequest<PagedResponse<OderStatusResponse>>
    {
        public int Status { get; set; }
    }
}
