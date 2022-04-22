using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LifeSimulation
{
    public partial class Form1 : Form
    {
        private int currentGeneratoin = 0;
        private Graphics graphics;
        private int resolution;
        private bool[,] field;
        private int rows;
        private int cols;

        public Form1()
        {
            InitializeComponent();
            Text = "Life Simulation";
        }

        private void StartGame()
        {
            if (timer1.Enabled)
                return;

            currentGeneratoin = 0;
            Text = $"Generation {currentGeneratoin}";

            nudDensity.Enabled = false;
            nudResolution.Enabled = false;
            resolution = (int)nudResolution.Value;
            rows = pictureBox1.Height / resolution;
            cols = pictureBox1.Width / resolution;
            //генерация первого поколения
            field = new bool[cols, rows];

            Random random = new Random();
            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    //генерация клетки
                    //если 0 - true, если 1 - false (чем меньше тру - тем меньше плотность)
                    field[x, y] = random.Next((int)nudDensity.Value) == 0;

                }
            }
            
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            graphics = Graphics.FromImage(pictureBox1.Image);
            //graphics.FillRectangle(Brushes.Crimson, 0, 0, resolution, resolution);
            timer1.Start();
        }

        private void StopGame()
        {
            if (!timer1.Enabled)
                return;
            timer1.Stop();

            nudDensity.Enabled = true;
            nudResolution.Enabled = true;
        }

        private void NextGeneration()
        {
            //цвет фона
            graphics.Clear(Color.Black);

            //генерация следующего поколения
            var newField = new bool[cols, rows];

            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    var neighboursCount = CountNeighbours(x, y);
                    //живая клетка по тек. координатам
                    var hasLife = field[x, y];

                    if (!hasLife && neighboursCount == 3)
                    {
                        //новая клетка
                        newField[x, y] = true;

                    }
                    else if (hasLife && neighboursCount < 2 || neighboursCount > 3)
                    {
                        //клетка погибает
                        newField[x, y] = false;
                    }
                    else
                    {
                        //ничего не меняется
                        newField[x, y] = field[x, y];
                    }

                    if (hasLife)
                    {
                        //генирируем игровое поле(рисуем квадраты)
                        graphics.FillRectangle(Brushes.Crimson, x * resolution, y * resolution, resolution, resolution);

                    }

                }
            }
            //новое поколение
            field = newField;
            //обновляем картинку
            pictureBox1.Refresh();
            Text = $"Generation {++currentGeneratoin}";
        }

        private int CountNeighbours(int x, int y)
        {
            int count = 0;

            //-1 узнаем о соседе слева, 2 - узнаем о соседе справа
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    //проходим по всем соседям
                    //cols) % cols; для случая корда координаты 0
                    var col = (x + i + cols) % cols;
                    var row = (y + j + rows) % rows;

                    //не включаем клетку вокруг которй идет расчет соседей
                    var isSelfChecking = col == x && row == y;
                    //считаем живых соседей
                    var hasLife = field[col, row];
                    if (hasLife && !isSelfChecking)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void bStart_Click(object sender, EventArgs e)
        {
            StartGame();
        }

        private void bStop_Click(object sender, EventArgs e)
        {
            StopGame();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!timer1.Enabled)
                return;
            //добавляем клетку левой кнопкой мыши
            if (e.Button == MouseButtons.Left)
            {
                var x = e.Location.X / resolution;
                var y = e.Location.Y / resolution;
                var validationPassed = ValidateMousePosition(x, y);
                if (validationPassed)
                {
                    field[x, y] = true;
                }
                
            }
            //удаляем клетку правой кнопкой мыши
            if (e.Button == MouseButtons.Right)
            {
                var x = e.Location.X / resolution;
                var y = e.Location.Y / resolution;
                var validationPassed = ValidateMousePosition(x, y);
                if (validationPassed)
                {
                    field[x, y] = false;
                }
            }
        }

        private bool ValidateMousePosition(int x, int y)
        {
            return x >= 0 && y >= 0 && x < cols && y < rows;
        }
    }
}
