using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProPayments.Client.Clients.ProPayments.Dtos
{
    public class ApiResponse<T>
    {
        public int StatusCode { get; set; }
        public string? ErrorMessage { get; set; }
        public T? Data { get; set; }
    }
}
