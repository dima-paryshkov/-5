using System;
using System.Linq;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;

namespace Лабораторная_работа__5_Компьютерный_сети
{
    class Program
    {
        public static void Main(string[] args)
        {
            const int timeout = 1000;

            // размер пакета
            const int packetSize = 32;

            // для точного измерения времени
            var stopWatch = new Stopwatch();

            // максимальное количесвтво прыжков
            int maxJumps = 30;
            
            // Преобразует строковое представление числа в эквивалентное ему 32-битовое целое число со знаком.
            if (args.Any()) int.TryParse(args[0], out maxJumps);

            // буфер для пакета
            var buffer = new byte[packetSize];

            // точки времени
            var timelist = new long[3];

            //вспомогательное значение
            int i = 1;

            //Ping и PingOptions классы и используются PingReply для отправки запроса проверки связи ICMP на узел

            //Позволяет приложению определить, доступен ли удаленный компьютер по сети.
            var ping = new Ping();

            //Используется для управления передачей пакетов данных Ping.
            var pingOptions = new PingOptions { DontFragment = true, Ttl = 1 };

            Console.Write("Введите адрес для трассировки: ");

            //ссылка
            var hostName = Console.ReadLine();

            Console.Write("Максимальное число прыжков {0} \n", maxJumps);

            try
            {
                //Предпринимает попытку отправки сообщения запроса проверки связи ICMP с заданным буфером 
                //данных на указанный компьютер и получения от него соответствующего сообщения ответа 
                //проверки связи ICMP.Эта перегруженная версия метода позволяет указать тайм - аут операции 
                var pingReply = ping.Send(hostName, timeout, buffer, pingOptions);
                do
                {
                for (int j = 0; j < 3; j++)
                    {
                        stopWatch.Reset();
                        stopWatch.Start();
                        pingReply = ping.Send(hostName, timeout, buffer, pingOptions);
                        stopWatch.Stop();
                        timelist[j] = stopWatch.ElapsedMilliseconds;
                    }
                    // проверяем корректность полученных результатов
                    if (pingReply == null || pingReply.Status == IPStatus.TimedOut)
                        Console.Write("{0,4}\t*\t*\t*\tПревышен интервал ожидания для запроса. \n", i);
                    else
                    {
                        try
                        {
                            //Экземпляр IPHostEntry, содержащий сведения об адресе, 
                            //относящиеся к узлу, указанному в параметре address
                            var entry = Dns.GetHostEntry(pingReply.Address); //Предоставляет класс контейнеров для сведений об адресе веб-узла.
                            Console.Write("{0,4}\t{1} мс\t{2} мс\t{3} мс\t{4} [{5}] \n", i, timelist[0],
                            timelist[1], timelist[2], entry.HostName, pingReply.Address);
                        }
                        catch
                        {
                            Console.Write("{0,4}\t{1} мс\t{2} мс\t{3} мс\t{4}\n", i, timelist[0],
                            timelist[1],
                            timelist[2], pingReply.Address);
                        }
                    }
                    pingOptions.Ttl = ++i;
                    //делать пока (если нет ответа или превышено максимальное количесвто пржыков или ответ не получен)
                } while (pingReply != null && (i < maxJumps && pingReply.Status != IPStatus.Success));
                Console.WriteLine("Трассировка завершена.");
                Console.ReadKey(true);
            }
            catch
            {
                Console.WriteLine("Этот хост неизвестен");
                Console.ReadKey(true);
            }
        }
    }
}