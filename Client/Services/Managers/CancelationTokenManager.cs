using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProPayments.Client.Services.Managers
{
    public class CancelationTokenManager
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        public CancellationToken Token => _cancellationTokenSource.Token;
        public void Cancel()
        {
            _cancellationTokenSource.Cancel();
        }

        public void Dispose()
        {
            _cancellationTokenSource.Dispose();
        }

    }
}
