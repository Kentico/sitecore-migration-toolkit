﻿using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using UMT.Sitecore.Abstractions;

namespace UMT.Sitecore.Converters
{
    public class DefaultFieldTypeConverter : IFieldTypeConverter
    {
        public virtual string Convert(Field field, Item item)
        {
            return field != null ? field.Value : string.Empty;
        }
    }
}
