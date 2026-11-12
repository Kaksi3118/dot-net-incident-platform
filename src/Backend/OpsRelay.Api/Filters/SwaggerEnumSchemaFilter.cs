using System.Globalization;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OpsRelay.Api.Filters;

public class SwaggerEnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (!context.Type.IsEnum)
        {
            return;
        }

        var names = Enum.GetNames(context.Type);
        var values = Enum.GetValues(context.Type).Cast<object>().ToArray();

        schema.Description = string.Join(
            Environment.NewLine,
            names.Select(name =>
                $"{Convert.ToInt32(Enum.Parse(context.Type, name), CultureInfo.InvariantCulture)} = {name}"));

        var enumNames = new OpenApiArray();
        foreach (var name in names)
        {
            enumNames.Add(new OpenApiString(name));
        }

        var enumValues = new OpenApiArray();
        foreach (var value in values)
        {
            enumValues.Add(
                new OpenApiInteger(Convert.ToInt32(value, CultureInfo.InvariantCulture)));
        }

        schema.Extensions["x-enumNames"] = enumNames;
        schema.Extensions["x-enumValues"] = enumValues;
    }
}
