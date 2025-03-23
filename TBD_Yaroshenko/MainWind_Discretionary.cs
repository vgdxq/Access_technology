using System;
using System.Configuration;
using System.Data;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace TBD_Yaroshenko
{
    public partial class MainWind_Discretionary : Form
    {
        private string _username = string.Empty; // Ініціалізація поля
        private string _securityLevel = string.Empty; // Ініціалізація поля
        private string _accessControlType = string.Empty; // Додано поле для типу контролю доступу
        private readonly string connectionString;

        public MainWind_Discretionary(string username, string accessControlType)
        {
            InitializeComponent();
            _username = username ?? throw new ArgumentNullException(nameof(username)); // Перевірка на null
            _accessControlType = accessControlType ?? throw new ArgumentNullException(nameof(accessControlType)); // Перевірка на null
            connectionString = ConfigurationManager.ConnectionStrings["dbtbdyaroshenko"]?.ConnectionString ?? throw new InvalidOperationException("Connection string not found.");

            LoadUserData(); // Завантажуємо дані користувача
            LoadUsersIntoComboBox(); // Завантажуємо список користувачів
            LoadFilesIntoComboBox(); // Завантажуємо список файлів для поточного користувача

            // Додаємо обробник події для кнопки передачі прав
            button_GrantAccess.Click += Button_GrantAccess_Click;

            if (!IsUserOwnerOfAnyFile())
            {
                // Якщо користувач не є власником, приховуємо SettingMenu
                SettingMenu.Visible = false;
            }
            else
            {
                // Якщо користувач є власником, показуємо SettingMenu
                SettingMenu.Visible = true;
            }
        }

        private bool IsUserOwnerOfAnyFile()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "SELECT COUNT(*) FROM USER_FILE_ACCESS WHERE USERNAME = @Username AND OWN = 1";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Username", _username);
                        int count = (int)cmd.ExecuteScalar();

                        // Якщо count > 0, користувач є власником хоча б одного файлу
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при перевірці прав власника: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        private void LoadUserData()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "SELECT * FROM USER_FILE_ACCESS WHERE USERNAME = @Username";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Username", _username);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string fileName = reader["FILE_NAME"]?.ToString() ?? string.Empty;
                                bool canRead = reader["CAN_READ"] != DBNull.Value && (bool)reader["CAN_READ"];
                                bool canWrite = reader["CAN_WRITE"] != DBNull.Value && (bool)reader["CAN_WRITE"];
                                bool canExecute = reader["CAN_EXECUTE"] != DBNull.Value && (bool)reader["CAN_EXECUTE"];
                                // Використовуйте ці дані для налаштування інтерфейсу
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при завантаженні даних: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Завантаження списку користувачів у comboBox_Users
        private void LoadUsersIntoComboBox()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "SELECT USERNAME FROM LOGIN_TBL";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            comboBox_Users.Items.Clear(); // Очищаємо список перед завантаженням
                            while (reader.Read())
                            {
                                string username = reader["USERNAME"]?.ToString() ?? string.Empty;
                                comboBox_Users.Items.Add(username); // Додаємо користувача до comboBox
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при завантаженні списку користувачів: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Завантаження списку файлів у comboBox_Files (лише для власника)
        private void LoadFilesIntoComboBox()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "SELECT FILE_NAME FROM USER_FILE_ACCESS WHERE USERNAME = @Username AND OWN = 1";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Username", _username);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            comboBox_Files.Items.Clear(); // Очищаємо список перед завантаженням
                            while (reader.Read())
                            {
                                string fileName = reader["FILE_NAME"]?.ToString() ?? string.Empty;
                                comboBox_Files.Items.Add(fileName); // Додаємо файл до comboBox
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при завантаженні списку файлів: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Обробник події для передачі прав
        private void Button_GrantAccess_Click(object sender, EventArgs e)
        {
            string selectedUser = comboBox_Users.SelectedItem?.ToString();
            string selectedFile = comboBox_Files.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(selectedUser) || string.IsNullOrEmpty(selectedFile))
            {
                MessageBox.Show("Будь ласка, виберіть користувача та файл.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool canRead = checkBox_Reading.Checked;
            bool canWrite = checkBox_Writing.Checked;
            bool canExecute = checkBox_Execute.Checked;

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // Перевіряємо, чи користувач вже має права на цей файл
                    string checkQuery = "SELECT COUNT(*) FROM USER_FILE_ACCESS WHERE USERNAME = @Username AND FILE_NAME = @FileName";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, con))
                    {
                        checkCmd.Parameters.AddWithValue("@Username", selectedUser);
                        checkCmd.Parameters.AddWithValue("@FileName", selectedFile);
                        int count = (int)checkCmd.ExecuteScalar();

                        if (count > 0)
                        {
                            // Оновлюємо існуючі права
                            string updateQuery = "UPDATE USER_FILE_ACCESS SET CAN_READ = @CanRead, CAN_WRITE = @CanWrite, CAN_EXECUTE = @CanExecute WHERE USERNAME = @Username AND FILE_NAME = @FileName";
                            using (SqlCommand updateCmd = new SqlCommand(updateQuery, con))
                            {
                                updateCmd.Parameters.AddWithValue("@CanRead", canRead);
                                updateCmd.Parameters.AddWithValue("@CanWrite", canWrite);
                                updateCmd.Parameters.AddWithValue("@CanExecute", canExecute);
                                updateCmd.Parameters.AddWithValue("@Username", selectedUser);
                                updateCmd.Parameters.AddWithValue("@FileName", selectedFile);
                                updateCmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            // Додаємо нові права
                            string insertQuery = "INSERT INTO USER_FILE_ACCESS (USERNAME, FILE_NAME, CAN_READ, CAN_WRITE, CAN_EXECUTE, OWN) VALUES (@Username, @FileName, @CanRead, @CanWrite, @CanExecute, 0)";
                            using (SqlCommand insertCmd = new SqlCommand(insertQuery, con))
                            {
                                insertCmd.Parameters.AddWithValue("@Username", selectedUser);
                                insertCmd.Parameters.AddWithValue("@FileName", selectedFile);
                                insertCmd.Parameters.AddWithValue("@CanRead", canRead);
                                insertCmd.Parameters.AddWithValue("@CanWrite", canWrite);
                                insertCmd.Parameters.AddWithValue("@CanExecute", canExecute);
                                insertCmd.ExecuteNonQuery();
                            }
                        }
                    }
                }

                MessageBox.Show("Права успішно надано.", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при наданні прав: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button_FileManager_Click(object sender, EventArgs e)
        {
            // Перевіряємо, чи користувач має доступ до файлів
            if (CheckUserHasAccess())
            {
                // Якщо доступ є, відкриваємо форму FileManager
                FileManager fileManager = new FileManager(_username, "Discretionary");
                fileManager.Show();
            }
            else
            {
                // Якщо доступу немає, показуємо повідомлення
                MessageBox.Show("No one has given you access to the files yet. Contact the resource owners.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Метод для перевірки доступу до файлів
        private bool CheckUserHasAccess()
        {
            var accessibleFiles = new List<string>();

            string query = "SELECT FILE_NAME FROM USER_FILE_ACCESS WHERE USERNAME = @username AND CAN_READ = 1";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@username", _username);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        accessibleFiles.Add(reader.GetString(0));
                    }
                }
            }

            return accessibleFiles.Count > 0; // Повертає true, якщо є доступні файли
        }

        private void button_LogOut_Click(object? sender, EventArgs e)
        {
            // Логіка для виходу з системи
            this.Close(); // Закриваємо поточну форму
            Form1 loginForm = new Form1(); // Повертаємося до форми авторизації
            loginForm.Show();
        }
    }
}