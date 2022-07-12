#if NET5_0
using Microsoft.OpenApi.Models;
using SAEON.Logs;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;
#endif
using System;
using Microsoft.Toolkit.Diagnostics;

namespace SAEON.Observations.Core
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class SwaggerNameAttribute : Attribute
    {
        public string Name { get; }

        public SwaggerNameAttribute(string name)
        {
            Name = name;
        }
    }

#if NET5_0
    public class SwaggerNameFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            Guard.IsNotNull(schema, nameof(schema));
            Guard.IsNotNull(context, nameof(context));
            //SAEONLogs.Verbose("Schema: {Schema}", schema.Title);
            if (schema?.Properties.Count == 0) return;
            // Classes to be renamed
            var nameAttribute = (SwaggerNameAttribute)Attribute.GetCustomAttribute(context.Type, typeof(SwaggerNameAttribute));
            if (nameAttribute is not null)
            {
                schema.Title = nameAttribute.Name;
            }
        }
    }
#endif
}
