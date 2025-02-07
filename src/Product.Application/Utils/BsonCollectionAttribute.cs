using System.Diagnostics.CodeAnalysis;

namespace Product.Application.Utils;

[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class BsonCollectionAttribute : Attribute
{
    public string CollectionName { get; }

    public BsonCollectionAttribute(string collectionName)
    {
        CollectionName = collectionName;
    }
}