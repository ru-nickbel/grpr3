using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Tao.FreeGlut;
//using Tao.DevIl;
using Tao.OpenGl;
using Tao.Platform;
using Tao.Platform.Windows;
using Telerik.WinControls.UI;
using Telerik.WinControls.UI.Data;
namespace grpr3
{
    public partial class FormMain : Form
    {
        GLMode mode;   //режим отрисовки

        ArrayList ObjList = new ArrayList();    //список с очередями отрисовки

        ObjPack Packet;   //общий пакет для передачи данных об отрисовываемом объекте


        public FormMain()
        {
            InitializeComponent();
            simpleOpenGlControl1.InitializeContexts();

            //Создание отдельных объектов с очередями для каждого из типов примитивов
            GLDrawing Points = new GLDrawing(simpleOpenGlControl1, GLMode.GLPoints);
            ObjList.Add(Points);
            GLDrawing Lines = new GLDrawing(simpleOpenGlControl1, GLMode.GLLines);
            ObjList.Add(Lines);
            GLDrawing Triangles = new GLDrawing(simpleOpenGlControl1, GLMode.GLTriangles);
            ObjList.Add(Triangles);
            GLDrawing Quads = new GLDrawing(simpleOpenGlControl1, GLMode.GLQuads);
            ObjList.Add(Quads);
            GLDrawing Teapots = new GLDrawing(simpleOpenGlControl1, GLMode.GLOther);
            ObjList.Add(Teapots);
            
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            Gl.glEnable(Gl.GL_LINE_STIPPLE);
            Glut.glutInit();
            Glut.glutInitDisplayMode(Glut.GLUT_RGB | Glut.GLUT_DOUBLE | Glut.GLUT_DEPTH);

            
            //Очистка поля отрисовки
            Gl.glClearColor(1.0f, 1.0f, 1.0f, 1.0f);
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

            //Установка поля ввода и матрицы преобразований
            Gl.glViewport(0, 0, simpleOpenGlControl1.Width, simpleOpenGlControl1.Height);

            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();

            Gl.glOrtho(0.0, simpleOpenGlControl1.Width, 0.0, simpleOpenGlControl1.Height,-1000,1000);

            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glLoadIdentity();

            radTileElementPoint_Click(sender, e);
            radDropDownListType.SelectedIndex = 0;
            radDropDownListMode.SelectedIndex = 2;
            radTrackBar1.Value = 20;
        }

        private void FormMain_Resize(object sender, EventArgs e)
        {
            Gl.glClearColor(1f, 1f, 1f, 1f);
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

            Gl.glViewport(0, 0, simpleOpenGlControl1.Width, simpleOpenGlControl1.Height);
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();

            Gl.glOrtho(0.0, simpleOpenGlControl1.Width, 0.0, simpleOpenGlControl1.Height, -1, 1);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
        }



        //------------ Методы рисования ---------------
        private void Draw(GLMode mode, ObjPack packet)
        {
            GLDrawing d = (GLDrawing)ObjList[(int)mode];
            d.DrawPrimitive(packet);
        }

        private void ReDraw(GLMode mode)
        {
            GLDrawing d = (GLDrawing)ObjList[(int)mode];
            d.ReDrawPrimitives();
        }

        private void ReDrawAll()
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            foreach (GLDrawing g in ObjList)
            {
                g.BGColor = labelBg.BackColor;
                g.ReDrawPrimitives();
            }
        }

        Point3D Get3DPoint(MouseEventArgs e, Color cl)
        //Снятие координат точки с экрана + Z
        {
            FormZ fr = new FormZ(this);
            fr.StartPosition = FormStartPosition.CenterParent;

            if (fr.ShowDialog() == DialogResult.OK)
            {
                Point3D p = new Point3D(e.X, simpleOpenGlControl1.Height - e.Y, fr.z, cl);
                return p;
            }
            return null;
        }

        private void simpleOpenGlControl1_MouseDown(object sender, MouseEventArgs e)
        //Отрисовка по щелчку на поле
        {
            Packet.size = radTrackBar1.Value;
            radDropDownListType_SelectedIndexChanged(sender, new PositionChangedEventArgs(radDropDownListType.SelectedIndex));
            radDropDownListMode_SelectedIndexChanged(sender, new PositionChangedEventArgs(radDropDownListMode.SelectedIndex));
            if (Packet.P1 == null)
            {
                Packet.P1 = Get3DPoint(e, label1.BackColor);
                label1.BorderStyle = BorderStyle.FixedSingle;
                if (mode == GLMode.GLPoints)
                {
                    Draw(GLMode.GLPoints, Packet);
                    ReDrawAll();
                    Packet.Dispose();
                }
                if (mode == GLMode.GLOther)
                {
                    Packet.size *= 5;
                    Draw(GLMode.GLOther, Packet);
                    ReDrawAll();
                    UnSelectPoints();
                }
                return;
            }

            if (Packet.P2 == null)
            {
                Packet.P2 = Get3DPoint(e, label2.BackColor);
                label2.BorderStyle = BorderStyle.FixedSingle;
                if (mode == GLMode.GLLines)
                {
                    Draw(GLMode.GLLines, Packet);
                    ReDrawAll();
                    UnSelectPoints();
                }

                return;
            }

            if (Packet.P3 == null)
            {
                Packet.P3 = Get3DPoint(e, label3.BackColor);
                label3.BorderStyle = BorderStyle.FixedSingle;
                if (mode == GLMode.GLTriangles)
                {
                    Draw(GLMode.GLTriangles, Packet);
                    ReDrawAll();
                    UnSelectPoints();
                }
                return;
            }

            if (Packet.P4 == null)
            {
                Packet.P4 = Get3DPoint(e, label4.BackColor);
                label4.BorderStyle = BorderStyle.FixedSingle;
                if (mode == GLMode.GLQuads)
                {
                    Draw(GLMode.GLQuads, Packet);
                    ReDrawAll();
                    UnSelectPoints();
                }
            }

        }




        //------------ Сбросы/установки кнопок и меток ---------------
        private void SetLabelColor(object sender, EventArgs e)
        //Общий метод для клика всех лейблов с цветами
        {
            Label lb = sender as Label;
            if (radColorDialog1.ShowDialog() == DialogResult.OK)
                lb.BackColor = radColorDialog1.SelectedColor;
        }

        private void labelBg_Click(object sender, EventArgs e)
        //Смена цвета фона
        {
            SetLabelColor(sender, e);
            ReDrawAll();

        }

        private void UnSelectPoints()
        //Сброс указания цвета вводимой точки
        {
            label1.BorderStyle = BorderStyle.None;
            label2.BorderStyle = BorderStyle.None;
            label3.BorderStyle = BorderStyle.None;
            label4.BorderStyle = BorderStyle.None;
            Packet.Dispose();
        }

        private void UnCheckButtons()
        //Снятие подсветки со всех кнопок
        {
            Gl.glDisable(Gl.GL_LIGHT0);
            Gl.glDisable(Gl.GL_LIGHTING);
            //Gl.glClearColor((float)bgcolor.R / 255f, (float)bgcolor.G / 255f, (float)bgcolor.B / 255f, 1f);
            foreach (RadTileElement r in radPanorama1.Items)
            {
                r.BorderColor = Color.FromArgb(119, 165, 221);
            }
            radTileElementTr.Image = Properties.Resources.tr;
            radTileElementLn.Image = Properties.Resources.pr_not;
            radTileElementRc.Image = Properties.Resources.pu_not;
            radTileElementTp.Image = Properties.Resources.chainik;
            radTileElementPt.Image = Properties.Resources.point_no;
            label1.Visible = true;
            label2.Visible = true;
            label3.Visible = true;
            label4.Visible = true;
            //radPanel3.Enabled = false;
        }



        //------------ События кнопок ---------------
        //точка
        private void radTileElementPoint_Click(object sender, EventArgs e)
        {
            mode = GLMode.GLPoints;
            ReDrawAll();
            UnCheckButtons();
            radTileElementPt.BorderColor = Color.FromArgb(0, 0, 0);
            radTileElementPt.Image = Properties.Resources.point_act;
            label1.Visible = true;
            label2.Visible = false;
            label3.Visible = false;
            label4.Visible = false;
        }

        //прямая
        private void radTileElementLine_Click(object sender, EventArgs e)
        {
            mode = GLMode.GLLines;
            ReDrawAll();
            UnCheckButtons();
            radTileElementLn.BorderColor = Color.FromArgb(0, 0, 0);
            radTileElementLn.Image = Properties.Resources.pr;
            label3.Visible = false;
            label4.Visible = false;
        }

        //треугольник
        private void radTileElementTriangle_Click(object sender, EventArgs e)
        {
            mode = GLMode.GLTriangles;
            ReDrawAll();
            UnCheckButtons();
            radTileElementTr.BorderColor = Color.FromArgb(0, 0, 0);
            radTileElementTr.Image = Properties.Resources.tr_act;
            label4.Visible = false;
        }

        // четырёхугольник
        private void radTileElementQuad_Click(object sender, EventArgs e)
        {
            mode = GLMode.GLQuads;
            ReDrawAll();
            UnCheckButtons();
            radTileElementRc.BorderColor = Color.FromArgb(0, 0, 0);
            radTileElementRc.Image = Properties.Resources.pu_act;
        }

        //чайник
        private void radTileElementTeapot_Click(object sender, EventArgs e)
        {
            mode = GLMode.GLOther;
            UnCheckButtons();
            radPanel3.Enabled = true;
            radTileElementTp.BorderColor = Color.FromArgb(0, 0, 0);
            radTileElementTp.Image = Properties.Resources.chainik_akt;
            label1.Visible = true;
            label2.Visible = false;
            label3.Visible = false;
            label4.Visible = false;
        }

        private void buttonRotateRight_Click(object sender, EventArgs e)
        {
            foreach (GLDrawing gl in ObjList)
                gl.AngleY += 10;
        }

        private void buttonRotateLeft_Click(object sender, EventArgs e)
        {
            foreach (GLDrawing gl in ObjList)
                gl.AngleY -= 10;
        }

        private void buttonScaleMinus_Click(object sender, EventArgs e)
        {
            foreach (GLDrawing gl in ObjList)
                gl.Scale += 10;
        }

        private void buttonScalePlus_Click(object sender, EventArgs e)
        {
            foreach (GLDrawing gl in ObjList)
                gl.Scale -= 10;
        }

        private void buttonLightEna_Click(object sender, EventArgs e)
        //Свет вкл.
        {
            Gl.glEnable(Gl.GL_LIGHTING);
            Gl.glEnable(Gl.GL_LIGHT0);
            Gl.glLightModelf(Gl.GL_LIGHT_MODEL_TWO_SIDE, Gl.GL_TRUE);
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, new float[] { 1, 1, 1, 0 });
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_SPECULAR, new float[] { 1f, 1f, 1f, 1f });
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_AMBIENT, new float[] { 1f, 1f, 1f, 1f });
            ReDrawAll();
            //simpleOpenGlControl1.Invalidate();
        }

        private void buttonLightDis_Click(object sender, EventArgs e)
        //Свет выкл.
        {
            Gl.glDisable(Gl.GL_LIGHTING);
            Gl.glDisable(Gl.GL_LIGHT0);
            ReDrawAll();
        }

        private void buttonClear_Click(object sender, EventArgs e)
        //Очистка поля отрисовки
        {
            foreach (GLDrawing gl in ObjList)
                gl.Dispose();
            ReDrawAll();
        }



        //------------ События списков ---------------
        private void radDropDownListType_SelectedIndexChanged(object sender, PositionChangedEventArgs e)
        //Смена типа линии
        {
            switch (radDropDownListType.SelectedIndex)
            {
                case 0: Packet.type = 32767;
                    break;
                case 1: Packet.type = 1;
                    break;
                case 2: Packet.type = 9090;
                    break;
                case 3: Packet.type = -1;
                    break;
                default: Packet.type = -1;
                    break;
            }
        }

        private void radDropDownListMode_SelectedIndexChanged(object sender, PositionChangedEventArgs e)
        //Выбор режима отрисовки
        {
            switch (radDropDownListMode.SelectedIndex)
            {
                case 0: Packet.mode = Gl.GL_POINT; break;
                case 1: Packet.mode = Gl.GL_LINE; break;
                case 2: Packet.mode = Gl.GL_FILL; break;
                case 3: Packet.mode = -1; break;
            }
        }    

    }
}
