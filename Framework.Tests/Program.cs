namespace Framework.Tests
{
    using WebApi;

    public class Program
    {
        public static void Main(string[] args)
        {
            ServiceContainer.Run(() => new OwinService());
        }
    }
}
