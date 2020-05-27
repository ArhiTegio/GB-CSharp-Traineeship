﻿using System;
using System.Reflection;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;

namespace Unity.Tiny.Animation.Editor
{
    static class UserBindingsRemapper
    {
        // Note: This needs to remain in sync with AuthoringComponentPostProcessor.cs
        // Will probably require exposing it publicly over there.
        const string k_AutoGeneratedAuthoringComponentTypeNameSuffix = "Authoring";

        static readonly Type[] k_ValidTypes = {typeof(IComponentData)};

        [MenuItem("Animation/Tiny Animation - Rebuild Bindings Map", false, 110)]
        public static void RebuildBindingsMap()
        {
            FillMap();
        }

        [InitializeOnLoadMethod]
        static void FillMap()
        {
            // Rebuild map on domain reload
            GatherGeneratedAuthoringComponentsBindings();
            GatherExplicitBindings();
        }

        static void GatherGeneratedAuthoringComponentsBindings()
        {
            // TODO: Do we want to support other types?
            var autoGeneratingTypes = TypeCache.GetTypesDerivedFrom<IComponentData>();

            foreach (var type in autoGeneratingTypes)
            {
                if (type.GetCustomAttribute<GenerateAuthoringComponentAttribute>() != null)
                {
                    // Only public instance fields are animatable
                    var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
                    foreach (var field in fields)
                    {
                        BindField(field, $"{type.Name}{k_AutoGeneratedAuthoringComponentTypeNameSuffix}", $"{type.Name}.{field.Name}");
                    }
                }
            }
        }

        static void GatherExplicitBindings()
        {
            var conversionTypes = TypeCache.GetTypesDerivedFrom<IConvertGameObjectToEntity>();
            foreach (var type in conversionTypes)
            {
                var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                foreach (var field in fields)
                {
                    var bindingAttribute = field.GetCustomAttribute<CreateAnimationBindingAttribute>();
                    if (bindingAttribute != null && (field.IsPublic || field.GetCustomAttribute<SerializeField>() != null))
                    {
                        // TODO: Also validate that the path is valid (exists; is public; is not read-only; is of animatable type (float for now, but probably float2, float3 and float4 one day))
                        if (bindingAttribute.RequestedComponentType == null)
                            throw new InvalidOperationException($"Invalid {typeof(CreateAnimationBindingAttribute)} binding argument: componentType cannot be null.");

                        if (string.IsNullOrEmpty(bindingAttribute.RequestedPropertyPath))
                            throw new InvalidOperationException($"Invalid {typeof(CreateAnimationBindingAttribute)} binding argument: propertyPath cannot be null.");

                        if (string.IsNullOrEmpty(bindingAttribute.BindsTo))
                            throw new InvalidOperationException($"Could not create a valid binding from field: {field.DeclaringType?.Name}.{field.Name}");

                        if (!IsTypeLegal(bindingAttribute.RequestedComponentType))
                            throw new InvalidOperationException($"Can not generate a binding with type: {bindingAttribute.RequestedComponentType}");

                        BindField(field, type.Name, bindingAttribute.BindsTo);
                    }
                }
            }
        }

        static void BindField(FieldInfo fieldInfo, string sourceNamePrefix, string destinationName)
        {
            if (fieldInfo.GetCustomAttribute<NotKeyableAttribute>() != null)
                return;

            var fieldType = fieldInfo.FieldType;
            if (fieldType.IsPrimitive)
            {
                if (BindingUtils.IsTypeAnimatable(fieldType))
                {
                    BindingsStore.CreateBindingNameRemap($"{sourceNamePrefix}.{fieldInfo.Name}", destinationName);
                    return;
                }
            }

            var fields = fieldType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var field in fields)
            {
                if (field.IsPublic || field.GetCustomAttribute<SerializeField>() != null)
                {
                    BindField(field, $"{sourceNamePrefix}.{fieldInfo.Name}", $"{destinationName}.{field.Name}");
                }
            }
        }

        static bool IsTypeLegal(Type type)
        {
            foreach (var validType in k_ValidTypes)
            {
                if (validType.IsAssignableFrom(type))
                    return true;
            }

            return false;
        }
    }
}
