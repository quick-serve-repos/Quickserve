﻿using System.Collections.Generic;
using QuickServe.Domain.Common;
using QuickServe.Domain.IngredientTypeTemplateSteps.Entities;
using QuickServe.Domain.ProductTemplates.Entities;

namespace QuickServe.Domain.TemplateSteps.Entities
{
    public class TemplateStep : AuditableBaseEntity
    {
        public TemplateStep()
        {
            IngredientTypeTemplateSteps = new HashSet<IngredientTypeTemplateStep>();
        }
        public TemplateStep(string name)
        {
            Name = name;
            IngredientTypeTemplateSteps = new HashSet<IngredientTypeTemplateStep>();
        }

        public void Update(string name)
        {
            Name = name;

        }
        public void Update(int status)
        {
            Status = status;

        }

        public long ProductTemplateId { get; set; }
        public string Name { get; set; } = null!;
        public int Status { get; set; }


        public virtual ProductTemplate ProductTemplate { get; set; } = null!;
        public virtual ICollection<IngredientTypeTemplateStep> IngredientTypeTemplateSteps { get; set; }
    }
}