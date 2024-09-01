namespace Probot.SubscriptionApi.Clients.Dtos
{
    public class ClientResponse<T>
    {
        public int StatusCode { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public T? Data { get; set; }
    }
}
