﻿using Enterspeed.Source.UmbracoCms.V9.Models.Configuration;

namespace Enterspeed.Source.UmbracoCms.V9.Extensions
{
    public static class EnterspeedConfigurationExtensions
    {
        public static EnterspeedUmbracoConfiguration GetPublishConfiguration(this EnterspeedUmbracoConfiguration me)
        {
            if (me == null)
            {
                return null;
            }

            return new EnterspeedUmbracoConfiguration
            {
                ApiKey = me.ApiKey,
                BaseUrl = me.BaseUrl,
                ConnectionTimeout = me.ConnectionTimeout,
                MediaDomain = me.MediaDomain
            };
        }

        public static EnterspeedUmbracoConfiguration GetPreviewConfiguration(this EnterspeedUmbracoConfiguration me)
        {
            if (me == null || string.IsNullOrWhiteSpace(me.PreviewApiKey))
            {
                return null;
            }

            return new EnterspeedUmbracoConfiguration
            {
                ApiKey = me.PreviewApiKey,
                BaseUrl = me.BaseUrl,
                ConnectionTimeout = me.ConnectionTimeout,
                MediaDomain = me.MediaDomain
            };
        }
    }
}
