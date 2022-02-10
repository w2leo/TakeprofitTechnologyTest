using System;
using System.Collections.Generic;
using System.IO;
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
                    Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} : read failed");
                }
                Thread.Sleep(1000);
                //Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} : Reconnecting");
            }
        }

        public static int GetNumberFromDictionary()
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

        static void Main(string[] args)
        {
             DateTime startTime = DateTime.Now;
             Console.WriteLine($"Start time = {startTime}");
             List<Thread> threads = new List<Thread>();
             for (int i = 0; i < MAX_THREADS; i++)
             {
                 threads.Add(new Thread(GetData));
                 threads[i].Start();
                 Thread.Sleep(2000);
             }

             for (int i = 0; i < MAX_THREADS; i++)
             {
                 threads[i].Join();
             }

             DateTime endTime = DateTime.Now;
             Console.WriteLine($"End time = {endTime}");
             Console.WriteLine($"Time duration = {endTime - startTime}");

             List<int> sortedData = dataDictionary.Values.ToList<int>();
             sortedData.Sort();

             int result;
             int dataLenght = sortedData.Count;
             int medianIndex = dataLenght / 2;

             if (dataLenght %2 != 0)
             {
                 result = sortedData[medianIndex];
             }
             else
             {
                 int highIndex = medianIndex;
                 int lowIndex = highIndex - 1;
                 result = (sortedData[lowIndex] + sortedData[highIndex]) / 2;
                 Console.WriteLine($"lowIndex = {lowIndex}. Value = {sortedData[lowIndex]}");
                 Console.WriteLine($"highIndex = {highIndex}. Value= {sortedData[highIndex]}");
             }


             File.WriteAllLines("sortedData.txt", sortedData.ConvertAll(x => x.ToString()));
             Console.WriteLine();

             Console.WriteLine("Result = " + result);
             File.WriteAllText("Result.txt", "Result = " + result);
       
            ServerIO server = new ServerIO(ADRESS, PORT);
            Console.WriteLine(server.GetStringData($"Check {res}\n"));

            Console.ReadKey();
        }





    }

}
