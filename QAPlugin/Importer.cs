using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.Collections.Generic;
using System.IO;

namespace QAPlugin
{
    public class Importer
    {
        [ImportMany]
	    private IEnumerable<IQAPlugin> plugins;

        public IEnumerable<IQAPlugin> DoImport()
        {
	        var directoryCatalog = new DirectoryCatalog(
		        Path.GetDirectoryName(
			        Assembly.GetExecutingAssembly().Location
			        ) + "\\Plugins"
		        );

	        var container = new CompositionContainer(directoryCatalog);

	        try
	        {
		        return container.GetExportedValues<IQAPlugin>();
	        }
	        catch (ReflectionTypeLoadException e)
	        {
				Console.WriteLine("Plugin Failed loading and is invalid");
				Console.WriteLine(e.LoaderExceptions); 
	        }

	        return null;
        }

	}
}
