using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Formatter.Serialization;
using Microsoft.OData;
using Microsoft.OData.Edm;
using System;

namespace SAEON.Observations.SensorThings
{
    public class SensorThingsODataSerializerProvider : DefaultODataSerializerProvider
    {
        private SensorThingsEntitySerializer _entitySerializer;

        public SensorThingsODataSerializerProvider(IServiceProvider rootContainer)
            : base(rootContainer)
        {
            _entitySerializer = new SensorThingsEntitySerializer(this);
        }

        public override ODataEdmTypeSerializer GetEdmTypeSerializer(IEdmTypeReference edmType)
        {
            if (edmType.IsEntity())
            {
                return _entitySerializer;
            }

            return base.GetEdmTypeSerializer(edmType);
        }
    }

    public class SensorThingsEntitySerializer : ODataResourceSerializer
    {
        public SensorThingsEntitySerializer(ODataSerializerProvider serializerProvider)
             : base(serializerProvider)
        {
        }

        public override ODataResource CreateResource(SelectExpandNode selectExpandNode, ResourceContext resourceContext)
        {
            ODataResource entry = base.CreateResource(selectExpandNode, resourceContext);

            if (entry != null && resourceContext.ResourceInstance is SensorThingsEntity entity)
            {
                entry.InstanceAnnotations.Add(new ODataInstanceAnnotation("iot.id", new ODataPrimitiveValue(entity.Id)));
                entry.InstanceAnnotations.Add(new ODataInstanceAnnotation("iot.selfLink", new ODataPrimitiveValue(entity.SelfLink)));
                foreach (var link in entity.NavigationLinks)
                {
                    entry.InstanceAnnotations.Add(new ODataInstanceAnnotation($"iot.navigationLink-{link}", new ODataPrimitiveValue($"{Config.BaseUrl}/{entity.EntitySetName}({entity.Id})/{link}")));
                }
            }

            return entry;
        }
    }
}
