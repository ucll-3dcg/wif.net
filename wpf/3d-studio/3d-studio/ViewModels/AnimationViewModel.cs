using Cells;
using Commands;
using ICSharpCode.AvalonEdit.Document;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
            this.IsDoneRendering = Cell.Create(false);
            this.MaximumFrameIndex = Cell.Derived(() => this.Frames.Count - 1);
            this.Messages = new TextDocument();
            this.Frames.CollectionChanged += (sender, e) => OnFrameCollectionChanged();
            this.ToggleAnimation = EnabledCommand.FromDelegate(OnToggleAnimation);
            this.ScaleToFill = Cell.Create(true);
            this.ToggleScale = EnabledCommand.CreateTogglingCommand(ScaleToFill);
            this.FullScreen = Cell.Create(false);
            this.ToggleFullScreen = EnabledCommand.CreateTogglingCommand(FullScreen);
            this.ExportFrame = EnabledCommand.FromDelegate(OnExportFrame);
            this.ExportMovie = CellCommand.FromDelegate(this.IsDoneRendering, OnExportMovie);
            this.CopyFrame = EnabledCommand.FromDelegate(OnCopyFrame);
        }

        private void OnToggleAnimation()
        {
            this.Timer.IsEnabled = !this.Timer.IsEnabled;
        }

        public ICommand ToggleAnimation { get; }

        public DispatcherTimer Timer { get; }

        private void OnTimerTick()
        {
            if (this.Frames.Count > 0)
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
            this.IsDoneRendering.Value = true;
        }

        private void Message(string message)
        {
            this.Messages.Text += message + "\n";
        }

        public Cell<int> CurrentFrameIndex { get; }

        public Cell<WriteableBitmap> CurrentFrame { get; }

        public Cell<int> MaximumFrameIndex { get; }

        public TextDocument Messages { get; }

        public Cell<bool> IsAnimating { get; }

        public Cell<bool> ScaleToFill { get; }

        public ICommand ToggleScale { get; }

        public Cell<bool> FullScreen { get; }

        public ICommand ToggleFullScreen { get; }

        public Cell<bool> IsDoneRendering { get; }

        public ICommand ExportFrame { get; }

        public ICommand ExportMovie { get; }

        public ICommand CopyFrame { get; }

        public IRenderReceiver CreateReceiver()
        {
            return new RendererReceiver(this);
        }

        private void OnCopyFrame()
        {
            Clipboard.SetImage(this.CurrentFrame.Value);
        }

        private void OnExportFrame()
        {
            var saveDialog = new SaveFileDialog()
            {
                Filter = "Gif Files|*.gif|Jpeg Files|*.jpeg|Png Files|*.png",
                AddExtension = true,
                OverwritePrompt = true,
                ValidateNames = true
            };

            if (saveDialog.ShowDialog() == true)
            {
                var frame = this.CurrentFrame.Value;
                var path = saveDialog.FileName;

                BitmapEncoder encoder;

                if (path.ToLower().EndsWith(".png"))
                {
                    encoder = new PngBitmapEncoder();
                }
                else if (path.ToLower().EndsWith(".jpeg"))
                {
                    encoder = new JpegBitmapEncoder();
                }
                else if (path.ToLower().EndsWith(".gif"))
                {
                    encoder = new GifBitmapEncoder();
                }
                else
                {
                    MessageBox.Show("Bug");
                    return;
                }

                encoder.Frames.Add(BitmapFrame.Create(frame));

                using (var file = File.OpenWrite(path))
                {
                    encoder.Save(file);
                }
            }
        }

        private void OnExportMovie()
        {
            var saveDialog = new SaveFileDialog()
            {
                Filter = "Gif Files|*.gif",
                AddExtension = true,
                OverwritePrompt = true,
                ValidateNames = true
            };

            if (saveDialog.ShowDialog() == true)
            {
                var frames = new List<WriteableBitmap>(this.Frames);
                var path = saveDialog.FileName;

                ThreadPool.QueueUserWorkItem(_ =>
                {
                    var encoder = new GifBitmapEncoder();

                    foreach (var frame in frames)
                    {
                        var bitmapFrame = BitmapFrame.Create(frame);
                        encoder.Frames.Add(bitmapFrame);
                    }

                    using (var file = File.OpenWrite(path))
                    {
                        encoder.Save(file);
                    }
                });
            }
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
