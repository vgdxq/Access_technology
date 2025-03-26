using System;
using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TBD_Yaroshenko
{
    public partial class Form1 : Form
    {
        private string cs = ConfigurationManager.ConnectionStrings["dbtbdyaroshenko"].ConnectionString; // Рядок підключення до БД
        private string complexPattern = "(?=^.{8,}$)((?=.*\\d)|(?=.*\\W+))(?![.\\n])(?=.*[A-Z])(?=.*[a-z]).*$"; // Патерн складності паролю

        public Form1()
        {
            InitializeComponent();
            // Додаємо варіанти контролю доступу у комбобокс
            comboBoxAccessControl.Items.AddRange(new string[] { "Mandatory", "Discretionary", "Role-Based" });
            comboBoxAccessControl.SelectedIndex = 0; // Встановлюємо значення за замовчуванням
        }

        // Обробник кнопки довідки
        private void довідкаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Performed by: Yaroshenko Iryna, group B-125-21-3-B");
        }

        // Метод для визначення складності паролю
        private string GetPasswordComplexity(string password)
        {
            return Regex.IsMatch(password, complexPattern) ? "High" : "Low";
        }

        // Метод для збереження історії паролів
        private void SavePasswordHistory(string username, string password)
        {
            try
            {
                using (var con = new SqlConnection(cs))
                using (var cmd = new SqlCommand(
                    "INSERT INTO PASSWORD_HISTORY (USERNAME, OLD_PASSWORD, CHANGE_DATE) VALUES (@username, @password, GETDATE())", con))
                {
                    cmd.Parameters.Add(new SqlParameter("@username", SqlDbType.VarChar, 50)).Value = username;
                    cmd.Parameters.Add(new SqlParameter("@password", SqlDbType.VarChar, 100)).Value = password;
                    con.Open();
                    cmd.ExecuteNonQuery();
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

        // Метод для перевірки чи пароль був використаний раніше
        private bool IsPasswordInHistory(string username, string password)
        {
            try
            {
                using (var con = new SqlConnection(cs))
                using (var cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM (SELECT TOP 3 OLD_PASSWORD FROM PASSWORD_HISTORY WHERE USERNAME = @username ORDER BY CHANGE_DATE DESC) AS LastPasswords WHERE OLD_PASSWORD = @password", con))
                {
                    cmd.Parameters.Add(new SqlParameter("@username", SqlDbType.VarChar, 50)).Value = username;
                    cmd.Parameters.Add(new SqlParameter("@password", SqlDbType.VarChar, 100)).Value = password;
                    con.Open();
                    return (int)cmd.ExecuteScalar() > 0;
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // Обробник зміни стану чекбоксу для показу паролю
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            textBox2.UseSystemPasswordChar = !checkBox1.Checked;
        }

        // Обробник зміни стану чекбоксу для перевірки складності паролю
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked && !Regex.IsMatch(textBox2.Text, complexPattern))
            {
                MessageBox.Show("The current password does not meet the new complexity requirements", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Обробник кнопки входу
        private void button1_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Fill in all fields for authorization", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string accessControlType = comboBoxAccessControl.SelectedItem?.ToString() ?? "Undefined";

            using (var con = new SqlConnection(cs))
            {
                con.Open();

                string query = "SELECT * FROM LOGIN_TBL WHERE USERNAME = @user AND PASS = @pass";
                var cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@user", textBox1.Text);
                cmd.Parameters.AddWithValue("@pass", textBox2.Text);
                var dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    dr.Read();
                    string securityLevel = dr["SECURITY_LEVEL"]?.ToString() ?? "Unclassified"; // Змінили ім'я змінної
                    string userRole = dr["ROLE"]?.ToString() ?? "User"; // Використовуємо інше ім'я змінної

                    dr.Close();
                    string updateQuery = "UPDATE LOGIN_TBL SET ACCESS_CONTROL_TYPE = @accessControlType WHERE USERNAME = @user";
                    var updateCmd = new SqlCommand(updateQuery, con);
                    updateCmd.Parameters.AddWithValue("@accessControlType", accessControlType);
                    updateCmd.Parameters.AddWithValue("@user", textBox1.Text);
                    updateCmd.ExecuteNonQuery();

                    MessageBox.Show($"Login successful! Security Level: {securityLevel}, Access Control Type: {accessControlType}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    if (accessControlType == "Discretionary")
                    {
                        MainWind_Discretionary mainWindDiscretionary = new MainWind_Discretionary(textBox1.Text, accessControlType);
                        mainWindDiscretionary.Show();
                    }
                    else if (accessControlType == "Role-Based")
                    {
                        MainWind_RoleBased mainWindRoleBased = new MainWind_RoleBased(textBox1.Text, userRole); // Використовуємо userRole
                        mainWindRoleBased.Show();
                    }
                    else
                    {
                        MainWind mainWind = new MainWind(textBox1.Text);
                        mainWind.Show();
                    }

                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Authorization error. Incorrect login or password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Обробник кнопки реєстрації
        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Fill in all fields", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (checkBox2.Checked && !Regex.IsMatch(textBox2.Text, complexPattern))
            {
                MessageBox.Show("The password does not meet the complexity requirements", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var con = new SqlConnection(cs))
                using (var checkCmd = new SqlCommand("SELECT COUNT(*) FROM LOGIN_TBL WHERE USERNAME = @user", con))
                {
                    checkCmd.Parameters.Add(new SqlParameter("@user", SqlDbType.VarChar, 50)).Value = textBox1.Text;
                    con.Open();
                    if ((int)checkCmd.ExecuteScalar() > 0)
                    {
                        MessageBox.Show("User already exists", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Отримуємо вибраний тип контролю доступу
                    string accessControlType = comboBoxAccessControl.SelectedItem?.ToString() ?? "Undefined";

                    using (var insertCmd = new SqlCommand(
                        "INSERT INTO LOGIN_TBL (USERNAME, PASS, PASSWORD_COMPLEXITY, SECURITY_LEVEL, ACCESS_CONTROL_TYPE) VALUES (@user, @pass, @complexity, 'Unclassified', @accessControlType)", con))
                    {
                        insertCmd.Parameters.Add(new SqlParameter("@user", SqlDbType.VarChar, 50)).Value = textBox1.Text;
                        insertCmd.Parameters.Add(new SqlParameter("@pass", SqlDbType.VarChar, 100)).Value = textBox2.Text;
                        insertCmd.Parameters.Add(new SqlParameter("@complexity", SqlDbType.VarChar, 10)).Value = GetPasswordComplexity(textBox2.Text);
                        insertCmd.Parameters.Add(new SqlParameter("@accessControlType", SqlDbType.VarChar, 20)).Value = accessControlType;
                        insertCmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("User added successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        // Обробник кнопки зміни паролю
        private void button3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Fill in all fields", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (checkBox2.Checked && !Regex.IsMatch(textBox2.Text, complexPattern))
            {
                MessageBox.Show("The new password does not meet complexity requirements", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (IsPasswordInHistory(textBox1.Text, textBox2.Text))
            {
                MessageBox.Show("Cannot use one of the last three passwords", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var con = new SqlConnection(cs))
                {
                    // Визначаємо складність нового пароля
                    string newPasswordComplexity = GetPasswordComplexity(textBox2.Text);

                    // Оновлюємо пароль і складність пароля
                    using (var cmd = new SqlCommand("UPDATE LOGIN_TBL SET PASS = @newPass, PASSWORD_COMPLEXITY = @complexity WHERE USERNAME = @user", con))
                    {
                        cmd.Parameters.Add(new SqlParameter("@user", SqlDbType.VarChar, 50)).Value = textBox1.Text;
                        cmd.Parameters.Add(new SqlParameter("@newPass", SqlDbType.VarChar, 100)).Value = textBox2.Text;
                        cmd.Parameters.Add(new SqlParameter("@complexity", SqlDbType.VarChar, 10)).Value = newPasswordComplexity;
                        con.Open();
                        if (cmd.ExecuteNonQuery() > 0)
                        {
                            SavePasswordHistory(textBox1.Text, textBox2.Text);
                            MessageBox.Show("Password successfully updated", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("User not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void Form1_Load(object sender, EventArgs e)
        {
            // Обробник завантаження форми
        }

      
    }
}