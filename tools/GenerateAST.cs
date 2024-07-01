namespace Tools;

public class GenerateAST {
    public static void Main(string[] args) {
        if (args.Length != 1) {
            Console.WriteLine("Usage: generate_ast <output_directory>");
            Environment.Exit(64);
        }
        string outputDir = args[0];
        DefineAST(outputDir, "Expr", new List<string>() {
                "Binary : Expr left, Token op, Expr right",
                "Grouping : Expr expression",
                "Literal : object value",
                "Unary : Token op, Expr right"
                });
    }

    private static void DefineAST(string outputDir, string baseName, List<string> types) {
        string path = $"{outputDir}/{baseName}.cs";
        StreamWriter writer = new StreamWriter(path);

        writer.WriteLine("namespace Lox;");
        writer.WriteLine();
        writer.WriteLine($"public abstract class {baseName} {{}}"); 
        writer.WriteLine();

        foreach(string type in types) {
            string className = type.Split(':')[0].Trim();
            string fields = type.Split(':')[1].Trim();
            DefineType(writer, baseName, className, fields);
        }

        writer.Close();
    }

    private static void DefineType(StreamWriter writer, string baseName, string className, string fieldList) {
        writer.WriteLine($"public class {className} : {baseName} {{");
        
        // constructor

        // declare fields
        string[] fields = fieldList.Split(", ");
        foreach(string field in fields) {
            writer.WriteLine($"    public readonly {field};");
        }
        writer.WriteLine();
        
        // store params in fields
        writer.WriteLine($"    public {className}({fieldList}) {{");
        foreach (string field in fields) {
            string name = field.Split(" ")[1];
            writer.WriteLine($"        this.{name} = {name};");
        }
        writer.WriteLine("    }");
        writer.WriteLine("}");
    }
}

