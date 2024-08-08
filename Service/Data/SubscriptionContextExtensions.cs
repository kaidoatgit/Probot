namespace ProPayments.Service.Data
{
    public static class SubscriptionContextExtensions
    {
        public static async Task EnsureSeedData(this SubscriptionContext context)
        {

            context.Database.EnsureCreated();

            /***
             * Example of removing the free plan from the existing options
             * I can manipulate data here and the migrations could be responsible only for adding/removing fields
             * 
             * var freePlan = context.Plans.Where(p => p.PlanName == PlanType.Free).ToList();
             * if (freePlan.Any())
             * {
             *    context.Plans.RemoveRange(freePlan);
             * }
             * await context.SaveChangesAsync();
             *  
            **/

            await Task.CompletedTask;
        }
    }
}
