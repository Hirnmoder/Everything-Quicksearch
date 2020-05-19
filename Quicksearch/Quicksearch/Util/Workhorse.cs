using System;
using System.Collections.Generic;
using System.Threading;

namespace Quicksearch.Util
{
    internal class Workhorse
    {
        private Thread Workthread;
        private object ThreadLockObj { get; } = new object();
        private Queue<Action> Tasks { get; } = new Queue<Action>();

        private bool _IsRunning = false;
        internal bool IsRunning => _IsRunning;

        private ManualResetEventSlim TaskEnqueuedEvent = new ManualResetEventSlim(false);

        internal Workhorse()
        {

        }

        internal void Enqueue(Action a)
        {
            lock (Tasks)
            {
                Tasks.Enqueue(a);
                TaskEnqueuedEvent.Set();
            }
            EnsureThreadWorking();
        }

        private void EnsureThreadWorking()
        {
            lock (ThreadLockObj)
            {
                if (Workthread == null)
                {
                    Workthread = new Thread(new ThreadStart(Work));
                }
                else
                {
                    if(!Workthread.IsAlive)
                    {
                        Workthread = new Thread(new ThreadStart(Work));
                    }
                }

                _IsRunning = true;
                if(Workthread.ThreadState == ThreadState.Unstarted)
                {
                    Workthread.IsBackground = true;
                    Workthread.Start();
                }
            }
        }

        internal void AbortAndClear()
        {
            _IsRunning = false;
            lock (Tasks)
            {
                Tasks.Clear();
            }
            TaskEnqueuedEvent.Set();
            lock (ThreadLockObj)
            {
                if (!(Workthread?.Join(500) ?? true))
                {
                    Workthread.Abort();
                }
                Workthread = null;
            }
        }

        private void Work()
        {
            try
            {
                while (_IsRunning)
                {
                    TaskEnqueuedEvent.Wait();
                    Action currentAction = null;
                    lock (Tasks)
                    {
                        while (Tasks.Count > 0)
                            currentAction = Tasks.Dequeue();
                        TaskEnqueuedEvent.Reset();
                    }
                    if (currentAction != null)
                    {
                        currentAction.Invoke();
                    }
                }
            }
            catch(ThreadAbortException) { }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        ~Workhorse()
        {
            AbortAndClear();
        }
    }
}
