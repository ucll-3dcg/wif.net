using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WifViewer.Rendering
{
    public class ExternalProcess
    {
        public string ExecutablePath { get; set; }

        public string CommandLineArguments { get; set; }

        public string Input { get; set; }

        public Action<string> OnOutputDataReceived { get; set; }

        public Action<string> OnErrorDataReceived { get; set; }

        public Action OnExited { get; set; }

        private Process process;

        public void Start()
        {
            process = new Process();
            process.StartInfo.FileName = this.ExecutablePath;
            process.StartInfo.Arguments = this.CommandLineArguments;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.EnableRaisingEvents = true;

            process.OutputDataReceived += (sender, e) => OnOutputDataReceived?.Invoke(e.Data);
            process.ErrorDataReceived += (sender, e) => OnErrorDataReceived?.Invoke(e.Data);
            process.Exited += (sender, e) => OnExited?.Invoke();

            process.Start();

            if (this.Input != null)
            {
                process.StandardInput.WriteLine(this.Input);
                process.StandardInput.Close();
            }

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
        }

        public void Stop()
        {
            if (process != null && !process.HasExited)
                process.Kill();
        }
    }

}
