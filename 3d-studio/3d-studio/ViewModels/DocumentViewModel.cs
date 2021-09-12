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
using WifViewer.Rendering;
using WifViewer.ViewModels;

namespace WifViewer.ViewModels
{
    public class DocumentViewModel
    {
        public DocumentViewModel() : this("", "untitled")
        {
            // NOP
        }

        public DocumentViewModel(string contents, string path)
        {
            this.Source = new TextDocument(contents);
            this.Path = Cell.Create(path);
            this.ShortPath = Cell.Derived(this.Path, DeriveShortPath);
            this.SaveScriptCommand = EnabledCommand.FromDelegate(OnSaveScript);
            this.SaveScriptAsCommand = EnabledCommand.FromDelegate(OnSaveScriptAs);
            this.RenderScript = EnabledCommand.FromDelegate(OnRenderScript);
            this.IsDirty = Cell.Create(false);
            this.Source.Changed += (s, e) => OnSourceChanged();
        }

        public TextDocument Source { get; }

        public Cell<string> Path { get; }

        public Cell<string> ShortPath { get; }

        public Cell<bool> IsDirty { get; }

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

        public ICommand SaveScriptCommand { get; }

        public ICommand SaveScriptAsCommand { get; }

        public ICommand RenderScript { get; }

        private string DeriveShortPath(string path)
        {
            return System.IO.Path.GetFileName(path);
        }

        private void OnRenderScript()
        {
            if ( HasFilename() && Configuration.AUTO_SAVE )
            {
                Save();
            }

            var animationVM = new AnimationViewModel();
            var raytracer = new Renderer();
            var receiver = animationVM.CreateReceiver();

            try
            {
                raytracer.Render(this.Source.Text, receiver);

                var viewer = new AnimationWindow(animationVM);
                viewer.Show();
            }
            catch ( Exception e )
            {
                MessageBox.Show(e.Message);
            }
        }

        private bool HasFilename()
        {
            return this.Path.Value != "untitled";
        }

        private void Save()
        {
            File.WriteAllText(this.Path.Value, this.SourceString);
            this.IsDirty.Value = false;
        }

        private void OnSaveScript()
        {
            if (!HasFilename())
            {
                OnSaveScriptAs();
            }
            else
            {
                Save();
            }
        }

        private void OnSaveScriptAs()
        {
            var fileDialog = new SaveFileDialog()
            {
                Filter = "Scripts|*.chai",
                AddExtension = true,
                CheckPathExists = true,
                OverwritePrompt = true
            };

            var result = fileDialog.ShowDialog();

            if (result == true)
            {
                this.Path.Value = fileDialog.FileName;
                OnSaveScript();
            }
        }

        private void OnSourceChanged()
        {
            this.IsDirty.Value = true;
        }
    }
}
