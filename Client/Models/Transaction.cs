namespace ProPayments.Client.Models
{
    public class Transaction
    {
        public string Hash { get; private set; }
        public decimal TotalAmount { get; set; }
        public string PaymentAddress { get; set; }
        public DateTime PaymentDate { get; private set; }

        // apenas para teste
        public static Transaction MockTransaction()
        {
            Transaction t = new()
            {
                TotalAmount = 0.1m
            };
            t.ConfirmTransaction("funciona");
            return t;
        }

        public void ConfirmTransaction(string hash)
        {
            Hash = hash;
            PaymentDate = DateTime.UtcNow;
        }
    }
}
