using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backbone.Core.Models
{
    public class ApiResponse
    {
        public string Message { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.Now;
        public int Status { get; set; }
        public string Code { get; set; }
    }

    public class ApiResponse<TDto> : ApiResponse
    {
        public TDto Data { get; set; }
    }
}
