using System;
using System.Collections.Generic;
using System.Data;
using QuickServe.Domain.Common;
using QuickServe.Domain.IngredientSessions.Entities;
using QuickServe.Domain.Stores.Entities;

namespace QuickServe.Domain.Sessions.Entities
{
    public class Session : AuditableBaseEntity
    {
        public Session()
        {
            IngredientSessions = new HashSet<IngredientSession>();
        }
        public Session(string name, long storeId, TimeSpan startTime, TimeSpan endTime, 
            int status, Store store, ICollection<IngredientSession> ingredientSessions)
        {
            Name = name;
            StoreId = storeId;
            StartTime = startTime;
            EndTime = endTime;
            Status = status;
            Store = store;
            IngredientSessions = new HashSet<IngredientSession>();
        }
        public void Update(string name, TimeSpan startTime, TimeSpan endTime)
        {
            Name = name;
            StartTime = startTime;
            EndTime = endTime;
        }
        public void Update(int status)
        {
            Status = status;
        }
        public string Name { get; set; } = null!;
        public long StoreId { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int Status { get; set; }

        public virtual Store Store { get; set; } = null!;
        public virtual ICollection<IngredientSession> IngredientSessions { get; set; }
    }
}