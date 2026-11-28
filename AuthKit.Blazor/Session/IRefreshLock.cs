using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Blazor.Session
{
    public interface IRefreshLock
    {
        Task<T> RunExclusiveAsync<T>(Func<Task<T>> action);
    }
}
