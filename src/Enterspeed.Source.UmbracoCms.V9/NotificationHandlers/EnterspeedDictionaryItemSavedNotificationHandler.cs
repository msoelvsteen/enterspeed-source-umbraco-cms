﻿using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.V9.Data.Models;
using Enterspeed.Source.UmbracoCms.V9.Data.Repositories;
using Enterspeed.Source.UmbracoCms.V9.Factories;
using Enterspeed.Source.UmbracoCms.V9.Handlers;
using Enterspeed.Source.UmbracoCms.V9.Services;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;

namespace Enterspeed.Source.UmbracoCms.V9.NotificationHandlers
{
    public class EnterspeedDictionaryItemSavedNotificationHandler : BaseEnterspeedNotificationHandler, INotificationHandler<DictionaryItemSavedNotification>
    {
        private readonly IEnterspeedJobFactory _enterspeedJobFactory;

        public EnterspeedDictionaryItemSavedNotificationHandler(
            IEnterspeedConfigurationService configurationService,
            IEnterspeedJobRepository enterspeedJobRepository,
            IEnterspeedJobsHandlingService enterspeedJobsHandlingService,
            IUmbracoContextFactory umbracoContextFactory,
            IScopeProvider scopeProvider,
            IEnterspeedJobFactory enterspeedJobFactory,
            IAuditService auditService) : base(
                  configurationService,
                  enterspeedJobRepository,
                  enterspeedJobsHandlingService,
                  umbracoContextFactory,
                  scopeProvider,
                  auditService)
        {
            _enterspeedJobFactory = enterspeedJobFactory;
        }

        public void Handle(DictionaryItemSavedNotification notification)
        {
            var isPublishConfigured = IsPublishConfigured();
            var isPreviewConfigured = IsPreviewConfigured();

            if (!isPublishConfigured && !isPreviewConfigured)
            {
                return;
            }

            var entities = notification.SavedEntities.ToList();
            var jobs = new List<EnterspeedJob>();
            foreach (var dictionaryItem in entities)
            {
                foreach (var translation in dictionaryItem.Translations)
                {
                    if (isPublishConfigured)
                    {
                        jobs.Add(_enterspeedJobFactory.GetPublishJob(dictionaryItem, translation.Language.IsoCode, EnterspeedContentState.Publish));
                    }

                    if (isPreviewConfigured)
                    {
                        jobs.Add(_enterspeedJobFactory.GetPublishJob(dictionaryItem, translation.Language.IsoCode, EnterspeedContentState.Preview));
                    }
                }
            }

            EnqueueJobs(jobs);
        }
    }
}
