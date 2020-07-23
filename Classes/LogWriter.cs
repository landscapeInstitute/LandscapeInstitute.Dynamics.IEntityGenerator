using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandscapeInstitute.Dynamics.IEntityGenerator.Classes
{
    public static class LogWriter
    {

        public static string OutputFile = "output.log";

        public static void LastFailed(string content)
        {
            File.WriteAllText(OutputFile, content);
        }

    }
}
