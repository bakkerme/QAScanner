using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading;
using Abot.Crawler;
using Abot.Poco;
using CsQuery;
using QAPlugin;
using ValueStorage;
using ValueStorage.ScannerDataLayer;

namespace QAScanner.Core
{
	internal class QaScanner
	{
		private String _scanUrl;

		private CancellationTokenSource _cancellationToken;
		private PoliteWebCrawler _crawler;
		private IEnumerable<IQAPlugin> _plugins;

		private List<Func<CrawledPage, List<ScanError>>> _crawlCompleteCallbacks = new List<Func<CrawledPage, List<ScanError>>>();

		public QaScanner()
		{
			var t = new Importer();
			_plugins = t.DoImport();

			try
			{
				foreach (IQAPlugin plugin in _plugins)
				{
					_crawlCompleteCallbacks.Add(plugin.RegisterHook("crawlComplete"));
				}
			}
			catch (ReflectionTypeLoadException e)
			{
				Console.WriteLine("Plugin Failed loading and is invalid");
				Console.WriteLine(e.LoaderExceptions); 
			}

		}

		/**
		 * Scan
		 * 
		 * Start the Scan with the set parameters
		 * */
		public void Scan()
		{
			_crawler = new PoliteWebCrawler();
			_crawler.PageCrawlStartingAsync += crawler_ProcessPageCrawlStarting;
			_crawler.PageCrawlCompletedAsync += crawler_ProcessPageCrawlCompleted;
			_crawler.PageCrawlDisallowedAsync += crawler_PageCrawlDisallowed;
			_crawler.PageLinksCrawlDisallowedAsync += crawler_PageLinksCrawlDisallowed;
			/*_crawler.ShouldCrawlPage(decision_ShouldCrawlPage);
			_crawler.ShouldDownloadPageContent(decision_ShouldDownloadPageContent);
			_crawler.ShouldCrawlPageLinks(decision_ShouldCrawlPageLinks);*/

			try
			{
				_cancellationToken = new CancellationTokenSource();
				CrawlResult result = _crawler.Crawl(new Uri(_scanUrl), _cancellationToken);
			}
			catch (UriFormatException e)
			{
				Console.WriteLine("URI provided is invalid and bad");
				Console.WriteLine(e.Message); 
			}
		}

		public void SetUrl(string url)
		{
			_scanUrl = url;
		}

		/**
		 * decision_ShouldCrawlPageLinks
		 * 
		 * Hook to make a decision if the page should be crawled
		 * */
		private CrawlDecision decision_ShouldCrawlPageLinks(CrawledPage arg1, CrawlContext arg2)
		{
			throw new NotImplementedException();
		}

		/**
		 * decision_ShouldDownloadPageContent
		 * 
		 * Hook to make a decision if the page content should be downloaded
		 * */
		private CrawlDecision decision_ShouldDownloadPageContent(CrawledPage arg1, CrawlContext arg2)
		{
			throw new NotImplementedException();
		}

		/**
		 * decision_ShouldCrawlPage
		 * 
		 * Hook to make a decision if the page should be crawled
		 * */
		private CrawlDecision decision_ShouldCrawlPage(PageToCrawl pageToCrawl, CrawlContext crawlContext)
		{
			throw new NotImplementedException();
		}

		void crawler_ProcessPageCrawlStarting(object sender, PageCrawlStartingArgs e)
		{
			PageToCrawl pageToCrawl = e.PageToCrawl;
			Console.WriteLine("About to crawl link {0} which was found on page {1}", pageToCrawl.Uri.AbsoluteUri, pageToCrawl.ParentUri.AbsoluteUri);
		}

		void crawler_ProcessPageCrawlCompleted(object sender, PageCrawlCompletedArgs e)
		{
			CrawledPage crawledPage = e.CrawledPage;

			if (crawledPage.WebException != null || crawledPage.HttpWebResponse.StatusCode != HttpStatusCode.OK)
				Console.WriteLine("Crawl of page failed {0}", crawledPage.Uri.AbsoluteUri);
			else
				Console.WriteLine("Crawl of page succeeded {0}", crawledPage.Uri.AbsoluteUri);

			if (string.IsNullOrEmpty(crawledPage.Content.Text))
				Console.WriteLine("Page had no content {0}", crawledPage.Uri.AbsoluteUri);

			
			var currentPage = new Page();
			currentPage.Url = crawledPage.Uri.AbsoluteUri;
			currentPage.Body = crawledPage.Content.Text;
			currentPage.StatusCode = crawledPage.HttpWebResponse.StatusCode.ToString();

			using (var ctx = new Context())
			{
				ctx.Pages.Add(currentPage);
				ctx.SaveChanges();
			}

			foreach (var callback in _crawlCompleteCallbacks)
			{
				List<ScanError> errorResponse = callback(crawledPage);

				foreach (var error in errorResponse)
				{
					error.Page = currentPage;

					using (var ctx = new Context())
					{
						ctx.Errors.Add(error);
						ctx.SaveChanges();
					}
				}
			}

		}

		void crawler_PageLinksCrawlDisallowed(object sender, PageLinksCrawlDisallowedArgs e)
		{
			CrawledPage crawledPage = e.CrawledPage;
			Console.WriteLine("Did not crawl the links on page {0} due to {1}", crawledPage.Uri.AbsoluteUri, e.DisallowedReason);
		}

		void crawler_PageCrawlDisallowed(object sender, PageCrawlDisallowedArgs e)
		{
			PageToCrawl pageToCrawl = e.PageToCrawl;
			Console.WriteLine("Did not crawl page {0} due to {1}", pageToCrawl.Uri.AbsoluteUri, e.DisallowedReason);
		}
	}
}
