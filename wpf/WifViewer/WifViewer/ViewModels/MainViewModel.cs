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

            this.NewScript = EnabledCommand.FromDelegate(OnNewScript);
            this.LoadScript = EnabledCommand.FromDelegate(OnLoadScript);
            this.LoadWif = EnabledCommand.FromDelegate(OnLoadWif);
        }

        public ObservableCollection<DocumentViewModel> Documents { get; }

        public Cell<DocumentViewModel> CurrentDocument { get; }


        public ICommand LoadScript { get; }

        public ICommand NewScript { get; }

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
                var source = File.ReadAllText(fileDialog.FileName);
                var document = new DocumentViewModel(source, path);

                AddDocument(document);
            }
        }

        private void AddDocument(DocumentViewModel document)
        {
            this.Documents.Add(document);
            this.CurrentDocument.Value = document;
        }
    }
}
