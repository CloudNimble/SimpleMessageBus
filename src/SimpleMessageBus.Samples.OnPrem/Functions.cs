using Microsoft.Azure.WebJobs;
using System;
using System.IO;

namespace SimpleMessageBus.Samples.OnPrem
{
    public class Functions
    {

        private IServiceProvider _provider;

        public Functions(IServiceProvider provider)
        {
            _provider = provider;
        }

        // Drop a file in the "convert" directory, and this function will reverse it
        // the contents and write the file to the "converted" directory.
        public void Converter(
            [FileTrigger(@"convert\{name}", "*.txt", autoDelete: true)] string file,
            [File(@"converted\{name}", FileAccess.Write)] out string converted)
        {
            char[] arr = file.ToCharArray();
            Array.Reverse(arr);
            converted = new string(arr);
        }

    }
}
