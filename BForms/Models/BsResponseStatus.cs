using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BForms.Models
{
    public enum BsResponseStatus
    {
        Success = 1,
        ValidationError = 2,
        AccessDenied = 3,
        ServerError = 4
    }
}
