using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server;

// Создаём модель нейрона
class Neuron
{
    // Задаём параметры нейрона
    private double[] weights = new double[] { 476, -29, 61};
    public double[] input;
    public double srSum;
    private double sum;

    // Инициализируем пришедшие данные
    public Neuron(double[] input)
    {
        this.input = input;
    }

    // Высчитываем полученный результат
    public double Calculate()
    {
        double sum = 1;

        for (int i = 0; i < weights.Length; i++)
        {
            sum += input[i+1] * weights[i];
        }

        this.sum = sum;
        return sum;
    }

    // Активация
    public bool Activate(double sum)
    {
        if (sum > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

// Создаём модель кабинета
class Place
{
    // Задаём параметры, необходимые для кабинета
    public double[] data;
    private bool haveFire;
    private double srSum;

    // Обращаемся к классу и высчитываем параметры
    public Place(double[] data)
    {
        // Передаём нейрону пришедшие данный
        Neuron neuron = new Neuron(data);
        
        // Высчитываем сумму
        srSum = neuron.Calculate();
        neuron.srSum = srSum;

        // Получаем занчение функции активации нейрона
        this.haveFire = neuron.Activate(srSum);
    }

    // Инициализируем класс
    public Place() { }

    // Функция получения информации о пожаре в кабинете
    public bool getInfo()
    {
        return haveFire;
    }
}
class Program
{
    static void Main(string[] args)
    {
        // Параметры подключения (IP и порт)
        Console.WriteLine("Port: ");
        int port = int.Parse(Console.ReadLine());
        Console.WriteLine("IP: ");
        string ip = Console.ReadLine();

        IPHostEntry ipHost = Dns.GetHostEntry(ip);
        IPAddress ipAddr = ipHost.AddressList[0];
        IPEndPoint iPEndPoint = new IPEndPoint(ipAddr, port);

        // Создаём сокет
        Socket sListenner = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        // Инициализируем подключённые кабинеты
        Console.WriteLine("Количество кабинетов подключённых к системе");
        int placesCount = int.Parse(Console.ReadLine());
        List<Place> places = new List<Place>();
        for (int i = 0; i < placesCount; i++)
        {
            places.Add(new Place(new double[] {0,0,0,0}));
        }

        List<int> placesNumbers = new List<int>();

        // Слушаем входящие сокеты
        try
        {
            // Связываем сокет с локальной конечной точкой
            sListenner.Bind(iPEndPoint);
            // Прослушиваем входящие сообщения(максимум 10 соединений в очереди)
            sListenner.Listen(10);

            while (true)
            { 
                // Приостонавлиаем программу, ожидая соединение,
                // создавая для него новый сокет
                Socket handler = sListenner.Accept();
                string data = null;

                // Клиент появился

                // Масив для хранения полученной информации
                byte[] bytes = new byte[1024];

                // Записываем полученные байты в массив
                int bytesRec = handler.Receive(bytes);
                // Переводим байты в строку
                data += Encoding.UTF8.GetString(bytes, 0, bytesRec);

                //Обрабатываем пришедшие данные
                string[] DataValues = data.Split(' ');

                int place = int.Parse(DataValues[0]);
                int temp = int.Parse(DataValues[1]);
                int MQ2 = int.Parse(DataValues[2]);
                int flame = int.Parse(DataValues[3]);

                bool haveFire = false;
                if (place != 0)
                {
                    if (placesNumbers.Contains(place) == false)
                    {
                        placesNumbers.Add(place);
                    }
                    
                    // Передаём данные соответствующему классу кабинета
                    double[] dt = new double[] { place, temp, MQ2, flame };
                    int indPl = placesNumbers.IndexOf(place);
                    places[indPl] = new Place(dt);
                    // Получаем информацию о пожаре
                    haveFire = places[placesNumbers.IndexOf(place)].getInfo();

                    // В зависимости от полученной информации выводим сообщение
                    if (haveFire == false)
                    {
                        Console.Clear();
                        Console.WriteLine("Пожар отсутсвует");
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("Пожар в кабинете {0}", place.ToString());
                        // Отправка информации о пожаре
                        handler.Send(Encoding.UTF8.GetBytes("true"));
                    }
                }

                

                // Останавливаем отправку и получение данных
                handler.Shutdown(SocketShutdown.Both);
                // Закрываем сокет
                handler.Close();

            }
        }
        catch (Exception ex)
        {
            // При возникновении ошибок выводим их
            Console.WriteLine("Error\n");
            Console.WriteLine(ex.ToString()); 
        }
    }
}
