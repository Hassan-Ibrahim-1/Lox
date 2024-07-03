﻿namespace Lox;

public class Lox {
    public static readonly Interpreter interpreter = new Interpreter();

    public static bool hadError = false;
    public static bool hadRuntimeError = false;

    public static void Main(string[] args) {
        if (args.Length > 1) {
            Console.WriteLine("Usage: cslox [script]");
            Environment.Exit(64);
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
            Run(sr.ReadToEnd());
        }
        catch (FileNotFoundException) {
            Console.WriteLine($"{path} not found!");
            hadError = true;
        }
        if (hadError) Environment.Exit(65);
        if (hadRuntimeError) Environment.Exit(70);
    }

    private static void RunPrompt() {
        while (true) {
            Console.Write("> ");
            string? line = Console.ReadLine();
            if (line == null) {
                break;
            }
            Run(line);
            hadError = false;
        }
    }

    private static void Run(string source) {
        Scanner scanner = new Scanner(source);
        List<Token> tokens = scanner.ScanTokens();
        Parser parser = new Parser(tokens);
        Expr expression = parser.Parse();

        if (hadError) return;
        interpreter.Interpret(expression);
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
        Console.WriteLine($"{e.Message}\n[ line { e.token.line} ]");
        hadRuntimeError = true;
    }

    private static void Report(int line, string location, string message) {
        Console.WriteLine($"[line {line}] Error {location}: {message}");
        hadError = true;
    }
}
