using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TBD_Yaroshenko
{
    public partial class Form2 : Form
    {
        private string folderPath = @"C:\Users\Ярина\Desktop\NAU2025\TBD_Yaroshenko\Data"; // Шлях до папки
        private string selectedFile = ""; // Для збереження вибраного файлу
        private string fileConfidentiality = ""; // Для збереження мітки конфіденційності
        private string connectionString = ConfigurationManager.ConnectionStrings["dbtbdyaroshenko"].ConnectionString; //Підключення до БД
        private string selectedUser = ""; // Для збереження вибраного користувача
        private string userRole = ""; // Для збереження рівня доступу користувача


        public Form2()
        {
            InitializeComponent();
            panelFileActions.Visible = false; // Ховаємо панель дій на початку
            panelActions.Visible = false; // Ховаємо панель для рівнів доступу на початку
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        //LOG OUT
        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            var form1 = Application.OpenForms["Form1"];
            if (form1 != null)
            {
                form1.Show();
            }
            else
            {
                MessageBox.Show("Form1 is not open.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // MENU SETTINGS
        private void button3_Click(object sender, EventArgs e)
        {
            panelSettings.Visible = !panelSettings.Visible;
            panelSettings.BringToFront();
        }

        private void comboBoxFiles_DropDown(object sender, EventArgs e)
        {
            LoadFiles(); // Викликаємо метод для завантаження файлів
        }
        private void LoadFiles()
        {
            if (Directory.Exists(folderPath)) // Перевіряємо, чи існує папка
            {
                string[] files = Directory.GetFiles(folderPath); // Отримуємо список файлів
                comboBoxFiles.Items.Clear(); // Очищаємо список перед оновленням

                foreach (string file in files)
                {
                    comboBoxFiles.Items.Add(Path.GetFileName(file)); // Додаємо тільки назви файлів
                }
            }
            else
            {
                MessageBox.Show("Папка не знайдена!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void comboBoxFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxFiles.SelectedItem != null)
            {
                selectedFile = comboBoxFiles.SelectedItem?.ToString() ?? string.Empty; // Отримуємо вибране ім'я файлу
                labelSelectedFile.Text = "Вибрано: " + selectedFile; // Відображаємо назву
                panelFileActions.Visible = true; // Показуємо панель дій
                panelFileActions.BringToFront(); // Переміщуємо вперед
            }
        }


        private void radioButtonSecret_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonSecret.Checked)
            {
                fileConfidentiality = "Secret";
            }
        }

        private void radioButtonConfidential_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonConfidential.Checked)
            {
                fileConfidentiality = "Confidentialy";
            }
        }

        private void radioButtonPublic_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonPublic.Checked)
            {
                fileConfidentiality = "Public";
            }
        }

        private void btnSaveConfidentiality_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(fileConfidentiality))
            {
                MessageBox.Show($"Мітка конфіденційності для файлу \"{selectedFile}\" успішно змінена на: {fileConfidentiality}", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Будь ласка, виберіть мітку конфіденційності.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void comboBoxUsers_DropDown(object sender, EventArgs e)
        {
            LoadUsers(); // Завантажуємо користувачів з БД
        }
        private void LoadUsers()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT USERNAME FROM LOGIN_TBL"; // Заміни "Users" на назву твоєї таблиці, де зберігаються користувачі

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        comboBoxUsers.Items.Clear(); // Очищаємо старі дані

                        while (reader.Read())
                        {
                            string username = reader["USERNAME"]?.ToString();
                            if (!string.IsNullOrEmpty(username))
                            {
                                comboBoxUsers.Items.Add(username); // Додаємо користувачів в ComboBox
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка при завантаженні користувачів: " + ex.Message);
            }
        }

        private void comboBoxUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxUsers.SelectedItem != null)
            {
                selectedUser = comboBoxUsers.SelectedItem?.ToString() ?? string.Empty; // Зберігаємо вибраного користувача
                labelSelectedUser.Text = "Вибрано користувача: " + selectedUser; // Відображаємо вибраного користувача
                panelActions.Visible = true; // Показуємо панель для рівнів доступу
                panelActions.BringToFront(); // Переміщаємо панель на передній план
            }
        }



        private void radioButtonAdmin_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonAdmin.Checked)
            {
                userRole = "Admin";
            }
        }

        private void radioButtonDeveloper_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonDeveloper.Checked)
            {
                userRole = "Developer";
            }
        }

        private void radioButtonUser_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonUser.Checked)
            {
                userRole = "User";
            }
        }

        private void btnSaveRole_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(userRole))
            {
                MessageBox.Show($"Рівень доступу для користувача \"{selectedUser}\" успішно змінено на: {userRole}", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Будь ласка, виберіть рівень доступу.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //FILE MANAGER
        private void buttonFileManager_Click_1(object sender, EventArgs e)
        {
            FileManager fileManagerForm = new FileManager();  // Створюємо нове вікно
            fileManagerForm.Show();  // Відкриваємо його
        }

        private void panelActions_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
