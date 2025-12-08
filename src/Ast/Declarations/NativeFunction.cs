using Ast.Expressions;

namespace Ast.Declarations;

public sealed class NativeFunction : AbstractFunctionDeclaration
{

    public NativeFunction(string name, List<string> parameters, Func<IReadOnlyList<decimal>, decimal> impl)
        : base(name, parameters) => Impl = impl;

    public Func<IReadOnlyList<decimal>, decimal> Impl { get; }

    public override void Accept(IAstVisitor visitor)
    {
        throw new InvalidOperationException($"Visitor cannot be applied to {GetType()}");
    }

    public decimal Invoke(IReadOnlyList<decimal> arguments)
    {
        return Impl(arguments);
    }
}