﻿using System.Collections.Generic;
using Enterspeed.Source.Sdk.Api.Models;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.Services;
using Umbraco.Cms.Core.Models;

namespace Enterspeed.Source.UmbracoCms.Models
{
    public class UmbracoDictionaryEntity : IEnterspeedEntity
    {
        private readonly IDictionaryItem _dictionaryItem;
        private readonly IEntityIdentityService _entityIdentityService;
        private readonly string _culture;

        public UmbracoDictionaryEntity(
            IDictionaryItem dictionaryItem,
            IEnterspeedPropertyService propertyService,
            IEntityIdentityService entityIdentityService,
            string culture)
        {
            _dictionaryItem = dictionaryItem;
            _entityIdentityService = entityIdentityService;
            _culture = culture;
            Properties = propertyService.GetProperties(_dictionaryItem, _culture);
        }

        public string Id => _entityIdentityService.GetId(_dictionaryItem, _culture);
        public string Type => "umbDictionary";
        public string Url => null;
        public string[] Redirects => null;
        public string ParentId => _dictionaryItem.ParentId.HasValue ? _entityIdentityService.GetId(_dictionaryItem.ParentId, _culture) : null;
        public IDictionary<string, IEnterspeedProperty> Properties { get; }
    }
}