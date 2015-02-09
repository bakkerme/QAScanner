namespace ValueStorage
{
	//(:scanId, :url, :body, :statusCode, :pageId)
	public class Page
	{
		public int Id { get; set; }
		public int ScanId { get; set; }
		public string Url { get; set; }
		public string Body { get; set; }
		public string StatusCode { get; set; }
	}
}
