using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;


namespace LauncherMvvmLight.Domain.Services.TaskService
{
    /// <summary>
    /// The Interface defining methods for Create Employee and Read All Employees  
    /// </summary>
    public interface ITaskSrv
    {
        int RunSerivceTest(string testID);
        void StopSerivceTest(string testID);


    }
    /// <summary>
    /// Class implementing IDataAccessService interface and implementing
    /// its methods by making call to the Entities using CompanyEntities object
    /// </summary>
    public class TaskSrv : ITaskSrv
    {
        // First create a runspace
        // You really only need to do this once. Each pipeline you create can run in this runspace.
       
        public TaskSrv()
        {

        }

        public int RunSerivceTest(string testID)
        {

            switch(testID)
            {
                case "Task1":
                    Console.WriteLine("Test= " + testID);
                    break;
                case "Task2":
                    Console.WriteLine("Test= " + testID);
                    break;

                case "Task3":
                    Console.WriteLine("Test= " + testID);
                    break;

                case "Task4":
                    Console.WriteLine("Test= " + testID);
                    break;

                case "Task5":
                    Console.WriteLine("Test= " + testID);
                    break;
                case "StopT":
                    StopSerivceTest(testID);
                    Console.WriteLine("Stop= " + testID);
                    break;
                default:
                    Console.WriteLine("Test unknown= " + testID);
                    break;

            }

            return 0;
        }

        public void StopSerivceTest(string testID)
        {
            throw new NotImplementedException();
        }

        private void Init()
        {
            // Summary:

            // 1. Two ways of using tasks
            //    Task.Factory.StartNew() creates and starts a Task
            //    new Task(() => { ... }) creates a task; use Start() to fire it
            // 2. Tasks take an optional 'object' argument
            //    Task.Factory.StartNew(x => { foo(x) }, arg);
            // 3. To return values, use Task<T> instead of Task
            //    To get the return value. use t.Result (this waits until task is complete)
            // 4. Use Task.CurrentId to identify individual tasks.
        }

    }    
}

namespace IntroducingTasks
{
    using System;
    using System.Diagnostics.PerformanceData;
    using System.Threading.Tasks;

    class IntroducingTasks
    {
        public static void Write(char c)
        {
            int i = 1000;
            while (i-- > 0)
            {
                Console.Write(c);
            }
        }

        public static void Write(object s)
        {
            int i = 1000;
            while (i-- > 0)
            {
                Console.Write(s.ToString());
            }
        }

        public static void CreateAndStartSimpleTasks()
        {
            // a Task is a unit of work in .NET land

            // here's how you make a simple task that does something
            Task.Factory.StartNew(() =>
            {
                //Console.WriteLine("Hello, Tasks!");
                Write('-');
            });

            // the argument is an action, so it can be a delegate, a lambda or an anonymous method

            Task t = new Task(() => Write('?'));
            t.Start(); // task doesn't start automatically!

            Write('.');
        }
/*
        static void Main(string[] args)
        {
            //CreateAndStartSimpleTasks();
            //TasksWithState();
            TasksWithReturnValues();

            Console.WriteLine("Main program done, press any key.");
            Console.ReadKey();
        }
*/
        public static int TextLength(object o)
        {
            Console.WriteLine($"\nTask with id {Task.CurrentId} processing object '{o}'...");
            return o.ToString().Length;
        }

        private static void TasksWithReturnValues()
        {
            string text1 = "testing", text2 = "this";
            var task1 = new Task<int>(TextLength, text1);
            task1.Start();
            var task2 = Task.Factory.StartNew(TextLength, text2);
            // getting the result is a blocking operation!
            Console.WriteLine($"Length of '{text1}' is {task1.Result}.");
            Console.WriteLine($"Length of '{text2}' is {task2.Result}.");
        }

        private static void TasksWithState()
        {
            // clumsy 'object' approach
            Task t = new Task(Write, "foo");
            t.Start();
            Task.Factory.StartNew(Write, "bar");
        }

       
    }

    class CancelingTasks
    {
        /*
        static void Main(string[] args)
        {
            CancelableTasks();
            MonitoringCancelation();
            CompositeCancelationToken();

            Console.WriteLine("Main program done, press any key.");
            Console.ReadKey();
        }
        */
        private static void WaitingForTimeToPass()
        {
            // we've already seen the classic Thread.Sleep

            var cts = new CancellationTokenSource();
            var token = cts.Token;
            var t = new Task(() =>
            {
                Console.WriteLine("You have 5 seconds to disarm this bomb by pressing a key");
                bool canceled = token.WaitHandle.WaitOne(5000);
                Console.WriteLine(canceled ? "Bomb disarmed." : "BOOM!!!!");
            }, token);
            t.Start();

            // unlike sleep and waitone
            // thread does not give up its turn
            Thread.SpinWait(10000);
            Console.WriteLine("Are you still here?");

            Console.ReadKey();
            cts.Cancel();
        }

        private static void CompositeCancelationToken()
        {
            // it's possible to create a 'composite' cancelation source that involves several tokens
            var planned = new CancellationTokenSource();
            var preventative = new CancellationTokenSource();
            var emergency = new CancellationTokenSource();

            // make a token source that is linked on their tokens
            var paranoid = CancellationTokenSource.CreateLinkedTokenSource(
              planned.Token, preventative.Token, emergency.Token);

            Task.Factory.StartNew(() =>
            {
                int i = 0;
                while (true)
                {
                    paranoid.Token.ThrowIfCancellationRequested();
                    Console.Write($"{i++}\t");
                    Thread.Sleep(100);
                }
            }, paranoid.Token);

            paranoid.Token.Register(() => Console.WriteLine("Cancelation requested"));

            Console.ReadKey();

            // use any of the aforementioned token soures
            emergency.Cancel();
        }

        private static void MonitoringCancelation()
        {
            var cts = new CancellationTokenSource();
            var token = cts.Token;

            // register a delegate to fire
            token.Register(() =>
            {
                Console.WriteLine("Cancelation has been requested.");
            });

            Task t = new Task(() =>
            {
                int i = 0;
                while (true)
                {
                    if (token.IsCancellationRequested) // 1. Soft exit
                                                       // RanToCompletion
                    {
                        break;
                    }
                    else
                    {
                        Console.Write($"{i++}\t");
                        Thread.Sleep(100);
                    }
                }
            });
            t.Start();

            // canceling multiple tasks
            Task t2 = Task.Factory.StartNew(() =>
            {
                char c = 'a';
                while (true)
                {
                    // alternative to what's below
                    token.ThrowIfCancellationRequested(); // 2. Hard exit, Canceled

                    if (token.IsCancellationRequested) // same as above, start HERE
                    {
                        // release resources, if any
                        throw new OperationCanceledException("No longer interested in printing letters.");
                    }
                    else
                    {
                        Console.Write($"{c++}\t");
                        Thread.Sleep(200);
                    }
                }
            }, token); // don't do token, show R# magic

            // cancellation on a wait handle
            Task.Factory.StartNew(() =>
            {
                token.WaitHandle.WaitOne();
                Console.WriteLine("Wait handle released, thus cancelation was requested");
            });

            Console.ReadKey();

            cts.Cancel();

            Thread.Sleep(1000); // cancelation is non-instant

            Console.WriteLine($"Task has been canceled. The status of the canceled task 't' is {t.Status}.");
            Console.WriteLine($"Task has been canceled. The status of the canceled task 't2' is {t2.Status}.");
            Console.WriteLine($"t.IsCanceled = {t.IsCanceled}, t2.IsCanceled = {t2.IsCanceled}");
        }

        private static void CancelableTasks()
        {
            var cts = new CancellationTokenSource();
            var token = cts.Token;
            Task t = new Task(() =>
            {
                int i = 0;
                while (true)
                {
                    if (token.IsCancellationRequested) // task cancelation is cooperative, no-one kills your thread
                        break;
                    else
                        Console.WriteLine($"{i++}\t");
                }
            });
            t.Start();

            // don't forget CancellationToken.None

            Console.ReadKey();
            cts.Cancel();
            Console.WriteLine("Task has been canceled.");
        }
    }
    class WaitingForTimeToPass
    {
        /*
        static void Main(string[] args)
        {
            // we've already seen the classic Thread.Sleep

            var cts = new CancellationTokenSource();
            var token = cts.Token;
            var t = new Task(() =>
            {
                Console.WriteLine("You have 5 seconds to disarm this bomb by pressing a key");
                bool canceled = token.WaitHandle.WaitOne(5000);
                Console.WriteLine(canceled ? "Bomb disarmed." : "BOOM!!!!");
            }, token);
            t.Start();

            // unlike sleep and waitone
            // thread does not give up its turn
            // avoiding a context switch
            Thread.SpinWait(10000);
            SpinWait.SpinUntil(() => false);
            Console.WriteLine("Are you still here?");

            Console.ReadKey();
            cts.Cancel();

            Console.WriteLine("Main program done, press any key.");
            Console.ReadKey();
        }
        */
    }
    class WaitingForTasks
    {
        /*
        static void Main(string[] args)
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(3));
            var token = cts.Token;

            var t = new Task(() =>
            {
                Console.WriteLine("I take 5 seconds");
                //Thread.Sleep(5000);

                for (int i = 0; i < 5; ++i)
                {
                    token.ThrowIfCancellationRequested();
                    Thread.Sleep(1000);
                }

                Console.WriteLine("I'm done.");
            });
            t.Start();

            var t2 = Task.Factory.StartNew(() => Thread.Sleep(3000), token);

            //t.Wait();
            //t.Wait(3000);

            // now introduce t2

            //Task.WaitAll(t, t2);
            //Task.WaitAny(t, t2);

            // start w/o token
            try
            {
                // throws on a canceled token
                Task.WaitAll(new[] { t, t2 }, 4000, token);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e);
            }

            Console.WriteLine($"Task t  status is {t.Status}.");
            Console.WriteLine($"Task t2 status is {t2.Status}.");

            Console.WriteLine("Main program done, press any key.");
            Console.ReadKey();
        }
        */
    }
    class ExceptionHandling
    {
        public static void BasicHandling()
        {
            var t = Task.Factory.StartNew(() =>
            {
                throw new InvalidOperationException("Can't do this!") { Source = "t" };
            });

            var t2 = Task.Factory.StartNew(() =>
            {
                var e = new AccessViolationException("Can't access this!");
                e.Source = "t2";
                throw e;
            });

            try
            {
                Task.WaitAll(t, t2);
            }
            catch (AggregateException ae)
            {
                foreach (Exception e in ae.InnerExceptions)
                {
                    Console.WriteLine($"Exception {e.GetType()} from {e.Source}.");
                }
            }
        }

        static void Mainx()
        {
            BasicHandling();

            //      try
            //      {
            //        IterativeHandling();
            //      }
            //      catch (AggregateException ae)
            //      {
            //        Console.WriteLine("Some exceptions we didn't expect:");
            //        foreach (var e in ae.InnerExceptions)
            //          Console.WriteLine($" - {e.GetType()}");
            //      }

            // escalation policy (use Insert Signature CA)
            TaskScheduler.UnobservedTaskException += (object sender, UnobservedTaskExceptionEventArgs args) =>
            {
                // this exception got handled
                args.SetObserved();

                var ae = args.Exception as AggregateException;
                ae?.Handle(ex =>
                {
                    Console.WriteLine($"Policy handled {ex.GetType()}.");
                    return true;
                });
            };

            IterativeHandling(); // throws


            Console.WriteLine("Main program done, press any key.");
            Console.ReadKey();
        }

        private static void IterativeHandling()
        {
            var cts = new CancellationTokenSource();
            var token = cts.Token;
            var t = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    token.ThrowIfCancellationRequested();
                    Thread.Sleep(100);
                }
            }, token);

            var t2 = Task.Factory.StartNew(() =>
            {
                throw null;
            });

            cts.Cancel();

            try
            {
                Task.WaitAll(t, t2);
            }
            catch (AggregateException ae)
            {
                // handle exceptions depending on whether they were expected or
                // handles all expected exceptions ('return true'), throws the
                // unhandled ones back as an AggregateException
                ae.Handle(e =>
                {
                    if (e is OperationCanceledException)
                    {
                        Console.WriteLine("Whoops, tasks were canceled.");
                        return true; // exception was handled
                    }
                    else
                    {
                        Console.WriteLine($"Something went wrong: {e}");
                        return false; // exception was NOT handled
                    }
                });
            }
            finally
            {
                // what happened to the tasks?
                Console.WriteLine("\tfaulted\tcompleted\tcancelled");
                Console.WriteLine($"t\t{t.IsFaulted}\t{t.IsCompleted}\t{t.IsCanceled}");
                Console.WriteLine($"t1\t{t2.IsFaulted}\t{t2.IsCompleted}\t{t2.IsCanceled}");
            }
        }
    }
}

