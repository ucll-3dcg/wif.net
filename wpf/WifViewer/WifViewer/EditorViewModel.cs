using Cells;
using Commands;
using ICSharpCode.AvalonEdit.Document;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WifViewer
{
    public class EditorViewModel
    {
        public EditorViewModel()
        {
            this.Source =new TextDocument();
            this.Path = Cell.Create("untitled");
            this.NewScript = EnabledCommand.FromDelegate(OnNewScript);
            this.LoadScript = EnabledCommand.FromDelegate(OnLoadScript);
            this.SaveScript = EnabledCommand.FromDelegate(OnSaveScript);
            this.RenderScript = EnabledCommand.FromDelegate(OnRenderScript);
            this.LoadWif = EnabledCommand.FromDelegate(OnLoadWif);
        }

        public TextDocument Source { get; }

        public Cell<string> Path { get; }

        public string SourceString
        {
            get
            {
                return Source.Text;
            }
            set
            {
                this.Source.Text = value;
            }
        }

        public ICommand LoadScript { get; }

        public ICommand NewScript { get; }

        public ICommand SaveScript { get; }

        public ICommand RenderScript { get; }

        public ICommand LoadWif { get; }

        private void OnLoadWif()
        {
            var fileDialog = new OpenFileDialog()
            {
                Filter = "Wif Files|*.wif",
                CheckFileExists = true
            };

            var result = fileDialog.ShowDialog();

            if (result == true)
            {
                var animationVM = new AnimationViewModel();
                WifLoader.LoadInSeparateThread(fileDialog.FileName, animationVM.CreateReceiver());                

                var viewer = new AnimationWindow(animationVM);
                viewer.Show();
            }
        }

        private void OnRenderScript()
        {
            var animationVM = new AnimationViewModel();
            var raytracer = new Renderer();
            var receiver = animationVM.CreateReceiver();
            raytracer.Render(this.Source.Text, receiver);

            var viewer = new AnimationWindow(animationVM);
            viewer.Show();
        }

        private void OnNewScript()
        {
            SourceString = "";
            this.Path.Value = "untitled";
        }

        private void OnLoadScript()
        {
            var fileDialog = new OpenFileDialog()
            {
                Filter = "Scripts|*.chai",
                CheckFileExists = true
            };

            var result = fileDialog.ShowDialog();

            if ( result == true )
            {
                var source = File.ReadAllText(fileDialog.FileName);
                this.SourceString = source;
                this.Path.Value = fileDialog.FileName;
            }
        }

        private void OnSaveScript()
        {
            if ( this.Path.Value == "untitled")
            {
                var fileDialog = new SaveFileDialog()
                {
                    Filter = "Scripts|*.chai",
                    AddExtension = true,
                    CheckPathExists = true,
                    OverwritePrompt = true
                };

                var result = fileDialog.ShowDialog();

                if ( result == true )
                {
                    this.Path.Value = fileDialog.FileName;
                }
                else
                {
                    return;
                }
            }

            File.WriteAllText(this.Path.Value, this.SourceString);
        }
    }
}
