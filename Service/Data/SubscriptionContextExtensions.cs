namespace ProPayments.Service.Data
{
    public static class SubscriptionContextExtensions
    {
        public static async Task EnsureSeedData(this SubscriptionContext context)
        {

            context.Database.EnsureCreated();

            /***
             * Example of removing the free product from the existing options
             * I can manipulate data here and the migrations could be responsible only for adding/removing fields
             * 
             * var freeProduct = context.Products.Where(p => p.ProductName == ProductName.Free).ToList();
             * if (freeProduct.Any())
             * {
             *    context.Products.RemoveRange(freeProduct);
             * }
             * await context.SaveChangesAsync();
             *  
            **/

            await Task.CompletedTask;
        }
    }
}
