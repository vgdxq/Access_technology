using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
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
        private string connectionString = ConfigurationManager.ConnectionStrings["dbtbdyaroshenko"].ConnectionString; // Підключення до БД
        private string selectedUser = ""; // Для збереження вибраного користувача
        private string userRole = ""; // Для збереження рівня доступу користувача

        // Додано властивість для зберігання ролі користувача
        public string UserRole { get; set; }

        public Form2()
        {
            InitializeComponent();
            panelFileActions.Visible = false; // Ховаємо панель дій на початку
            panelActions.Visible = false; // Ховаємо панель для рівнів доступу на початку
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            // Перевіряємо роль користувача і відображаємо кнопку button_settings лише для адміністратора
            if (UserRole == "Admin")
            {
                button_settings.Visible = true;
            }
            else
            {
                button_settings.Visible = false;
            }
        }

        // LOG OUT
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
        private void button_settings_Click(object sender, EventArgs e)
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
                fileConfidentiality = "Confidential";
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
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = "IF EXISTS (SELECT 1 FROM FILE_ACCESS WHERE FILE_NAME = @fileName) " +
                                      "UPDATE FILE_ACCESS SET CONFIDENTIALITY_LEVEL = @confidentiality WHERE FILE_NAME = @fileName " +
                                      "ELSE " +
                                      "INSERT INTO FILE_ACCESS (FILE_NAME, CONFIDENTIALITY_LEVEL) VALUES (@fileName, @confidentiality)";
                        SqlCommand cmd = new SqlCommand(query, connection);
                        cmd.Parameters.AddWithValue("@fileName", selectedFile);
                        cmd.Parameters.AddWithValue("@confidentiality", fileConfidentiality);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show($"Мітка конфіденційності для файлу \"{selectedFile}\" успішно змінена на: {fileConfidentiality}", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Не вдалося зберегти мітку конфіденційності.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Помилка при збереженні мітки конфіденційності: " + ex.Message, "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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
                    string query = "SELECT USERNAME FROM LOGIN_TBL";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        comboBoxUsers.Items.Clear(); // Очищаємо старі дані

                        while (reader.Read())
                        {
                            string username = reader["USERNAME"]?.ToString();
                            if (!string.IsNullOrEmpty(username))
                            {
                                comboBoxUsers.Items.Add(username);
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
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        // Отримуємо складність пароля користувача
                        string passwordComplexityQuery = "SELECT PASSWORD_COMPLEXITY FROM LOGIN_TBL WHERE USERNAME = @username";
                        SqlCommand passwordComplexityCmd = new SqlCommand(passwordComplexityQuery, connection);
                        passwordComplexityCmd.Parameters.AddWithValue("@username", selectedUser);
                        string passwordComplexity = passwordComplexityCmd.ExecuteScalar()?.ToString();

                        // Перевіряємо, чи пароль складний для високих рівнів доступу
                        if ((userRole == "Admin" || userRole == "Developer") && passwordComplexity != "High")
                        {
                            MessageBox.Show("Для призначення ролей 'Admin' або 'Developer' пароль користувача повинен бути складним.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // Оновлюємо роль користувача
                        string updateRoleQuery = "UPDATE LOGIN_TBL SET ROLE = @role WHERE USERNAME = @username";
                        SqlCommand updateRoleCmd = new SqlCommand(updateRoleQuery, connection);
                        updateRoleCmd.Parameters.AddWithValue("@role", userRole);
                        updateRoleCmd.Parameters.AddWithValue("@username", selectedUser);

                        int rowsAffected = updateRoleCmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show($"Рівень доступу для користувача \"{selectedUser}\" успішно змінено на: {userRole}", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Не вдалося зберегти рівень доступу.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Помилка при збереженні рівня доступу: " + ex.Message, "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Будь ласка, виберіть рівень доступу.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // FILE MANAGER
        private void buttonFileManager_Click_1(object sender, EventArgs e)
        {
            FileManager fileManagerForm = new FileManager(UserRole);  // Передаємо роль користувача
            fileManagerForm.Show();  // Відкриваємо FileManager
        }

        private void panelActions_Paint(object sender, PaintEventArgs e)
        {
        }
    }
}