﻿using MetroFramework.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TestService.ViewModels;

namespace TestView
{
    public partial class FormResultOfTest : MetroForm
    {
        private StatViewModel result;
        private static int border = 60;

        public FormResultOfTest(StatViewModel result)
        {
            this.result = result;
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;
            this.Style = MetroFramework.MetroColorStyle.Teal;
            ShadowType = MetroFormShadowType.DropShadow;
            Initialize();
        }

        private void Initialize()
        { 
           
                if (result!=null && result.StatCategories != null)
                {
                    dataGridView1.DataSource = result.StatCategories;
                    dataGridView1.Columns[0].Visible = false;
                    dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }

            labelMark.Text = result.Mark.ToString();

            if (result.Mark>border) {
                labelResult.Text = "Успешно";
                labelResult.ForeColor = Color.Green;
            }
            else
            {
                labelResult.Text = "Провален";
                labelResult.ForeColor = Color.Red;
            }

        }

        private void back_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void Form_Load(object sender, EventArgs e)
        {
            Initialize();
        }
    }
}
