using System;
using System.Collections.Generic;
using System.Text;

namespace FileManager
{
    class Area
    {
        //Класс определяет некоторую область на экране
        //по начальным координатам, ширине и высоте
        public int height;
        public int width;
        public int startPointX;
        public int startPointY;

        public Area(int _startPointX, int _startPointY, int _width, int _height)
        {
           height = _height;
           width = _width;
           startPointX = _startPointX;
           startPointY = _startPointY;
        }

        //Метод очищает эту область
        public void Clear()
        {
            for (int i = startPointY; i < startPointY + height; i++)
            {
                Console.SetCursorPosition(startPointX, i);
                Console.WriteLine(new string (' ', width));
            }
        }
    }
}
