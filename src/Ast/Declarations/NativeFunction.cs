using Runtime;

using ValueType = Runtime.ValueType;

namespace Ast.Declarations;

/// <summary>
/// Определение встроенной функции языка.
/// </summary>
public sealed class NativeFunction : AbstractFunctionDeclaration
{
    private readonly Func<IReadOnlyList<Value>, Value> _implementation;

    public NativeFunction(
        string name,
        IReadOnlyList<NativeFunctionParameter> parameters,
        ValueType resultType,
        Func<IReadOnlyList<Value>, Value> implementation
    )
        : base(name, parameters)
    {
        ResultType = resultType;
        _implementation = implementation;
    }

    public Value Invoke(IReadOnlyList<Value> arguments)
    {
        return _implementation(arguments);
    }

    public override void Accept(IAstVisitor visitor)
    {
        throw new InvalidOperationException($"Visitor cannot be applied to {GetType()}");
    }
}