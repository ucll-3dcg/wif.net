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
        public const string RAYTRACER_PATH = @"E:\repos\ucll\shared\scripts\raytracer.exe";

        // If the script has a filename, it will be saved upon rendering.
        public const bool AUTO_SAVE = false;
    }
}
