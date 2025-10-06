namespace BonzoByte.Core.Helpers
{
    public static class AssertUtil
    {
        public static void Expect(bool condition, string message, string? ctx = null)
        {
            if (condition) return;
            var prev = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ASSERT] {message} {(ctx != null ? $"| {ctx}" : "")}");
            Console.ForegroundColor = prev;
        }
    }
}