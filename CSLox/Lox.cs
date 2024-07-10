namespace Lox;

public class Lox {
    public static readonly Interpreter interpreter = new Interpreter();

    public static bool hadError = false;
    public static bool hadRuntimeError = false;

    public static void Main(string[] args) {
        if (args.Length > 1) {
            Console.WriteLine("Usage: cslox [script]");
            System.Environment.Exit(64);
        }
        else if (args.Length == 1) {
            RunFile(args[0]);
        } 
        else {
            RunPrompt();
        }
    }
    private static void RunFile(string path) {
        try {
            StreamReader sr = new StreamReader(Path.GetFullPath(path));
            Run(sr.ReadToEnd(), false);
        }

        catch (FileNotFoundException) {
            Console.WriteLine($"{path} not found!");
            hadError = true;
        }
        if (hadError) System.Environment.Exit(65);
        if (hadRuntimeError) System.Environment.Exit(70);
    }

    private static void RunPrompt() {
        while (true) {
            Console.Write("> ");
            string? line = Console.ReadLine();
            if (line == null) {
                break;
            }
            Run(line, true);
            hadError = false;
        }
    }

    private static void Run(string source, bool repl) {
        Scanner scanner = new Scanner(source);
        List<Token> tokens = scanner.ScanTokens();

        Parser parser = new Parser(tokens, repl);
        List<Stmt> statements = parser.Parse();

        if (hadError) return; // Syntax error
        
        Resolver resolver = new Resolver(interpreter);
        resolver.Resolve(statements);

        if (hadError) return; // Resolver error

        interpreter.Interpret(statements);
    } 
    
    public static void Error(int line, string message) {
        Report(line, "", message);
    }

    public static void Error(Token token, string message) {
        if (token.type == TokenType.EOF) {
            Report(token.line, "at end", message);
        }
        else {
            Report(token.line, token.lexeme, message);
        }
    }

    public static void RuntimeError(RuntimeError e) {
        Console.WriteLine($"[line { e.token.line}] {e.Message}");
        hadRuntimeError = true;
    }

    private static void Report(int line, string location, string message) {
        Console.WriteLine($"[line {line}] Error {location}: {message}");
        hadError = true;
    }
}
