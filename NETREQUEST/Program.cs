using AngleSharp.Parser.Html;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace NETREQUEST
{
    class Program
    {
        
        private static readonly List<Thread> _threadList = new List<Thread>();//Для остановки потоков
        private static readonly object _locker = new object(); 
        private static int _scoreProxy;
        private const int CountThreads = 20;

        static void Main(string[] args)
        {
            Console.ResetColor();
            Console.Write("Текст: ");
            var text = Console.ReadLine();

            Console.Write("Ник: ");
            var name = Console.ReadLine();

            Console.Write("Кол-во запросов: ");
            var count = Console.ReadLine();


            var cookies = new CookieContainer();
            string response = Request.Get(cookies, $"https://f3.cool/{name}");

            var id = Request.GetUserId(response);

            var threads = new Thread[CountThreads];

            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(delegate () { Spam(cookies, text, id, count); });
                _threadList.Add(threads[i]);
                threads[i].Start();
            }
        }

        private static void Spam(CookieContainer cookies, string text, string id, string count)
        {
            var lines = File.ReadAllLines("goodProxy.txt");

            var countLines = lines.Length - 1;

            for (var i = 0; i < Convert.ToInt32(count); i++)
            {
                var ip = string.Empty;

                lock (_locker)
                {
                    if (_scoreProxy == countLines)
                        _scoreProxy = 0;

                    _scoreProxy++;
                    ip = lines[_scoreProxy];
                }

                var proxy = ip.Split(':');

                var response = Request.Post(cookies, $"https://f3.cool/api/v1/users/{id}/questions", text, proxy[0], Convert.ToInt32(proxy[1]));

                if (response != "Bad")
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Ok");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Bad");
                }

            }

            Console.ResetColor();
            Console.WriteLine($"Программа успешно отправила {Convert.ToInt32(count)} запросов");
            Console.ReadLine();
        }
    }
}
