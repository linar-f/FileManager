using System;
using System.Collections.Generic;
using System.Text;

namespace FileManager
{
    [Serializable]
    class Set
    {
        public string name;
        public int level;
        public string status;
        public bool open;
        public string path;
  
        //Класс представляет собой набор элементов из которых потом формируется дерево
        //Здесь содержаться характеристики: имя, уровень, статус, открыта папка или закрыта, полный путь
        //Уровень - это положение по оси y на экране. Статус - запись указывающая на диск, папку или файл
        public Set(string _name, int _level, string _status, bool _open, string _path)
        {
            name = _name;
            level = _level;
            status = _status;
            open = _open;
            path = _path;
        }
    }
}
