using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Tao.FreeGlut;
using Tao.OpenGl;
using Tao.Platform;
using Tao.Platform.Windows;
//using Tao.DevIl;

namespace grpr3
{
    // ----- В помощь ----- 

    enum GLMode { GLPoints, GLLines, GLTriangles, GLQuads, GLOther } //Режим отрисовки

    class Point3D
    //Точка трехмерного пространства
    {
        public int x, y, z;
        public Color rgb;

        public Point3D(int x1, int y1, int z1)
        {
            x = x1;
            y = y1;
            z = z1;
        }

        public Point3D(int x1, int y1, int z1, Color rgb1)
        {
            x = x1;
            y = y1;
            z = z1;
            rgb = rgb1;
        }
    }

    abstract class GLBase
    //Абстрактный базовый класс для графических примитивов
    {
        public abstract void Draw();
    }

    class GLPoint : GLBase
    //Класс точки
    {
        protected Point3D p;
        protected int size;

        public GLPoint(Point3D p1, int size1)
        {
            p = p1;
            size = size1;
        }

        public override void Draw()
        {
            Gl.glColor3ub(p.rgb.R, p.rgb.G, p.rgb.B);
            Gl.glPointSize(size);
            
            Gl.glBegin(Gl.GL_POINTS);
                Gl.glVertex3i(p.x, p.y, p.z);
            Gl.glEnd();
        }
    }

    class GLLine : GLPoint
    //Класс линии
    {
        protected Point3D pnext;
        protected short type;

        public GLLine(Point3D p1, Point3D p2, int size1, short type1)
            : base(p1, size1)
        {
            pnext = p2;
            type = type1;
            
        }

        public override void Draw()
        {
            Gl.glEnable(Gl.GL_LINE_STIPPLE);
            Gl.glLineWidth(size);
            Gl.glLineStipple(1, type);
            Gl.glBegin(Gl.GL_LINES);
                Gl.glColor3ub(p.rgb.R, p.rgb.G, p.rgb.B);
                Gl.glVertex3i(p.x, p.y, p.z);

                Gl.glColor3ub(pnext.rgb.R, pnext.rgb.G, pnext.rgb.B);
                Gl.glVertex3i(pnext.x, pnext.y, pnext.z);
            Gl.glEnd();
            Gl.glLineStipple(1, 0xFFFF);
            Gl.glDisable(Gl.GL_LINE_STIPPLE);
        }
    }

    class GLTriangle : GLLine
    //Класс треугольника
    {
        protected Point3D pnext2;

        public GLTriangle(Point3D p1, Point3D p2, Point3D p3, int size1, short type1)
            : base(p1, p2, size1, type1)
        {
            pnext2 = p3;
        }

        public override void Draw()
        {
            Gl.glEnable(Gl.GL_LINE_STIPPLE);
            Gl.glLineWidth(size);
            Gl.glLineStipple(1, type);
            Gl.glBegin(Gl.GL_TRIANGLES);
                Gl.glColor3ub(p.rgb.R, p.rgb.G, p.rgb.B);
                Gl.glVertex3i(p.x, p.y, p.z);

                Gl.glColor3ub(pnext.rgb.R, pnext.rgb.G, pnext.rgb.B);
                Gl.glVertex3i(pnext.x, pnext.y, pnext.z);

                Gl.glColor3ub(pnext2.rgb.R, pnext2.rgb.G, pnext2.rgb.B);
                Gl.glVertex3i(pnext2.x, pnext2.y, pnext2.z);

            Gl.glEnd();
            Gl.glLineStipple(1, 0xFFFF);
            Gl.glDisable(Gl.GL_LINE_STIPPLE);
        }
    }

    class GLQuad : GLTriangle
    //Класс четырехугольника
    {
        protected Point3D pnext3;

        public GLQuad(Point3D p1, Point3D p2, Point3D p3, Point3D p4, int size1, short type1)
            : base(p1, p2, p3, size1, type1)
        {
            pnext3 = p4;
        }

        public override void Draw()
        {
            Gl.glEnable(Gl.GL_LINE_STIPPLE);
            Gl.glLineWidth(size);
            Gl.glLineStipple(1, type);
            Gl.glBegin(Gl.GL_QUADS);
                Gl.glColor3ub(p.rgb.R, p.rgb.G, p.rgb.B);
                Gl.glVertex3i(p.x, p.y, p.z);

                Gl.glColor3ub(pnext.rgb.R, pnext.rgb.G, pnext.rgb.B);
                Gl.glVertex3i(pnext.x, pnext.y, pnext.z);

                Gl.glColor3ub(pnext2.rgb.R, pnext2.rgb.G, pnext2.rgb.B);
                Gl.glVertex3i(pnext2.x, pnext2.y, pnext2.z);

                Gl.glColor3ub(pnext3.rgb.R, pnext3.rgb.G, pnext3.rgb.B);
                Gl.glVertex3i(pnext3.x, pnext3.y, pnext3.z);

            Gl.glEnd();
            Gl.glLineStipple(1, 0xFFFF);
            Gl.glDisable(Gl.GL_LINE_STIPPLE);
        }
    }

    class GLTeapot : GLPoint
    //Чайник
    {
        int mode;

        static uint mGlTextureObject = 0;

        public GLTeapot(Point3D p1, int size1, int mode1)
            : base(p1, size1)
        {
            mode = mode1;
        }

        public override void Draw()
        {
            Gl.glLineWidth(1);
            Gl.glPointSize(1);

            if (mode == -1)
            {
                //TexLoad();
                // включаем режим текстурирования
                Gl.glEnable(Gl.GL_TEXTURE_2D);
                // включаем режим текстурирования , указывая индификатор mGlTextureObject
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, mGlTextureObject);
            }
            else
            {
                Gl.glDisable(Gl.GL_TEXTURE_2D);
            }

            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_DIFFUSE, new float[] { 1, 1, 1, 1 });
            Gl.glPushMatrix();
                Gl.glTranslatef(p.x, p.y, 0);
                Gl.glColor3ub(p.rgb.R, p.rgb.G, p.rgb.B);
                Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, mode);
                Glut.glutSolidTeapot(size);
            Gl.glPopMatrix();
            Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL);
        }

/*        private static void TexLoad()
        {
            int imageId;
            // создаем изображение с индификатором imageId
            Il.ilGenImages(1, out imageId);
            // делаем изображение текущим
            Il.ilBindImage(imageId);

            // адрес изображения полученный с помощью окна выбра файла
            string url = @"bm.bmp";

            // пробуем загрузить изображение
            if (Il.ilLoadImage(url))
            {

                // если загрузка прошла успешно
                // сохраняем размеры изображения
                int width = Il.ilGetInteger(Il.IL_IMAGE_WIDTH);
                int height = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT);

                // определяем число бит на пиксель
                int bitspp = Il.ilGetInteger(Il.IL_IMAGE_BITS_PER_PIXEL);

                switch (bitspp) // в зависимости оп полученного результата
                {

                    // создаем текстуру используя режим GL_RGB или GL_RGBA
                    case 24:
                        mGlTextureObject = MakeGlTexture(Gl.GL_RGB, Il.ilGetData(), width, height);
                        break;
                    case 32:
                        mGlTextureObject = MakeGlTexture(Gl.GL_RGBA, Il.ilGetData(), width, height);
                        break;

                }
                // очищаем память
                Il.ilDeleteImages(1, ref imageId);
            }
        }   

        // создание текстуры в панями openGL
        private static uint MakeGlTexture(int Format, IntPtr pixels, int w, int h)
        {

            // ид текстурного объекта
            uint texObject;

            // генерируем текстурный объект
            Gl.glGenTextures(1, out texObject);

            // устанавливаем режим упаковки пикселей
            Gl.glPixelStorei(Gl.GL_UNPACK_ALIGNMENT, 1);

            // создаем привязку к только что созданной текстуре
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, texObject);

            // устанавливаем режим фильтрации и повторения текстуры
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
            Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_REPLACE);

            // создаем RGB или RGBA текстуру
            switch (Format)
            {

                case Gl.GL_RGB:
                    Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGB, w, h, 0, Gl.GL_RGB, Gl.GL_UNSIGNED_BYTE, pixels);
                    break;

                case Gl.GL_RGBA:
                    Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA, w, h, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, pixels);
                    break;
            }

            // возвращаем идентификатор текстурного объекта

            return texObject;

        }               */

    }
}
