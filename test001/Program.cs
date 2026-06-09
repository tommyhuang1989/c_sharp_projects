// See https://aka.ms/new-console-template for more information
// using System.Reflection.Metadata.Ecma335;

// Console.WriteLine("Hello, World!");
// List<string> list = new List<string>() { "Red", "Yellow", "Blue" };

// var result = list.Where(
//     x => x.Contains("Y")
// ).ToList();

namespace test001
{
    internal class Program
    {
        public delegate void MyDelegate();
    public static MyDelegate md;

    public static void Main(string[] args)
    {
        // md += test1;
        // md += test2;
        // md = test3;
        // md();

        // test_catch();

        // test_delegate();
        test_background_thread(false);
        // test_background_thread(true);
    }

    public static void test_background_thread(Boolean isBackground)
    {
        Thread foregroundThread = new Thread(() =>
        {
            Console.WriteLine("前台线程开始运行");
            Thread.Sleep(5000); // 模拟耗时操作
            Console.WriteLine("前台线程结束运行");//如果设置为后台线程后，这里可能不会执行（如果主线程结束，则不会执行）
        });

        foregroundThread.IsBackground = isBackground;//设置为后台线程
        foregroundThread.Start();
        Console.WriteLine("主线程结束");
        // 即使主线程结束，前台线程仍会继续运行，直到完成
    }

    public static void test_delegate()
    {
        Action action = () => {
            System.Console.WriteLine("test1");
        };

        Action<string, int> action2 = (name, age) => {
            System.Console.WriteLine($"{name} is {age} years old.");
        };

        Func<string> func = () => {
            return "fixed name";
        };

        Func<string, string> func2 = (name) => {
            return $"{name} is kof";
        };

        Predicate<int> predicate = (age) => {
            return age > 18;
        };

        action();
        action2("tommy", 36);
        var name = func();
        var content = func2("tommy");
        System.Console.WriteLine(name);
        System.Console.WriteLine(content);
        System.Console.WriteLine(predicate(19));
    }

    public static string test_catch()
    {
        try {
            System.Console.WriteLine("test1");
            return "return string";
        } catch (Exception e) {
            System.Console.WriteLine(e.Message);
        }
        finally {
            System.Console.WriteLine("finally");
        }
        
        return "return all string";
    }

    public static void test1()
    {
        System.Console.WriteLine("test1");
    }

    public static void test2()
    {
        System.Console.WriteLine("test2");
    }

    public static void test3()
    {
        System.Console.WriteLine("test3");
    }
    }
}