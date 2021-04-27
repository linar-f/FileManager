using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace FileManager
{
    class Command
    {
        //Класс обрабатывает команды вводимые в консоль пользователем
        public Command()
        {

        }

        //Метод запоминает путь имя и статус копируемого элемента (файла или папки)
        public (string path, string name, string status) Copy(Tree tree, int position)
        {
            string path = tree.condition[position].path;
            string name = tree.condition[position].name;
            string status = tree.condition[position].status;
            return (path, name, status);
        }

        //Метод копирующий папку со всем её вложениями
        public void CopyDir(string oldDir, string newDir)
        {
            DirectoryInfo dir = new DirectoryInfo(oldDir);
            Directory.CreateDirectory($"{newDir}\\{dir.Name}");
            foreach (string originalFile in Directory.GetFiles(oldDir))
            {
                string newFile = $"{newDir}\\{dir.Name}\\{Path.GetFileName(originalFile)}";
                File.Copy(originalFile, newFile);
            }
            foreach (string originalDir in Directory.GetDirectories(oldDir))
            {
                CopyDir(originalDir, $"{newDir}\\{dir.Name}");
            }
        }

        //Метод вставляет в место определенное курсором файл или папку
        public void Paste (Tree tree, int position, string originalPath, string originalName, string originalStatus)
        {
            if (originalStatus == "file")
            {
                if (tree.condition[position].status != "file")
                {
                    int lev = tree.condition[position].level;
                    string pasteDir = tree.condition[position].path;
                    try
                    {
                        File.Copy(originalPath, $"{pasteDir}\\{originalName}");
                        if (tree.condition[position].open == false)
                        {
                            Tree.expandTree(tree, position);
                        }
                        else
                        {
                            tree.condition.Insert(position + 1, new Set(originalName, lev + 3, "file", false, $"{pasteDir}\\{originalName}"));
                        }
                    }
                    catch (Exception e)
                    {
                        Console.SetCursorPosition(0, 45);
                        Console.WriteLine(e.Message);
                    }
                    tree.Refresh(tree, position);
                }
                if (tree.condition[position].status == "file")
                {
                    DirectoryInfo info = new DirectoryInfo(tree.condition[position].path);
                    info = Directory.GetParent(tree.condition[position].path);
                    string pasteDir = info.FullName;
                    File.Copy(originalPath, $"{pasteDir}\\{originalName}");
                    int lev = tree.condition[position].level;
                    tree.condition.Insert(position + 1, new Set(originalName, lev, "file", false, $"{pasteDir}\\{originalName}"));
                    tree.Refresh(tree, position);
                }
            }
                
            if (originalStatus == "dir" & tree.condition[position].status != "file")
            {
                string pasteDir = tree.condition[position].path;
                int lev = tree.condition[position].level;
                
                try
                {
                    CopyDir(originalPath, pasteDir);
                    if (tree.condition[position].open == false)
                    {
                        Tree.expandTree(tree, position);
                    }
                    else
                    {
                        tree.condition.Insert(position + 1, new Set(originalName, lev + 3, "dir", false, $"{pasteDir}\\{originalName}"));
                    }
                }
                catch (Exception e)
                {
                    Console.SetCursorPosition(0, 45);
                    Console.WriteLine(e.Message);
                }
                tree.Refresh(tree, position);
            }
            if (originalStatus == "dir" & tree.condition[position].status == "file")
            {
                DirectoryInfo info = new DirectoryInfo(tree.condition[position].path);
                info = Directory.GetParent(tree.condition[position].path);
                string pasteDir = info.FullName;
                CopyDir(originalPath, pasteDir);
                int lev = tree.condition[position].level;
                tree.condition.Insert(position + 1, new Set(originalName, lev, "dir", false, $"{pasteDir}\\{originalName}"));
                tree.Refresh(tree, position);
            }
        }

        //Метод создает новую папку. Место создания определяется положением курсора
        internal void MakeDir(Tree tree, int position, string name)
        {
            try
            {
                if (tree.condition[position].status != "file")
                {
                    string pasteDir = tree.condition[position].path;
                    string pathString = Path.Combine(pasteDir, name);
                    Directory.CreateDirectory(pathString);
                    int lev = tree.condition[position].level;
                    if (tree.condition[position].open == false)
                    {
                        Tree.expandTree(tree, position);
                    }
                    else
                    {
                        tree.condition.Insert(position + 1, new Set(name, lev + 3, "dir", false, pathString));
                    }
                }
                if (tree.condition[position].status == "file")
                {
                    DirectoryInfo info = new DirectoryInfo(tree.condition[position].path);
                    info = Directory.GetParent(tree.condition[position].path);
                    string pasteDir = info.FullName;
                    string pathString = Path.Combine(pasteDir, name);
                    Directory.CreateDirectory($"{pasteDir}\\{name}");
                    int lev = tree.condition[position].level;
                    tree.condition.Insert(position + 1, new Set(name, lev, "dir", false, pathString));
                    
                }
            }
            catch (Exception e)
            {
                Console.SetCursorPosition(0, 45);
                Console.WriteLine(e.Message);
            }
            tree.Refresh(tree, position);
        }

        //Метод создаёт файл по положению курсора
        internal void MakeFile(Tree tree, int position, string name)
        {
            try
            {
                if (tree.condition[position].status != "file")
                {
                    string pasteDir = tree.condition[position].path;
                    string pathString = Path.Combine(pasteDir, name);
                    File.Create(pathString);
                    int lev = tree.condition[position].level;
                    if (tree.condition[position].open == false)
                    {
                        Tree.expandTree(tree, position);
                    }
                    else
                    {
                        tree.condition.Insert(position + 1, new Set(name, lev + 3, "file", false, pathString));
                    }
                }
                if (tree.condition[position].status == "file")
                {
                    DirectoryInfo info = new DirectoryInfo(tree.condition[position].path);
                    info = Directory.GetParent(tree.condition[position].path);
                    string pasteDir = info.FullName;
                    string pathString = Path.Combine(pasteDir, name);
                    File.Create(pathString);
                    int lev = tree.condition[position].level;
                    tree.condition.Insert(position + 1, new Set(name, lev, "file", false, pathString));
                }
            }
            catch (Exception e)
            {
                Console.SetCursorPosition(0, 45);
                Console.WriteLine(e.Message);
            }
            tree.Refresh(tree, position);
        }

        //Метод удаляет файл или папку, и удаляет их из списка (tree) по положению курсора (position)
        internal void Delete(Tree tree, int position)
        {
            if (tree.condition[position].status == "file" & File.Exists(tree.condition[position].path))
            {
                try
                {
                    File.Delete($"{tree.condition[position].path}");
                    tree.condition.RemoveAt(position);
                    tree.Refresh(tree, position);
                }
                catch (IOException e)
                {
                    Console.SetCursorPosition(0, 45);
                    Console.WriteLine(e.Message);
                    return;
                }
            }
            else if (tree.condition[position].status == "dir" & Directory.Exists(tree.condition[position].path))
            {
                try
                {
                    if (tree.condition[position].open == true)
                    {
                        Tree.compressionTree(tree, position);
                        Directory.Delete($"{tree.condition[position].path}", true);
                        tree.condition.RemoveAt(position);
                        tree.Refresh(tree, position);
                    }
                    else
                    {
                        Directory.Delete($"{tree.condition[position].path}", true);
                        tree.condition.RemoveAt(position);
                        tree.Refresh(tree, position);
                    }

                }
                catch (IOException e)
                {
                    Console.SetCursorPosition(0, 45);
                    Console.WriteLine(e.Message);
                    return;
                }
            }
        }
    }
}
