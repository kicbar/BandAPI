﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;

namespace BandAPI.Helpers
{
    public static class ObjectExtension
    {
        public static ExpandoObject ShapeDate<TSource>(this TSource source, string fields)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var dataShapedObject = new ExpandoObject();

            if (string.IsNullOrWhiteSpace(fields))
            {
                var propertyInfos = typeof(TSource).GetProperties(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                foreach (var propertyInfo in propertyInfos)
                {
                    var propertyValue = propertyInfo.GetValue(source);

                    ((IDictionary<string, object>)dataShapedObject).Add(propertyInfo.Name, propertyValue);
                }
                return dataShapedObject;
            }

            var fieldAfterSplit = fields.Split(",");
            foreach (var field in fieldAfterSplit)
            {
                var propertyName = field.Trim();
                var propertyInfo = typeof(TSource).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (propertyInfo == null)
                    throw new Exception(propertyName.ToString() + " was not found.");

                var propertyValue = propertyInfo.GetValue(source);    

                ((IDictionary<string, object>)dataShapedObject).Add(propertyInfo.Name, propertyValue);
            }

            return dataShapedObject;
        }
    }
}
