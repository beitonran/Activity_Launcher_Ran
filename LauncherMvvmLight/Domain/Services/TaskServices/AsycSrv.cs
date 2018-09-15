using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Net;

namespace LauncherMvvmLight.Domain.Services.TaskService
{
    /// <summary>
    /// The Interface defining methods for Create Employee and Read All Employees  
    /// </summary>
    public interface IAsyncSrv
    {
        int RunSerivceTest(string testID);
        void StopSerivceTest(string testID);


    }
    /// <summary>
    /// Class implementing IDataAccessService interface and implementing
    /// its methods by making call to the Entities using CompanyEntities object
    /// </summary>
    public class AsyncSrv : IAsyncSrv
    {
        // First create a runspace
        // You really only need to do this once. Each pipeline you create can run in this runspace.

        public AsyncSrv()
        {

        }

        public int RunSerivceTest(string testID)
        {

            switch (testID)
            {
                case "ASync1":
                    Console.WriteLine("Async Test= " + testID);
                    break;
                case "ASync2":
                    Console.WriteLine("Async Test= " + testID);
                    break;

                case "ASync3":
                    Console.WriteLine("Async Test= " + testID);
                    break;

                case "ASync4":
                    Console.WriteLine("Async Test= " + testID);
                    break;

                case "ASync5":
                    Console.WriteLine("Async Test= " + testID);
                    break;
                case "AStopT":
                    StopSerivceTest(testID);
                    Console.WriteLine("Async Stop= " + testID);
                    break;
                default:
                    Console.WriteLine("Async Test unknown= " + testID);
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

        //Task.Run
        public async Task DoStuff()
        {
            await Task.Run(() =>
            {
                LongRunningOperation();
            });
        }

        private static async Task<string> LongRunningOperation()
        {
            int counter;

            for (counter = 0; counter < 50000; counter++)
            {
                Console.WriteLine(counter);
            }

            return "Counter = " + counter;
        }
    }

}


