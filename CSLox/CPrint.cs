namespace Lox;

// TODO: Get rid of this later
public class CPrint {
    public static void Print(string str, ConsoleColor color) {
        Console.ForegroundColor = color;
        Console.WriteLine(str);
        Console.ResetColor();
    }
}
