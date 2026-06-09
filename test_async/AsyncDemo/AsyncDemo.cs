using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncDemo;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== C# Async Programming Demo ===\n");

        // 1. 基本的 async/await
        Console.WriteLine("1. Basic async/await:");
        string result = await SimpleAsyncMethod();
        Console.WriteLine($"   Result: {result}\n");

        // 2. 多个 Task 并行执行
        Console.WriteLine("2. Parallel tasks (Task.WhenAll):");
        var task1 = DelayAndReturnAsync("Task 1", 1500);
        var task2 = DelayAndReturnAsync("Task 2", 1000);
        var task3 = DelayAndReturnAsync("Task 3", 500);
        string[] results = await Task.WhenAll(task1, task2, task3);
        foreach (var r in results)
            Console.WriteLine($"   -> {r}");
        Console.WriteLine();

        // 3. 顺序执行
        Console.WriteLine("3. Sequential tasks (await one by one):");
        await DelayAndPrintAsync("Step A", 500);
        await DelayAndPrintAsync("Step B", 300);
        await DelayAndPrintAsync("Step C", 200);
        Console.WriteLine();

        // 4. 异常处理
        Console.WriteLine("4. Exception handling:");
        try
        {
            await FailingTaskAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   Caught: {ex.GetType().Name}: {ex.Message}\n");
        }

        // 5. CancellationToken
        Console.WriteLine("5. CancellationToken:");
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(500); // 500ms 后取消
        try
        {
            await LongRunningTaskAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("   Task was cancelled as expected.\n");
        }

        // 6. Task.Run 在后台线程执行 CPU 密集型工作
        Console.WriteLine("6. Task.Run (CPU-bound work on thread pool):");
        int cpuResult = await Task.Run(() => ComputeFibonacci(40));
        Console.WriteLine($"   Fibonacci(40) = {cpuResult}\n");

        // 7. ConfigureAwait(false) - 不恢复到原始上下文
        Console.WriteLine("7. ConfigureAwait(false) demo (no UI context needed):");
        string configResult = await ConfigureAwaitExample();
        Console.WriteLine($"   {configResult}\n");

        // 8. ValueTask 适用场景
        Console.WriteLine("8. ValueTask (avoid allocation when result is cached):");
        int cacheResult1 = await GetOrComputeCachedAsync(42);
        int cacheResult2 = await GetOrComputeCachedAsync(42); // 走缓存，同步返回
        Console.WriteLine($"   Cache results: {cacheResult1}, {cacheResult2}\n");

        // 9. Async streams (C# 8.0+)
        Console.WriteLine("9. Async streams (IAsyncEnumerable):");
        await foreach (var number in GenerateNumbersAsync(5))
        {
            Console.WriteLine($"   Received: {number}");
        }
        Console.WriteLine();

        Console.WriteLine("=== Demo Complete ===");
    }

    // 1. 最基本的异步方法
    static async Task<string> SimpleAsyncMethod()
    {
        await Task.Delay(500); // 模拟异步 IO 操作
        return "Hello from async method!";
    }

    // 2. 返回值的异步方法
    static async Task<string> DelayAndReturnAsync(string name, int delayMs)
    {
        await Task.Delay(delayMs);
        return $"{name} completed after {delayMs}ms";
    }

    // 3. 无返回值的异步方法
    static async Task DelayAndPrintAsync(string step, int delayMs)
    {
        await Task.Delay(delayMs);
        Console.WriteLine($"   {step} done ({delayMs}ms)");
    }

    // 4. 会失败的异步方法
    static async Task FailingTaskAsync()
    {
        await Task.Delay(300);
        throw new InvalidOperationException("Something went wrong in async method!");
    }

    // 5. 支持取消的异步方法
    static async Task LongRunningTaskAsync(CancellationToken token)
    {
        for (int i = 0; i < 10; i++)
        {
            token.ThrowIfCancellationRequested();
            Console.WriteLine($"   Working... step {i + 1}");
            await Task.Delay(300, token);
        }
        Console.WriteLine("   Completed without cancellation.");
    }

    // 6. CPU 密集型计算
    static int ComputeFibonacci(int n)
    {
        if (n <= 1) return n;
        return ComputeFibonacci(n - 1) + ComputeFibonacci(n - 2);
    }

    // 7. ConfigureAwait
    static async Task<string> ConfigureAwaitExample()
    {
        await Task.Delay(200).ConfigureAwait(false);
        // 在非 UI 环境下，ConfigureAwait(false) 避免强制回到原始 SynchronizationContext
        return "ConfigureAwait(false) used - no context capture";
    }

    // 8. ValueTask 示例（带缓存以减少分配）
    static readonly Dictionary<int, int> s_cache = new();
    static readonly object s_cacheLock = new();

    static async ValueTask<int> GetOrComputeCachedAsync(int input)
    {
        lock (s_cacheLock)
        {
            if (s_cache.TryGetValue(input, out int cached))
                return cached; // 同步返回，不分配 Task 对象
        }

        // 缓存未命中，异步计算
        int result = await ComputeExpensiveAsync(input);
        lock (s_cacheLock)
        {
            s_cache[input] = result;
        }
        return result;
    }

    static async Task<int> ComputeExpensiveAsync(int input)
    {
        await Task.Delay(200);
        return input * 2;
    }

    // 9. Async streams
    static async IAsyncEnumerable<int> GenerateNumbersAsync(int count)
    {
        for (int i = 1; i <= count; i++)
        {
            await Task.Delay(300);
            yield return i;
        }
    }
}
