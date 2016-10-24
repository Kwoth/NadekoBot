namespace NadekoBot
{
    public class Program
    {
        public static void Main(string[] args) {
            try
            {
                new NadekoBot().RunAsync(args).GetAwaiter().GetResult();
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine("NadekoBot Crashed with Exception: {0}", e);
            }
        }
    }
}
