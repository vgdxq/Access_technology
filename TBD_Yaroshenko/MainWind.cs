using System;
using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Security.AccessControl;

namespace TBD_Yaroshenko
{
    public partial class MainWind : Form
    {
        private string complexPattern = "(?=^.{10,}$)((?=.*\\d)|(?=.*\\W+))(?![.\\n])(?=.*[A-Z])(?=.*[a-z]).*$";
        private string _username; // Додаємо приватну змінну для зберігання імені користувача
        private string _securityLevel; // Додаємо змінну для зберігання рівня доступу
        private string accessControlType;
        private string cs = ConfigurationManager.ConnectionStrings["dbtbdyaroshenko"].ConnectionString;

        public MainWind()
        {
            InitializeComponent();
        }

        public MainWind(string username)
        {
            InitializeComponent();
            _username = username; // Зберігаємо ім'я користувача

            // Отримуємо рівень доступу користувача з бази даних
            _securityLevel = GetUserSecurityLevel(_username);

            // Встановлюємо видимість кнопки button_Settings
            button_Settings.Visible = (_securityLevel == "Administrative");

            // Приховуємо GroupBox на початку
            SettingMenu.Visible = false;

            // Заповнюємо ComboBox для рівнів доступу
            comboBox_NewUserLevel.Items.AddRange(new string[] { "Unclassified", "FOUO", "Confidential", "Secret", "Top Secret" });
            comboBox_NewFileLevel.Items.AddRange(new string[] { "Unclassified", "FOUO", "Confidential", "Secret", "Top Secret" });
        }

        // Метод для отримання рівня доступу користувача з бази даних
        private string GetUserSecurityLevel(string username)
        {
            string securityLevel = "Unclassified"; // Значення за замовчуванням

            try
            {
                using (SqlConnection con = new SqlConnection(cs))
                {
                    con.Open();
                    string query = "SELECT SECURITY_LEVEL FROM LOGIN_TBL WHERE USERNAME = @Username";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add(new SqlParameter("@Username", SqlDbType.VarChar, 50)).Value = username;

                        var result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            securityLevel = result.ToString();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Помилка бази даних: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return securityLevel;
        }

        private void buttonFileManager_Click(object sender, EventArgs e)
        {
            // Перевіряємо, чи ім'я користувача не є порожнім або null
            if (string.IsNullOrEmpty(_username))
            {
                MessageBox.Show("Ім'я користувача не вказано.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Відкриваємо форму FileManager і передаємо ім'я користувача
            FileManager fileManager = new FileManager(_username, accessControlType);
            fileManager.Show();
        }

        private void button_Settings_Click(object sender, EventArgs e)
        {
            // Перевіряємо, чи користувач є адміністратором
            if (_securityLevel == "Administrative")
            {
                // Показуємо GroupBox з налаштуваннями
                SettingMenu.Visible = true;

                // Завантажуємо користувачів та файли
                LoadUsers();
                LoadFiles();
            }
            else
            {
                MessageBox.Show("Доступ до налаштувань заборонено.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Метод для завантаження користувачів у comboBox_Users
        private void LoadUsers()
        {
            try
            {
                comboBox_Users.Items.Clear(); // Очищаємо список перед завантаженням

                using (SqlConnection con = new SqlConnection(cs))
                {
                    con.Open();
                    string query = "SELECT USERNAME FROM LOGIN_TBL";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                comboBox_Users.Items.Add(reader["USERNAME"].ToString());
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Помилка бази даних: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Метод для завантаження файлів у comboBox_Files
        private void LoadFiles()
        {
            try
            {
                comboBox_Files.Items.Clear(); // Очищаємо список перед завантаженням

                using (SqlConnection con = new SqlConnection(cs))
                {
                    con.Open();
                    string query = "SELECT FILE_NAME FROM FILE_ACCESS";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                comboBox_Files.Items.Add(reader["FILE_NAME"].ToString());
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Помилка бази даних: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private string GetUserPassword(string username)
        {
            string password = string.Empty;

            try
            {
                using (SqlConnection con = new SqlConnection(cs))
                {
                    con.Open();
                    string query = "SELECT PASS FROM LOGIN_TBL WHERE USERNAME = @Username";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add(new SqlParameter("@Username", SqlDbType.VarChar, 50)).Value = username;

                        var result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            password = result.ToString();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Помилка бази даних: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return password;
        }

        private void button_SaveUserLevel_Click_1(object sender, EventArgs e)
        {
            if (comboBox_Users.SelectedItem == null || comboBox_NewUserLevel.SelectedItem == null)
            {
                MessageBox.Show("Виберіть користувача та новий рівень доступу.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedUser = comboBox_Users.SelectedItem.ToString();
            string newLevel = comboBox_NewUserLevel.SelectedItem.ToString();

            // Перевіряємо, чи новий рівень є "Top Secret" або "Secret"
            if (newLevel == "Top Secret" || newLevel == "Secret")
            {
                // Отримуємо пароль користувача
                string password = GetUserPassword(selectedUser);

                // Перевіряємо довжину паролю
                if (password.Length <= 10)
                {
                    MessageBox.Show("Для встановлення цього рівня доступу пароль повинен бути довшим за 10 символів.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

        
            }

            try
            {
                using (SqlConnection con = new SqlConnection(cs))
                {
                    con.Open();
                    string query = "UPDATE LOGIN_TBL SET SECURITY_LEVEL = @NewLevel WHERE USERNAME = @Username";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add(new SqlParameter("@NewLevel", SqlDbType.VarChar, 20)).Value = newLevel;
                        cmd.Parameters.Add(new SqlParameter("@Username", SqlDbType.VarChar, 50)).Value = selectedUser;

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show($"Рівень доступу для користувача {selectedUser} успішно оновлено.", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Користувача не знайдено.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Помилка бази даних: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Обробник події для збереження нового CONFIDENTIALITY_LEVEL для файлу

        private void button_SaveFileLevel_Click_1(object sender, EventArgs e)
        {
            if (comboBox_Files.SelectedItem == null || comboBox_NewFileLevel.SelectedItem == null)
            {
                MessageBox.Show("Виберіть файл та новий рівень конфіденційності.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedFile = comboBox_Files.SelectedItem.ToString();
            string newLevel = comboBox_NewFileLevel.SelectedItem.ToString();

            try
            {
                using (SqlConnection con = new SqlConnection(cs))
                {
                    con.Open();
                    string query = "UPDATE FILE_ACCESS SET CONFIDENTIALITY_LEVEL = @NewLevel WHERE FILE_NAME = @FileName";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add(new SqlParameter("@NewLevel", SqlDbType.VarChar, 20)).Value = newLevel;
                        cmd.Parameters.Add(new SqlParameter("@FileName", SqlDbType.VarChar, 100)).Value = selectedFile;

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show($"Рівень конфіденційності для файлу {selectedFile} успішно оновлено.", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Файл не знайдено.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Помилка бази даних: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button_LogOut_Click(object sender, EventArgs e)
        {
            // Повертаємося до форми авторизації (Form1)
            Form1 loginForm = new Form1();
            loginForm.Show(); // Показуємо форму авторизації
            this.Close(); // Закриваємо поточну форму (MainWind)
        }

        private void comboBox_NewUserLevel_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void MainWind_Load(object sender, EventArgs e)
        {

        }

       
    }
}