using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Catalogo.Api.Configurations;

public class SwaggerSchemaFilterConfig : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema?.Properties == null)
        {
            return;
        }

        // Encontrar todas as propriedades que possuem o atributo SwaggerExclude
        var excludedProperties = context.Type.GetProperties()
            .Where(prop => prop.GetCustomAttribute<SwaggerExcludeAttribute>() != null);

        foreach (var excludedProperty in excludedProperties)
        {
            // Converter a propriedade para o formato camelCase para coincidir com o Swagger
            var jsonPropertyName = char.ToLowerInvariant(excludedProperty.Name[0]) + excludedProperty.Name.Substring(1);

            // Remover a propriedade do esquema
            if (schema.Properties.ContainsKey(jsonPropertyName))
            {
                schema.Properties.Remove(jsonPropertyName);
            }
        }
    }
}



[AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Field | AttributeTargets.Property)]
public class SwaggerExcludeAttribute : Attribute { }