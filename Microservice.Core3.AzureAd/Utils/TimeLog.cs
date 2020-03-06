using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

// ReSharper disable UnusedMember.Global
namespace Microservice.Core3.AzureAd.Utils
{
    /// <summary>
    /// Functions to write the times in the console log, to debug
    /// </summary>
    public static class TimeLog
    {
        private static readonly object TimesLock = new object();
        private static List<Time> _times = new List<Time>();

        /// <summary>
        /// Add a record to the log and start it
        /// </summary>
        /// <param name="name"></param>
        /// <param name="level"></param>
        public static Guid Start(string name, int level)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            Time time = new Time(name, level, watch);

            lock (TimesLock)
                _times.Add(time);

            return time.Guid;
        }

        /// <summary>
        /// Stop the time of an element of the log
        /// </summary>
        /// <param name="guid"></param>
        public static void Stop(Guid guid)
        {
            lock (TimesLock)
                _times.FirstOrDefault(t => t.Guid == guid)?.Watch.Stop();
        }

        /// <summary>
        /// Add the time of a call to the log while still making it asynchronous
        /// </summary>
        /// <param name="task"></param>
        /// <param name="name"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static async Task Call(Task task, string name, int level)
        {
            Guid guid = Start($"Call {name}", level);
            await task;
            Stop(guid);
        }

        /// <summary>
        /// Add the time of a call to the log while still making it asynchronous
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <param name="name"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static async Task<T> Call<T>(Task<T> task, string name, int level)
        {
            Guid guid = Start($"Call {name}", level);
            T result = await task;
            Stop(guid);

            return result;
        }

        /// <summary>
        /// Write on the console all the times
        /// </summary>
        public static void ToConsole()
        {
            Console.WriteLine(Environment.NewLine + new string('-', 104) + Environment.NewLine);

            lock (TimesLock)
            {
                _times = _times.OrderBy(t => t.Level).ThenBy(t => t.Start).ToList();
                foreach (Time time in _times)
                {
                    string result = $"{time.Start:HH:mm:ss:fffff} -> {time.Name} -> {time.Watch.Elapsed:mm\\:ss\\:fff} seg";
                    result = new string(' ', time.Level * 2) + result;
                    Console.WriteLine(result);
                }

                _times.Clear();
            }

            Console.WriteLine(Environment.NewLine + new string('-', 104) + Environment.NewLine);
        }

        private class Time
        {
            public Guid Guid { get; }
            public DateTimeOffset Start { get; }
            public string Name { get; }
            public int Level { get; }
            public Stopwatch Watch { get; }

            public Time(string name, int level, Stopwatch watch)
            {
                Guid = Guid.NewGuid();
                Start = DateTimeOffset.Now;
                Name = name;
                Level = level;
                Watch = watch;
            }
        }

    }
}
