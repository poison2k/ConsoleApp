﻿using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using CommandDotNet;
using CommandDotNet.Diagnostics;
using CommandDotNet.Extensions;
using static System.Environment;

namespace TestApp.Extensions
{
    public static class ObjectExtensions
    {
        internal static bool IsNullValue(this object? obj)
        {
            return obj == null || obj == DBNull.Value;
        }

        internal static string? ValueToString(this object value, IArgument forArgument)
        {
            if (value.IsNullValue())
            {
                return null;
            }

            if (forArgument.IsObscured())
            {
                return Password.ValueReplacement;
            }

            if (value is string str)
            {
                return str;
            }

            if (value is IEnumerable collection)
            {
                return collection.Cast<object>().Select(i => i?.ToString()).ToCsv(", ");
            }

            return value.ToString();
        }



        /// <summary>
        /// <see cref="Indent"/> is only used if the object is <see cref="IIndentableToString"/>
        /// </summary>
        internal static string? ToIndentedString(this object? value, Indent indent)
        {
            return value is IIndentableToString logToString
                ? logToString.ToString(indent.Increment())
                : value is Exception exception
                    ? exception.Print(indent.Increment(),
                        includeProperties: true,
                        includeData: true)
                    : value?.ToString();
        }

        internal static object CloneWithPublicProperties(this object original, bool recurse = true)
        {
            if (original == null)
            {
                throw new ArgumentNullException(nameof(original));
            }

            if (original is ICloneable cloneable)
            {
                return cloneable.Clone();
            }

            var type = original.GetType();
            object clone;
            try
            {
                clone = Activator.CreateInstance(type);
            }
            catch (MissingMethodException e)
            {
                throw new MissingMethodException($"{e.Message}. The type must implement {nameof(ICloneable)} or a parameterless constructor.", e);
            }

            type
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Where(p => p.CanRead && p.CanWrite)
                .ForEach(p =>
                {
                    var value = p.GetValue(original);
                    if (value != null)
                    {
                        if (recurse && p.PropertyType != typeof(string) && p.PropertyType.IsClass)
                        {
                            value = CloneWithPublicProperties(value);
                        }
                        p.SetValue(clone, value);
                    }
                });

            return clone;
        }
    }
}
