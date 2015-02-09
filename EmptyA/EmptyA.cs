using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Abot.Poco;
using CsQuery;
using QAPlugin;
using ValueStorage;
using ExtensionMethods;

namespace EmptyA
{
	[Export(typeof(IQAPlugin))]
    public class EmptyA : IQAPlugin
    {
		private const String Author = "Bakker";
		private const String Name = "EmptyA";
		private const String Description = "Finds empty a tags in the body";

		private const String ErrorDescription = "Found empty a tag";


		public ScanError Perform()
		{
			throw new NotImplementedException();
		}

		public string getAuthor()
		{
			return Author;
		}

		public string getName()
		{
			return Name;
		}

		public string getDescription()
		{
			return Description;
		}

		public Func<CrawledPage, List<ScanError>> RegisterHook(string hookType)
		{
			switch (hookType)
			{
				case "crawlComplete":
					return HandleCrawlComplete;
			}

			return null;
		}

		private List<ScanError> HandleCrawlComplete(CrawledPage pageContent)
		{
			string body = pageContent.Content.Text;
			CQ dom = body;
			body = dom.Render();

			//This is an extension method
			CQ result = dom["a[href='']"];
			var resultList = result.ToList();

			var returnErrorList = new List<ScanError>();
			foreach(var find in resultList)
			{
				var error = new ScanError();
				error.Description = ErrorDescription;
				error.LineNumber = this.FindStringLineNumber(find.Render(), body);
				error.Code = find.Render();

				returnErrorList.Add(error);
			}

			return returnErrorList;
		}
    }
}
