using QuickServe.Application.DTOs.Ingredients.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickServe.Application.DTOs.Sessions.Response
{
    public class SessionResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public List<IngredientInSessionResponse> Ingredients { get; set; }
    }
}
