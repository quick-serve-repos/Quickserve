using QuickServe.Application.DTOs;
using QuickServe.Application.Parameters;
using System.Collections.Generic;

namespace QuickServe.Application.Wrappers
{
    public class PagedResponse<T> : BaseResult<List<T>>
    {
        public int PageNumber { get; }
        public int PageSize { get; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
        public PagedResponse(Domain.Orders.Dtos.OrderDto result)
        {

        }
        public PagedResponse(Error error) : base(error)
        {
        }

        public PagedResponse(PagenationResponseDto<T> model, PagenationRequestParameter request)
        {
            PageNumber = request.PageNumber;
            PageSize = request.PageSize;
            TotalItems = model.Count;
            TotalPages = TotalItems / PageSize;
            if (TotalItems % PageSize > 0) TotalPages++;

            this.Data = model.Data;
            this.Success = true;
        }
        public PagedResponse(PagenationResponseDto<T> model, int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalItems = model.Count;
            TotalPages = TotalItems / PageSize;
            if (TotalItems % PageSize > 0) TotalPages++;

            this.Data = model.Data;
            this.Success = true;
        }
    }

    
}
