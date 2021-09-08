namespace Example.Producer.Sales
{
    public class Program
    {
        static void Main(string[] args)
        {
            CustomerUpdateGenerator generator = new CustomerUpdateGenerator();
            generator.Run();
        }
    }
}