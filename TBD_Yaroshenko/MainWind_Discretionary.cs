using System;
using System.Configuration;
using System.Data;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace TBD_Yaroshenko
{
    public partial class MainWind_Discretionary : Form
    {
        private string _username = string.Empty; // Ім'я поточного користувача
        private string _accessControlType = string.Empty; // Тип контролю доступу (дискреційний/мандатний)
        private readonly string connectionString; // Рядок підключення до БД

        public MainWind_Discretionary(string username, string accessControlType)
        {
            InitializeComponent();
            _username = username ?? throw new ArgumentNullException(nameof(username)); // Перевірка на null
            _accessControlType = accessControlType ?? throw new ArgumentNullException(nameof(accessControlType)); // Перевірка на null
            connectionString = ConfigurationManager.ConnectionStrings["dbtbdyaroshenko"]?.ConnectionString ?? throw new InvalidOperationException("Connection string not found.");

            LoadUserData(); // Завантаження даних користувача
            LoadUsersIntoComboBox(); // Завантаження списку користувачів
            LoadFilesIntoComboBox(); // Завантаження списку файлів для поточного користувача
            comboBox_Users.SelectedIndexChanged += ComboBox_UsersOrFiles_SelectedIndexChanged;
            comboBox_Files.SelectedIndexChanged += ComboBox_UsersOrFiles_SelectedIndexChanged;

            // Обробник події для кнопки передачі прав
            button_GrantAccess.Click += Button_GrantAccess_Click;

            if (!IsUserOwnerOfAnyFile())
            {
                // Приховуємо меню налаштувань, якщо користувач не є власником файлів
                SettingMenu.Visible = false;
            }
            else
            {
                // Показуємо меню налаштувань для власника
                SettingMenu.Visible = true;
            }
        }

        // Перевірка, чи є користувач власником хоча б одного файлу
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

                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking owner rights: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // Завантаження даних користувача
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
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading user data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Завантаження списку користувачів у комбобокс
        private void LoadUsersIntoComboBox()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "SELECT USERNAME FROM LOGIN_TBL WHERE USERNAME <> @CurrentUser";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@CurrentUser", _username);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            comboBox_Users.Items.Clear();
                            while (reader.Read())
                            {
                                string username = reader["USERNAME"]?.ToString() ?? string.Empty;
                                comboBox_Users.Items.Add(username);
                            }
                        }
                    }
                }

                if (comboBox_Users.Items.Count > 0)
                {
                    comboBox_Users.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users list: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Завантаження списку файлів у комбобокс (лише для власника)
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
                            comboBox_Files.Items.Clear();
                            while (reader.Read())
                            {
                                string fileName = reader["FILE_NAME"]?.ToString() ?? string.Empty;
                                comboBox_Files.Items.Add(fileName);
                            }
                        }
                    }
                }

                if (comboBox_Files.Items.Count > 0)
                {
                    comboBox_Files.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading files: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Обробник події для передачі прав доступу
        private void Button_GrantAccess_Click(object sender, EventArgs e)
        {
            string selectedUser = comboBox_Users.SelectedItem?.ToString();
            string selectedFile = comboBox_Files.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(selectedUser) || string.IsNullOrEmpty(selectedFile))
            {
                MessageBox.Show("Please select both user and file.", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool canRead = checkBox_Reading.Checked;
            bool canWrite = checkBox_Writing.Checked;
            bool canExecute = IsExecutableFile(selectedFile) && checkBox_Execute.Checked;

            // Check password complexity if granting execute permission
            if (canExecute)
            {
                string complexity = GetUserPasswordComplexity(selectedUser);
                if (complexity != "High")
                {
                    MessageBox.Show("Execute permission can only be granted to users with High password complexity.\n" +
                                   $"User '{selectedUser}' has {complexity} complexity password.",
                                   "Permission Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    checkBox_Execute.Checked = false;
                    return;
                }
            }

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string checkQuery = "SELECT COUNT(*) FROM USER_FILE_ACCESS WHERE USERNAME = @Username AND FILE_NAME = @FileName";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, con))
                    {
                        checkCmd.Parameters.AddWithValue("@Username", selectedUser);
                        checkCmd.Parameters.AddWithValue("@FileName", selectedFile);
                        int count = (int)checkCmd.ExecuteScalar();

                        if (count > 0)
                        {
                            string updateQuery = @"UPDATE USER_FILE_ACCESS 
                                 SET CAN_READ = @CanRead, 
                                     CAN_WRITE = @CanWrite, 
                                     CAN_EXECUTE = @CanExecute 
                                 WHERE USERNAME = @Username 
                                 AND FILE_NAME = @FileName";
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
                            string insertQuery = @"INSERT INTO USER_FILE_ACCESS 
                                 (USERNAME, FILE_NAME, CAN_READ, CAN_WRITE, CAN_EXECUTE, OWN) 
                                 VALUES (@Username, @FileName, @CanRead, @CanWrite, @CanExecute, 0)";
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

                MessageBox.Show("Access rights granted successfully.", "Success",
                               MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error granting access rights: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Обробник події для кнопки менеджера файлів
        private void button_FileManager_Click(object sender, EventArgs e)
        {
            if (!CheckUserHasAnyAccess())
            {
                MessageBox.Show("You don't have access to any files.", "Access denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (FileManager fileManager = new FileManager(_username, "Discretionary"))
            {
                fileManager.ShowDialog();
                fileManager.Dispose();
            }
        }

        // Перевірка, чи є у користувача доступ до будь-яких файлів
        private bool CheckUserHasAnyAccess()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"SELECT COUNT(*) 
                        FROM USER_FILE_ACCESS 
                        WHERE USERNAME = @username 
                        AND (CAN_READ = 1 OR CAN_EXECUTE = 1)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", _username);
                    int accessibleFilesCount = (int)cmd.ExecuteScalar();
                    return accessibleFilesCount > 0;
                }
            }
        }

        // Обробник події для виходу з системи
        private void button_LogOut_Click(object? sender, EventArgs e)
        {
            this.Close();
            Form1 loginForm = new Form1();
            loginForm.Show();
        }

        // Обробник зміни вибраного файлу
        private void comboBox_Files_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_Files.SelectedItem == null)
                return;

            string selectedFile = comboBox_Files.SelectedItem.ToString();
            bool isExecutable = IsExecutableFile(selectedFile);

            checkBox_Execute.Enabled = isExecutable;
            checkBox_Execute.Checked = false;

            if (!isExecutable)
            {
                toolTip1.SetToolTip(checkBox_Execute, "This file type doesn't support execution");
            }
            else
            {
                toolTip1.RemoveAll();
            }

            if (comboBox_Users.SelectedItem == null)
                return;

            string selectedUser = comboBox_Users.SelectedItem.ToString();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string query = @"SELECT CAN_READ, CAN_WRITE, CAN_EXECUTE, DURATION_SECONDS 
                            FROM USER_FILE_ACCESS 
                            WHERE USERNAME = @Username 
                            AND FILE_NAME = @FileName";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Username", selectedUser);
                        cmd.Parameters.AddWithValue("@FileName", selectedFile);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                checkBox_Reading.Checked = reader.GetBoolean(reader.GetOrdinal("CAN_READ"));
                                checkBox_Writing.Checked = reader.GetBoolean(reader.GetOrdinal("CAN_WRITE"));

                                if (isExecutable)
                                {
                                    checkBox_Execute.Checked = reader.GetBoolean(reader.GetOrdinal("CAN_EXECUTE"));
                                }

                                object duration = reader["DURATION_SECONDS"];
                                if (duration != DBNull.Value && duration != null)
                                {
                                    numericUpDown_Duration.Value = Convert.ToInt32(duration);
                                }
                                else
                                {
                                    numericUpDown_Duration.Value = 0;
                                }
                            }
                            else
                            {
                                checkBox_Reading.Checked = false;
                                checkBox_Writing.Checked = false;
                                checkBox_Execute.Checked = false;
                                numericUpDown_Duration.Value = 0;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading access rights: {ex.Message}",
                               "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Перевірка, чи є файл виконуваним
        private bool IsExecutableFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return false;

            string extension = Path.GetExtension(fileName).ToLower();
            return extension == ".exe" || extension == ".lnk";
        }

        // Оновлення тривалості доступу
        private void Button_UpdateDuration_Click_1(object sender, EventArgs e)
        {
            string selectedFile = comboBox_Files.SelectedItem?.ToString();
            string selectedUser = comboBox_Users.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(selectedFile) || string.IsNullOrEmpty(selectedUser))
            {
                MessageBox.Show("Please select both file and user to update access time.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int newDuration = (int)numericUpDown_Duration.Value;

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "UPDATE USER_FILE_ACCESS SET DURATION_SECONDS = @Duration WHERE USERNAME = @Username AND FILE_NAME = @FileName";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Duration", newDuration);
                        cmd.Parameters.AddWithValue("@Username", selectedUser);
                        cmd.Parameters.AddWithValue("@FileName", selectedFile);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Access time updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Failed to update access time. Make sure the record exists in the database.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating access time: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Обробник зміни вибору користувача або файлу
        private void ComboBox_UsersOrFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_Users.SelectedItem == null || comboBox_Files.SelectedItem == null)
                return;

            string selectedUser = comboBox_Users.SelectedItem.ToString();
            string selectedFile = comboBox_Files.SelectedItem.ToString();
            bool isExecutable = IsExecutableFile(selectedFile);

            checkBox_Execute.Enabled = isExecutable;

            if (!isExecutable)
            {
                checkBox_Execute.Checked = false;
                toolTip1.SetToolTip(checkBox_Execute, "This file type doesn't support execution");
            }
            else
            {
                string complexity = GetUserPasswordComplexity(selectedUser);
                if (complexity != "High")
                {
                    toolTip1.SetToolTip(checkBox_Execute,
                        "Execute permission requires High password complexity.\n" +
                        $"Current user has {complexity} complexity password.");
                }
                else
                {
                    toolTip1.SetToolTip(checkBox_Execute, "User meets password complexity requirements for execute permission");
                }
            }

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = @"SELECT CAN_READ, CAN_WRITE, CAN_EXECUTE 
                            FROM USER_FILE_ACCESS 
                            WHERE USERNAME = @Username 
                            AND FILE_NAME = @FileName";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Username", selectedUser);
                        cmd.Parameters.AddWithValue("@FileName", selectedFile);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                checkBox_Reading.Checked = reader.GetBoolean(reader.GetOrdinal("CAN_READ"));
                                checkBox_Writing.Checked = reader.GetBoolean(reader.GetOrdinal("CAN_WRITE"));

                                if (isExecutable)
                                {
                                    checkBox_Execute.Checked = reader.GetBoolean(reader.GetOrdinal("CAN_EXECUTE"));
                                }
                            }
                            else
                            {
                                checkBox_Reading.Checked = false;
                                checkBox_Writing.Checked = false;
                                checkBox_Execute.Checked = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading access rights: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetUserPasswordComplexity(string username)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "SELECT PASSWORD_COMPLEXITY FROM LOGIN_TBL WHERE USERNAME = @Username";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        object result = cmd.ExecuteScalar();
                        return result?.ToString() ?? "Low"; // Default to Low if not found
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking password complexity: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "Low"; // Default to Low on error
            }
        }
    }
}