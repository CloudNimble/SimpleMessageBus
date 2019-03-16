using Microsoft.Azure.WebJobs;
using System;
using System.IO;

namespace SimpleMessageBus.Samples.OnPrem
{

    /// <summary>
    /// This class represents a set of test functions to ensure that Azure WebJobs works locally file FileTriggers.
    /// </summary>
    public class Functions
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="converted"></param>
        /// <remarks>
        /// Drop a file in the "convert" directory, and this function will reverse the contents and write the file to the "converted" directory.
        /// </remarks>
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