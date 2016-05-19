//
// MainLoop.cs
//
// Authors:
//   Alan McGovern alan.mcgovern@gmail.com
//
// Copyright (C) 2008 Alan McGovern
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using System;
using System.Collections.Generic;
using System.Threading;

namespace MonoTorrent.Client {
    
    public delegate void MainLoopResult(object result);
    public delegate object MainLoopJob();
    public delegate void MainLoopTask();

    public sealed class MainLoop {
        
        private class SchedulerTask {
            public Exception StoredException;
            public MainLoopTask Task;

            public void Execute() {
                try {
                    Task();
                } catch (Exception ex) {
                    StoredException = ex;
                    throw;
                }
            }
        }

        AutoResetEvent handle = new AutoResetEvent(false);
        Queue<SchedulerTask> tasks = new Queue<SchedulerTask>();
        internal Thread thread;

        public MainLoop(string name) {
            thread = new Thread(Loop);
            thread.Name = name;
            thread.IsBackground = true;
            thread.Start();
        }

        void Loop() {
            while (true) {
                SchedulerTask task = null;
                lock (tasks) {
                    if (tasks.Count > 0)
                        task = tasks.Dequeue();
                }

                if (task == null) {
                    handle.WaitOne();
                } else {
                    task.Execute();
                }
                Thread.Sleep(10);
            }
        }

        /// <summary> Queues an action that is asynchronously executed. </summary>
        public void Queue(MainLoopTask action) {
            SchedulerTask task = new SchedulerTask();
            task.Task = action;
            
            lock (tasks) {
                tasks.Enqueue(task);
                handle.Set();
            }
        }
    }
}