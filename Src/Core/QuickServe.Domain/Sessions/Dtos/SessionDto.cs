using QuickServe.Domain.Sessions.Entities;
using QuickServe.Domain.Stores.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuickServe.Domain.Sessions.Dtos
{
    public class SessionDto
    {
        public SessionDto() { }

        public SessionDto(Session session) {
            Id = session.Id;
            Name = session.Name;
            StoreId = session.StoreId;
            StartTime = session.StartTime;
            EndTime = session.EndTime;
            Status = session.Status;
            Created = session.Created;
            CreatedBy = session.CreatedBy;
            LastModified = session.LastModified;
            LastModifiedBy = session.LastModifiedBy;
        }

        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public long StoreId { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int Status { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime Created { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModified { get; set; }
        public virtual Store Store { get; set; } = null!;
    }
}
