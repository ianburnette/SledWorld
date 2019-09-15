using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Extensions
{
    public static class AwaitExtensions
    {
        public static TaskAwaiter GetAwaiter(this TimeSpan timeSpan) => Task.Delay(timeSpan).GetAwaiter();
    }
}