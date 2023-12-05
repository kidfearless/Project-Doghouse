namespace ConsoleApp1
{
    internal class Program
    {
        public void Test()
        {
            var list = new List<string>();
            var any = list.Where(x => x > 0)?.Any();
            var any2 = list.Where(t => true).ToList();
            var any3 = list.Where(t => true).FirstOrDefault().Length;
            var any4 = list.Where(t => true)?.ToList();
            var any5 = list.Where(t => true)?.ToList();
            int? var = null;
            var exp = var.Value;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }
    }
}
