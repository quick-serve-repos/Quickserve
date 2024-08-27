using System;
using System.ComponentModel.DataAnnotations;
using QuickServe.Domain.Stores.Entities;

namespace QuickServe.Domain.Stores.Dtos
{
    public class StoreDto
    {
        public StoreDto()
        {
        }

        public StoreDto(Store store)
        {
            Id = store.Id;
            Name = store.Name;
            Address = store.Address;
            Created = store.Created;
            CreatedBy = store.CreatedBy;
            StoreManager = store.StoreManager;

        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime Created { get; set; }
        public string? StoreManager { get; set; }

    }
}