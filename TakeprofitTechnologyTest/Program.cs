using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace TakeprofitTechnologyTest
{
    class Program
    {
        const int MAX_THREADS = 50;
        const string ADRESS = "88.212.241.115";
        const int PORT = 2013;
        const int MAX_NUMBER_VALUE = 2018;
        const int THREAD_DELAY = 1000;
        static Dictionary<int, int> dataDictionary = new Dictionary<int, int>();
        static object locker = new object();

        static void Main(string[] args)
        {
            List<Thread> activeThreads = new List<Thread>();
            CreateNewThreads(MAX_THREADS, activeThreads, GetData);
            StartThreadsWork(activeThreads);
            JoinToAwaitThreads(activeThreads);

            CheckAnswer();
            Console.ReadKey();
        }

        static string FormatMessage(int value)
        {
            return $"{value}\n";
        }

        static void GetData()
        {
            int sendData = 0;
            int receivedNumber;
            bool successfullRead = true;
            while (sendData >= 0)
            {
                try
                {
                    ServerIO server = new ServerIO(ADRESS, PORT);
                    if (successfullRead)
                    {
                        lock (locker)
                        {
                            sendData = GetNumberFromDictionary();
                        }
                        successfullRead = false;
                    }
                    if (sendData >= 0)
                    {
                        receivedNumber = server.GetIntData(FormatMessage(sendData));

                        if (receivedNumber > 0)
                        {
                            lock (locker)
                            {
                                SetDataToDictionary(sendData, receivedNumber);
                            }
                            successfullRead = true;
                        }
                    }
                }
                catch
                {
                    Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} : read failed. Reconnecting.");
                }
                Thread.Sleep(THREAD_DELAY);
            }
        }

        static int GetNumberFromDictionary()
        {
            int key = dataDictionary.Count + 1;
            if (key <= MAX_NUMBER_VALUE)
            {
                dataDictionary.Add(key, 0);
                return key;
            }
            else return -1;
        }

        static void SetDataToDictionary(int key, int value)
        {
            dataDictionary[key] = value;
        }

        static void CreateNewThreads(int threadCount, List<Thread> threads, ThreadStart threadMethod)
        {
            for (int i = 0; i < threadCount; i++)
            {
                threads.Add(new Thread(GetData));
            }
        }

        static void StartThreadsWork(List<Thread> threads)
        {
            foreach (var thread in threads)
            {
                thread.Start();
                Thread.Sleep(THREAD_DELAY);
            }
        }

        static void JoinToAwaitThreads(List<Thread> threads)
        {
            foreach (var thread in threads)
            {
                thread.Join();
            }
        }

        static double CountMedian(List<int> inputData)
        {
            inputData.Sort();
            double result;
            int dataLenght = inputData.Count;
            int medianIndex = dataLenght / 2;
            if (dataLenght % 2 != 0)
            {
                result = inputData[medianIndex];
            }
            else
            {
                int highIndex = medianIndex;
                int lowIndex = highIndex - 1;
                result = (double)((inputData[lowIndex] + inputData[highIndex])) / 2;
            }
            return result;
        }

        static void CheckAnswer()
        {
            double result = CountMedian(dataDictionary.Values.ToList<int>());
            ServerIO server = new ServerIO(ADRESS, PORT);
            server.ReadAndGetStringData($"Check {result}\n", out string dataReceived);
            Console.WriteLine(dataReceived);
        }
    }
}

