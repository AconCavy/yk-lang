using YKLang.Statements;

namespace YKLang;

public record InterpretableObject(string Source, IReadOnlyList<Statement> Statements);
