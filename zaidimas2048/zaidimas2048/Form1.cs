using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace zaidimas2048
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Form zaidimas;
        Button[,] buttons;
        Random rand = new Random();
        private void bnt4x4_Click(object sender, EventArgs e)
        {
            this.Hide();
            zaidimas.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            createGameForm();
        }

        private void createGameForm()
        {
            zaidimas = new Form();
            zaidimas.Text = "4x4";
            zaidimas.StartPosition = FormStartPosition.Manual;
            zaidimas.Location = new Point(0, 0);
            zaidimas.Size = new Size(270, 400);
            zaidimas.BackColor = Color.LightPink;
            zaidimas.FormClosed += zaidimasColsed;
            zaidimas.KeyDown += KeyboardDown;

            buttons = new Button[4, 4];

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    buttons[i, j] = new Button();
                    buttons[i, j].Text = " ";
                    buttons[i, j].BackColor = Color.LightGray;
                    buttons[i, j].Size = new Size(50, 50);
                    buttons[i, j].Location = new Point(i*60+10,j*60+10);
                    buttons[i, j].Enabled = false;

                    zaidimas.Controls.Add(buttons[i,j]);
                }
            }

            addNewNum();
            addNewNum();
        }

        private void addNewNum()
        {
            if(isGameOver(4))
            {
                MessageBox.Show("GameOver");
                this.Show();
                zaidimas.Close();
                return;
            }
            int iLoc, jLoc;
            do
            {
                iLoc = rand.Next(0, 4);
                jLoc = rand.Next(0, 4);
            } while (buttons[iLoc, jLoc].Text != " ");
            buttons[iLoc, jLoc].Text = "2";
        }

        private bool isGameOver(int bSize)
        {

            for (int i = 0; i < bSize; i++)
            {
                for (int j = 0; j < bSize; j++)
                {
                    if(buttons[i, j].Text == " ")
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void KeyboardDown(object sender, KeyEventArgs e)
        {
            //MessageBox.Show(e.KeyValue.ToString());
            switch(e.KeyValue)
            {
                case 39:
                    moveRigth(4);
                    break;
                case 37:
                    moveLeft(4);
                    break;
                case 38:
                    moveUp(4);
                    break;
                case 40:
                    moveDown(4);
                    break;
            }
        }

        private void moveRigth(int bSize)
        {
            for (int j = 0; j <= 3; j++)
            {
                for (int i = bSize - 2; i >= 0; i--)
                {
                    if (buttons[i + 1, j].Text == " " && buttons[i, j].Text != " ")
                    {
                        buttons[i + 1, j].Text = buttons[i, j].Text;
                        buttons[i, j].Text = " ";
                        if (i < bSize - 2)
                        {

                            i += 2;
                        }
                    }
                    else if (buttons[i + 1, j].Text != " " && buttons[i + 1, j].Text == buttons[i, j].Text)
                    {
                        int a, b;
                        a = int.Parse(buttons[i + 1, j].Text);
                        b = int.Parse(buttons[i, j].Text);
                        buttons[i + 1, j].Text = (a + b).ToString();
                        buttons[i, j].Text = " ";
                    }
                }
            }
            addNewNum();
        }

        private void moveLeft(int bSize)
        {
            for (int j = 0; j <= 3; j++)
            {
                for (int i = 1; i < bSize; i++)
                {
                    if (buttons[i - 1, j].Text == " " && buttons[i, j].Text != " ")
                    {
                        buttons[i - 1, j].Text = buttons[i, j].Text;
                        buttons[i, j].Text = " ";
                        if (i > 1)
                        {

                            i -= 2;
                        }
                    }
                    else if (buttons[i - 1, j].Text != " " && buttons[i - 1, j].Text == buttons[i, j].Text)
                    {
                        int a, b;
                        a = int.Parse(buttons[i - 1, j].Text);
                        b = int.Parse(buttons[i, j].Text);
                        buttons[i - 1, j].Text = (a + b).ToString();
                        buttons[i, j].Text = " ";
                    }
                }
            }
            addNewNum();
        }

        private void moveDown(int bSize)
        {
            for (int i = 0; i <= 3; i++)
            {
                for (int j = bSize - 2; j >= 0; j--)
                {
                    if (buttons[i, j + 1].Text == " " && buttons[i, j].Text != " ")
                    {
                        buttons[i, j + 1].Text = buttons[i, j].Text;
                        buttons[i, j].Text = " ";
                        if (j < bSize - 2)
                        {

                            j += 2;
                        }
                    }
                    else if (buttons[i, j + 1].Text != " " && buttons[i, j + 1].Text == buttons[i, j].Text)
                    {
                        int a, b;
                        a = int.Parse(buttons[i, j + 1].Text);
                        b = int.Parse(buttons[i, j].Text);
                        buttons[i, j + 1].Text = (a + b).ToString();
                        buttons[i, j].Text = " ";
                    }
                }
            }
            addNewNum();
        }

        private void moveUp(int bSize)
        {
            for (int i = 0; i <= 3; i++)
            {
                for (int j = 1; j < bSize; j++)
                {
                    if (buttons[i, j - 1].Text == " " && buttons[i, j].Text != " ")
                    {
                        buttons[i, j - 1].Text = buttons[i, j].Text;
                        buttons[i, j].Text = " ";
                        if (j > 1)
                        {

                            j -= 2;
                        }
                    }
                    else if (buttons[i, j - 1].Text != " " && buttons[i, j - 1].Text == buttons[i, j].Text)
                    {
                        int a, b;
                        a = int.Parse(buttons[i, j - 1].Text);
                        b = int.Parse(buttons[i, j].Text);
                        buttons[i, j - 1].Text = (a + b).ToString();
                        buttons[i, j].Text = " ";
                    }
                }
            }
            addNewNum();
        }

        private void btnClick(object sender, EventArgs e)
        {
            MessageBox.Show(((Button)sender).Name);
        }
        
        private void zaidimasColsed(object sender, EventArgs e)
        {
            this.Show();
            createGameForm();
        }
    }
}
