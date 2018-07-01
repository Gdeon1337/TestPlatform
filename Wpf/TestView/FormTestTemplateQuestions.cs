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
    public partial class FormTestTemplateQuestions : Form
    {
        public List<PatternCategoryViewModel> listPC { get; set; }
        List<QuestionViewModel> listQ;

        private BindingSource source;
        public FormTestTemplateQuestions()
        {
            listPC = new List<PatternCategoryViewModel>();
            InitializeComponent();
            Initialize();
        }
        private void Initialize() {
            listQ = Task.Run(() => ApiClient.GetRequestData<List<QuestionViewModel>>("api/category/GetListQuestions/" + listPC[0].CategoryId)).Result;
            if (listPC != null)
            {
                dataGridViewCategories.DataSource = listPC;
                dataGridViewCategories.Columns[0].Visible = false;
                dataGridViewCategories.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            if (listPC[0].PatternQuestions != null)
            {
                dataGridViewTestQuestions.DataSource = listPC[0].PatternQuestions;
                dataGridViewTestQuestions.Columns[0].Visible = false;
                dataGridViewTestQuestions.Columns[1].Visible = false;
                dataGridViewTestQuestions.Columns[2].Visible = false;
                dataGridViewTestQuestions.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            if (listQ != null)
            {
                dataGridViewQuestions.DataSource = listQ;
                dataGridViewTestQuestions.Columns[0].Visible = false;
                dataGridViewTestQuestions.Columns[1].Visible = false;
                dataGridViewTestQuestions.Columns[3].Visible = false;
                dataGridViewTestQuestions.Columns[9].Visible = false;
                dataGridViewTestQuestions.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }
        //>
        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridViewCategories.SelectedRows.Count == 1)
            {
                int id = Convert.ToInt32(dataGridViewCategories.SelectedRows[0].Cells[0].Value);
                //listQ.Add(new PatternQuestionsBindingModel
                //{
                //    QuestionId = id


                //});
            }
        }
        //>>
        private void button5_Click(object sender, EventArgs e)
        {
            //for (int i = 0; i < listC.Count; i++) {
            //    listQ.Add(new PatternQuestionsBindingModel
            //    {
            //        QuestionId = listC[i].Id
            //    });
            //}
        }
        //<
        private void button6_Click(object sender, EventArgs e)
        {

            if (dataGridViewCategories.SelectedRows.Count == 1)
            {
                int id = Convert.ToInt32(dataGridViewCategories.SelectedRows[0].Cells[0].Value);
                listQ.RemoveAt(id);
            }
        }
        //<<
        private void button2_Click(object sender, EventArgs e)
        {
            listQ.Clear();
        }
        //save
        private void button3_Click(object sender, EventArgs e)
        {
            var form = new FormTestTemplate();
            //form.listQ = listQ;
            if (form.ShowDialog() == DialogResult.OK)
            {
                Close();
            }
        }
        //nazad
        private void button4_Click(object sender, EventArgs e)
        {
            Close();
        }

        // ПКМ -> Обновить
        private void MouseDown_Form(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(MousePosition);
            }
        }
        private void ОбновитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Initialize();
        }

        private void Form_Load(object sender, EventArgs e)
        {
            Initialize();
        }

        private void dataGridViewCategories_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
