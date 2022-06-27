using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;


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
            List<ListNode> arr = new();
            ListNode temp = new();

            temp = Head;

            //переводим ноды в лист
            do
            {
                arr.Add(temp);
                temp = temp.Next;
            } while (temp != null);

            //записываем в файл; а также добавляем данные для хранения индекса узла .Random
            using StreamWriter w = new(s);
            foreach (ListNode n in arr)
                w.WriteLine(n.Data.ToString() + ":" + arr.IndexOf(n.Rand).ToString());
        }

        public void Deserialize(FileStream s, int counter)
        {
            List<ListNode> arr = new();
            ListNode temp = new();
            Count = 0;
            Head = temp;
            string line;
            List<String> show = new();
            //пробуем создать файл и создать лист состоящий из нодов
            try
            {
                using (StreamReader sr = new(s))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (!line.Equals(""))
                        {
                            Count++;
                            temp.Data = line;
                            ListNode next = new();
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
                int i = 0;
                //возвращаем референсы к случайным нодам и восстанавливаем данные
                foreach (ListNode n in arr)
                {
                    if (i < counter)
                    {
                        n.Rand = arr[Convert.ToInt32(n.Data.Split(':')[1])];
                        n.Data = n.Data.Split(':')[0];
                        i++;
                        show.Add(n.Data.Split(':')[0]);
                    }
                }
                MessageBox.Show("Ваши десериализированные данные"+"\n"+String.Join("\n", show));
            }
            catch (Exception e)
            {
                MessageBox.Show("Не удалось обработать файл данных, возможно, он поврежден, подробности:" + "\n" + e.Message + "\n" + "Можете перезагрузить приложение");
            }

        }
    }


    public partial class MainWindow : Window
    {
        static Random rand = new();

        ListNode head = new();
        ListNode tail = new();
        ListNode temp = new();

        FileStream fs = new("saber.dat", FileMode.OpenOrCreate);

        //создаём следующий ноуд
        static ListNode AddNode(ListNode prev,int counting, string ListData)
        {
            ListNode result = new()
            {
                Prev = prev,
                Next = null,
                Data = ListData 
            };
            prev.Next = result;
            return result;
        }

        //создаём отсылку к случайному ноду 
        static ListNode RandomNode(ListNode _head, int _length)
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


            //первый нод
            button1_Copy.Visibility = Visibility.Hidden;

            

            tail = head;

            //try
            //{
            //    var lines = File.ReadLines("saber.dat");
            //    foreach (string line in lines)
            //    {
            //        ListView2.Items.Add(line);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    ListView2.Items.Add("Данные не загружены" + ex);
            //}
            //если данные второго хвоста равняются данным хвоста, то мы думаем что данные загружены верно
            //if (second.Tail.Data == first.Tail.Data) label.Content = "Успешно загружены данные!";

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            ListView.Items.Add(textBox.Text);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            head.Data = ListView.Items.GetItemAt(0).ToString(); // первый ноуд 
            List<string> test = new(); //лист для перевода текста из listview
            for (int i = 0; i < ListView.Items.Count; i++)
            {
                test.Add(item: ListView.Items.GetItemAt(i).ToString());
                tail = AddNode(tail, i, test[i]);
            }
            temp = head;

            //добавить отсылку к случайному ноду
            for (int i = 0; i < ListView.Items.Count; i++)
            {
                temp.Rand = RandomNode(head, ListView.Items.Count);
                temp = temp.Next;
            }

            //Объявить первый лист
            ListRand first = new()
            {
                Head = head,
                Tail = tail,
                Count = ListView.Items.Count
            };
            //Создать либо открыть файл и сериализировать лист

            first.Serialize(fs);
            MessageBox.Show("Данные сериализированны");
            button1.Visibility = Visibility.Hidden;
            button1_Copy.Visibility = Visibility.Visible;

            // fs.Close();
        }

        private void button1_Copy_Click(object sender, RoutedEventArgs e)
        {
            //Десериализировать во втором листе
            ListRand second = new();
            try
            {
                fs = new FileStream("saber.dat", FileMode.Open);
            }
            catch (Exception ee)
            {
                MessageBox.Show("Не удалось обработать файл данных, возможно, он поврежден, подробности:" + "\n" + ee.Message + "\n" + "Можете перезагрузить приложение");
            }
            second.Deserialize(fs, ListView.Items.Count);
            //fs.Close();
        }
    }
}
