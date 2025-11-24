namespace Listener.Containers;

public interface ISupportedTypesContainer
{
    IReadOnlyList<Type> SupportedTypes { get; }
}
