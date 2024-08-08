using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProPayments.Client.Exceptions
{
    public class UserException : Exception
    {
        public int StatusCodes { get; }

        public UserException(int statusCodes, string message) : base(message)
        {
            StatusCodes = statusCodes;
        }
    }
}
