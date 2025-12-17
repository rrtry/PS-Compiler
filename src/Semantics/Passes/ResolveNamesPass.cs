using Ast.Declarations;
using Ast.Expressions;
using Ast.Statements;

using Runtime;

using Semantics.Symbols;

namespace Semantics.Passes;

/// <summary>
/// Проход по AST, устанавливающий соответствие имён и символов (объявлений).
/// </summary>
public sealed class ResolveNamesPass : AbstractPass
{
    /// <summary>
    /// В таблицу символов складываются объявления.
    /// </summary>
    private SymbolsTable _symbols;

    public ResolveNamesPass(SymbolsTable globalSymbols)
    {
        _symbols = globalSymbols;
    }

    public override void Visit(FunctionCallExpression e)
    {
        base.Visit(e);
        e.Function = _symbols.GetFunctionDeclaration(e.Name);
    }

    public override void Visit(VariableExpression e)
    {
        base.Visit(e);
        e.Variable = _symbols.GetVariableDeclaration(e.Name);
    }

    public override void Visit(VariableDeclaration d)
    {
        base.Visit(d);
        d.DeclaredType = d.DeclaredTypeName != null ? _symbols.GetTypeDeclaration(d.DeclaredTypeName) : null;
        _symbols.DeclareVariable(d);
    }

    public override void Visit(FunctionDeclaration d)
    {
        d.ResultType = d.DeclaredTypeName != null ? _symbols.GetTypeDeclaration(d.DeclaredTypeName).ResultType : _symbols.GetTypeDeclaration("void").ResultType;
        d.DeclaredType = d.DeclaredTypeName != null ? _symbols.GetTypeDeclaration(d.DeclaredTypeName) : null;

        _symbols.DeclareFunction(d);
        _symbols = new SymbolsTable(_symbols);

        try
        {
            base.Visit(d);
        }
        finally
        {
            _symbols = _symbols.Parent!;
        }
    }

    public override void Visit(ParameterDeclaration d)
    {
        base.Visit(d);
        d.Type = _symbols.GetTypeDeclaration(d.TypeName);
        _symbols.DeclareVariable(d);
    }

    public override void Visit(IfElseStatement e)
    {
        _symbols = new SymbolsTable(_symbols);
        try
        {
            base.Visit(e);
        }
        finally
        {
            _symbols = _symbols.Parent!;
        }
    }

    public override void Visit(ForLoopStatement e)
    {
        _symbols = new SymbolsTable(_symbols);
        try
        {
            base.Visit(e);
        }
        finally
        {
            _symbols = _symbols.Parent!;
        }
    }

    public override void Visit(ForLoopIteratorDeclaration d)
    {
        base.Visit(d);
        _symbols.DeclareVariable(d);
    }
}