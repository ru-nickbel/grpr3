using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Tao.FreeGlut;
using Tao.OpenGl;
using Tao.Platform;
using Tao.Platform.Windows;


namespace grpr3
{
    struct ObjPack
    //Пакет для обмена данными между классом отрисовки и формой
    {
        public Point3D P1, P2, P3, P4;   //Точки объекта
        public short type;               //Тип линии
        public int size;                 //Размеры
        public int mode;                 //Режим отрисовки

        public void Dispose()
        //Очистка пакета
        {
            P1 = null;
            P2 = null;
            P3 = null;
            P4 = null;
            type = -1;
            size = 1;
            mode = Gl.GL_FILL;
        }
    }

    class GLDrawing
    //Класс отрисовки примитивов на форме
    {

        SimpleOpenGlControl s; //Контекст отрисовки

        //Переменные объектов отрисовки
        GLMode g; //Режим отрисовки
        const int NMAX = 64; //Максимальное количество элементов одного типа
        GLBase[] glb; //Массив элементов для отрисовки
        int obcnt; //Размер массива элементов

        //Переменные среды отрисовки
        Color bgcolor = Color.White; //Цвет фона 
        int angleY = 0;   //Угол поворота вокруг У
        int scale = 100;    //Масштаб
        int zbox = 100;     //Размер коробки

        public Color BGColor
        {
            get { return bgcolor; }
            set
            {
                bgcolor = value;
                this.ReDrawPrimitives();
            }
        }

        public int AngleY
        {
            get { return angleY; }
            set
            {
                angleY = value;
                Gl.glPushMatrix(); //проверить
                    Gl.glTranslatef(s.Width / 2, 0, 0);
                    Gl.glRotatef(angleY, 0, 1, 0);
                    Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
                    ReDrawPrimitives();
                Gl.glPopMatrix();
                
            }
        }

        public int Scale
        {
            get { return scale; }
            set
            {
                scale = value;
                Gl.glPushMatrix();
                    float sc = (float)scale / 100;
                    Gl.glTranslatef(s.Width / 2, 0, 0);
                    Gl.glScalef(sc, sc, sc);
                    Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
                    ReDrawPrimitives();
                Gl.glPopMatrix();
            }
        }

        public int Zbox
        {
            get { return zbox; }
            set
            {
                zbox = value;
                Gl.glOrtho(0.0, s.Width, 0.0, s.Height, -zbox, zbox);
                ReDrawPrimitives();
            }
        }
        
        public GLDrawing(SimpleOpenGlControl s1, GLMode g1)
        //Инициализация экземпляра класса
        {
            s = s1;
            obcnt = 0;
            g = g1;
            glb = new GLBase[NMAX];

        }

        public void DrawPrimitive(ObjPack packet)
        //Отрисовка нужного примитива
        {
            switch (g)
            {
                case GLMode.GLPoints:
                    glb[obcnt] = new GLPoint(packet.P1, packet.size);
                    break;
                case GLMode.GLLines:
                    glb[obcnt] = new GLLine(packet.P1, packet.P2, packet.size, packet.type);
                    break;
                case GLMode.GLTriangles:
                    glb[obcnt] = new GLTriangle(packet.P1, packet.P2, packet.P3, packet.size, packet.type);
                    break;
                case GLMode.GLQuads:
                    glb[obcnt] = new GLQuad(packet.P1, packet.P2, packet.P3, packet.P4, packet.size, packet.type);
                    break;
                case GLMode.GLOther:
                    glb[obcnt] = new GLTeapot(packet.P1, packet.size, packet.mode);
                    break;
            }
            
            obcnt++;
        }

        public void ReDrawPrimitives()
        //Перерисовка всех объектов из массива
        {

            Gl.glClearColor((float)bgcolor.R / 255f, (float)bgcolor.G / 255f, (float)bgcolor.B / 255f, (float)bgcolor.A / 255f);
            for (int i = 0; i < obcnt; i++)
                glb[i].Draw();
            s.Invalidate();
        }

        public void Dispose()
        //Ликвидация отрисованного
        {
            obcnt = 0;
        }
    }
}
