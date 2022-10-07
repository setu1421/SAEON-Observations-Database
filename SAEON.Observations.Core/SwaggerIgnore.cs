#if NET5_0
using Microsoft.OpenApi.Models;
using SAEON.Logs;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;
#endif
using CommunityToolkit.Diagnostics;
using System;

namespace SAEON.Observations.Core
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SwaggerIgnoreAttribute : Attribute { }

#if NET5_0
    public class SwaggerIgnoreFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            Guard.IsNotNull(schema, nameof(schema));
            Guard.IsNotNull(context, nameof(context));
            //SAEONLogs.Verbose("Schema: {Schema}", schema.Title);
            if (schema?.Properties.Count == 0) return;
            var excludedProperties = context.Type.GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(SwaggerIgnoreAttribute)));
            //SAEONLogs.Information("Excluded: {Names}", string.Join("; ", excludedProperties.Select(p => p.Name)).OrderBy(p => p));
            foreach (var excludedProperty in excludedProperties)
            {
                if (schema.Properties.ContainsKey(excludedProperty.Name.ToCamelCase()))
                {
                    schema.Properties.Remove(excludedProperty.Name.ToCamelCase());
                }
            }
        }
    }
#endif
}
