﻿using System.IO;

namespace Lox;

public class Lox {
    public static bool hadError = false;

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
//        byte[] bytes = File.ReadAllBytes(Path.GetFullPath(path));
        try {
            StreamReader sr = new StreamReader(Path.GetFullPath(path));
            Run(sr.ReadToEnd());
        }
        catch (FileNotFoundException) {
            Console.WriteLine($"{path} not found!");
        }
        if (hadError) Environment.Exit(65);
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

        foreach (Token token in tokens) {
            Console.WriteLine(token);
        }
    } 
    
    public static void Error(int line, String message) {
        Report(line, "", message);
    }

    private static void Report(int line, string location, string message) {
        Console.WriteLine($"[line {line}] Error {location}: {message}");
        hadError = true;
    }
}
