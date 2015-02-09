using System.Dynamic;

namespace ValueStorage
{
	//  (scanId, pageId, errorText, lineNumber, code)
	public class ScanError
	{
		public int Id { get; set; }
		public int ScanId { get; set; }
		public Page Page { get; set; }
		public int LineNumber { get; set; }
		public string Description { get; set; }
		public string Code { get; set; }
	}
}
