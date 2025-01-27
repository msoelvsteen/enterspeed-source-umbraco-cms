﻿using Enterspeed.Source.Sdk.Api.Models.Properties;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Enterspeed.Source.UmbracoCms.V9.Services.DataProperties
{
    public interface IEnterspeedPropertyValueConverter
    {
        bool IsConverter(IPublishedPropertyType propertyType);
        IEnterspeedProperty Convert(IPublishedProperty property, string culture);
    }
}
