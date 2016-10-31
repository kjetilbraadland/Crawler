using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Abot.Core;
using Abot.Crawler;
using Abot.Poco;
using HtmlAgilityPack;

namespace Crawler.Model.Abot
{
    class AbotManager
    {
        public delegate void MessageSentEventHandler(object sender, MessageEventArgs args);
        public event MessageSentEventHandler MessageUpdate;
        private string _outputfolder;

        public async void RunCrawl(string aUri, string aOutputFolder, CrawlConfiguration crawlConfig)
        {
            _outputfolder = aOutputFolder;
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            PoliteWebCrawler crawler = new PoliteWebCrawler(crawlConfig);

                        crawler.PageCrawlStartingAsync += crawler_ProcessPageCrawlStarting;
            crawler.PageCrawlCompletedAsync += crawler_ProcessPageCrawlCompleted;
            crawler.PageCrawlDisallowedAsync += crawler_PageCrawlDisallowed;
            crawler.PageLinksCrawlDisallowedAsync += crawler_PageLinksCrawlDisallowed;

            await Task.Run(() =>
            {
                CrawlResult result = crawler.Crawl(new Uri(aUri),
                    cancellationTokenSource);

                OnMessageReceived(
                    result.ErrorOccurred
                        ? $"Crawl of {result.RootUri.AbsoluteUri} completed with error: {result.ErrorException.Message}"
                        : $"Crawl of {result.RootUri.AbsoluteUri} completed without error.");
            }, cancellationTokenSource.Token);

        }

        protected virtual void OnMessageReceived(string message)
        {
            MessageUpdate?.Invoke(this, new MessageEventArgs(message));
        }

        void crawler_ProcessPageCrawlStarting(object sender, PageCrawlStartingArgs e)
        {
            PageToCrawl pageToCrawl = e.PageToCrawl;
            OnMessageReceived(
                $"About to crawl link {pageToCrawl.Uri.AbsoluteUri} which was found on page {pageToCrawl.ParentUri.AbsoluteUri}");
        }

        void crawler_ProcessPageCrawlCompleted(object sender, PageCrawlCompletedArgs e)
        {
            CrawledPage crawledPage = e.CrawledPage;

            if (crawledPage.WebException != null || crawledPage.HttpWebResponse.StatusCode != HttpStatusCode.OK)
                OnMessageReceived($"Crawl of page failed {crawledPage.Uri.AbsoluteUri}");
            else
                OnMessageReceived($"Crawl of page succeeded {crawledPage.Uri.AbsoluteUri}");

            if (string.IsNullOrEmpty(crawledPage.Content.Text))
                OnMessageReceived($"Page had no content {crawledPage.Uri.AbsoluteUri}");

            var htmlAgilityPackDocument = crawledPage.HtmlDocument; //Html Agility Pack parser
            SaveHtmlDocument(htmlAgilityPackDocument);
            var angleSharpHtmlDocument = crawledPage.AngleSharpHtmlDocument; //AngleSharp parser
        }

        private void SaveHtmlDocument(HtmlDocument aDoc)
        {
            try
            {
                var stream = new FileStream(_outputfolder + Guid.NewGuid() + ".html", FileMode.CreateNew);
                aDoc.Save(stream);
            }
            catch (Exception ex)
            {
                OnMessageReceived(ex.Message);
            }
        }


        void crawler_PageLinksCrawlDisallowed(object sender, PageLinksCrawlDisallowedArgs e)
        {
            CrawledPage crawledPage = e.CrawledPage;
            OnMessageReceived($"Did not crawl the links on page {crawledPage.Uri.AbsoluteUri} due to {e.DisallowedReason}");
        }

        void crawler_PageCrawlDisallowed(object sender, PageCrawlDisallowedArgs e)
        {
            PageToCrawl pageToCrawl = e.PageToCrawl;
            OnMessageReceived($"Did not crawl page {pageToCrawl.Uri.AbsoluteUri} due to {e.DisallowedReason}");
        }
    }
}
