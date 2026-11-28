using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Blazor.Session
{
    public sealed class JsRefreshLock : IRefreshLock
    {
        private readonly AuthKitJs _js;
        private readonly SemaphoreSlim _gate = new(1, 1);

        public JsRefreshLock(AuthKitJs js) => _js = js;

        public async Task<T> RunExclusiveAsync<T>(Func<Task<T>> action)
        {
            await _gate.WaitAsync();   // داخل نفس التبويب / الدائرة
            try
            {
                var ran = false;
                T result = default!;

                async Task<bool> Run()
                {
                    ran = true;
                    result = await action();
                    return true;
                }

                try
                {
                    using var reference = DotNetObjectReference.Create(new LockCallback(Run));
                    await _js.WithLockAsync("authkit-refresh", reference, nameof(LockCallback.RunAsync));
                }
                catch (Exception ex) when (!ran && ex is JSException or InvalidOperationException or JSDisconnectedException)
                {
                    // Web Locks أو JS غير متاحين: نكمل بقفل التبويب فقط
                }

                if (!ran) result = await action();
                return result;
            }
            finally { _gate.Release(); }
        }
    }

    public sealed class LockCallback
    {
        private readonly Func<Task<bool>> _run;
        public LockCallback(Func<Task<bool>> run) => _run = run;

        [JSInvokable]
        public Task<bool> RunAsync() => _run();
    }
}
