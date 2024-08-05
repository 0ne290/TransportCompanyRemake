namespace Application.Dtos;

public abstract class PropertiesSetFactCheckBase
{
    public bool PropertyIsSet(string propertyName) => _setProperties.Contains(propertyName);

    protected void SetProperty(string propertyName) => _setProperties.Add(propertyName);
    
    private readonly HashSet<string> _setProperties = new();
}