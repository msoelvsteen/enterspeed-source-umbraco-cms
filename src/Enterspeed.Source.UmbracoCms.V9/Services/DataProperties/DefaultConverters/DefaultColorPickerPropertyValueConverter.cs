﻿using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.V9.Extensions;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PropertyEditors.ValueConverters;

namespace Enterspeed.Source.UmbracoCms.V9.Services.DataProperties.DefaultConverters
{
    public class DefaultColorPickerPropertyValueConverter : IEnterspeedPropertyValueConverter
    {
        public bool IsConverter(IPublishedPropertyType propertyType)
        {
            return propertyType.EditorAlias.Equals("Umbraco.ColorPicker");
        }

        public IEnterspeedProperty Convert(IPublishedProperty property, string culture)
        {
            var colorValue = string.Empty;
            var colorLabel = string.Empty;

            if (UseLabel(property.PropertyType))
            {
                var colorPickerValue = property.GetValue<ColorPickerValueConverter.PickedColor>(culture);
                colorValue = colorPickerValue.Color;
                colorLabel = colorPickerValue.Label;
            }
            else
            {
                colorValue = property.GetValue<string>(culture);
                colorLabel = colorValue;
            }

            var colorPickerProperties = new List<IEnterspeedProperty>
            {
                new StringEnterspeedProperty("color", colorValue),
                new StringEnterspeedProperty("label", colorLabel)
            };

            var properties = colorPickerProperties.ToDictionary(x => x.Name, x => x);
            return new ObjectEnterspeedProperty(property.Alias, properties);
        }

        private static bool UseLabel(IPublishedPropertyType propertyType)
        {
            return ConfigurationEditor.ConfigurationAs<ColorPickerConfiguration>(propertyType.DataType.Configuration).UseLabel;
        }
    }
}
