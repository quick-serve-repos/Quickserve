using Microsoft.EntityFrameworkCore;
using QuickServe.Application.DTOs;
using QuickServe.Application.Interfaces.Repositories;
using QuickServe.Application.Utils;
using QuickServe.Application.Utils.Enums;
using QuickServe.Domain.ProductTemplates.Entities;
using QuickServe.Domain.TemplateSteps.Dtos;
using QuickServe.Domain.TemplateSteps.Entities;
using QuickServe.Infrastructure.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Infrastructure.Persistence.Repositories
{
    public class TemplateStepRepository : GenericRepository<TemplateStep>, ITemplateStepRepository
    {
        private readonly DbSet<TemplateStep> templateSteps;
        private readonly DbSet<ProductTemplate> productTemlates;

        public TemplateStepRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            templateSteps = dbContext.Set<TemplateStep>();
            productTemlates = dbContext.Set<ProductTemplate>();
        }
        public async Task<bool> ExistNameAsync(long productTemplateId, string name)
        {
            return await templateSteps.AnyAsync(t => t.Name.ToLower() == name.ToLower()
            && t.ProductTemplateId == productTemplateId);
        }

        public async Task<TemplateStep> FindByIdAsync(long id)
        {
            return await templateSteps.Include(ts=> ts.ProductTemplate)
                .Include(ts=> ts.IngredientTypeTemplateSteps)
                .ThenInclude(it=> it.IngredientType)
                .FirstOrDefaultAsync(ts=> ts.Id == id);
        }

        public async Task<PagenationResponseDto<TemplateStepDTO>> GetPagedListAsync(long productTemplateId, int pageNumber, int pageSize, string name)
        {
            if (await productTemlates.AnyAsync(p => p.Id == productTemplateId) == false){
                throw new Exception("Không tìm thấy sản phẩm mẫu");
            }
            var query = templateSteps.Where(t=> t.ProductTemplateId == productTemplateId)
                .OrderByDescending(c => c.Created).AsQueryable();
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.Name.Contains(name));
            }

            return await Paged(
                query.Select(c => new TemplateStepDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Status = c.Status,
                    Created = TimeZoneConverter.ConvertToUserTimeZone(c.Created),
                    CreatedBy = c.CreatedBy,
                    ProductTemplateId = productTemplateId,
                    LastModified = c.LastModified.HasValue
                    ? TimeZoneConverter.ConvertToUserTimeZone(c.LastModified.Value)
                    : (DateTime?)null,  // Xử lý giá trị NULL
                    LastModifiedBy = c.LastModifiedBy ?? null // Xử lý giá trị NULL
                }),
                pageNumber,
                pageSize);
        }

        public async Task<PagenationResponseDto<TemplateStepDTO>> GetPagedListByAcitveStatusAsync(long productTemplateId, int pageNumber, int pageSize, string name)
        {
            var query = templateSteps.Where(t => t.ProductTemplateId == productTemplateId
            && t.Status == (int) TemplateStepStatus.Active)
                .OrderBy(c => c.Created).AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.Name.Contains(name));
            }


            return await Paged(
                query.Select(c => new TemplateStepDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Status = c.Status,
                    Created = TimeZoneConverter.ConvertToUserTimeZone(c.Created),
                    CreatedBy = c.CreatedBy,
                    ProductTemplateId = productTemplateId,
                    LastModified = c.LastModified.HasValue
                    ? TimeZoneConverter.ConvertToUserTimeZone(c.LastModified.Value)
                    : (DateTime?)null,  // Xử lý giá trị NULL
                    LastModifiedBy = c.LastModifiedBy ?? null // Xử lý giá trị NULL
                }),
                pageNumber,
                pageSize);
        }
    }
}
