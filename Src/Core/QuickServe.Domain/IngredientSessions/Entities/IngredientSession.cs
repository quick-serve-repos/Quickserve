using QuickServe.Domain.Common;
using QuickServe.Domain.Ingredients.Entities;
using QuickServe.Domain.Sessions.Entities;

namespace QuickServe.Domain.IngredientSessions.Entities
{
    public class IngredientSession: AuditableBaseEntity
    {
        public long IngredientId { get; set; }
        public long SessionId { get; set; }
        public int Quantity { get; set; }
        public int SoldQuantity { get; set; }

        public virtual Ingredient Ingredient { get; set; } = null!;
        public virtual Session Session { get; set; } = null!;
    }
}
