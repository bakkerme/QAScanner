using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using CsQuery;
using QAPlugin;

namespace ExtensionMethods
{
	public static class PluginExtensions
	{
		public static int FindStringLineNumber(this IQAPlugin iqaPlugin, String needle, String haystack)
		{
			//Ensure both are the same
			needle = HttpUtility.HtmlDecode(needle);
			haystack = HttpUtility.HtmlDecode(haystack);

			//Spit the strings at new line to arrays
			string[] needleLines = needle.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
			string[] haystackLines = haystack.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

			//Trim both needle and haystack lines
			for (int i = 0; i < needleLines.Length; i++)
			{
				needleLines[i] = needleLines[i].Trim();
			}

			for (int i = 0; i < haystackLines.Length; i++)
			{
				haystackLines[i] = haystackLines[i].Trim();
			}

			int matchLineNum = -1;
			for (int i = 0; i < haystackLines.Length; i++)
			{
				//Initial match
				if (haystackLines[i].Contains(needleLines[0]))
				{
					bool gotMatch = false;
					//Lets check for a full match
					for (int j = 0; j < needleLines.Length; j++)
					{
						//If we aint find no match
						if (!haystackLines[i + j].Contains(needleLines[j]))
							break;

						if (j == needleLines.Length - 1)
						{
							gotMatch = true;
						}
					}

					if (gotMatch)
					{
						matchLineNum = i;
						break;
					}
				}

			}

			return matchLineNum + 1;
		}
	}
}
