using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace LauncherMvvmLight.Domain.Services.SyncService
{
    /// <summary>
    /// The Interface defining methods for Create Employee and Read All Employees  
    /// </summary>
    public interface ISyncSrv
    {
        int RunSerivceTest(string testID);
        void StopSerivceTest(string testID);
    }
    /// <summary>
    /// Class implementing IDataAccessService interface and implementing
    /// its methods by making call to the Entities using CompanyEntities object
    /// </summary>
    public class SyncSrv : ISyncSrv
    {
        // First create a runspace
        // You really only need to do this once. Each pipeline you create can run in this runspace.


        public SyncSrv()
        {

        }

        private void Init()
        {

        }

        public int RunSerivceTest(string testID)
        {
            switch (testID)
            {
                case "Sync1":

                    Console.WriteLine("S-Test= " + testID);
                    break;
                case "Sync2":
                    Console.WriteLine("S-Test= " + testID);
                    break;

                case "Sync3":
                    Console.WriteLine("S-Test= " + testID);
                    break;

                case "Sync4":
                    //MutexExample.LocalMutex();
                    Console.WriteLine("S-Test= " + testID);
                    break;

                case "Sync5":
                    //MutexExample.GlobalMutex();
                    Console.WriteLine("S-Test= " + testID);
                    break;
                case "StopT":
                    StopSerivceTest(testID);
                    Console.WriteLine("Stop= " + testID);
                    break;
                default:
                    Console.WriteLine("S-Test unknown= " + testID);
                    break;

            }

            return 0;
        }

        public void StopSerivceTest(string testID)
        {
            throw new NotImplementedException();
        }
    }



    //****************************************************************************************
    //  MUTEX SAMPLE
    // bool haveLock = mutex.WaitOne();
    //...
    // if (haveLock) mutex.ReleaseMutex();
    //****************************************************************************************
    class MutexExample
    {
        class BankAccount
        {
            public int Balance { get; private set; }

            public BankAccount(int balance)
            {
                Balance = balance;
            }

            public void Deposit(int amount)
            {
                Balance += amount;
            }

            public void Withdraw(int amount)
            {
                Balance -= amount;
            }

            public void Transfer(BankAccount where, int amount)
            {
                where.Balance += amount;
                Balance -= amount;
            }
        }

        public static void LocalMutex()
        {
            var tasks = new List<Task>();
            var ba = new BankAccount(0);
            var ba2 = new BankAccount(0);

            // many synchro types deriving from WaitHandle
            // Mutex = mutual exclusion

            // two types of mutexes
            // this is a _local_ mutex
            Mutex mutex = new Mutex();
            Mutex mutex2 = new Mutex();

            for (int i = 0; i < 10; ++i)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; ++j)
                    {
                        bool haveLock = mutex.WaitOne();
                        try
                        {
                            ba.Deposit(1); // deposit 10000 overall
                        }
                        finally
                        {
                            if (haveLock) mutex.ReleaseMutex();
                        }
                    }
                }));
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; ++j)
                    {
                        bool haveLock = mutex2.WaitOne();
                        try
                        {
                            ba2.Deposit(1); // deposit 10000
                        }
                        finally
                        {
                            if (haveLock) mutex2.ReleaseMutex();
                        }
                    }
                }));

                // transfer needs to lock both accounts
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        bool haveLock = Mutex.WaitAll(new[] { mutex, mutex2 });
                        try
                        {
                            ba.Transfer(ba2, 1); // transfer 10k from ba to ba2
                        }
                        finally
                        {
                            if (haveLock)
                            {
                                mutex.ReleaseMutex();
                                mutex2.ReleaseMutex();
                            }
                        }
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());

            Console.WriteLine($"Final balance is: ba={ba.Balance}, ba2={ba2.Balance}.");
        }

        public static void GlobalMutex()
        {
            const string appName = "MyApp";
            Mutex mutex;
            try
            {
                mutex = Mutex.OpenExisting(appName);
                Console.WriteLine($"Sorry, {appName} is already running.");
                return;
            }
            catch (WaitHandleCannotBeOpenedException e)
            {
                Console.WriteLine("We can run the program just fine.");
                // first arg = whether to give current thread initial ownership
                mutex = new Mutex(false, appName);
            }

            Console.ReadKey();
        }
    }

}