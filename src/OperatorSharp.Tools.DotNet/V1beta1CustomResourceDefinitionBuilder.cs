using k8s;
using k8s.Models;
using Newtonsoft.Json.Schema;
using OperatorSharp.CustomResources;
using OperatorSharp.CustomResources.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OperatorSharp.Tools.DotNet
{
    public class V1beta1CustomResourceDefinitionBuilder : ICustomResourceDefinitionBuilder
    {
        public CustomResourceDefinitionContext BuildDefinition(Type resourceType)
        {
            if (!resourceType.Implements<CustomResource>()) throw new ArgumentException("Custom Resource Definitions can only be generated for custom resources", nameof(resourceType));

            try
            {
                var apiVersion = GetAttribute<ApiVersionAttribute>(resourceType).ApiVersion;
                var plural = GetAttribute<PluralNameAttribute>(resourceType).PluralName;
                var scope = GetAttribute<ResourceScopeAttribute>(resourceType).ResourceScope;
                var kind = GetAttribute<KindAttribute>(resourceType).Kind;
                var shortNames = new List<string> { GetAttribute<ShortNameAttribute>(resourceType).ShortName };

                var crd = new V1beta1CustomResourceDefinition
                {
                    ApiVersion = "apiextensions.k8s.io/v1beta1",
                    Kind = "CustomResourceDefinition",
                    Metadata = new V1ObjectMeta
                    {
                        Name = $"{plural}.{apiVersion.Group}"
                    },
                    Spec = new V1beta1CustomResourceDefinitionSpec
                    {
                        Group = apiVersion.Group,
                        Scope = scope.ToString(),
                        Names = new V1beta1CustomResourceDefinitionNames(kind, plural, shortNames: shortNames, singular: kind.ToLower()),
                        Versions = new List<V1beta1CustomResourceDefinitionVersion>
                        {
                            BuildVersion(apiVersion, resourceType)
                        }
                    },

                };

                if (resourceType.Implements<IStatusEnabledCustomResource>())
                {
                    crd.Spec.Subresources = new V1beta1CustomResourceSubresources(status: new object());
                }

                return new CustomResourceDefinitionContext<V1beta1CustomResourceDefinition>(crd.Metadata.Name, crd);
            }
            catch (ArgumentException argEx)
            {
                throw new MissingMetadataAttributeException(argEx.Message, argEx);
            }
        }

        public V1beta1CustomResourceDefinitionVersion BuildVersion(ApiVersion apiVersion, Type type)
        {
            JsonSchemaGenerator schemaGenerator = new JsonSchemaGenerator();
            var schema = schemaGenerator.Generate(type);
            schema.Properties = new Dictionary<string, JsonSchema>(schema.Properties.Where(kvp => kvp.Key == "spec" || kvp.Key == "status"));

            var version = new V1beta1CustomResourceDefinitionVersion(apiVersion.Version, true, true);
            version.Schema = new V1beta1CustomResourceValidation();
            version.Schema.OpenAPIV3Schema = MapSchemaToKubernetesModels(schema);

            return version;
        }

        private V1beta1JSONSchemaProps MapSchemaToKubernetesModels(JsonSchema schema)
        {
            var result = new V1beta1JSONSchemaProps();
            result.Type = (schema.Type & ~JsonSchemaType.Null).ToString().ToLower();
            result.Pattern = schema.Pattern;

            if (schema.Properties != null)
            {
                result.Properties = new Dictionary<string, V1beta1JSONSchemaProps>();

                foreach (var property in schema.Properties)
                {
                    result.Properties.Add(property.Key, MapSchemaToKubernetesModels(property.Value));
                }
            }

            if (schema.Items != null)
            {
                result.Items = schema.Items.Select(item => MapSchemaToKubernetesModels(item));
            }

            return result;
        }

        public static TAttribute GetAttribute<TAttribute>(Type sourceType) where TAttribute : Attribute
        {
            TAttribute attribute = Attribute.GetCustomAttribute(sourceType, typeof(TAttribute)) as TAttribute;

            if (attribute == null) { throw new ArgumentException($"Could not find the {typeof(TAttribute).Name} attribute on {sourceType.Name}"); }

            return attribute;
        }
    }
}
