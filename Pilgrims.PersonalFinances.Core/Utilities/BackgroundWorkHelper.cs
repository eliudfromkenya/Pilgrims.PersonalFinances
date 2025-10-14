using System.ComponentModel;

namespace Pilgrims.PersonalFinances.Core.Utilities
{
    internal class BackgroundWorkHelper
    {
        private readonly ValueMonitor<int> percentageProgress = new(0);
        private readonly ValueMonitor<TimeSpan> timeLeft = new(TimeSpan.MaxValue);
        private DateTime startTime;
        private List<Action>? toDo;
        private BackgroundWorker? worker;

        public BackgroundWorkHelper()
        {
            IsParallel = false;
            BackgroundWorker.WorkerReportsProgress = true;
            BackgroundWorker.WorkerSupportsCancellation = true;
            percentageProgress.ValueChanged += PercentageProgress_ValueChanged;

            BackgroundWorker.DoWork += Worker_DoWork;
        }

        public BackgroundWorkHelper(List<Action> actionsToDo)
            : this()
        {
            toDo = actionsToDo;
        }

        public BackgroundWorker BackgroundWorker => worker ??= new BackgroundWorker();

        public bool IsParallel { get; set; }

        public IValueMonitor<TimeSpan> TimeLeft => timeLeft;

        public int Total => toDo == null ? 0 : toDo.Count;

        public void SetActionsTodo(List<Action> toDoActions, bool cancelCurrent = false)
        {
            if (BackgroundWorker.IsBusy && cancelCurrent)
                BackgroundWorker.CancelAsync();

            BackgroundWorker.DoWork -= Worker_DoWork;
            BackgroundWorker.DoWork += Worker_DoWork;
            BackgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            toDo = toDoActions;
        }

        private void BackgroundWorker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (sender is BackgroundWorker worker)
                worker.Dispose();
        }

        private void PercentageProgress_ValueChanged(int oldValue, int newValue)
        {
            BackgroundWorker.ReportProgress(newValue);
        }

        private void Worker_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (toDo == null) throw new InvalidOperationException("You must provide actions to execute");
            Thread.Sleep(10);
            var total = toDo.Count;
            startTime = DateTime.Now;
            var current = 0;

            if (IsParallel == false)
                foreach (var next in toDo)
                {
                    next();
                    current++;
                    if (worker?.CancellationPending ?? false) return;
                    percentageProgress.Value = (int)(current / (double)total * 100.0);
                    var passedMs = (DateTime.Now - startTime).TotalMilliseconds;
                    var oneUnitMs = passedMs / current;
                    var leftMs = (total - current) * oneUnitMs;
                    timeLeft.Value = TimeSpan.FromMilliseconds(leftMs);
                }
            else
                try
                {
                    Parallel.For(0, total,
                        (index, loopstate) =>
                        {
                            toDo.ElementAt(index)();
                            if (worker?.CancellationPending ?? false) loopstate.Stop();
                            Interlocked.Increment(ref current);

                            percentageProgress.Value = (int)(current / (double)total * 100.0);
                            var passedMs = (DateTime.Now - startTime).TotalMilliseconds;
                            var oneUnitMs = passedMs / current;
                            var leftMs = (total - current) * oneUnitMs;
                            timeLeft.Value = TimeSpan.FromMilliseconds(leftMs);
                        });
                }
                catch (Exception ex)
                {
                    NotifyError("Background Action Error", ex);
                }
        }
    }
}