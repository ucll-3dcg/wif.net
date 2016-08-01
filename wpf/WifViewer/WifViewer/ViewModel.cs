using Cells;
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

            Open = new EnabledCommand(OnOpen);
            Refresh = new EnabledCommand(OnRefresh);
            Path = Cell.Create<string>();
            Path.ValueChanged += OnPathChanged;
            Frames = Cell.Create(new List<WriteableBitmap>());
            CurrentFrameIndex = Cell.Create(0);
            CurrentFrame = Cell.Derived(Frames, CurrentFrameIndex, (f, i) => i < f.Count ? f[i] : null);
            LastFrameIndex = Cell.Derived(Frames, f => f.Count);
            LoadFailed = Cell.Create(false);
        }

        public ICommand Open { get; }

        public ICommand Refresh { get; }

        private void OnOpen()
        {
            var filename = view.AskUserForFilename();

            if ( filename != null )
            {
                Path.Value = filename;
            }
        }

        private void OnRefresh()
        {
            LoadWif();
        }

        private void OnPathChanged()
        {
            if (Path.Value != null)
            {
                LoadWif();
            }
        }

        private void LoadWif()
        {
            CurrentFrameIndex.Value = 0;

            try
            {
                Frames.Value = WifLoader.Load(Path.Value);
                LoadFailed.Value = false;
            }
            catch (Exception)
            {
                Frames.Value = new List<WriteableBitmap>();
                LoadFailed.Value = true;
            }
        }

        public Cell<string> Path { get; }

        private Cell<List<WriteableBitmap>> Frames { get; }

        public Cell<WriteableBitmap> CurrentFrame { get; }

        public Cell<int> CurrentFrameIndex { get; }

        public Cell<int> LastFrameIndex { get; }

        public Cell<bool> LoadFailed { get; }
    }

    public interface IView
    {
        string AskUserForFilename();
    }
}
