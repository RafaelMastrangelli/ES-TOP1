namespace ESTop1.Api.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class RequerAssinaturaAttribute : Attribute
{
    public string Recurso { get; }

    public RequerAssinaturaAttribute(string recurso)
    {
        Recurso = recurso;
    }
}
