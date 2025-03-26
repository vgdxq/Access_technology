using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;

namespace TBD_Yaroshenko
{
    public partial class MainWind_RoleBased : Form
    {
        private string _username;
        private string _userRole;
        private string cs = ConfigurationManager.ConnectionStrings["dbtbdyaroshenko"].ConnectionString;
        private string _dataFolderPath = @"C:\Users\Ярина\Desktop\NAU2025\TBD_Yaroshenko\TBD_Yaroshenko\Data";

        public MainWind_RoleBased(string username, string role)
        {
            InitializeComponent();
            _username = username;
            _userRole = role;

            ConfigureUIBasedOnRole();
            LoadUserData();

            button_Settings.Visible = _userRole.Equals("Admin", StringComparison.OrdinalIgnoreCase);
            SettingMenu.Visible = false;
            groupBox_Files.Visible = false;

            if (_userRole.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                InitializeRoleManagement();
                InitializeFileManagement();
            }

            // Підписка на події


        }

        public MainWind_RoleBased(string username) : this(username, "") { }

        #region Role Management

        private void InitializeRoleManagement()
        {
            LoadUsersToComboBox();
            LoadAvailableRoles();
        }

        private void LoadUsersToComboBox()
        {
            try
            {
                comboBox_Users.Items.Clear();

                using (var con = new SqlConnection(cs))
                using (var cmd = new SqlCommand("SELECT USERNAME FROM LOGIN_TBL ORDER BY USERNAME", con))
                {
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            comboBox_Users.Items.Add(reader["USERNAME"].ToString());
                        }
                    }
                }

                if (comboBox_Users.Items.Count > 0)
                    comboBox_Users.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження користувачів: {ex.Message}");
            }
        }

        private void LoadAvailableRoles()
        {
            comboBox_UserRole.Items.Clear();
            comboBox_UserRole.Items.AddRange(new[] { "Admin", "Developer", "User" });

            if (comboBox_UserRole.Items.Count > 0)
                comboBox_UserRole.SelectedIndex = 0;
        }

        private void comboBox_Users_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_Users.SelectedItem == null) return;

            LoadUserRole(comboBox_Users.SelectedItem.ToString());
        }

        private void LoadUserRole(string username)
        {
            try
            {
                using (var con = new SqlConnection(cs))
                using (var cmd = new SqlCommand("SELECT ROLE FROM LOGIN_TBL WHERE USERNAME = @username", con))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    con.Open();
                    var role = cmd.ExecuteScalar()?.ToString();

                    if (role != null)
                        comboBox_UserRole.SelectedItem = role;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження ролі: {ex.Message}");
            }
        }

        private void button_SaveRoleChange_Click_1(object sender, EventArgs e)
        {
            if (comboBox_Users.SelectedItem == null || comboBox_UserRole.SelectedItem == null)
            {
                MessageBox.Show("Виберіть користувача та роль");
                return;
            }

            var username = comboBox_Users.SelectedItem.ToString();
            var newRole = comboBox_UserRole.SelectedItem.ToString();

            try
            {
                using (var con = new SqlConnection(cs))
                using (var cmd = new SqlCommand(
                    "UPDATE LOGIN_TBL SET ROLE = @role WHERE USERNAME = @username", con))
                {
                    cmd.Parameters.AddWithValue("@role", newRole);
                    cmd.Parameters.AddWithValue("@username", username);
                    con.Open();

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        MessageBox.Show($"Роль {username} змінена на {newRole}");

                        if (username.Equals(_username))
                        {
                            _userRole = newRole;
                            ConfigureUIBasedOnRole();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка зміни ролі: {ex.Message}");
            }
        }

        #endregion

        #region File Management

        private void InitializeFileManagement()
        {
            LoadFilesToComboBox();
            comboBox_NewFileLevel.Items.AddRange(new[] { "Public", "Confidential", "Secret" });
            comboBox_NewFileLevel.SelectedIndex = 0;
        }

        private void LoadFilesToComboBox()
        {
            try
            {
                comboBox_Files.Items.Clear();

                // Файли з папки
                var files = Directory.GetFiles(_dataFolderPath)
                    .Select(Path.GetFileName)
                    .Where(f => !string.IsNullOrEmpty(f))
                    .ToList();

                // Файли з бази даних
                using (var con = new SqlConnection(cs))
                using (var cmd = new SqlCommand("SELECT FILE_NAME FROM FILE_ACCESS", con))
                {
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var dbFile = reader["FILE_NAME"].ToString();
                            if (!files.Contains(dbFile))
                                files.Add(dbFile);
                        }
                    }
                }

                comboBox_Files.Items.AddRange(files.ToArray());

                if (comboBox_Files.Items.Count > 0)
                    comboBox_Files.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження файлів: {ex.Message}");
            }
        }

        private void comboBox_Files_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_Files.SelectedItem == null) return;

            var selectedFile = comboBox_Files.SelectedItem.ToString();
            LoadFileConfidentialityLevel(selectedFile);
            LoadAccessRights(selectedFile);
        }

        private void LoadFileConfidentialityLevel(string fileName)
        {
            try
            {
                using (var con = new SqlConnection(cs))
                using (var cmd = new SqlCommand(
                    "SELECT CONFIDENTIALITY_LEVEL_RB FROM FILE_ACCESS WHERE FILE_NAME = @fileName", con))
                {
                    cmd.Parameters.AddWithValue("@fileName", fileName);
                    con.Open();
                    var level = cmd.ExecuteScalar()?.ToString();

                    comboBox_NewFileLevel.SelectedItem = level ?? "Public";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження рівня конфіденційності: {ex.Message}");
            }
        }

        private void LoadAccessRights(string fileName)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("ROLE", typeof(string));
                dt.Columns.Add("R", typeof(bool));
                dt.Columns.Add("W", typeof(bool));
                dt.Columns.Add("E", typeof(bool));
                dt.Columns.Add("TIME_s", typeof(int));

                using (SqlConnection con = new SqlConnection(cs))
                {
                    string query = @"
                SELECT ROLE, CAN_READ, CAN_WRITE, CAN_EXECUTE, DURATION_SECONDS
                FROM FILE_ACCESS_RB
                WHERE FILE_NAME = @fileName";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@fileName", fileName);
                        con.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                dt.Rows.Add(
                                    reader["ROLE"].ToString(),
                                    Convert.ToBoolean(reader["CAN_READ"]),
                                    Convert.ToBoolean(reader["CAN_WRITE"]),
                                    Convert.ToBoolean(reader["CAN_EXECUTE"]),
                                    Convert.ToInt32(reader["DURATION_SECONDS"])
                                );
                            }
                        }
                    }
                }

                // Only add default rows if no data was found
                if (dt.Rows.Count == 0)
                {
                    dt.Rows.Add("Admin", false, false, false, 0);
                    dt.Rows.Add("Developer", false, false, false, 0);
                    dt.Rows.Add("User", false, false, false, 0);
                }

                dataGridView_AccessRights.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження прав доступу: {ex.Message}");
            }
        }

        private void button_SaveAccessRights_Click(object sender, EventArgs e)
        {
            if (comboBox_Files.SelectedItem == null) return;

            string fileName = comboBox_Files.SelectedItem.ToString();

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                using (SqlTransaction transaction = con.BeginTransaction())
                {
                    try
                    {
                        // Delete old records
                        using (SqlCommand deleteCmd = new SqlCommand(
                            "DELETE FROM FILE_ACCESS_RB WHERE FILE_NAME = @fileName", con, transaction))
                        {
                            deleteCmd.Parameters.AddWithValue("@fileName", fileName);
                            deleteCmd.ExecuteNonQuery();
                        }

                        // Insert new records
                        foreach (DataGridViewRow row in dataGridView_AccessRights.Rows)
                        {
                            if (row.IsNewRow) continue;

                            using (SqlCommand insertCmd = new SqlCommand(
                                @"INSERT INTO FILE_ACCESS_RB 
                        (FILE_NAME, ROLE, CAN_READ, CAN_WRITE, CAN_EXECUTE, DURATION_SECONDS)
                        VALUES (@fileName, @role, @read, @write, @execute, @duration)",
                                con, transaction))
                            {
                                insertCmd.Parameters.AddWithValue("@fileName", fileName);
                                insertCmd.Parameters.AddWithValue("@role", row.Cells["ROLE"].Value.ToString());
                                insertCmd.Parameters.AddWithValue("@read", Convert.ToBoolean(row.Cells["R"].Value));
                                insertCmd.Parameters.AddWithValue("@write", Convert.ToBoolean(row.Cells["W"].Value));
                                insertCmd.Parameters.AddWithValue("@execute", Convert.ToBoolean(row.Cells["E"].Value));
                                insertCmd.Parameters.AddWithValue("@duration", Convert.ToInt32(row.Cells["TIME_s"].Value));

                                insertCmd.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                        MessageBox.Show("Права доступу успішно збережено");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Помилка збереження прав доступу: {ex.Message}");
                        // Log the full error details
                        Debug.WriteLine(ex.ToString());
                    }
                }
            }
        }



        private void button_ApplyFileLevel_Click(object sender, EventArgs e)
        {
            if (comboBox_Files.SelectedItem == null || comboBox_NewFileLevel.SelectedItem == null)
            {
                MessageBox.Show("Виберіть файл та рівень конфіденційності");
                return;
            }

            var fileName = comboBox_Files.SelectedItem.ToString();
            var newLevel = comboBox_NewFileLevel.SelectedItem.ToString();

            try
            {
                using (var con = new SqlConnection(cs))
                {
                    con.Open();

                    // Перевірка наявності файлу в базі
                    using (var checkCmd = new SqlCommand(
                        "SELECT COUNT(*) FROM FILE_ACCESS WHERE FILE_NAME = @fileName", con))
                    {
                        checkCmd.Parameters.AddWithValue("@fileName", fileName);
                        var exists = (int)checkCmd.ExecuteScalar() > 0;

                        // Оновлення або додавання запису
                        var query = exists
                            ? "UPDATE FILE_ACCESS SET CONFIDENTIALITY_LEVEL_RB = @level WHERE FILE_NAME = @fileName"
                            : "INSERT INTO FILE_ACCESS (FILE_NAME, CONFIDENTIALITY_LEVEL_RB) VALUES (@fileName, @level)";

                        using (var modifyCmd = new SqlCommand(query, con))
                        {
                            modifyCmd.Parameters.AddWithValue("@level", newLevel);
                            modifyCmd.Parameters.AddWithValue("@fileName", fileName);
                            modifyCmd.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show($"Рівень конфіденційності файлу {fileName} змінено на {newLevel}");
                    LoadAccessRights(fileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка зміни рівня конфіденційності: {ex.Message}");
            }
        }

        private void button_RefreshFiles_Click_1(object sender, EventArgs e)
        {
            LoadFilesToComboBox();
        }

        #endregion

        #region Common Methods

        private void LoadUserData()
        {
            if (string.IsNullOrEmpty(_userRole))
            {
                try
                {
                    using (var con = new SqlConnection(cs))
                    using (var cmd = new SqlCommand("SELECT ROLE FROM LOGIN_TBL WHERE USERNAME = @username", con))
                    {
                        cmd.Parameters.AddWithValue("@username", _username);
                        con.Open();
                        _userRole = cmd.ExecuteScalar()?.ToString() ?? "User";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка завантаження ролі: {ex.Message}");
                    _userRole = "User";
                }
            }

            ConfigureUIBasedOnRole();
        }

        private void ConfigureUIBasedOnRole()
        {
            button_Settings.Visible = _userRole.Equals("Admin", StringComparison.OrdinalIgnoreCase);
            button_FileSet.Visible = _userRole.Equals("Admin", StringComparison.OrdinalIgnoreCase);
        }

        private void button_Settings_Click_1(object sender, EventArgs e)
        {
            if (_userRole.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                SettingMenu.Visible = !SettingMenu.Visible;
        }

        private void button_FileSet_Click_1(object sender, EventArgs e)
        {
            if (_userRole.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                groupBox_Files.Visible = !groupBox_Files.Visible;
        }

        private void button_FileManager_Click(object sender, EventArgs e)
        {
            new FileManagerRB(_username, _userRole).Show();
        }

        private void button_LogOut_Click(object sender, EventArgs e)
        {
            new Form1().Show();
            this.Close();
        }

        #endregion

    }
}