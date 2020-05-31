using System.IO;
using System.Linq;
using System.Reflection;

namespace Quicksearch.Util
{
    internal static class ResourceLoader
    {
        internal static string ReadResourceFile(string filename, Assembly a = null)
        {
            if (a == null)
                a = Assembly.GetExecutingAssembly();
            var names = a.GetManifestResourceNames();
            var resourcePath = names.Single(n => n.EndsWith(filename));

            using(var s = a.GetManifestResourceStream(resourcePath))
            {
                using (var r = new StreamReader(s))
                {
                    return r.ReadToEnd();
                }
            }
        }

    }
}
