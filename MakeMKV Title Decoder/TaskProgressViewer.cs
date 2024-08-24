using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MakeMKV_Title_Decoder {

    public interface TaskProgress {
        public UInt32 Current { get; }
        public UInt32 CurrentMax { get; }

        public UInt32 Total { get; }
        public UInt32 TotalMax { get; }
    }

    public struct SimpleProgress : TaskProgress {
        
        public UInt32 Current { get; set; }
        public UInt32 CurrentMax { get; set; }

        public UInt32 Total { get; set; }
        public UInt32 TotalMax { get; set; }

        public SimpleProgress() {
            this.Current = 0;
            this.CurrentMax = 0;

            this.Total = 0;
            this.TotalMax = 0;
        }

        public SimpleProgress(UInt32 total, UInt32 totalMax) {
            this.Current = 0;
            this.CurrentMax = 0;

            this.Total = total;
            this.TotalMax = totalMax;
        }

        public SimpleProgress(UInt32 current, UInt32 currentMax, UInt32 total, UInt32 totalMax) {
            this.Current = current;
            this.CurrentMax = currentMax;

            this.Total = total;
            this.TotalMax = totalMax;
        }

    }

    public partial class TaskProgressViewer : Form {

        public TaskProgressViewer() {
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
    }

    public class TaskProgressViewer<TTask, TProgress> : TaskProgressViewer 
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
