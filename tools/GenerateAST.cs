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
                "Unary : Token op, Expr right",
                "Variable : Token name",
                "Assignment : Token name, Expr value",
                "Logic : Expr left, Token op, Expr right",
                "Ternary : Expr conditional, Expr thenBranch, Expr elseBranch",
                "Call : Expr callee, Token paren, List<Expr> arguments",
                "FunctionExpr: List<Token> parameters, List<Stmt> body",
            });
        DefineAST(outputDir, "Stmt", new List<string>() {
                "Expression : Expr expression",
                "Block : List<Stmt> statements",
                "Print : Expr expression",
                "Var : Token name, Expr initializer",
                "If : Expr condition, Stmt thenBranch, Stmt elseBranch",
                "While : Expr condition, Stmt body",
                "Break : Null null",
                "Function : Token name, FunctionExpr functionExpr",
                "Return : Token keyword, Expr value",
                "Class : Token name, List<Function> methods",
            });
    }

    private static void DefineAST(string outputDir, string baseName, List<string> types) {
        string path = $"{outputDir}/{baseName}.cs";
        StreamWriter writer = new StreamWriter(path);
        writer.WriteLine("namespace Lox;");

        writer.WriteLine();

        DefineVisitor(writer, baseName, types);

        writer.WriteLine($"public abstract class {baseName} {{"); 

        writer.WriteLine("    public abstract R Accept<R>(IVisitor<R> visitor);");

        writer.WriteLine('}');

        writer.WriteLine();

        foreach(string type in types) {
            string className = type.Split(':')[0].Trim();
            string fields = type.Split(':')[1].Trim();
            DefineType(writer, baseName, className, fields);
        }

        writer.Close();
    }

    private static void DefineVisitor(StreamWriter writer, string baseName, List<string> types) {
        writer.WriteLine($"public interface IVisitor<R> {{");

        foreach (string type in types) {
            string typeName = type.Split(':')[0].Trim();
            writer.WriteLine($"    R Visit{typeName}{baseName}({typeName} {baseName.ToLower()});");
        }

        writer.WriteLine("}");
        writer.WriteLine();
    }

    private static void DefineType(StreamWriter writer, string baseName, string className, string fieldList) {
        writer.WriteLine($"public class {className} : {baseName} {{");

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

        writer.WriteLine("    public override R Accept<R>(IVisitor<R> visitor) {");
        writer.WriteLine($"        return visitor.Visit{className}{baseName}(this);");
        writer.WriteLine("    }");

        writer.WriteLine("}");
    }
}

