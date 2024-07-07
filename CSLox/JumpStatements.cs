namespace Lox;

// Thrown when a break statement is encountered
// Caught by the loop itself
public class BreakStmt : Exception;

// Thrown when a continue statement is encountered
// Caught by the loop itself
public class ContinueStmt : Exception;
