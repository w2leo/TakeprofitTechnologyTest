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
        const int THREAD_START_DELAY = 2000;
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

        public static void SetDataToDictionary(int key, int value)
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
                Thread.Sleep(THREAD_START_DELAY);
            }

            for (int i = 0; i < MAX_THREADS; i++)
            {
                threads[i].Join();
            }

            DateTime endTime = DateTime.Now;
            Console.WriteLine($"End time = {endTime}");
            Console.WriteLine($"Time duration = {endTime - startTime}");

            double result = CountMedian(dataDictionary.Values.ToList<int>());


            File.WriteAllLines("sortedData.txt", sortedData.ConvertAll(x => x.ToString()));
            Console.WriteLine();

            Console.WriteLine("Result = " + result);
            File.WriteAllText("Result.txt", "Result = " + result);


            //double result = 4925680.5;
            ServerIO server = new ServerIO(ADRESS, PORT);
            //Console.WriteLine(server.GetStringData($"74\n"));
            server.GetStringData($"Check {result}\n", out string dataReceived);

            // server.GetStringData($"Register\n", out string dataReceived);

            // dataReceived = dataReceived.Split('\r')[0];
            //if (dataReceived[dataReceived.Length - 1] != '\n')
            //     dataReceived += "\n";
            //  Thread.Sleep(10000);
            //  Console.WriteLine("Sleep ended");

            //  server.GetStringData($"{dataReceived}|100\n", out string dataReceived2);
            //  int xx = server.GetIntData($"{dataReceived}|100\n");

            Console.WriteLine(dataReceived);
            //  Console.WriteLine(dataReceived2);
            // Console.WriteLine($"Received int : {xx}");
            //File.WriteAllText("NewTask.txt", dataReceived);


            Console.ReadKey();
        }


        public static double CountMedian(List<int> inputData)
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



    }

}
