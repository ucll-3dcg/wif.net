using Cells;
using Commands;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WifViewer.ViewModels;

namespace WifViewer
{
    public class MainViewModel
    {
        public MainViewModel()
        {
            this.Documents = new ObservableCollection<DocumentViewModel>();
            this.CurrentDocument = Cell.Create<DocumentViewModel>(null);

            this.NewScriptCommand = EnabledCommand.FromDelegate(OnNewScript);
            this.LoadScriptCommand = EnabledCommand.FromDelegate(OnLoadScript);
            this.LoadWifCommand = EnabledCommand.FromDelegate(OnLoadWif);
            this.LoadCommand = EnabledCommand.FromDelegate(OnLoad);
        }

        public ObservableCollection<DocumentViewModel> Documents { get; }

        public Cell<DocumentViewModel> CurrentDocument { get; }

        public ICommand LoadCommand { get; }

        public ICommand LoadScriptCommand { get; }

        public ICommand NewScriptCommand { get; }

        public ICommand LoadWifCommand { get; }

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
                var path = fileDialog.FileName;

                LoadWif(path);
            }
        }

        private void LoadWif(string path)
        {
            var animationVM = new AnimationViewModel();
            WifLoader.LoadInSeparateThread(path, animationVM.CreateReceiver());

            var viewer = new AnimationWindow(animationVM);
            viewer.Show();
        }

        private void OnNewScript()
        {
            var document = new DocumentViewModel();

            AddDocument(document);
        }

        private void OnLoadScript()
        {
            var fileDialog = new OpenFileDialog()
            {
                Filter = "Scripts|*.chai",
                CheckFileExists = true
            };

            var result = fileDialog.ShowDialog();

            if (result == true)
            {
                var path = fileDialog.FileName;
                LoadScript(path);
            }
        }

        private void LoadScript(string path)
        {
            var source = File.ReadAllText(path);
            var document = new DocumentViewModel(source, path);

            AddDocument(document);
        }

        private void OnLoad()
        {
            var fileDialog = new OpenFileDialog()
            {
                Filter = "Supported Files|*.chai;*.wif|Scripts|*.chai|Renderings|*.wif",
                CheckFileExists = true
            };

            var result = fileDialog.ShowDialog();

            if (result == true)
            {
                var path = fileDialog.FileName;

                if ( path.ToLower().EndsWith(".wif") )
                {
                    LoadWif(path);
                }
                else
                {
                    LoadScript(path);
                }
            }
        }

        private void AddDocument(DocumentViewModel document)
        {
            this.Documents.Add(document);
            this.CurrentDocument.Value = document;
        }
    }
}
