using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;

namespace FileManager
{
    class Cursor
    {
        public int position;

        //Класс отвечает за работу курсора
        public Cursor()
        {

        }
        public Cursor(ConsoleKeyInfo key, Tree tree, int number)
        {
            position = Move(key, tree, number);
        }
        //Метод рисует курсор
        public void Draw(Set set, int position)
        {
            //Количество элементов на одной странице
            int countElement = 30;
            double quotient = (position + 0.0) / countElement;
            int k = Convert.ToInt32(Math.Floor(quotient));
            int num = position - countElement * k;

            Console.BackgroundColor = ConsoleColor.Yellow;
            if (set.status == "dir")
                if (set.open == false)
                {
                    Console.SetCursorPosition(set.level, num);
                    Console.WriteLine($"+{set.name}");
                }
                else
                {
                    Console.SetCursorPosition(set.level, num);
                    Console.WriteLine($"-{set.name}");
                }

            else
            {
                Console.SetCursorPosition(set.level, num);
                Console.WriteLine($" {set.name}");
            }
            Console.ResetColor();
        }

        //Метод удаляет курсор
        public void Clear(Set set, int position)
        {
            Console.ResetColor();
            if (set.status == "dir")
                if (set.open == false)
                {
                    Console.SetCursorPosition(set.level, position);
                    Console.WriteLine($"+{set.name}");
                }
                else
                {
                    Console.SetCursorPosition(set.level, position);
                    Console.WriteLine($"-{set.name}");
                }
            else
            {
                Console.SetCursorPosition(set.level, position);
                Console.WriteLine($" {set.name}");
            }
        }

        //Метод отвечает за перемещения курсора вверх и вниз
        //Также он реагирует на нажатие Enter при входе или выходе из папки
        //Возвращает новое положение курсора
        public static int Move(ConsoleKeyInfo key, Tree tree, int position)
        {
            int countElement = 30;
            double quotient = (position + 0.0) / countElement;
            int k = Convert.ToInt32(Math.Floor(quotient));
            int num = position - countElement * k;
 
            Cursor cursor = new Cursor();
            if (key.Key == ConsoleKey.DownArrow & position < tree.condition.Count - 1)
            {
                Area areaInf = new Area(0, 31, 101, 12);
                areaInf.Clear();
                //Проверяем, если положение курсора в конце листа
                //то оно будет кратно количеству элементов на листе countElement
                if ((position + 1.0) % countElement == 0.0)
                {
                    cursor.Clear(tree.condition[position], num);
                    num = 0;
                    position = position + 1;
                    tree.DDraw(tree, position);
                }
                else
                {
                    cursor.Clear(tree.condition[position], num);
                    num = num + 1;
                    position = position + 1;
                }
                
            }
            if (key.Key == ConsoleKey.UpArrow & position != 0)
            {
                Area areaInf = new Area(0, 31, 101, 12);
                areaInf.Clear();
                if ((position) % countElement == 0.0)
                {
                    cursor.Clear(tree.condition[position], num);
                    num = 29;
                    position = position - 1;
                    tree.DDraw(tree, position);
                }
                else
                {
                    cursor.Clear(tree.condition[position], num);
                    num = num - 1;
                    position = position - 1;
                }
            }
            if (key.Key == ConsoleKey.Enter)
            {
                if (tree.condition[position].open == false)
                {
                    tree = Tree.expandTree(tree, position);
                }
                else
                {
                    tree = Tree.compressionTree(tree, position);
                }
                tree.DDraw(tree, position);
            }
            cursor.Draw(tree.condition[position], num);
            //Сохраняем в файл текущее состояние дерева
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream("condition.dat", FileMode.OpenOrCreate))
            {
                formatter.Serialize(stream, tree);
            }
            //Сохраняем в файл текущее положение курсора
            BinaryFormatter form= new BinaryFormatter();
            using (FileStream stream = new FileStream("position.dat", FileMode.OpenOrCreate))
            {
                formatter.Serialize(stream, position);
            }
            return position;
        }
    }
}
