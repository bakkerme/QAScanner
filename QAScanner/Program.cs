using System;
using QAPlugin;
using QAScanner.Core;

namespace QAScanner
{
	class Program
	{
		static void Main(string[] args)
		{
			var qascanner = new QaScanner();
			qascanner.SetUrl("http://192.168.186.132");
			qascanner.Scan();
		}
	}
}
