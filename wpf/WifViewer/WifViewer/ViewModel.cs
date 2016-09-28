using Cells;
using Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace WifViewer
{
    public class ViewModel
    {
        private readonly IView view;

        public ViewModel(IView view)
        {
            this.view = view;

            Open = EnabledCommand.FromDelegate(OnOpen);
            Refresh = EnabledCommand.FromDelegate(OnRefresh);
            Export = EnabledCommand.FromDelegate(OnExport);
            ToggleAnimation = EnabledCommand.FromDelegate(OnToggleAnimation);
            Path = Cell.Create<string>();
            Path.ValueChanged += OnPathChanged;
            Frames = Cell.Create(new List<WriteableBitmap>());
            CurrentFrameIndex = Cell.Create(0);
            CurrentFrame = Cell.Derived(Frames, CurrentFrameIndex, (f, i) => i < f.Count ? f[i] : null);
            LastFrameIndex = Cell.Derived(Frames, f => f.Count - 1);
            LoadFailed = Cell.Create(false);
            IsAnimating = Cell.Create(false);            

            // Opened at startup
            Path.Value = @"e:\temp\output\test.wif";
        }

        public ICommand Open { get; }

        public ICommand Refresh { get; }

        public ICommand Export { get; }

        public ICommand ToggleAnimation { get; }

        private void OnToggleAnimation()
        {
            IsAnimating.Value = !IsAnimating.Value;
        }

        private void OnOpen()
        {
            var filename = view.GetWifPath();

            if ( filename != null )
            {
                Path.Value = filename;
            }
        }

        private void OnRefresh()
        {
            LoadWif(Path.Value);
        }

        private void OnPathChanged()
        {
            if (Path.Value != null)
            {
                LoadWif(Path.Value);
            }
        }

        private void LoadWif(string path)
        {
            CurrentFrameIndex.Value = 0;
            IsAnimating.Value = false;

            try
            {
                Frames.Value = WifLoader.Load(path);
                LoadFailed.Value = false;
            }
            catch (Exception)
            {
                Frames.Value = new List<WriteableBitmap>();
                LoadFailed.Value = true;
            }
        }

        private void OnExport()
        {
            if ( Frames.Value != null )
            {
                var path = view.GetExportPath();

                if (path != null)
                {
                    MovieExporter.Export(Frames.Value, path);
                }
            }            
        }

        public void Tick()
        {
            if ( IsAnimating.Value )
            {
                OnNextFrame();
            }
        }

        private void OnNextFrame()
        {
            if ( Frames.Value != null && Frames.Value.Count != 0 )
            {
                CurrentFrameIndex.Value = (CurrentFrameIndex.Value + 1) % Frames.Value.Count;
            }
        }

        public Cell<string> Path { get; }

        private Cell<List<WriteableBitmap>> Frames { get; }

        public Cell<WriteableBitmap> CurrentFrame { get; }

        public Cell<int> CurrentFrameIndex { get; }

        public Cell<int> LastFrameIndex { get; }

        public Cell<bool> LoadFailed { get; }

        public Cell<bool> IsAnimating { get; }
    }

    public interface IView
    {
        string GetWifPath();

        string GetExportPath();
    }
}
