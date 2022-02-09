using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace TakeprofitTechnologyTest
{
    class Program
    {
        const int MAX_THREADS = 1000;
        const string ADRESS = "88.212.241.115";
        const int PORT = 2013;
        const int MAX_NUMBER_VALUE = 2018;
        static Dictionary<int, int> dataDictionary = new Dictionary<int, int>();
        static object locker = new object();

        static string FormatMessage(int value)
        {
            return $"{value}\n";
        }

        public static void GetData()
        {
            
            int sendData = 0;
            int receivedNumber;
            bool successfullRead = new bool();
            successfullRead = true;
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
                        receivedNumber = server.GetData(FormatMessage(sendData));

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
                    //Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} : failed read");
                }
            }
        }

        public static int GetNumberFromDictionary()
        {
            int key = dataDictionary.Count+1;
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

        static void Main(string[] args)
        {
            List<Thread> threads = new List<Thread>();
            for (int i = 0; i < MAX_THREADS; i++)
            {
                threads.Add(new Thread(GetData));
                threads[i].Start();
            }

            for (int i = 0; i < MAX_THREADS; i++)
            {
                threads[i].Join();
            }

            List<int> sortedData = dataDictionary.Values.ToList<int>();
            sortedData.Sort();

            double result;
            double medianIndex = (dataDictionary.Count + 1) / 2;
            if (medianIndex == (int)medianIndex)
            {
                result = dataDictionary[(int)medianIndex];
            }
            else
            {
                int lowIndex = (int)medianIndex;
                int highIndex = lowIndex + 1;
                result = (dataDictionary[lowIndex] + dataDictionary[highIndex]) / 2;
            }

            Console.WriteLine();
            Console.WriteLine("Result = " + result);
            Console.ReadKey();
        }





    }

}
