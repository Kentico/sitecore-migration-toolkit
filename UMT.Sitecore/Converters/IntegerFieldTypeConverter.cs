﻿using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace UMT.Sitecore.Converters
{
    public class IntegerFieldTypeConverter : BaseFieldTypeConverter
    {
        public override object Convert(Field field, Item item)
        {
            return (object)(int.TryParse(field.Value, out var number) ? number : 0);
        }
    }
}
