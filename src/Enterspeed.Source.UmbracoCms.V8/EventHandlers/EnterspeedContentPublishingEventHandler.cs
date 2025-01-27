using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.V8.Data.Models;
using Enterspeed.Source.UmbracoCms.V8.Data.Repositories;
using Enterspeed.Source.UmbracoCms.V8.Factories;
using Enterspeed.Source.UmbracoCms.V8.Services;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Scoping;
using Umbraco.Core.Services;
using Umbraco.Core.Services.Implement;
using Umbraco.Web;

namespace Enterspeed.Source.UmbracoCms.V8.EventHandlers
{
    public class EnterspeedContentPublishingEventHandler : BaseEnterspeedEventHandler, IComponent
    {
        private readonly IEnterspeedJobFactory _enterspeedJobFactory;

        public EnterspeedContentPublishingEventHandler(
            IUmbracoContextFactory umbracoContextFactory,
            IEnterspeedJobRepository enterspeedJobRepository,
            IEnterspeedJobsHandlingService jobsHandlingService,
            IEnterspeedConfigurationService configurationService,
            IScopeProvider scopeProvider,
            IEnterspeedJobFactory enterspeedJobFactory)
            : base(
                umbracoContextFactory, enterspeedJobRepository, jobsHandlingService, configurationService, scopeProvider)
        {
            _enterspeedJobFactory = enterspeedJobFactory;
        }

        public void Initialize()
        {
            ContentService.Publishing += ContentServicePublishing;
        }

        public void ContentServicePublishing(IContentService sender, ContentPublishingEventArgs e)
        {
            var isPublishConfigured = _configurationService.IsPublishConfigured();
            var isPreviewConfigured = _configurationService.IsPreviewConfigured();

            if (!isPublishConfigured && !isPreviewConfigured)
            {
                return;
            }

            // This only handles variants that has been unpublished. Publishing is handled in the ContentCacheUpdated method
            var entities = e.PublishedEntities.ToList();
            var jobs = new List<EnterspeedJob>();
            using (var context = _umbracoContextFactory.EnsureUmbracoContext())
            {
                foreach (var content in entities)
                {
                    if (!content.ContentType.VariesByCulture())
                    {
                        continue;
                    }

                    List<IContent> descendants = null;

                    foreach (var culture in content.AvailableCultures)
                    {
                        var isCultureUnpublished = e.IsUnpublishingCulture(content, culture);

                        if (isCultureUnpublished)
                        {
                            if (isPublishConfigured)
                            {
                                jobs.Add(_enterspeedJobFactory.GetDeleteJob(content, culture, EnterspeedContentState.Publish));
                            }

                            if (isPreviewConfigured)
                            {
                                jobs.Add(_enterspeedJobFactory.GetDeleteJob(content, culture, EnterspeedContentState.Preview));
                            }

                            if (descendants == null)
                            {
                                descendants = Current.Services.ContentService
                                    .GetPagedDescendants(content.Id, 0, int.MaxValue, out var totalRecords).ToList();
                            }

                            foreach (var descendant in descendants)
                            {
                                if (descendant.ContentType.VariesByCulture())
                                {
                                    var descendantCultures = descendant.AvailableCultures;
                                    if (descendantCultures.Contains(culture))
                                    {
                                        if (isPublishConfigured)
                                        {
                                            jobs.Add(_enterspeedJobFactory.GetDeleteJob(descendant, culture, EnterspeedContentState.Publish));
                                        }

                                        if (isPreviewConfigured)
                                        {
                                            jobs.Add(_enterspeedJobFactory.GetDeleteJob(descendant, culture, EnterspeedContentState.Preview));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            EnqueueJobs(jobs);
        }

        public void Terminate()
        {
        }
    }
}