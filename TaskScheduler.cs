using System;
using System.Collections.Generic;
using System.Threading;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight
{
    public class TaskScheduler : ModSystem
    {
        public static TaskScheduler Instance;

        private List<Task> tasks;

        public override void Load()
        {
            Instance = this;
            tasks = new List<Task>();
        }

        public void AddDelayedTask(Action action, int delay)
        {

            tasks.Add(new DelayedTask(action, delay));
        }

        public void AddIntervalTask(Action action, int totalTime, int executions)
        {
            tasks.Add(new IntervalTask(action, totalTime, executions));
        }

        public void AddContinuousTask(Action action, int duration)
        {
            tasks.Add(new ContinuousTask(action, duration));
        }

        public override void PostUpdateEverything()
        {
            for (int i = 0; i < tasks.Count; i++)
            {
                Task task = tasks[i];

                if (task is DelayedTask delayedTask)
                {
                    if (delayedTask.delay <= 0)
                    {
                        delayedTask.action.Invoke();
                        tasks.RemoveAt(i);
                        i--;
                    }
                    delayedTask.delay--;
                }

                if (task is IntervalTask intervalTask)
                {
                    intervalTask.timeLeft--;
                    if (intervalTask.executionIndex < intervalTask.executionTimes.Length &&
     intervalTask.timeLeft == intervalTask.executionTimes[intervalTask.executionIndex])
                    {
                        intervalTask.action.Invoke();
                        intervalTask.executionIndex++;
                    }

                    if (intervalTask.executionIndex >= intervalTask.executionTimes.Length || intervalTask.timeLeft <= 0)
                    {
                        tasks.RemoveAt(i);
                        i--;
                    }
                }

                if (task is ContinuousTask continuousTask)
                {
                    continuousTask.action.Invoke();
                    continuousTask.duration--;

                    if (continuousTask.duration <= 0)
                    {
                        tasks.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        internal class Task
        {
            internal Action action;

            internal Task(Action action)
            {
                this.action = action;
            }
        }

        internal class DelayedTask : Task
        {
            internal int delay;
            internal DelayedTask(Action action, int delay) : base(action)
            {
                this.delay = delay;
            }
        }

        internal class IntervalTask : Task
        {
            internal int timeLeft;
            internal int[] executionTimes;
            internal int executionIndex;

            internal IntervalTask(Action action, int totalTime, int executions) : base(action)
            {
                this.timeLeft = totalTime;
                executionIndex = 0;
                executionTimes = new int[executions];

                int interval = totalTime / executions;
                for (int i = 0; i < executions; i++)
                {
                    executionTimes[i] = totalTime - interval * (i + 1);
                }
            }
        }

        internal class ContinuousTask : Task
        {
            internal int duration;
            internal ContinuousTask(Action action, int duration) : base(action)
            {
                this.duration = duration;
            }
        }
    }
}
