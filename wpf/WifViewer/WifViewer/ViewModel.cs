using Cells;
using Commands;
using ICSharpCode.AvalonEdit.Document;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
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
            SelectRaytracer = EnabledCommand.FromDelegate(OnSelectRaytracer);
            NewChai = EnabledCommand.FromDelegate(OnNewChai);
            OpenChai = EnabledCommand.FromDelegate(OnOpenChai);
            SaveChai = EnabledCommand.FromDelegate(OnSaveChai);
            SaveAsChai = EnabledCommand.FromDelegate(OnSaveAsChai);
            Build = EnabledCommand.FromDelegate(OnBuild);
            Refresh = EnabledCommand.FromDelegate(OnRefresh);
            Export = EnabledCommand.FromDelegate(OnExport);
            ToggleAnimation = EnabledCommand.FromDelegate(OnToggleAnimation);
            SetRaytracer = EnabledCommand.FromDelegate(OnSetRaytracer);
            ClearConsole = EnabledCommand.FromDelegate(OnClearConsole);
            Path = Cell.Create<string>();
            Path.ValueChanged += OnPathChanged;
            RaytracerPath = Cell.Create<string>();
            ChaiPath = Cell.Create<string>();
            ChaiPath.ValueChanged += OnChaiPathChanged;
            Frames = Cell.Create(new List<WriteableBitmap>());
            CurrentFrameIndex = Cell.Create(0);
            CurrentFrame = Cell.Derived(Frames, CurrentFrameIndex, (f, i) => i < f.Count ? f[i] : null);
            LastFrameIndex = Cell.Derived(Frames, f => f.Count - 1);
            LoadFailed = Cell.Create(false);
            LoadChaiFailed = Cell.Create(false);
            IsAnimating = Cell.Create(false);
            FramesToBuild = Cell.Create(0);
            BuiltFrames = Cell.Create(0);
            IsRaytracing = Cell.Create(false);
            IsRaytracing.ValueChanged += reloadWifTmp;
            ConsoleText = Cell.Create(new TextDocument());

            ChaiScript = Cell.Create(new TextDocument());
            IsEditorPrestine = Cell.Create(true);
            ChaiScript.Value.TextChanged += Value_TextChanged;

            // Opened at startup
            Path.Value = @"e:\temp\output\test.wif";
            ChaiPath.Value = null;

        }

        public ICommand Open { get; }
        public ICommand SelectRaytracer { get; }
        public ICommand NewChai { get; }
        public ICommand OpenChai { get; }
        public ICommand SaveChai { get; }
        public ICommand SaveAsChai { get; }
        public ICommand Build { get; }

        public ICommand Refresh { get; }

        public ICommand Export { get; }

        public ICommand ToggleAnimation { get; }
        public ICommand SetRaytracer { get; }
        public ICommand ClearConsole { get; }

        private void OnToggleAnimation()
        {
            IsAnimating.Value = !IsAnimating.Value;
        }

        private void OnOpen()
        {
            var filename = view.GetWifPath();

            if (filename != null)
            {
                Path.Value = filename;
            }
        }

        private void OnNewChai()
        {

            if (CanClearCurrentWork())
            {

                ChaiPath.Value = null;
                ChaiScript.Value = new TextDocument();

                IsEditorPrestine.Value = true;
                ChaiScript.Value.TextChanged += Value_TextChanged;

            }
            
        }

        private bool CanClearCurrentWork()
        {
            if (!IsEditorPrestine.Value)
            {
                var answer = view.SaveChanges();

                if (answer == MessageBoxResult.Cancel)
                {
                    return false;
                }
                else if (answer == MessageBoxResult.Yes)
                {
                    if (!SaveActiveChai(false)) { return false; }
                }
            }

            return true;
        }

        private void OnOpenChai()
        {
            if (CanClearCurrentWork())
            {

                var filename = view.GetChaiPath();

                if (filename != null)
                {
                    ChaiPath.Value = "";
                    ChaiPath.Value = filename;
                }
            }
        }

        private void OnSelectRaytracer()
        {
            var filename = view.GetRaytracerPath();

            if (filename != null)
            {
                RaytracerPath.Value = filename;
            }
        }

        private void OnSaveChai()
        {
            SaveActiveChai(false);
        }

        private void OnSaveAsChai()
        {
            SaveActiveChai(true);
        }

        private void OnBuild()
        {
            if (IsRaytracing.Value)
            { //TODO error that we ar allready raytracing :p
                return;
            }
            if (RaytracerPath.Value == null || !System.IO.File.Exists(RaytracerPath.Value))
            {
                SetRaytracerPath();
            }
            if (ChaiScript.Value.Text.Contains("{{wif}}") && System.IO.File.Exists(RaytracerPath.Value))

            {

                System.IO.File.WriteAllText("chai.tmp", ChaiScript.Value.Text.Replace("{{wif}}", "wif.tmp"));
                IsRaytracing.Value = true;

                Thread thr = GetRunnerThread(RaytracerPath.Value, @"-schai.tmp");
                FramesToBuild.Value = 0;
                BuiltFrames.Value = 0;
                OnClearConsole();
                OnClearPlayer();
                thr.Start();



            }
            else if (RaytracerPath.Value == null || !System.IO.File.Exists(RaytracerPath.Value))
            {

                view.NoRaytracerFound();

            }
            else
            {
                view.NoWifTagFound();
            }

        }

        private void SetRaytracerPath()
        {

            var result = view.GetRaytracerPath();
            if(result!=null && result.Length>0) //Only set if selected :)
            {

                RaytracerPath.Value = result;
            }
            
        }

        private void reloadWifTmp()
        {

            
            if(IsRaytracing.Value==false)
            {

                Path.Value = "wif.tmp";
                LoadWif("wif.tmp");

                IsAnimating.Value = true;
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

        private void OnChaiPathChanged()
        {
            if (ChaiPath.Value != null)
            {
                LoadChai(ChaiPath.Value);
            }
        }

        private void OnSetRaytracer()
        {
            SetRaytracerPath();
        }

        private void OnClearConsole()
        {
            ConsoleText.Value = new TextDocument();
        }

        private void OnClearPlayer()
        {
            Path.Value = "";
        }

        private Thread GetRunnerThread(string cmdPath, string args)
        {




            ThreadStart ths = new ThreadStart(() => {
                var proc = new Process();
                proc.StartInfo.FileName = cmdPath;
                proc.StartInfo.Arguments = args;


                // set up output redirection
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.UseShellExecute = false;
                proc.EnableRaisingEvents = true;
                proc.StartInfo.CreateNoWindow = true;
                // see below for output handler

                proc.ErrorDataReceived += proc_DataReceived;
                proc.OutputDataReceived += proc_DataReceived;


                bool ret = proc.Start();

                proc.BeginErrorReadLine();
                proc.BeginOutputReadLine();
                //is ret what you expect it to be....

                proc.WaitForExit();

                Application.Current.Dispatcher.Invoke(new Action(() => {

                    IsRaytracing.Value = false;
                }));
            });
            Thread th = new Thread(ths);
            return th;

        }

        void proc_DataReceived(object sender, DataReceivedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() => {

                if(e.Data==null)
                {
                    return;
                }

                // [default] 5 frames per second, 0.2s per frame, 25 frames
                Regex r = new Regex(@"^.*\[default\].*frame,\s(?<frames>\d+)\sframes",
                          RegexOptions.None, TimeSpan.FromMilliseconds(150));
                var s = r.Match(e.Data);
                if(s.Success)
                {

                    FramesToBuild.Value = Int32.Parse(r.Match(e.Data).Result("${frames}"));
                }

                // Writing WIF frame #24
                Regex r0 = new Regex(@"^.*Writing\sWIF\sframe\s#(?<frames>\d+)",
                          RegexOptions.None, TimeSpan.FromMilliseconds(150));
                var s0 = r0.Match(e.Data);
                if (s0.Success)
                {

                    BuiltFrames.Value = Int32.Parse(r0.Match(e.Data).Result("${frames}"))+1;
                }

                ConsoleText.Value.Insert(0, e.Data + Environment.NewLine);
                ConsoleText.Refresh();
            }));
        }


        private bool SaveActiveChai(bool saveAs)
        {
            var filename = ChaiPath.Value;

            bool fileExists = false;

            try { 
                fileExists = System.IO.File.Exists(filename);
            }
            catch (Exception)
            {}

            if (!saveAs && fileExists)
            {
                try
                {
                    System.IO.File.WriteAllText(filename, ChaiScript.Value.Text);
                    return true;
                }
                catch (Exception)
                {

                    LoadChaiFailed.Value = true;
                    return false;
                }
            }
            else
            {
                //Ask for filename to save
                filename = view.GetChaiSavePath();
                if (filename != null)
                {


                    try
                    {
                        System.IO.File.WriteAllText(filename, ChaiScript.Value.Text);
                        ChaiPath.Value = filename;
                        LoadChaiFailed.Value = false;
                        return true;
                    }
                    catch (Exception)
                    {

                        LoadChaiFailed.Value = true;
                        return false;
                    }

                }
                else
                {
                    return false;
                }
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



        private void LoadChai(string path)
        {
            LoadChaiFailed.Value = false;

            try
            {

                ChaiScript.Value = new TextDocument(System.IO.File.ReadAllText(path));
                IsEditorPrestine.Value = true;
                ChaiScript.Value.TextChanged += Value_TextChanged;

                LoadChaiFailed.Value = true;
            }
            catch (Exception)
            {

                LoadChaiFailed.Value = true;
            }
        }

        private void Value_TextChanged(object sender, EventArgs e)
        {

            IsEditorPrestine.Value = false;
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
        public Cell<string> RaytracerPath { get; }

        public Cell<string> ChaiPath { get; }

        public Cell<TextDocument> ConsoleText { get; }

        private Cell<List<WriteableBitmap>> Frames { get; }

        public Cell<WriteableBitmap> CurrentFrame { get; }

        public Cell<int> CurrentFrameIndex { get; }

        public Cell<int> LastFrameIndex { get; }

        public Cell<bool> LoadFailed { get; }

        public Cell<bool> LoadChaiFailed { get; }

        public Cell<bool> IsRaytracing { get; }

        public Cell<int> FramesToBuild { get; }

        public Cell<int> BuiltFrames { get; }

        public Cell<bool> IsAnimating { get; }

        public Cell<bool> IsEditorPrestine { get; }
        public Cell<TextDocument> ChaiScript { get; }
    }

    public interface IView
    {
        string GetWifPath();

        string GetRaytracerPath();

        string GetChaiPath();

        string GetChaiSavePath();

        string GetExportPath();

        void NoWifTagFound();

        void NoRaytracerFound();
        MessageBoxResult SaveChanges();
    }
}
