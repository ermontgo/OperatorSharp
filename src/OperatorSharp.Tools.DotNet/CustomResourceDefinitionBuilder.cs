using k8s;
using k8s.Models;
using OperatorSharp.CustomResources;
using OperatorSharp.CustomResources.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace OperatorSharp.Tools.DotNet
{
    public class CustomResourceDefinitionBuilder
    {
        public V1CustomResourceDefinition BuildDefinition(Type resourceType)
        {
            if (!resourceType.Implements<CustomResource>()) throw new ArgumentException("Custom Resource Definitions can only be generated for custom resources", nameof(resourceType));

            try
            {
                var apiVersion = GetAttribute<ApiVersionAttribute>(resourceType).ApiVersion;
                var plural = GetAttribute<PluralNameAttribute>(resourceType).PluralName;
                var scope = GetAttribute<ResourceScopeAttribute>(resourceType).ResourceScope;
                var kind = GetAttribute<KindAttribute>(resourceType).Kind;
                var shortNames = new List<string> { GetAttribute<ShortNameAttribute>(resourceType).ShortName };

                var crd = new V1CustomResourceDefinition
                { 
                    ApiVersion = "apiextensions.k8s.io/v1beta1",
                    Kind = "CustomResourceDefinition",
                    Metadata = new V1ObjectMeta
                    {
                        Name = $"{plural}.{apiVersion.Group}"
                    },
                    Spec = new V1CustomResourceDefinitionSpec
                    {
                        Group = apiVersion.Group,
                        Scope = scope.ToString(),
                        Names = new V1CustomResourceDefinitionNames(kind, plural, shortNames: shortNames, singular: kind.ToLower()),
                        Versions = new List<V1CustomResourceDefinitionVersion>
                        {
                            BuildVersion(apiVersion, resourceType)
                        }
                    }
                };

                return crd;
            }
            catch (ArgumentException argEx)
            {
                throw new MissingMetadataAttributeException(argEx.Message, argEx);
            }
        }

        public V1CustomResourceDefinitionVersion BuildVersion(ApiVersion apiVersion, Type type)
        {
            var version = new V1CustomResourceDefinitionVersion(apiVersion.Version, true, true);

            if (type.Implements<IStatusEnabledCustomResource>())
            {
                version.Subresources = new V1CustomResourceSubresources(status: new object());
            }

            return version;
        }

        public static TAttribute GetAttribute<TAttribute>(Type sourceType) where TAttribute : Attribute
        {
            TAttribute attribute = Attribute.GetCustomAttribute(sourceType, typeof(TAttribute)) as TAttribute;

            if (attribute == null) { throw new ArgumentException($"Could not find the {typeof(TAttribute).Name} attribute on {sourceType.Name}"); }

            return attribute;
        }
    }
}
