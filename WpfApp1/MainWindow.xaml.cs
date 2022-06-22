using System.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    class ListNode
    {
        public ListNode Prev;
        public ListNode Next;
        public ListNode Rand; // произвольный элемент внутри списка
        public string Data;
    }

    class ListRand
    {
        public ListNode Head;
        public ListNode Tail;
        public int Count;

        public void Serialize(FileStream s)
        {
            List<ListNode> arr = new List<ListNode>();
            ListNode temp = new ListNode();
            temp = Head;

            //переводим ноды в лист
            do
            {
                arr.Add(temp);
                temp = temp.Next;
            } while (temp != null);

            //записываем файл; данные изменяем для хранения индекса узла .Random
            using (StreamWriter w = new StreamWriter(s))
                foreach (ListNode n in arr)
                    w.WriteLine(n.Data.ToString() + ":" + arr.IndexOf(n.Rand).ToString());
        }

        public void Deserialize(FileStream s)
        {
            List<ListNode> arr = new List<ListNode>();
            ListNode temp = new ListNode();
            Count = 0;
            Head = temp;
            string line;

            //пробуем создать файл и создать лист состоящий из нодов
            try
            {
                using (StreamReader sr = new StreamReader(s))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (!line.Equals(""))
                        {
                            Count++;
                            temp.Data = line;
                            ListNode next = new ListNode();
                            temp.Next = next;
                            arr.Add(temp);
                            next.Prev = temp;
                            temp = next;
                        }
                    }
                }

                //объявляем хвост для листа
                Tail = temp.Prev;
                Tail.Next = null;

                //возвращаем референсы к случайным нодам и восстанавливаем данные
                foreach (ListNode n in arr)
                {
                    n.Rand = arr[Convert.ToInt32(n.Data.Split(':')[1])];
                    n.Data = n.Data.Split(':')[0];
                    
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Не удалось обработать файл данных, возможно, он поврежден, подробности:" + "\n" + e.Message + "\n" + "Можете перезагрузить приложение");
            }
        }
    }


    public partial class MainWindow : Window
    {
        static Random rand = new Random();

        //создаём следующий ноуд
        static ListNode addNode(ListNode prev)
        {
            ListNode result = new ();
            result.Prev = prev;
            result.Next = null;
            result.Data = rand.Next(0, 100).ToString();
            prev.Next = result;
            return result;
        }

        //создаём референс к следующему ноду 
        static ListNode randomNode(ListNode _head, int _length)
        {
            int k = rand.Next(0, _length);
            int i = 0;
            ListNode result = _head;
            while (i < k)
            {
                result = result.Next;
                i++;
            }
            return result;
        }
        public MainWindow()
        {
            InitializeComponent();

            //счётчик нодов для тестирования
            int length = 7;

            //первый нод
            ListNode head = new ListNode();
            ListNode tail = new ListNode();
            ListNode temp = new ListNode();

            head.Data = rand.Next(0, 1000).ToString();

            tail = head;

            for (int i = 1; i < length; i++)
                tail = addNode(tail);

            temp = head;

            //добавить отсылку к случайному ноду
            for (int i = 0; i < length; i++)
            {
                temp.Rand = randomNode(head, length);
                temp = temp.Next;
            }

            //Объявить первый лист
            ListRand first = new ListRand();
            first.Head = head;
            first.Tail = tail;
            first.Count = length;

            
            //СОздать\открыть файл и сериализировать лист
            FileStream fs = new FileStream("saber.dat", FileMode.OpenOrCreate);
            first.Serialize(fs);
            

            //Десериализировать во втором листе
            ListRand second = new ListRand();
            try
            {
                fs = new FileStream("saber.dat", FileMode.Open);
            }
            catch (Exception e)
            {
                MessageBox.Show("Не удалось обработать файл данных, возможно, он поврежден, подробности:" + "\n" + e.Message + "\n" + "Можете перезагрузить приложение");
            }
            second.Deserialize(fs);

            //если данные второго хвоста равняются данным хвоста, то мы думаем что данные загружены верно
            if (second.Tail.Data == first.Tail.Data) label.Content = "Успешно загружены данные!";
            Console.Read();

        }
    }
}
