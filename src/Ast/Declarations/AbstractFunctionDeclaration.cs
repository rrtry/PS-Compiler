namespace Ast.Declarations;

public abstract class AbstractFunctionDeclaration : Declaration
{
    public AbstractFunctionDeclaration(string name, List<string> parameters)
    {
        Name = name;
        Parameters = parameters;
    }

    public string Name { get; }

    public List<string> Parameters { get; }
}