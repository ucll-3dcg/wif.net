using Cells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WifViewer;

namespace WifViewer.ViewModels
{
    public class ConfigurationViewModel
    {
        public ConfigurationViewModel()
        {
            this.RayTracerPath = Cell.Create(Configuration.RAYTRACER_PATH);
            this.AutoSave = Cell.Create(Configuration.AUTO_SAVE);
        }

        public Cell<string> RayTracerPath { get; }

        public Cell<bool> AutoSave { get; }

        public void AcceptChanges()
        {
            Configuration.RAYTRACER_PATH = this.RayTracerPath.Value;
            Configuration.AUTO_SAVE = this.AutoSave.Value;
        }
    }
}
