using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.Json;

namespace SystAnalys_lr1
{
    public partial class Form1 : Form
    {
        DrawGraph G;
        List<Vertex> V;
        List<Edge> E;
        int[,] AMatrix; //матрица смежности
        int[,] IMatrix; //матрица инцидентности

        int selected1; //выбранные вершины, для соединения линиями
        int selected2;

        public bool closeApp;
        public bool back;

        public Form1()
        {
            InitializeComponent();
            V = new List<Vertex>();
            G = new DrawGraph(sheet.Width, sheet.Height);
            E = new List<Edge>();
            sheet.Image = G.GetBitmap();
            this.closeApp = true;
        }

        public Form1(string json, string filename)
        {
            InitializeComponent();

            GraphVE graphVE = JsonSerializer.Deserialize<GraphVE>(json);

            this.filePath.Text = filename;
            V = graphVE.vertex;
            G = new DrawGraph(sheet.Width, sheet.Height);
            E = graphVE.edges;
            G.drawALLGraph(V, E);
            matchings_Click(null, null);
            sheet.Image = G.GetBitmap();
            this.filePath.Text = filename;
            this.closeApp = true;
        }

        //кнопка - выбрать вершину
        private void selectButton_Click(object sender, EventArgs e)
        {
            // selectButton.Enabled = false;
            drawVertexButton.Enabled = true;
            drawEdgeButton.Enabled = true;
            deleteButton.Enabled = true;
            // clearEdgesAllocate();
            G.clearSheet();
            G.drawALLGraph(V, E);
            sheet.Image = G.GetBitmap();
            selected1 = -1;
        }

        //кнопка - рисовать вершину
        private void drawVertexButton_Click(object sender, EventArgs e)
        {
            drawVertexButton.Enabled = false;
            // selectButton.Enabled = true;
            drawEdgeButton.Enabled = true;
            deleteButton.Enabled = true;
            // clearEdgesAllocate();
            G.clearSheet();
            G.drawALLGraph(V, E);
            sheet.Image = G.GetBitmap();
        }

        //кнопка - рисовать ребро
        private void drawEdgeButton_Click(object sender, EventArgs e)
        {
            drawEdgeButton.Enabled = false;
            // selectButton.Enabled = true;
            drawVertexButton.Enabled = true;
            deleteButton.Enabled = true;
            // clearEdgesAllocate();
            G.clearSheet();
            G.drawALLGraph(V, E);
            sheet.Image = G.GetBitmap();
            selected1 = -1;
            selected2 = -1;
        }

        //кнопка - удалить элемент
        private void deleteButton_Click(object sender, EventArgs e)
        {
            deleteButton.Enabled = false;
            // selectButton.Enabled = true;
            drawVertexButton.Enabled = true;
            drawEdgeButton.Enabled = true;
            G.clearSheet();
            G.drawALLGraph(V, E);
            sheet.Image = G.GetBitmap();
        }

        //кнопка - удалить граф
        private void deleteALLButton_Click(object sender, EventArgs e)
        {
            // selectButton.Enabled = true;
            drawVertexButton.Enabled = true;
            drawEdgeButton.Enabled = true;
            deleteButton.Enabled = true;
            const string message = "Вы действительно хотите полностью удалить граф?";
            const string caption = "Удаление";
            var MBSave = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (MBSave == DialogResult.Yes)
            {
                V.Clear();
                E.Clear();
                G.clearSheet();
                sheet.Image = G.GetBitmap();
                listBoxMatrix.Items.Clear();
            }
        }

        //кнопка - матрица смежности
        private void buttonAdj_Click(object sender, EventArgs e)
        {
            createAdjAndOut();
        }

        //кнопка - матрица инцидентности 
        private void buttonInc_Click(object sender, EventArgs e)
        {
            createIncAndOut();
        }

        private void sheet_MouseClick(object sender, MouseEventArgs e)
        {
            //нажата кнопка "рисовать вершину"
            if (drawVertexButton.Enabled == false)
            {
                Vertex newVertex = new Vertex(e.X, e.Y);
                bool res = true;
                foreach (Vertex vertex in V)
                {
                    if (findDistance(newVertex, vertex) < (G.R * 1.5 + G.R))
                    {
                        res = false; 
                        break;
                    }
                }

                if (res)
                {
                    V.Add(newVertex);
                    G.drawVertex(e.X, e.Y, ((char)('a' + (V.Count - 1))).ToString());
                    sheet.Image = G.GetBitmap();
                }
                else
                {
                    if (listBoxMatrix.Items.Count > 0 && listBoxMatrix.ForeColor != Color.Red)
                        listBoxMatrix.Items.Clear(); 
                    listBoxMatrix.Items.Add("Вершина находится близко!");
                    listBoxMatrix.ForeColor = Color.Red;
                }
            }
            //нажата кнопка "рисовать ребро"
            if (drawEdgeButton.Enabled == false)
            {
                if (e.Button == MouseButtons.Left)
                {
                    for (int i = 0; i < V.Count; i++)
                    {
                        if (Math.Pow((V[i].x - e.X), 2) + Math.Pow((V[i].y - e.Y), 2) <= G.R * G.R)
                        {
                            if (selected1 == -1)
                            {
                                G.drawSelectedVertex(V[i].x, V[i].y);
                                selected1 = i;
                                sheet.Image = G.GetBitmap();
                                break;
                            }
                            if (selected2 == -1)
                            {
                                G.drawSelectedVertex(V[i].x, V[i].y);
                                selected2 = i;
                                E.Add(new Edge(selected1, selected2));
                                // clearEdgesAllocate();
                                G.drawEdge(V[selected1], V[selected2], E[E.Count - 1], E.Count - 1);
                                selected1 = -1;
                                selected2 = -1;
                                sheet.Image = G.GetBitmap();
                                break;
                            }
                        }
                    }
                }
                if (e.Button == MouseButtons.Right)
                {
                    if ((selected1 != -1) &&
                        (Math.Pow((V[selected1].x - e.X), 2) + Math.Pow((V[selected1].y - e.Y), 2) <= G.R * G.R))
                    {
                        G.drawVertex(V[selected1].x, V[selected1].y, (selected1 + 1).ToString());
                        selected1 = -1;
                        sheet.Image = G.GetBitmap();
                    }
                }
            }
            //нажата кнопка "удалить элемент"
            if (deleteButton.Enabled == false)
            {
                bool flag = false; //удалили ли что-нибудь по ЭТОМУ клику
                //ищем, возможно была нажата вершина
                for (int i = 0; i < V.Count; i++)
                {
                    if (Math.Pow((V[i].x - e.X), 2) + Math.Pow((V[i].y - e.Y), 2) <= G.R * G.R)
                    {
                        for (int j = 0; j < E.Count; j++)
                        {
                            if ((E[j].v1 == i) || (E[j].v2 == i))
                            {
                                E.RemoveAt(j);
                                j--;
                            }
                            else
                            {
                                if (E[j].v1 > i) E[j].v1--;
                                if (E[j].v2 > i) E[j].v2--;
                            }
                        }
                        V.RemoveAt(i);
                        clearEdgesAllocate();
                        listBoxMatrix.Items.Clear();
                        flag = true;
                        break;
                    }
                }
                //ищем, возможно было нажато ребро
                if (!flag)
                {
                    for (int i = 0; i < E.Count; i++)
                    {
                        if (E[i].v1 == E[i].v2) //если это петля
                        {
                            if ((Math.Pow((V[E[i].v1].x - G.R - e.X), 2) + Math.Pow((V[E[i].v1].y - G.R - e.Y), 2) <= ((G.R + 2) * (G.R + 2))) &&
                                (Math.Pow((V[E[i].v1].x - G.R - e.X), 2) + Math.Pow((V[E[i].v1].y - G.R - e.Y), 2) >= ((G.R - 2) * (G.R - 2))))
                            {
                                E.RemoveAt(i);
                                clearEdgesAllocate();
                                listBoxMatrix.Items.Clear();
                                flag = true;
                                break;
                            }
                        }
                        else //не петля
                        {
                            if (((e.X - V[E[i].v1].x) * (V[E[i].v2].y - V[E[i].v1].y) / (V[E[i].v2].x - V[E[i].v1].x) + V[E[i].v1].y) <= (e.Y + 4) &&
                                ((e.X - V[E[i].v1].x) * (V[E[i].v2].y - V[E[i].v1].y) / (V[E[i].v2].x - V[E[i].v1].x) + V[E[i].v1].y) >= (e.Y - 4))
                            {
                                if ((V[E[i].v1].x <= V[E[i].v2].x && V[E[i].v1].x <= e.X && e.X <= V[E[i].v2].x) ||
                                    (V[E[i].v1].x >= V[E[i].v2].x && V[E[i].v1].x >= e.X && e.X >= V[E[i].v2].x))
                                {
                                    E.RemoveAt(i);
                                    clearEdgesAllocate();
                                    listBoxMatrix.Items.Clear();
                                    flag = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                //если что-то было удалено, то обновляем граф на экране
                if (flag)
                {
                    G.clearSheet();
                    G.drawALLGraph(V, E);
                    sheet.Image = G.GetBitmap();
                }
            }
        }

        //создание матрицы смежности и вывод в листбокс
        private void createAdjAndOut()
        {
            AMatrix = new int[V.Count, V.Count];
            G.fillAdjacencyMatrix(V.Count, E, AMatrix);
            listBoxMatrix.Items.Clear();
            string sOut = "    ";
            for (int i = 0; i < V.Count; i++)
                sOut += (i + 1) + " ";
            listBoxMatrix.Items.Add(sOut);
            for (int i = 0; i < V.Count; i++)
            {
                sOut = (i + 1) + " | ";
                for (int j = 0; j < V.Count; j++)
                    sOut += AMatrix[i, j] + " ";
                listBoxMatrix.Items.Add(sOut);
            }
        }

        //создание матрицы инцидентности и вывод в листбокс
        private void createIncAndOut()
        {
            if (E.Count > 0)
            {
                IMatrix = new int[V.Count, E.Count];
                G.fillIncidenceMatrix(V.Count, E, IMatrix);
                listBoxMatrix.Items.Clear();
                string sOut = "    ";
                for (int i = 0; i < E.Count; i++)
                    sOut += (char)('a' + i) + " ";
                listBoxMatrix.Items.Add(sOut);
                for (int i = 0; i < V.Count; i++)
                {
                    sOut = (i + 1) + " | ";
                    for (int j = 0; j < E.Count; j++)
                        sOut += IMatrix[i, j] + " ";
                    listBoxMatrix.Items.Add(sOut);
                }
            }
            else
                listBoxMatrix.Items.Clear();
        }

        //поиск элементарных цепей
        private void chainButton_Click(object sender, EventArgs e)
        {
            listBoxMatrix.Items.Clear();
            //1-white 2-black
            int[] color = new int[V.Count];
            for (int i = 0; i < V.Count - 1; i++)
                for (int j = i + 1; j < V.Count; j++)
                {
                    for (int k = 0; k < V.Count; k++)
                        color[k] = 1;
                    DFSchain(i, j, E, color, (i + 1).ToString());
                }
        }

        //обход в глубину. поиск элементарных цепей. (1-white 2-black)
        private void DFSchain(int u, int endV, List<Edge> E, int[] color, string s)
        {
            //вершину не следует перекрашивать, если u == endV (возможно в нее есть несколько путей)
            if (u != endV)  
                color[u] = 2;
            else
            {
                listBoxMatrix.Items.Add(s);
                return;
            }
            for (int w = 0; w < E.Count; w++)
            {
                if (color[E[w].v2] == 1 && E[w].v1 == u)
                {
                    DFSchain(E[w].v2, endV, E, color, s + "-" + (E[w].v2 + 1).ToString());
                    color[E[w].v2] = 1;
                }
                else if (color[E[w].v1] == 1 && E[w].v2 == u)
                {
                    DFSchain(E[w].v1, endV, E, color, s + "-" + (E[w].v1 + 1).ToString());
                    color[E[w].v1] = 1;
                }
            }
        }

        //поиск элементарных циклов
        private void cycleButton_Click(object sender, EventArgs e)
        {
            listBoxMatrix.Items.Clear();
            //1-white 2-black
            int[] color = new int[V.Count];
            for (int i = 0; i < V.Count; i++)
            {
                for (int k = 0; k < V.Count; k++)
                    color[k] = 1;
                List<int> cycle = new List<int>();
                cycle.Add(i + 1);
                DFScycle(i, i, E, color, -1, cycle);
            }
        }

        //обход в глубину. поиск элементарных циклов. (1-white 2-black)
        //Вершину, для которой ищем цикл, перекрашивать в черный не будем. Поэтому, для избежания неправильной
        //работы программы, введем переменную unavailableEdge, в которой будет хранится номер ребра, исключаемый
        //из рассмотрения при обходе графа. В действительности это необходимо только на первом уровне рекурсии,
        //чтобы избежать вывода некорректных циклов вида: 1-2-1, при наличии, например, всего двух вершин.

        private void DFScycle(int u, int endV, List<Edge> E, int[] color, int unavailableEdge, List<int> cycle)
        {
            //если u == endV, то эту вершину перекрашивать не нужно, иначе мы в нее не вернемся, а вернуться необходимо
            if (u != endV)
                color[u] = 2;
            else
            {
                if (cycle.Count >= 2)
                {
                    cycle.Reverse();
                    string s = cycle[0].ToString();
                    for (int i = 1; i < cycle.Count; i++)
                        s += "-" + cycle[i].ToString();
                    bool flag = false; //есть ли палиндром для этого цикла графа в листбоксе?
                    for (int i = 0; i < listBoxMatrix.Items.Count; i++)
                        if (listBoxMatrix.Items[i].ToString() == s)
                        {
                            flag = true;
                            break;
                        }
                    if (!flag)
                    {
                        cycle.Reverse();
                        s = cycle[0].ToString();
                        for (int i = 1; i < cycle.Count; i++)
                            s += "-" + cycle[i].ToString();
                        listBoxMatrix.Items.Add(s);
                    }
                    return;
                }
            }
            for (int w = 0; w < E.Count; w++)
            {
                if (w == unavailableEdge)
                    continue;
                if (color[E[w].v2] == 1 && E[w].v1 == u)
                {
                    List<int> cycleNEW = new List<int>(cycle);
                    cycleNEW.Add(E[w].v2 + 1);
                    DFScycle(E[w].v2, endV, E, color, w, cycleNEW);
                    color[E[w].v2] = 1;
                }
                else if (color[E[w].v1] == 1 && E[w].v2 == u)
                {
                    List<int> cycleNEW = new List<int>(cycle);
                    cycleNEW.Add(E[w].v1 + 1);
                    DFScycle(E[w].v1, endV, E, color, w, cycleNEW);
                    color[E[w].v1] = 1;
                }
            }
        }

        // ++
        // Событие для кнопки расчета
        private void matchings_Click(object sender, EventArgs e)
        {
            if (V.Count == 0)
            {
                MessageBox.Show("Введите вершину!");
                return;
            }    

            listBoxMatrix.Items.Clear();
            listBoxMatrix.ForeColor = Color.Black;

            List<Edge> matchingsList = findMaxMatchingVector(E);

            listBoxMatrix.Items.Add("Наибольшее паросочетание:");
            foreach (Edge edge in matchingsList)
            {
                listBoxMatrix.Items.Add(((char)('a' + edge.v1)).ToString() + " --- " + ((char)('a' + edge.v2)).ToString());
            }

            foreach (Edge edge in E)
            {
                edge.allocate = false;
                foreach (Edge edgeMatching in matchingsList)
                {
                    if (edge == edgeMatching)
                    {
                        edge.allocate = true;
                    }
                }
            }

            G.clearSheet();
            G.drawALLGraph(V, E);
            sheet.Image = G.GetBitmap();
        }

        // Поиск максимального соответствия
        private List<Edge> findMaxMatchingVector(List<Edge> E)
        {
            List<Edge> maxMatching = new List<Edge>();

            foreach (Edge currentEdge in E)
            {
                List<Edge> mathcingsList = findAllMatchings(E, currentEdge);

                List<Edge> newMaxMatching = findMaxMatchingFromList(mathcingsList);

                newMaxMatching.Add(currentEdge);

                if (newMaxMatching.Count > maxMatching.Count)
                {
                    maxMatching = newMaxMatching;
                }
            }
            return maxMatching;
        }

        // Поиск всех соответствий
        private List<Edge> findAllMatchings(List<Edge> E, Edge edge)
        {     
            List<Edge> matchingsList = new List<Edge> { };
            foreach (Edge currentEdge in E)
            {
                if (currentEdge == edge)
                {
                    continue;
                }

                List<Edge> checkEdges = new List<Edge> { currentEdge, edge };
                if (!hasMatchingPoints(checkEdges))
                {
                    matchingsList.Add(currentEdge);
                }

            }

            return matchingsList;
        }

        // Поиск наибольшего соответствия между ребер.
        private List<Edge> findMaxMatchingFromList(List<Edge> edges) 
        {
            if (!hasMatchingPoints(edges))
            {
                return edges;
            }

            List<Edge> maxMatchings = new List<Edge>();

            for (int i = 0; i < edges.Count; i++)
            {
                List<Edge> newEdges = new List<Edge>();
                for (int j = i + 1; j < edges.Count; j++)
                {
                    List<Edge> checkEdges = new List<Edge>{ edges[i], edges[j] };
                    
                    if (!hasMatchingPoints(checkEdges))
                    {
                        newEdges.Add(edges[j]);
                    }
                }

                if (newEdges.Count > 0)
                {
                    List<Edge> newMaxMatchings = findMaxMatchingFromList(newEdges);
                    newMaxMatchings.Add(edges[i]);
                    // Если количество не смежных ребер оказалось больше чем предыдущее
                    if (newMaxMatchings.Count > maxMatchings.Count)
                    {
                        maxMatchings = newMaxMatchings;
                    }
                    else
                    {
                        if (1 > maxMatchings.Count)
                            maxMatchings = new List<Edge>{ edges[i] };
                    }
                }
            }
            return maxMatchings;
        }

        // Проверка вершин ребра на сооветствие вершинами графа
        private bool hasMatchingPoints(List<Edge> edges)
        {
            for (int i = 0; i < edges.Count; i++)
            {
                for (int j = i + 1; j < edges.Count; j++)
                {
                    if (edges[i].v1 == edges[j].v1 ||
                        edges[i].v1 == edges[j].v2 ||
                        edges[i].v2 == edges[j].v1 ||
                        edges[i].v2 == edges[j].v2)
                        return true;
                }
            }
            return false;
        }

        // Очищение пометки для ребер.
        private void clearEdgesAllocate()
        {
            foreach (Edge edge in E)
            {
                edge.allocate = false;
            }

            G.clearSheet();
            G.drawALLGraph(V, E);
        }

        // Поиск расстояния между вершинами
        private double findDistance(Vertex v1, Vertex v2)
        {
            return Math.Sqrt((v2.x - v1.x) * (v2.x - v1.x) + (v2.y - v1.y) * (v2.y - v1.y));
        }

        // --


        //О программе
        private void about_Click(object sender, EventArgs e)
        {
            aboutForm FormAbout = new aboutForm();
            FormAbout.ShowDialog();
        }

        // Сохранение графа в файл через диалог сохранения
        private void saveButton_Click(object sender, EventArgs e)
        {
            if (sheet.Image != null)
            {
                SaveFileDialog savedialog = new SaveFileDialog();
                savedialog.Title = "Сохранить проект как...";
                savedialog.OverwritePrompt = true;
                savedialog.CheckPathExists = true;
                // savedialog.Filter = "Image Files(*.BMP)|*.BMP|Image Files(*.JPG)|*.JPG|Image Files(*.GIF)|*.GIF|Image Files(*.PNG)|*.PNG|All files (*.*)|*.*";
                savedialog.Filter = "Json files(*.json)|*.json";
                savedialog.ShowHelp = true;
                if (savedialog.ShowDialog() == DialogResult.OK)
                {
                    string fileNameForSave = savedialog.FileName;
                    bool isJSON = fileNameForSave.Contains(".json");   

                    if (isJSON)
                    {
                        try
                        {
                            GraphVE graphVE = new GraphVE(V, E);
                            string json = JsonSerializer.Serialize<GraphVE>(graphVE);
                            File.WriteAllText(savedialog.FileName, json);
                            this.filePath.Text = savedialog.FileName;
                            MessageBox.Show("Проект сохранен!");
                        }
                        catch
                        {
                            MessageBox.Show("Ошибка при сохранении файла JSON", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }   
                    else
                    {
                        try
                        {
                            sheet.Image.Save(savedialog.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                        }
                        catch
                        {
                            MessageBox.Show("Невозможно сохранить изображение", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Проект не сохранен!");
                }
            }
        }

        // Сохранеие графа в текущий открытий файл
        private void saveInCurrent_Click(object sender, EventArgs e)
        {
            if (sheet.Image != null)
            {
                if (filePath.Text == "")
                {
                    SaveFileDialog savedialog = new SaveFileDialog();
                    savedialog.Title = "Сохранить картинку как...";
                    savedialog.OverwritePrompt = true;
                    savedialog.CheckPathExists = true;
                    // savedialog.Filter = "Image Files(*.BMP)|*.BMP|Image Files(*.JPG)|*.JPG|Image Files(*.GIF)|*.GIF|Image Files(*.PNG)|*.PNG|All files (*.*)|*.*";
                    savedialog.Filter = "Json files(*.json)|*.json";
                    savedialog.ShowHelp = true;
                    if (savedialog.ShowDialog() == DialogResult.OK)
                    {
                        string fileNameForSave = savedialog.FileName;
                        bool isJSON = fileNameForSave.Contains(".json");

                        if (isJSON)
                        {
                            try
                            {
                                GraphVE graphVE = new GraphVE(V, E);
                                string json = JsonSerializer.Serialize<GraphVE>(graphVE);
                                File.WriteAllText(fileNameForSave, json);
                                filePath.Text = fileNameForSave;
                                MessageBox.Show("Проект сохранен!");
                            }
                            catch
                            {
                                MessageBox.Show("Ошибка при сохранении файла JSON", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            try
                            {
                                sheet.Image.Save(fileNameForSave, System.Drawing.Imaging.ImageFormat.Jpeg);
                            }
                            catch
                            {
                                MessageBox.Show("Невозможно сохранить изображение", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Проект не сохранен!");
                    }
                }
                else
                {
                    try
                    {
                        GraphVE graphVE = new GraphVE(V, E);
                        string json = JsonSerializer.Serialize<GraphVE>(graphVE);
                        File.WriteAllText(filePath.Text, json);
                        MessageBox.Show("Проект сохранен!");
                    }
                    catch
                    {
                        MessageBox.Show("Ошибка при сохранении файла JSON", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // Событие закрытия формы.
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.back)
                return;

            DialogResult res = (new CloseForm()).ShowDialog();

            if (res == DialogResult.Cancel)
            {
                e.Cancel = true;
            }
            else if (res == DialogResult.OK)
            {
                this.closeApp = true;
            }
            else if (res == DialogResult.No) 
            {
                this.closeApp = false;
            }
           
        }
        private void backInMainMenu_Click(object sender, EventArgs e)
        {
            this.back = true;
            this.closeApp = false;
            this.Close();
        }
    }
}
