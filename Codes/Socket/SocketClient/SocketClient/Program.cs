using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System;

namespace SocketClient;

class Program
{

    static void Main(string[] args)
    {
        // Получаем дынные для подключения к серверу по сокету
        Console.WriteLine("Port: ");
        int port = int.Parse(Console.ReadLine());
        Console.WriteLine("IP: ");
        string IP = Console.ReadLine();


        // Получаем данные с Arduino через COM порт
        SerialPort _serialPort = new SerialPort();
        _serialPort.PortName = "COM5"; // Выбираем COM порт
        _serialPort.BaudRate = 9600; // Скорость передачи данных

        _serialPort.Open(); // Открываем порт

        // Получаем данные с порта и записыаем в массив
        while (true)
        {
            int[] dataToServ = new int[4];
            var dataFromArduino = _serialPort.ReadLine();
            int ind = 0;
            // Дожидаемся строки "[StartData]", чтобы начать принимать данные
            if (dataFromArduino.StartsWith("[StartData]"))
            {
                // принимаем данные пока не появится строка "[StopData]"
                while (!(dataFromArduino.StartsWith("[StopData]")) && (ind <= 3))
                {
                    dataFromArduino = _serialPort.ReadLine();

                    dataToServ[ind] = Int32.Parse(dataFromArduino);

                    ind++;
                }
            }

            // Отправляем данные на сервер
            try
            {
                // Пробуем отправить сообщение
                SendMessageFromSocket(port, IP, dataToServ, _serialPort);
            }
            catch (Exception ex)
            {
                // При возникновении ошибки выводим её
                Console.WriteLine(ex.ToString());
            }

            Thread.Sleep(500);
        }

        static void SendMessageFromSocket(int port, string IP, int[] dataToServ, SerialPort _serialPort)
        {
            // Соединяемся с устройством
            IPHostEntry ipHost = Dns.GetHostEntry(IP);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint iPEndPoint = new IPEndPoint(ipAddr, port);

            // Создаём сокет
            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Соединямся с сокетом
            sender.Connect(iPEndPoint);

            string data = "";

            // Отправляем данные
            foreach (var item in dataToServ)
            {
                data += item.ToString() + " ";
            }

            int countData = sender.Send(Encoding.UTF8.GetBytes(data.ToString()));
            Console.WriteLine(data);
            // Получаем информаций с сервера
            string recData = Encoding.UTF8.GetString(new byte[] {}, 0, sender.Receive(new byte[] {}));
            // Обрабатываем пришедшую информацию
            if (recData == "true")
            {
                _serialPort.Write("1");
            }
            

            // Останавливаем отправку и получение данных
            sender.Shutdown(SocketShutdown.Both);
            // Закрываем сокет
            sender.Close();
        }
    } 
}