﻿using System.Collections.Generic;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.V9.Extensions;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;

namespace Enterspeed.Source.UmbracoCms.V9.Services.DataProperties.DefaultConverters
{
    public class DefaultDropdownPropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        public bool IsConverter(IPublishedPropertyType propertyType)
        {
            return propertyType.EditorAlias.Equals("Umbraco.DropDown.Flexible");
        }

        public IEnterspeedProperty Convert(IPublishedProperty property, string culture)
        {
            var isMultiple = property.PropertyType.DataType.ConfigurationAs<DropDownFlexibleConfiguration>().Multiple;
            var arrayItems = new List<IEnterspeedProperty>();
            if (isMultiple)
            {
                var items = property.GetValue<string[]>(culture);
                foreach (var item in items)
                {
                    arrayItems.Add(new StringEnterspeedProperty(item));
                }
            }
            else
            {
                var value = property.GetValue<string>(culture);
                if (!string.IsNullOrWhiteSpace(value))
                {
                    arrayItems.Add(new StringEnterspeedProperty(value));
                }
            }

            return new ArrayEnterspeedProperty(property.Alias, arrayItems.ToArray());
        }
    }
}
