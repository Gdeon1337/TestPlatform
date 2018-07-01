﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TestService.BindingModels;
using TestService.ViewModels;

namespace TestView
{
    public partial class FormTestTemplate : Form
    {
        public int Id { set { id = value; } }

        private int? id;

        private int idCat;

        private int check;
        private List<CategoryViewModel> listC;
        private List<PatternCategoryViewModel> listPC;
        public List<PatternQuestionsBindingModel> listQ { get; set; }
        public FormTestTemplate()
        {
            InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            List<GroupViewModel> list =
                    Task.Run(() => ApiClient.GetRequestData<List<GroupViewModel>>("api/Group/GetList")).Result;
            if (list != null)
            {
                comboBox1.DataSource = list;
                comboBox1.ValueMember = "Id";
                comboBox1.DisplayMember = "Name";
                comboBox1.SelectedItem = null;
            }
            if (id.HasValue)
            {
                try
                {
                    var pattern = Task.Run(() => ApiClient.GetRequestData<PatternViewModel>("api/Pattern/Get/" + id.Value)).Result;
                    textBox1.Text = pattern.Name;
                    listPC = pattern.PatternCategories;
                    comboBox1.SelectedItem = pattern.UserGroupId;
                    if (pattern.PatternCategories.Count > 0)
                    {
                        dataGridView2.Rows[0].Selected = true;
                        textBoxEasy.Text = pattern.PatternCategories[0].Easy.ToString();
                        textBoxMid.Text = pattern.PatternCategories[0].Middle.ToString();
                        textBoxDif.Text = pattern.PatternCategories[0].Complex.ToString();
                        textBoxCount.Text = pattern.PatternCategories[0].Count.ToString();
                    }
                }
                catch (Exception ex)
                {
                    while (ex.InnerException != null)
                    {
                        ex = ex.InnerException;
                    }
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                listPC = new List<PatternCategoryViewModel>();
            }
            dataGridView2.DataSource = listPC;
            dataGridView2.Columns[0].Visible = false;
            dataGridView2.Columns[2].Visible = false;
            dataGridView2.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            listC = Task.Run(() => ApiClient.GetRequestData<List<CategoryViewModel>>("api/Category/GetList")).Result;
            if (listC != null)
            {
                dataGridView1.DataSource = listC;
                dataGridView1.Columns[0].Visible = false;
                dataGridView1.Columns[2].Visible = false;

                dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }
        //сохранить
        private void button3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("Введите название", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            SaveCategory();
            Task task;
            string name = textBox1.Text;
            if (id.HasValue)
            {
                List<PatternCategoriesBindingModel> bin = new List<PatternCategoriesBindingModel>(listPC.Count);

                for (int i = 0; i < listPC.Count; i++)
                {
                    bin.Add(new PatternCategoriesBindingModel
                    {
                        PatternId = id.Value,
                        CategoryId = listPC[i].CategoryId,
                        Middle = listPC[i].Middle,
                        Copmlex = listPC[i].Complex,
                        Easy = listPC[i].Easy
                    });
                }
                if (listQ.Count != 0)
                {

                    for (int i = 0; i < listQ.Count; i++)
                    {
                        listQ[i].PatternId = id.Value;
                    }
                    task = Task.Run(() => ApiClient.PostRequestData("api/Pattern/UpdElement", new PatternBindingModel
                    {
                        Id = id.Value,
                        Name = name,
                        PatternCategories = bin,
                        PatternQuestions = listQ


                    }));
                }
                else
                {
                    task = Task.Run(() => ApiClient.PostRequestData("api/Pattern/UpdElement", new PatternBindingModel
                    {
                        Id = id.Value,
                        Name = name,
                        PatternCategories = bin


                    }));
                }
            }
            else
            {

                List<PatternCategoriesBindingModel> bin = new List<PatternCategoriesBindingModel>(listPC.Count);

                for (int i = 0; i < listPC.Count; i++)
                {
                    bin.Add(new PatternCategoriesBindingModel
                    {
                        CategoryId = listPC[i].CategoryId,
                        Middle = listPC[i].Middle,
                        Copmlex = listPC[i].Complex,
                        Easy = listPC[i].Easy

                    });
                }
                if (listQ.Count != 0)
                {
                    task = Task.Run(() => ApiClient.PostRequestData("api/Pattern/AddElement", new PatternBindingModel
                    {
                        Name = name,
                        PatternCategories = bin,
                        PatternQuestions = listQ

                    }));
                }
                else
                {
                    task = Task.Run(() => ApiClient.PostRequestData("api/Pattern/AddElement", new PatternBindingModel
                    {
                        Name = name,
                        PatternCategories = bin

                    }));
                }

            }
            task.ContinueWith((prevTask) => MessageBox.Show("Сохранение прошло успешно. Обновите список", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information),
                TaskContinuationOptions.OnlyOnRanToCompletion);
            task.ContinueWith((prevTask) =>
            {
                var ex = (Exception)prevTask.Exception;
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }, TaskContinuationOptions.OnlyOnFaulted);

            Close();

        }
        //отмена 
        private void button4_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            var form = new FormTestTemplateQuestions();
            form.listQ = listQ;
            if (form.ShowDialog() == DialogResult.OK)
            {
                Initialize();
            }
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView2.SelectedRows.Count == 1)
            {
                idCat = Convert.ToInt32(dataGridView2.SelectedRows[0].Cells[0].Value);
                textBoxEasy.Text = listPC[idCat].Easy.ToString();
                textBoxMid.Text = listPC[idCat].Middle.ToString();
                textBoxDif.Text = listPC[idCat].Complex.ToString();
                textBoxCount.Text = listPC[idCat].Count.ToString();


            }
        }

        private int Sum()
        {
            int easy = 0;
            int mid = 0;
            int complex = 0;
            if ((!string.IsNullOrEmpty(textBoxEasy.Text) && !Int32.TryParse(textBoxEasy.Text, out easy)) || easy < 0 ||
                (!string.IsNullOrEmpty(textBoxMid.Text) && !Int32.TryParse(textBoxMid.Text, out mid)) || mid < 0 ||
                (!string.IsNullOrEmpty(textBoxDif.Text) && !Int32.TryParse(textBoxDif.Text, out complex)) || complex < 0)
            {
                MessageBox.Show("Неверные значения", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
            int sum = easy + mid + complex;
            textBoxCount.Text = sum.ToString();
            return sum;
        }
        private void textBoxEasy_TextChanged(object sender, EventArgs e)
        {
            Sum();
        }

        private void textBoxDif_TextChanged(object sender, EventArgs e)
        {
            Sum();
        }

        private void textBoxMid_TextChanged(object sender, EventArgs e)
        {
            Sum();
        }
        // >
        private void button1_Click(object sender, EventArgs e)
        {

            if (dataGridView1.SelectedRows.Count == 1)
            {
                int Id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells[0].Value);
                if (id.HasValue)
                {
                    listPC.Add(new PatternCategoryViewModel
                    {
                        PatternId = id.Value,
                        CategoryId = Id,
                        CategoryName = listC.FirstOrDefault(rec=>rec.Id == Id).Name


                    });
                }
                else
                {
                    listPC.Add(new PatternCategoryViewModel
                    {
                        CategoryId = Id,
                        CategoryName = listC.FirstOrDefault(rec => rec.Id == Id).Name
                    });
                }
            }
        }
        // >>
        private void button5_Click(object sender, EventArgs e)
        {
            if (id.HasValue)
            {
                for (int i = 0; i < listC.Count; i++)
                {
                    listPC.Add(new PatternCategoryViewModel
                    {
                        PatternId = id.Value,
                        CategoryId = listC[i].Id,
                        CategoryName = listC[i].Name


                    });
                }
            }
            else
            {
                for (int i = 0; i < listC.Count; i++)
                {
                    listPC.Add(new PatternCategoryViewModel
                    {
                        CategoryId = listC[i].Id,
                        CategoryName = listC[i].Name
                    });
                }
            }
        }
        // <
        private void button6_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count == 1)
            {
                int Id = Convert.ToInt32(dataGridView2.SelectedRows[0].Cells[0].Value);
                listPC.Remove(listPC.FirstOrDefault(rec => rec.Id == Id));
                
            }
        }
        // <<
        private void button2_Click(object sender, EventArgs e)
        {
            listPC.Clear();
        }

        // ПКМ -> Обновить
        private void MouseDown_Form(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(MousePosition);
            }
        }
        private void обновитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Initialize();
        }

        //Сохранить категорию в шаблон 
        private void button8_Click(object sender, EventArgs e)
        {
            SaveCategory();
        }

        private void SaveCategory()
        {
            if (dataGridView1.SelectedRows.Count == 1)
            {
                int count = Sum();
                if (Convert.ToInt32(textBoxCount.Text) <= 0 || count < 0)
                {
                    MessageBox.Show("Заполните количество вопросов", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;

                }
                var element = listPC.FirstOrDefault(rec => rec.Id == Convert.ToInt32(dataGridView2.SelectedRows[0].Cells[0].Value));
                element.Count = count;
                element.Easy = Convert.ToInt32(textBoxEasy.Text);
                element.Middle = Convert.ToInt32(textBoxMid.Text);
                element.Complex = Convert.ToInt32(textBoxDif.Text);
            }
        }

        private void Form_Load(object sender, EventArgs e)
        {
            Initialize();
        }
    }
}
