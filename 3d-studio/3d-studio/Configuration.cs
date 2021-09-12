using Cells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WifViewer
{
    public static class Configuration
    {
        // Backslashes allowed, don't bother changing them into forward slashes
        public static string RAYTRACER_PATH = @"G:\repos\ucll\3dcg\raytracer\raytracer\x64\Release\raytracer.exe";

        // If the script has a filename, it will be saved upon rendering.
        public static bool AUTO_SAVE = true;
    }
}
