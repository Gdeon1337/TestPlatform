﻿using MetroFramework.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using TestService.BindingModels;
using TestService.ViewModels;

namespace TestView
{
    public partial class FormUserEdit : MetroForm
    {
        public string Id { set { id = value; } }

        private string id;

        public FormUserEdit()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;
            this.Style = MetroFramework.MetroColorStyle.Teal;
            ShadowType = MetroFormShadowType.DropShadow;
            if (FormStatisticsMain.DarkTheme)
            {
                Theme = MetroFramework.MetroThemeStyle.Dark;
                label1.ForeColor = Color.White;
                label2.ForeColor = Color.White;
                label3.ForeColor = Color.White;
                label4.ForeColor = Color.White;
                label5.ForeColor = Color.White;            
            }
            else
            {
                Theme = MetroFramework.MetroThemeStyle.Light;
                label1.ForeColor = Color.Black;
                label2.ForeColor = Color.Black;
                label3.ForeColor = Color.Black;
                label4.ForeColor = Color.Black;
                label5.ForeColor = Color.Black;
            }
        }

        private async void Initialize() {
            try
            {
                var groups = await ApiClient.GetRequestData<List<GroupViewModel>>("api/Group/GetList");
                comboBoxGroups.DisplayMember = "Name";
                comboBoxGroups.ValueMember = "Id";
                comboBoxGroups.DataSource = groups;
                comboBoxGroups.SelectedItem = null;
            }
            catch(Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (!string.IsNullOrEmpty(id))
            {
                try
                {
                    var user = await ApiClient.GetRequestData<UserViewModel>("api/User/Get/" + id);
                    textBoxFIO.Text = user.FIO;
                    textBoxUserName.Text = user.UserName;
                    textBoxEmail.Text = user.Email;
                    if(user.GroupId !=null) {
                        comboBoxGroups.SelectedValue = user.GroupId;
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
        }
        //сохранить
        private async void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxFIO.Text))
            {
                MessageBox.Show("Заполните ФИО", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrEmpty(textBoxUserName.Text))
            {
                MessageBox.Show("Заполните Логин", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if(string.IsNullOrEmpty(id) && string.IsNullOrEmpty(textBoxPassword.Text))
            {
                MessageBox.Show("Заполните Пароль", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            string fio = textBoxFIO.Text;
            string username = textBoxUserName.Text;
            string password = textBoxPassword.Text;
            string mail = textBoxEmail.Text;
            if (!string.IsNullOrEmpty(mail))
            {
                if (!Regex.IsMatch(mail, @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$"))
                {
                    MessageBox.Show("Неверный формат для электронной почты", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    if (comboBoxGroups.SelectedValue != null)
                    {
                        int groupId = Convert.ToInt32(comboBoxGroups.SelectedValue);
                        await ApiClient.PostRequestData("api/User/UpdElement", new UserBindingModel
                        {
                            Id = id,
                            UserName = username,
                            Email = mail,
                            GroupId = groupId,
                            FIO = fio,
                            PasswordHash = password
                        });
                    }
                    else
                    {
                        await ApiClient.PostRequestData("api/User/UpdElement", new UserBindingModel
                        {
                            Id = id,
                            UserName = username,
                            Email = mail,
                            FIO = fio,
                            PasswordHash = password
                        });
                    }
                }
                else
                {
                    if (comboBoxGroups.SelectedValue != null)
                    {
                        int groupId = Convert.ToInt32(comboBoxGroups.SelectedValue);
                        await ApiClient.PostRequestData("api/User/AddElement", new UserBindingModel
                        {
                            UserName = username,
                            Email = mail,
                            GroupId = groupId,
                            FIO = fio,
                            PasswordHash = password
                        });
                    }
                    else
                    {
                        await ApiClient.PostRequestData("api/User/AddElement", new UserBindingModel
                        {
                            UserName = username,
                            Email = mail,
                            FIO = fio,
                            PasswordHash = password
                        });
                    }

                }
                MessageBox.Show("Сохранение прошло успешно. Обновите список", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Close();
        }
        //cancel
        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FormUserEdit_Load(object sender, EventArgs e)
        {
            Initialize();
        }
    }
}
