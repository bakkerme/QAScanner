using System.Data.Entity;

namespace ValueStorage.ScannerDataLayer
{
	public class Context: DbContext 
	{
		public Context(): base()
		{ }
		public DbSet<ScanError> Errors { get; set; }
			public DbSet<Page> Pages { get; set; }
	}
}
