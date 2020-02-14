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
            if (entry != null)
            {
                if (entry != null && resourceContext.ResourceInstance is SensorThingsGuidIdEntity guidIdEntity)
                {
                    entry.InstanceAnnotations.Add(new ODataInstanceAnnotation("iot.id", new ODataPrimitiveValue(guidIdEntity.Id)));
                    entry.InstanceAnnotations.Add(new ODataInstanceAnnotation("iot.selfLink", new ODataPrimitiveValue(guidIdEntity.SelfLink)));
                    foreach (var link in guidIdEntity.NavigationLinks)
                    {
                        entry.InstanceAnnotations.Add(new ODataInstanceAnnotation($"iot.navigationLink-{link}", new ODataPrimitiveValue($"{Config.BaseUrl}/{guidIdEntity.EntitySetName}({guidIdEntity.Id})/{link}")));
                    }
                }
                else if (entry != null && resourceContext.ResourceInstance is SensorThingsIntIdEntity intIdEntity)
                {
                    entry.InstanceAnnotations.Add(new ODataInstanceAnnotation("iot.id", new ODataPrimitiveValue(intIdEntity.Id)));
                    entry.InstanceAnnotations.Add(new ODataInstanceAnnotation("iot.selfLink", new ODataPrimitiveValue(intIdEntity.SelfLink)));
                    foreach (var link in intIdEntity.NavigationLinks)
                    {
                        entry.InstanceAnnotations.Add(new ODataInstanceAnnotation($"iot.navigationLink-{link}", new ODataPrimitiveValue($"{Config.BaseUrl}/{intIdEntity.EntitySetName}({intIdEntity.Id})/{link}")));
                    }
                }
            }
            return entry;
        }
    }
}
