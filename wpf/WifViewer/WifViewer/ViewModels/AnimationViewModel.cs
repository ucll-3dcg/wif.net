using Cells;
using Commands;
using ICSharpCode.AvalonEdit.Document;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WifViewer.Rendering;

namespace WifViewer.ViewModels
{
    public class AnimationViewModel
    {
        public AnimationViewModel()
        {
            this.Timer = new DispatcherTimer(TimeSpan.FromMilliseconds(50), DispatcherPriority.Background, (o, e) => OnTimerTick(), Application.Current.Dispatcher)
            {
                IsEnabled = false
            };

            this.Frames = new ObservableCollection<WriteableBitmap>();
            this.CurrentFrameIndex = Cell.Create(0);
            this.CurrentFrame = Cell.Derived(this.CurrentFrameIndex, DeriveCurrentFrame);
            this.MaximumFrameIndex = Cell.Derived(() => this.Frames.Count - 1);
            this.Status = Cell.Create("Rendering...");
            this.Messages = new TextDocument();
            this.Frames.CollectionChanged += (sender, e) => OnFrameCollectionChanged();
            this.ToggleAnimation = EnabledCommand.FromDelegate(OnToggleAnimation);
            this.ScaleToFill = Cell.Create(true);
            this.ToggleScale = EnabledCommand.CreateTogglingCommand(ScaleToFill);
            this.FullScreen = Cell.Create(false);
            this.ToggleFullScreen = EnabledCommand.CreateTogglingCommand(FullScreen);
        }

        private void OnToggleAnimation()
        {
            this.Timer.IsEnabled = !this.Timer.IsEnabled;
        }

        public ICommand ToggleAnimation { get; }

        public DispatcherTimer Timer { get; }

        private void OnTimerTick()
        {
            if ( this.Frames.Count > 0 )
            {
                this.CurrentFrameIndex.Value = (this.CurrentFrameIndex.Value + 1) % this.Frames.Count;
            }
        }

        private void OnFrameCollectionChanged()
        {
            this.CurrentFrame.Refresh();
            this.MaximumFrameIndex.Refresh();
        }

        private WriteableBitmap DeriveCurrentFrame(int index)
        {
            if (index >= this.Frames.Count)
            {
                return null;
            }
            else
            {
                return this.Frames[index];
            }
        }

        public ObservableCollection<WriteableBitmap> Frames { get; }

        private void FrameRendered(WriteableBitmap frame)
        {
            this.Frames.Add(frame);
        }

        private void LastFrameRendered()
        {
            this.Status.Value = "Finished";
        }

        private void Message(string message)
        {
            this.Messages.Text += message + "\n";
        }

        public Cell<int> CurrentFrameIndex { get; }

        public Cell<WriteableBitmap> CurrentFrame { get; }

        public Cell<int> MaximumFrameIndex { get; }

        public Cell<string> Status { get; }

        public TextDocument Messages { get; }

        public Cell<bool> IsAnimating { get; }

        public Cell<bool> ScaleToFill { get; }

        public ICommand ToggleScale { get; }

        public Cell<bool> FullScreen { get; }

        public ICommand ToggleFullScreen { get; }

        public IRenderReceiver CreateReceiver()
        {
            return new RendererReceiver(this);
        }

        private class RendererReceiver : IRenderReceiver
        {
            private readonly AnimationViewModel parent;

            public RendererReceiver(AnimationViewModel parent)
            {
                this.parent = parent;
            }

            public void FrameRendered(WriteableBitmap frame)
            {
                Action action = () => parent.FrameRendered(frame);
                Application.Current.Dispatcher.BeginInvoke(action);
            }

            public void RenderingDone()
            {
                Action action = () => parent.LastFrameRendered();
                Application.Current.Dispatcher.BeginInvoke(action);
            }

            public void Message(string message)
            {
                Action action = () => parent.Message(message);
                Application.Current.Dispatcher.BeginInvoke(action);
            }
        }
    }
}
