#if NETCOREAPP3_1
using Microsoft.OpenApi.Models;
using SAEON.Logs;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;

namespace SAEON.Observations.Core
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SwaggerIgnoreAttribute : Attribute
    {
    }

    internal static class StringExtensions
    {
        internal static string ToCamelCase(this string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return char.ToLowerInvariant(value[0]) + value.Substring(1);
        }
    }

    public class SwaggerIgnoreFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema == null) throw new ArgumentNullException(nameof(schema));
            if (context == null) throw new ArgumentNullException(nameof(context));
            SAEONLogs.Verbose("Schema: {Schema}", schema.Title);
            if (schema?.Properties.Count == 0) return;
            var excludedProperties = context.Type.GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(SwaggerIgnoreAttribute)));
            SAEONLogs.Information("Excluded: {Names}", string.Join("; ", excludedProperties.Select(p => p.Name)).OrderBy(p => p));
            foreach (var excludedProperty in excludedProperties)
            {
                if (schema.Properties.ContainsKey(excludedProperty.Name.ToCamelCase()))
                {
                    schema.Properties.Remove(excludedProperty.Name.ToCamelCase());
                }
            }
        }
    }
}
#endif
