using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
//using System.Runtime.Serialization.Formatters.Binary;


namespace FileManager
{
    [Serializable]
    class Tree
    {
        public List<Set> condition;

        public Tree()
        {
            condition = GetTree();
        }

        //Метод получает начальное дерево из дисков, папок и файлов
        public static List<Set> GetTree()
        {
            List<Set> currentTree = new List<Set>();
            Console.SetCursorPosition(0, 0);
            //Сначала получаем диски
            string[] allDrives = Directory.GetLogicalDrives();
            if (allDrives.Length != 0)
            {
                //Для каждого диска получаем список папок и файлов
                for (int i = 0; i < allDrives.Length; i++)
                {
                    string currentDir = allDrives[i];
                    string[] listDir = Directory.GetDirectories(currentDir);
                    string[] files = Directory.GetFiles(currentDir);
                    //Добавляем к дереву диски
                    currentTree.Add(new Set(currentDir, 0, "drive", true, allDrives[i]));
                    //Добавляем к дереву папки
                    for (int j = 0; j < listDir.Length; j++)
                    {
                        DirectoryInfo dir = new DirectoryInfo(listDir[j]);
                        currentTree.Add(new Set(dir.Name, 3, "dir", false, listDir[j]));
                    }
                    //Добавляем файлы
                    for (int j = 0; j < files.Length; j++)
                    {
                        FileInfo file = new FileInfo(files[j]);
                        currentTree.Add(new Set(file.Name, 3, "file", false, files[j]));
                    }
                }
            }
         return currentTree;
        }

        //Метод делает вставку в существующее дерево в точке postition
        //В список добавляются папки и файлы имеющиеся в дирректории
        public static Tree expandTree(Tree tree, int position)
        {
            int count = tree.condition.Count;
            string path = tree.condition[position].path;
            int level = tree.condition[position].level;
            string[] files = null;
            string[] listDir = null;
            if (Directory.Exists(path))
            {
                try
                {
                    files = Directory.GetFiles(path);
                    tree.condition[position].open = true;
                    for (int j = 0; j < files.Length; j++)
                    {
                        FileInfo file = new FileInfo(files[j]);
                        tree.condition.Insert(position + j + 1, new Set(file.Name, level + 3, "file", false, files[j]));
                    }
                }
                catch (UnauthorizedAccessException e)
                {
                    Console.SetCursorPosition(0, 45);
                    Console.WriteLine(e.Message);
                    return(tree);
                }
                try
                {
                    listDir = Directory.GetDirectories(path);
                    for (int j = 0; j < listDir.Length; j++)
                    {
                        DirectoryInfo dir = new DirectoryInfo(listDir[j]);
                        tree.condition.Insert(position + j + 1, new Set(dir.Name, level + 3, "dir", false, listDir[j]));
                    }
                }
                catch (UnauthorizedAccessException e)
                {
                    Console.SetCursorPosition(0, 45);
                    Console.WriteLine(e.Message);
                    return (tree);
                }
            }
         return tree;
        }

        //Метод убирает из списка (дерева) элементы внутри папки, если папка закрывается
        public static Tree compressionTree(Tree tree, int position)
        {
            string path = tree.condition[position].path;
            int countDir = 0;
            if (Directory.Exists(path))
            {
                try
                {
                    countDir = Directory.GetDirectories(path).Length;
                    for (int i = position + 1; i <= position + countDir; i++)
                    {
                        if (tree.condition[i].open == true)
                        {
                            compressionTree(tree, i);
                        }
                    }
                    tree.condition[position].open = false;
                    int countFiles = Directory.GetFiles(path).Length;
                    tree.condition.RemoveRange(position + 1, countDir + countFiles);
                }
                catch (UnauthorizedAccessException e)
                {
                    Console.SetCursorPosition(0, 45);
                    Console.WriteLine(e.Message);
                    return (tree);
                }
            }
            return tree;
        }

        //Выводит дерево постранично (количество строк на одной странице = 30)
        public void DDraw(Tree tree, int position)
        {
            //Console.Clear();
            Area areaTree = new Area(0, 0, 101, 30);
            areaTree.Clear();
            int countElement = 30;
            //kmax - коэффициент, определяющий количество страниц
            int kmax = Convert.ToInt32(Math.Ceiling((tree.condition.Count + 0.0)/countElement));
            //k - коэффициент, определяющий номер страницы на которой находится курсор
            int k = Convert.ToInt32(Math.Ceiling((position + 1.0) / countElement));
            //В этом условии происходит вывод элементов для любых страниц, кроме последней
            if (k < kmax)
            {
                for (int i = countElement * (k - 1); i < countElement * k; i++)
                {
                    Console.SetCursorPosition(tree.condition[i].level, i - countElement * (k - 1));
                    if (tree.condition[i].status == "dir")
                        if (tree.condition[i].open == false)
                        {
                            //Console.SetCursorPosition(set.level, position);
                            Console.WriteLine($"+{tree.condition[i].name}");
                        }
                        else
                        {
                            //Console.SetCursorPosition(set.level, position);
                            Console.WriteLine($"-{tree.condition[i].name}");
                        }
                    else
                    {
                        Console.WriteLine($" {tree.condition[i].name}");
                    }
                }
            }
            //А здесь вывод последней страницы или единственной
            if (k == kmax)
            {
                for (int i = countElement * (k - 1); i < tree.condition.Count; i++)
                {
                    Console.SetCursorPosition(tree.condition[i].level, i - countElement * (k - 1));
                    if (tree.condition[i].status == "dir")
                        if (tree.condition[i].open == false)
                        {
                            //Console.SetCursorPosition(set.level, position);
                            Console.WriteLine($"+{tree.condition[i].name}");
                        }
                        else
                        {
                            //Console.SetCursorPosition(set.level, position);
                            Console.WriteLine($"-{tree.condition[i].name}");
                        }
                    else
                    {
                        Console.WriteLine($" {tree.condition[i].name}");
                    }
                }
            }
        }

        //Метод, обновляющий дерево на экране, сохраняя положение курсора на экране
        public void Refresh(Tree tree, int position)
        {
            tree.DDraw(tree, position);
            Cursor cur = new Cursor();
            cur.Draw(tree.condition[position], position);
        }

        //Метод выводит информацию о элементе дерева, 
        //элемент выбирается по его номеру в списке (posititon)
        public void WriteInfo (Tree tree, int position)
        {
            Console.SetCursorPosition(0, 32);
            if (tree.condition[position].status == "file")
                if (File.Exists(tree.condition[position].path))
                {
                    Console.WriteLine($"Имя файла: {tree.condition[position].name}");
                    Console.SetCursorPosition(0, 34);
                    Console.WriteLine($"Путь: {tree.condition[position].path}");
                    Console.SetCursorPosition(0, 36);
                    FileInfo fileInfo = new FileInfo(tree.condition[position].path);
                    Console.WriteLine($"Размер файла: {fileInfo.Length / 1024} Кб");
                    Console.SetCursorPosition(0, 38);
                    Console.WriteLine($"Атрибуты файла: {File.GetAttributes(tree.condition[position].path)}");
                }
            if (tree.condition[position].status == "dir")
                if (Directory.Exists(tree.condition[position].path))
                {
                    Console.WriteLine($"Имя папки: {tree.condition[position].name}");
                    Console.SetCursorPosition(0, 34);
                    Console.WriteLine($"Путь: {tree.condition[position].path}");
                    Console.SetCursorPosition(0, 36);
                    DirectoryInfo dirInfo = new DirectoryInfo(tree.condition[position].path);
                    Console.WriteLine($"Атрибуты папки: {dirInfo.Attributes}");
                }

            if (tree.condition[position].status == "drive")
            {
                DriveInfo driveInfo = new DriveInfo(tree.condition[position].path);
                Console.WriteLine($"Диск: {driveInfo.Name}");
                Console.SetCursorPosition(0, 34);
                Console.WriteLine($"Файловая система: {driveInfo.DriveFormat}");
                Console.SetCursorPosition(0, 36);
                Console.WriteLine($"Тип диска: {driveInfo.DriveType}");
                Console.SetCursorPosition(0, 38);
                Console.WriteLine($"Полный размер: {driveInfo.TotalSize / (1024 * 1024)} Мб");
                Console.SetCursorPosition(0, 40);
                Console.WriteLine($"Свободное место: {driveInfo.TotalFreeSpace / (1024 * 1024)} Мб");
            }
            Console.SetCursorPosition(0, 45);
        }

        //Метод проходит по дереву и проверяет существуют ли записанные в
        //дерево папки и файлы по их путям. Значение true будет только, если
        //все существуют. Если хоть по одному пути будет значение false, значит на выходе false.
        public static bool Exist(Tree tree)
        {
            bool exist = true;
            for (int i = 0; i < tree.condition.Count; i++)
            {
                if (tree.condition[i].status == "dir")
                {
                    if (Directory.Exists(tree.condition[i].path))
                    { 
                    }
                    else
                    {
                        exist = false;
                        break;
                    }
                }
                if (tree.condition[i].status == "file")
                {
                    if (File.Exists(tree.condition[i].path))
                    {
                    }
                    else
                    {
                        exist = false;
                        break;
                    }
                }
            }
            return exist;
        }
    }
}

