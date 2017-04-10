using Microsoft.Msagl.Drawing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace project1
{
    public partial class Form1 : Form
    {
        string str1 = "", str2 = "";
        int rad1 = 0, rad2 = 0, back = 0;
        Graph g3 = new Graph();
        FileStream fileStream;
        StreamReader streamReader;

        public Form1()
        {
            InitializeComponent();
            gViewer1.Graph = g3;
            gViewer2.Graph = g3;
            Start(dataGridView1);
            Start(dataGridView2);
            toolStripStatusLabel1.Text = "Ready to start";
        }
    
        //Заполнение матрицы пустыми строками
        private void Start(DataGridView dataGridView)
        {
            dataGridView.RowHeadersDefaultCellStyle.Padding = new Padding(2);
            dataGridView.RowHeadersWidth = 35;
            dataGridView.ColumnCount = 20;
            dataGridView.RowCount = 20;
            for (int i = 0; i < dataGridView.ColumnCount; i++)
            {
                dataGridView.Columns[i].Width = 20;
                dataGridView.Rows[i].Height = 20;
                dataGridView.Columns[i].Name = (i + 1).ToString();
                dataGridView.Rows[i].HeaderCell.Value = Convert.ToString(i + 1);
                for (int j = 0; j < dataGridView.ColumnCount; j++)
                    dataGridView.Rows[i].Cells[j].Value = "";
            }
        }

        private void graph1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Open();
            if (back != 1)
                Func0(1);
        }

        private void graph2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Open();
            if (back != 1)
                Func0(2);
        }

        //Ввод графа из файла
        private void Open()
        {
            str1 = str2 = "";
            back = 0;
            toolStripStatusLabel1.Text = "Opening file...";
            try
            {
                DialogResult result = openFileDialog1.ShowDialog();
                if (result == DialogResult.OK)
                {
                    fileStream = new FileStream(openFileDialog1.FileName, FileMode.Open);
                    streamReader = new StreamReader(fileStream);
                    try
                    {
                        string line;
                        while ((line = streamReader.ReadLine()) != null)
                        {
                            while (line.IndexOf("  ") != -1)
                                line = line.Replace("  ", " ");
                            if (line != " ")
                            {
                                line = line.Trim();
                                if (System.Text.RegularExpressions.Regex.Match(line, @"^[\s\d]*$").Success)
                                {
                                    if (str1 == "")
                                        str1 = line;
                                    else if (str2 == "")
                                        str2 = line;
                                }
                            }
                        }
                    }
                    catch (Exception) { }
                    streamReader.Close();
                    fileStream.Close();
                    toolStripStatusLabel1.Text = "Open file: " + openFileDialog1.FileName;
                }
                else
                {
                    back = 1;
                    toolStripStatusLabel1.Text = "Opening canceled";
                }
            }
            catch (Exception) { }
        }
        
        //Функция выбора номера графа для построения
        private void Func0(int index)
        {
            int[] ME1 = { }, MV1 = { }, MV2 = { }, ME2 = { };
            int[,] AdjMatr1 = { }, AdjMatr2 = { };
            Graph g1 = new Graph();
            Graph g2 = new Graph();
            if (index == 1)
                Func1(ME1, MV1, AdjMatr1, listBox1, dataGridView1, g1, gViewer1, 1);
            else
                Func1(ME2, MV2, AdjMatr2, listBox2, dataGridView2, g2, gViewer2, 2);
            if (!string.IsNullOrEmpty(dataGridView1.Rows[0].Cells[0].Value.ToString()) && !string.IsNullOrEmpty(dataGridView2.Rows[0].Cells[0].Value.ToString()))
                compareToolStripMenuItem.Enabled = true;
        }

        //Основная функция
        private void Func1(int[] ME, int[] MV, int[,] AdjMatr, ListBox listBox, DataGridView dataGridView, Graph g, Microsoft.Msagl.GraphViewerGdi.GViewer gViewer, int zip)
        {
            try
            {
                gViewer.Graph = g3;
                Start(dataGridView);
                listBox.Items.Clear();
                while (str1.IndexOf("0") != -1)
                    throw new Exception("Wrong format of the first line.");
                ME = str1.Split(' ').Select(int.Parse).ToArray();
                MV = str2.Split(' ').Select(int.Parse).ToArray();
                int vertex = MV.Length, edge = ME.Length;
                AdjMatr = new int[vertex, vertex];
                if (edge != MV[MV.Length - 1]) throw new Exception("Incorrectly stated the number of arcs in the MV array.");
                if (vertex > 20) throw new Exception("Too many vertices.");
                if (edge > 50) throw new Exception("Too many arcs.");
                for (int i = 0; i < ME.Length; i++)
                    if (ME[i] > vertex)
                        throw new Exception("Link to a non-existent vertex.");
                for (int i = 1; i < MV.Length; i++)
                    if (MV[i] < MV[i - 1])
                        throw new Exception("MV array elements are not in ascending order.");
                listBox.Items.Add("Graph:");
                listBox.Items.Add("ME = " + str1);
                listBox.Items.Add("MV = " + str2);
                listBox.Items.Add("");
                listBox.Items.Add("Vertex: " + vertex);
                listBox.Items.Add("Arc: " + edge);
                listBox.Items.Add("List of arc:");
                for (int i = 0; i < vertex; i++)
                    for (int j = 0; j < vertex; j++)
                        AdjMatr[i, j] = 0;
                //Построение матрицы смежности
                Matrix(ME, MV, AdjMatr, g, listBox, vertex);
                addEdge(AdjMatr, vertex, dataGridView);
                gViewer.Graph = g;
                if (zip == 1) deleteToolStripMenuItem.Enabled = true;
                else deleteGraph2ToolStripMenuItem.Enabled = true;
                clearAllToolStripMenuItem.Enabled = true;
                //Удаление висячих вершин
                VertexDel(AdjMatr, vertex, listBox);
                //Нахождение радиуса графа
                int rad = 0;
                rad = Compare(AdjMatr, vertex, rad);
                if (rad == 100000) rad = 0;
                if (zip == 1) rad1 = rad;
                else rad2 = rad;
            }
            catch (FormatException)
            {
                toolStripStatusLabel1.Text = "Build error";
                MessageBox.Show("Incorrect entry graph.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (string.IsNullOrEmpty(dataGridView1.Rows[0].Cells[0].Value.ToString()))
                {
                    deleteToolStripMenuItem.Enabled = false;
                    if (deleteGraph2ToolStripMenuItem.Enabled == false)
                        clearAllToolStripMenuItem.Enabled = false;
                }
                if (string.IsNullOrEmpty(dataGridView2.Rows[0].Cells[0].Value.ToString()))
                {
                    deleteGraph2ToolStripMenuItem.Enabled = false;
                    if (deleteToolStripMenuItem.Enabled == false)
                        clearAllToolStripMenuItem.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                toolStripStatusLabel1.Text = "Build error";
                MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (string.IsNullOrEmpty(dataGridView1.Rows[0].Cells[0].Value.ToString()))
                {
                    deleteToolStripMenuItem.Enabled = false;
                    if (deleteGraph2ToolStripMenuItem.Enabled == false)
                        clearAllToolStripMenuItem.Enabled = false;
                }
                if (string.IsNullOrEmpty(dataGridView2.Rows[0].Cells[0].Value.ToString()))
                {
                    deleteGraph2ToolStripMenuItem.Enabled = false;
                    if (deleteToolStripMenuItem.Enabled == false)
                        clearAllToolStripMenuItem.Enabled = false;
                }
            }
        }

        //Построение матрицы смежности
        private int[,] Matrix(int[] ME, int[] MV, int[,] AdjMatr, Graph g, ListBox listBox, int vertex)
        {
            int v = 0;
            for (int i = 0; i < vertex; i++)
                while (v < MV[i])
                {
                    AdjMatr[i, ME[v] - 1] = 1;
                    listBox.Items.Add((i + 1) + " " + ME[v]);
                    g.AddEdge(Convert.ToString(i + 1), Convert.ToString(ME[v]));
                    v++;
                }
            return AdjMatr;
        }

        //Удаление висячих вершин
        private int[,] VertexDel(int[,] AdjMatr, int vertex, ListBox listBox)
        {
            int s = 0, ind = 0;
            for (int k = 0; k < vertex; k++)
                for (int i = 0; i < vertex; i++)
                {
                    for (int j = 0; j < vertex; j++)
                    {
                        s += AdjMatr[i, j];
                        s += AdjMatr[j, i];
                    }
                    if (s == 1)
                    {
                        for (int j = 0; j < vertex; j++)
                        {
                            AdjMatr[i, j] = 0;
                            AdjMatr[j, i] = 0;
                        }
                        ind++;
                    }
                    s = 0;
                }
            listBox.Items.Add("Dangling vertex: " + ind);
            return AdjMatr;
        }

        //Нахождение радиуса графа
        private int Compare(int[,] AdjMatr, int n, int rad)
        {
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    if (AdjMatr[i, j] == 0)
                        AdjMatr[i, j] = 100000;
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    for (int k = 0; k < n; k++)
                        AdjMatr[j, k] = Math.Min(AdjMatr[j, k], AdjMatr[j, i] + AdjMatr[i, k]);
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    if (AdjMatr[i, j] == 100000)
                        AdjMatr[i, j] = 0;
            int[] Ecc = new int[n];
            for (int i = 0; i < n; i++)
            {
                Ecc[i] = 0;
                for (int j = 0; j < n; j++)
                    Ecc[i] = Math.Max(Ecc[i], AdjMatr[i, j]);
            }
            for (int i = 0; i < n; i++)
                if (Ecc[i] == 0)
                    Ecc[i] = 100000;
            rad = Ecc[0];
            for (int i = 1; i < n; i++)
                rad = Math.Min(rad, Ecc[i]);
            return rad;
        }

        //Вывод матрицы на экран
        private void addEdge(int[,] AdjMatr, int vertex, DataGridView dataGridView)
        {
            for (int i = 0; i < vertex; i++)
                for (int j = 0; j < vertex; j++)
                    dataGridView.Rows[i].Cells[j].Value = AdjMatr[i, j];
        }

        //Метод, который вызывает ввод графа с клавиатуры
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "Creature...";
            Form2 form2 = new Form2();
            form2.StartPosition = FormStartPosition.CenterParent;
            form2.ShowDialog(this);
            toolStripStatusLabel1.Text = "Сreating canceled";
            if (form2.DialogResult == DialogResult.OK)
            {
                str1 = form2.ReturnData1();
                str2 = form2.ReturnData2();
                while (str1.IndexOf("  ") != -1)
                    str1 = str1.Replace("  ", " ");
                while (str2.IndexOf("  ") != -1)
                    str2 = str2.Replace("  ", " ");
                str1 = str1.Trim();
                str2 = str2.Trim();
                toolStripStatusLabel1.Text = "Created a new graph (Graph " + form2.ReturnData3() + ")";
                Func0(form2.ReturnData3());
            }
        }

        //Очистка программы
        private void clearAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            Start(dataGridView1);
            Start(dataGridView2);
            gViewer1.Graph = g3;
            gViewer2.Graph = g3;
            compareToolStripMenuItem.Enabled = false;
            toolStripStatusLabel1.Text = "Ready to start";
            clearAllToolStripMenuItem.Enabled = false;
            deleteGraph2ToolStripMenuItem.Enabled = false;
            deleteToolStripMenuItem.Enabled = false;
        }

        //О программе
        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3();
            form3.StartPosition = FormStartPosition.CenterParent;
            form3.ShowDialog(this);
        }

        //Удаление первого графа
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            Start(dataGridView1);
            gViewer1.Graph = g3;
            toolStripStatusLabel1.Text = "Removed Graph 1";
            compareToolStripMenuItem.Enabled = false;
            deleteToolStripMenuItem.Enabled = false;
            if (deleteGraph2ToolStripMenuItem.Enabled == false)
                clearAllToolStripMenuItem.Enabled = false;
        }

        //Удаление второго графа
        private void deleteGraph2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
            Start(dataGridView2);
            gViewer2.Graph = g3;
            toolStripStatusLabel1.Text = "Removed Graph 2";
            compareToolStripMenuItem.Enabled = false;
            deleteGraph2ToolStripMenuItem.Enabled = false;
            if (deleteToolStripMenuItem.Enabled == false)
                clearAllToolStripMenuItem.Enabled = false;
        }

        //Помощь
        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form4 form4 = new Form4();
            form4.StartPosition = FormStartPosition.CenterParent;
            form4.ShowDialog(this);
        }

        //Сравнение графов
        private void compareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s = "Radius Graph 1 = " + rad1 + "                                    \nRadius Graph 2 = " + rad2;
            if (rad1 == rad2)
                s += "\n\nThe graphs are equivalent!";
            else s += "\n\nThe graphs are not equivalent!";
            MessageBox.Show(s, "Result", MessageBoxButtons.OK);
            listBox1.Items.Add("Radius: " + rad1);
            listBox2.Items.Add("Radius: " + rad2);
        }

        //Закрыть программу
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}