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
        private string _username; // Ім'я поточного користувача
        private string _securityLevel; // Рівень безпеки користувача
        private string accessControlType; // Тип контролю доступу
        private string cs = ConfigurationManager.ConnectionStrings["dbtbdyaroshenko"].ConnectionString; // Рядок підключення до БД

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

            // Встановлюємо видимість кнопки налаштувань (тільки для адміністраторів)
            button_Settings.Visible = (_securityLevel == "Administrative");

            // Приховуємо GroupBox налаштувань на початку
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
                MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return securityLevel;
        }

        // Обробник події для кнопки менеджера файлів
        private void buttonFileManager_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_username))
            {
                MessageBox.Show("Username not specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            FileManager fileManager = new FileManager(_username, accessControlType);
            fileManager.Show();
        }

        // Обробник події для кнопки налаштувань
        private void button_Settings_Click(object sender, EventArgs e)
        {
            if (_securityLevel == "Administrative")
            {
                SettingMenu.Visible = true;
                LoadUsers();
                LoadFiles();
            }
            else
            {
                MessageBox.Show("Access to settings is denied.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Метод для завантаження списку користувачів
        private void LoadUsers()
        {
            try
            {
                comboBox_Users.Items.Clear();

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
                MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Метод для завантаження списку файлів
        private void LoadFiles()
        {
            try
            {
                comboBox_Files.Items.Clear();

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
                MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Метод для отримання паролю користувача
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
                MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return password;
        }

        // Обробник події для збереження нового рівня доступу користувача
        private void button_SaveUserLevel_Click_1(object sender, EventArgs e)
        {
            if (comboBox_Users.SelectedItem == null || comboBox_NewUserLevel.SelectedItem == null)
            {
                MessageBox.Show("Please select a user and new access level.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedUser = comboBox_Users.SelectedItem.ToString();
            string newLevel = comboBox_NewUserLevel.SelectedItem.ToString();

            if (newLevel == "Top Secret" || newLevel == "Secret")
            {
                string password = GetUserPassword(selectedUser);

                if (password.Length <= 10)
                {
                    MessageBox.Show("Password must be longer than 10 characters for this access level.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                            MessageBox.Show($"Access level for user {selectedUser} updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("User not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Обробник події для збереження нового рівня конфіденційності файлу
        private void button_SaveFileLevel_Click_1(object sender, EventArgs e)
        {
            if (comboBox_Files.SelectedItem == null || comboBox_NewFileLevel.SelectedItem == null)
            {
                MessageBox.Show("Please select a file and new confidentiality level.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                            MessageBox.Show($"Confidentiality level for file {selectedFile} updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("File not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Обробник події для виходу з системи
        private void button_LogOut_Click(object sender, EventArgs e)
        {
            Form1 loginForm = new Form1();
            loginForm.Show();
            this.Close();
        }

        private void comboBox_NewUserLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Обробник зміни вибраного рівня доступу користувача
        }

        private void MainWind_Load(object sender, EventArgs e)
        {
            // Обробник завантаження форми
        }
    }
}