#if NETCOREAPP3_0
using Microsoft.OpenApi.Models;
using SAEON.Logs;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;
using System.Reflection;

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
            Logging.Information("Schema: {Schema}", schema.Title);
            if (!(context.ApiModel is ApiObject model)) return;
            if (schema?.Properties == null || model?.ApiProperties == null) return;
            var excludedProperties = context.ApiModel.Type?.GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(SwaggerIgnoreAttribute)));
            Logging.Information("Excluded: {Names}", string.Join("; ", excludedProperties.Select(p => p.Name)).OrderBy(p => p));
            var excludedSchemaProperties = model.ApiProperties.Where(ap => excludedProperties.Any(pi => pi.Name.ToCamelCase() == ap.MemberInfo.Name.ToCamelCase()));

            foreach (var propertyToExclude in excludedSchemaProperties)
            {
                schema.Properties.Remove(propertyToExclude.ApiName);
            }
        }
    }
}
#endif
