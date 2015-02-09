using System;
using System.Collections.Generic;
using Abot.Poco;
using ValueStorage;

namespace QAPlugin
{
	public interface IQAPlugin
	{
		ScanError Perform();
		String getAuthor();
		String getName();
		String getDescription();

		Func<CrawledPage, List<ScanError>> RegisterHook(string hookType);
	}
}