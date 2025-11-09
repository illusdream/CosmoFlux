using System;
using System.Diagnostics;

namespace ilsFramework.Core
{
    public static class TimeConsumUtils
    {
        public static void LogTimeConsum(Action needGetTimeConsum)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            needGetTimeConsum?.Invoke();
            watch.Stop();
            $"消耗时间：{watch.ElapsedMilliseconds}ms=>{watch.ElapsedMilliseconds/1000}s".LogSelf();
        }
    }
}