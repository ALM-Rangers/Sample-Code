//---------------------------------------------------------------------
// <copyright file="DeterministicTaskScheduler.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The DeterministicTaskScheduler type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Test
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Scheduler to use in unit tests to make sure that tasks are executed one at a time in sequence and can test when all tasks have finished executing.
    /// </summary>
    public class DeterministicTaskScheduler : TaskScheduler
    {
        /// <summary>
        /// The logger used to log activity.
        /// </summary>
        private ILogger logger = new Logger();

        /// <summary>
        /// Queue of tasks to process.
        /// </summary>
        private Queue<Task> queue = new Queue<Task>();

        /// <summary>
        /// The worker thread.
        /// </summary>
        private Thread workerThread;

        /// <summary>
        /// Starts the processing of the first task.
        /// </summary>
        public void Start()
        {
            this.workerThread = new Thread(new ThreadStart(this.ProcessQueue));
            this.workerThread.Start();
        }

        /// <summary>
        /// Waits for all the tasks to finish executing.
        /// </summary>
        public void Wait()
        {
            this.workerThread.Join();
        }

        /// <summary>
        /// For debugger support only, generates an enumerable of Task instances currently queued to the scheduler waiting to be executed. 
        /// </summary>
        /// <returns>An enumerable that allows a debugger to traverse the tasks currently queued to this scheduler.</returns>
        protected override IEnumerable<Task> GetScheduledTasks()
        {
            this.logger.Log(TraceEventType.Verbose, "GetScheduledTasks");
            return new Task[0];
        }

        /// <summary>
        /// Queues a <see cref="Task"/> to the scheduler. 
        /// </summary>
        /// <param name="task">The <see cref="Task"/> to be queued. </param>
        protected override void QueueTask(Task task)
        {
            this.logger.Log(TraceEventType.Verbose, "Queue task");
            lock (this.queue)
            {
                this.queue.Enqueue(task);
            }
        }

        /// <summary>
        /// Determines whether the <see cref="Task"/> can be executed synchronously in this call, and if it can, executes it.
        /// </summary>
        /// <param name="task">The <see cref="Task"/> to be executed. </param>
        /// <param name="taskWasPreviouslyQueued">A Boolean denoting whether or not task has previously been queued. If this parameter is True, then the task
        /// may have been previously queued (scheduled); if False, then the task is known not to have been queued, and this call is being made in order to execute the task inline without queuing it.
        /// </param>
        /// <returns>A Boolean value indicating whether the task was executed inline.</returns>
        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            this.logger.Log(TraceEventType.Verbose, "TryExecuteTaskInline");
            return this.RunTheTask(task);
        }

        /// <summary>
        /// Worker method to process the tasks in the queue.
        /// </summary>
        private void ProcessQueue()
        {
            try
            {
                Task next = null;
                do
                {
                    lock (this.queue)
                    {
                        if (this.queue.Count > 0)
                        {
                            next = this.queue.Dequeue();
                        }
                        else
                        {
                            next = null;
                        }
                    }

                    if (next != null)
                    {
                        this.RunTheTask(next);
                    }
                }
                while (next != null);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Deterministic scheduler thread ended with exception: {0}", ex.Message);
            }
        }

        /// <summary>
        /// Actually runs a task.
        /// </summary>
        /// <param name="task">The task to run.</param>
        /// <returns>The result of the call to execute the task.</returns>
        private bool RunTheTask(Task task)
        {
            bool ans = false;
            try
            {
                this.logger.Log(TraceEventType.Verbose, "Starting execution of task");
                ans = this.TryExecuteTask(task);
            }
            finally
            {
                this.logger.Log(TraceEventType.Verbose, "Task execution complete");
            }

            return ans;
        }
    }
}
