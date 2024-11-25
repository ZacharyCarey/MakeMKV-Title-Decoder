using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace MakeMKV_Title_Decoder {

    public partial class TaskProgressViewerForm : Form {

        public TaskProgressViewerForm() {
            InitializeComponent();
        }

        virtual protected async void TaskProgress_Load(object sender, EventArgs e) {

        }

        private void SetProgress(ProgressBar bar, uint value, uint max) {
            if (max == 0)
            {
                bar.Enabled = false;
                bar.Value = 0;
                return;
            } else
            {
                value = Math.Min(value, max);
                bar.Maximum = (int)max;
                bar.Value = (int)value;
                bar.Enabled = true;
            }
        }

        protected void SetProgress(TaskProgress progress) {
            SetProgress(CurrentProgress, progress.Current, progress.CurrentMax);
            SetProgress(TotalProgress, progress.Total, progress.TotalMax);
        }

        public static void Run<TProgress>(Action<IProgress<TProgress>> action) where TProgress : TaskProgress
        {
            var progressViewer = new TaskProgressViewer<Task, TProgress>((IProgress<TProgress> progress) =>
            {
                return Task.Run(() => {
                    action(progress);
                });
            });
            progressViewer.ShowDialog();
        }

        public static TResult Run<TProgress, TResult>(Func<IProgress<TProgress>, TResult> func) where TProgress : TaskProgress
        {
            var progressViewer = new TaskProgressViewer<Task<TResult>, TProgress>((IProgress<TProgress> progress) =>
            {
                return Task<TResult>.Run(() => {
                    return func(progress);
                });
            });
            progressViewer.ShowDialog();
            return progressViewer.Task.Result;
        }
    }

    public class TaskProgressViewer<TTask, TProgress> : TaskProgressViewerForm 
        where TProgress : TaskProgress 
        where TTask : Task 
    {

        public TTask? Task { get; private set; } = null;
        IProgress<TProgress> Progress;
        Func<IProgress<TProgress>, TTask> TaskGetter;

        public TaskProgressViewer(Func<IProgress<TProgress>, TTask> task) {
            TaskGetter = task;
            this.Progress = new Progress<TProgress>(this.SetProgress);
        }

        protected override async void TaskProgress_Load(object sender, EventArgs e) {
            Task = TaskGetter(Progress);
            await Task;
            this.Close();
        }

        private void SetProgress(TProgress progress) {
            base.SetProgress(progress);
        }

    }
}
