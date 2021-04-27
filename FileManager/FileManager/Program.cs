using System.Configuration;
using System.Collections.Specialized;
using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

namespace FileManager
{
    class Program
    {
        static void Main(string[] args)
        {
            //Определяем размеры окна и буфера
            Console.SetWindowSize(101, 50);
            Console.SetBufferSize(101, 50);
            //Начальные значения
            int position = 0;
            string originalPath = "";
            string originalName = "";
            string originalStatus = "";
            
            Tree deserilizeTree = new Tree();
            Tree tree = new Tree();
            //Смотрим, если сохраняли предыдущее состояние
            if (File.Exists($"{Directory.GetCurrentDirectory()}\\condition.dat")& File.Exists($"{Directory.GetCurrentDirectory()}\\position.dat"))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (FileStream stream = new FileStream("condition.dat", FileMode.OpenOrCreate))
                {
                    try
                    {
                        deserilizeTree = (Tree)formatter.Deserialize(stream);
                    }
                    catch (Exception e)
                    {
                        tree.DDraw(tree, position);
                        Console.SetCursorPosition(0, 45);
                        Console.WriteLine(e.Message);
                    }
                }
                BinaryFormatter form = new BinaryFormatter();
                using (FileStream stream = new FileStream("position.dat", FileMode.OpenOrCreate))
                {
                    try
                    {
                        position = (int)formatter.Deserialize(stream);
                    }
                    catch (Exception e)
                    {
                        Console.SetCursorPosition(0, 45);
                        Console.WriteLine(e.Message);
                    }
                }
            }
            //Если значения не пустые, то рисуем предыдущее состояние системы
            if (deserilizeTree != null & position !=0)
            {
                //Здесь проверяется были ли изменения по сравнению
                //с последним сохранением. Если их не было, мы выведем
                //на экран последнее состояние дерева. Иначе формируем список заново.
                if (Tree.Exist(deserilizeTree)== true)
                {
                    tree = deserilizeTree;
                }
            }
            tree.DDraw(tree, position);
            Cursor cursor = new Cursor();
            cursor.Draw(tree.condition[position], position);
            tree.WriteInfo(tree, position);
            Console.SetCursorPosition(0, 30);
            Console.WriteLine(new string('-', 101));
            Console.SetCursorPosition(0, 44);
            Console.WriteLine(new string('-', 101));
            Console.SetCursorPosition(0, 45);
            ConsoleKeyInfo key;
            
            while (true)
            {
                while (true)
                {
                    //Получаем событие нажатия клавишы
                    key = Console.ReadKey();
                    //Обращаемся к методу отвечающему за перемещения курсора
                    position = Cursor.Move(key, tree, position);
                    //По данному положению курсора выводим информацию о диске, папке или файле
                    tree.WriteInfo(tree, position);
                    //Условие выхода из цикла и при этом очищаем область командной строки
                    if (key.Key == ConsoleKey.F1)
                    {
                        Area areaComm = new Area(0, 45, 101, 3);
                        areaComm.Clear();
                        Console.SetCursorPosition(0, 45);
                        break;
                    }
                }
                //Далее обрабатываем команды вводимые пользователем
                //Если команда есть, то запускаем соответствующий метод
                string word = Console.ReadLine();
                Command command = new Command();
                if (word == "copy")
                {
                    (originalPath, originalName, originalStatus) = command.Copy(tree, position);
                }
                if (word == "paste" & originalPath != "")
                {
                    command.Paste(tree, position, originalPath, originalName, originalStatus);
                }
                if (word == "delete")
                {
                    command.Delete(tree, position);
                }
                if (word == "mkdir")
                {
                    Console.WriteLine("Введите имя создаваемой папки");
                    string name = Console.ReadLine();
                    command.MakeDir(tree, position, name);
                }
                if (word == "mkfile")
                {
                    Console.WriteLine("Введите имя создаваемого файла");
                    string name = Console.ReadLine();
                    command.MakeFile(tree, position, name);
                }
                Console.SetCursorPosition(0, 45);
            }
        }
    }
}
