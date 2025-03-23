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
        private string cs = ConfigurationManager.ConnectionStrings["dbtbdyaroshenko"].ConnectionString;
        private string complexPattern = "(?=^.{8,}$)((?=.*\\d)|(?=.*\\W+))(?![.\\n])(?=.*[A-Z])(?=.*[a-z]).*$";

        public Form1()
        {
            InitializeComponent();
            comboBoxAccessControl.Items.AddRange(new string[] { "Mandatory", "Discretionary", "Role-Based" });
            comboBoxAccessControl.SelectedIndex = 0; // Значення за замовчуванням
        }

        private void довідкаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Performed by: Yaroshenko Iryna, group B-125-21-3-B");
        }

        private string GetPasswordComplexity(string password)
        {
            return Regex.IsMatch(password, complexPattern) ? "High" : "Low";
        }

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
                MessageBox.Show($"Помилка бази даних: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

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
                MessageBox.Show($"Помилка бази даних: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            textBox2.UseSystemPasswordChar = !checkBox1.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked && !Regex.IsMatch(textBox2.Text, complexPattern))
            {
                MessageBox.Show("The current password does not meet the new complexity requirements", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //private void button1_Click_1(object sender, EventArgs e)
        //{
        //    //if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text))
        //    //{
        //    //    MessageBox.Show("Fill in all fields for authorization", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //    //    return;
        //    //}

        //    //try
        //    //{
        //    //    using (var con = new SqlConnection(cs))
        //    //    using (var cmd = new SqlCommand("SELECT * FROM LOGIN_TBL WHERE USERNAME = @user AND PASS = @pass", con))
        //    //    {
        //    //        cmd.Parameters.Add(new SqlParameter("@user", SqlDbType.VarChar, 50)).Value = textBox1.Text;
        //    //        cmd.Parameters.Add(new SqlParameter("@pass", SqlDbType.VarChar, 100)).Value = textBox2.Text;
        //    //        con.Open();
        //    //        using (var dr = cmd.ExecuteReader())
        //    //        {
        //    //            if (dr.HasRows)
        //    //            {
        //    //                dr.Read();
        //    //                string role = dr["SECURITY_LEVEL"]?.ToString() ?? "Unclassified";
        //    //                MessageBox.Show($"Login successful! Security Level: {role}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

        //    //                // Відкриваємо MainWind і передаємо ім'я користувача
        //    //                MainWind mainWind = new MainWind(textBox1.Text);
        //    //                mainWind.Show();
        //    //                this.Hide();
        //    //            }
        //    //            else
        //    //            {
        //    //                MessageBox.Show("Authorization error. Incorrect login or password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    //            }
        //    //        }
        //    //    }
        //    //}
        //    catch (SqlException ex)
        //    {
        //        MessageBox.Show($"Помилка бази даних: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Помилка: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}
        private void button1_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Fill in all fields for authorization", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Отримуємо вибраний тип розмежування з comboBoxAccessControl
            string accessControlType = comboBoxAccessControl.SelectedItem?.ToString() ?? "Undefined";

            using (var con = new SqlConnection(cs))
            {
                con.Open();

                // Перевірка авторизації
                string query = "SELECT * FROM LOGIN_TBL WHERE USERNAME = @user AND PASS = @pass";
                var cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@user", textBox1.Text);
                cmd.Parameters.AddWithValue("@pass", textBox2.Text);
                var dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    dr.Read();
                    string role = dr["SECURITY_LEVEL"]?.ToString() ?? "Unclassified";

                    // Оновлення типу розмежування в базі даних
                    dr.Close(); // Закриваємо DataReader перед виконанням наступного запиту
                    string updateQuery = "UPDATE LOGIN_TBL SET ACCESS_CONTROL_TYPE = @accessControlType WHERE USERNAME = @user";
                    var updateCmd = new SqlCommand(updateQuery, con);
                    updateCmd.Parameters.AddWithValue("@accessControlType", accessControlType);
                    updateCmd.Parameters.AddWithValue("@user", textBox1.Text);
                    updateCmd.ExecuteNonQuery();

                    MessageBox.Show($"Login successful! Security Level: {role}, Access Control Type: {accessControlType}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Відкриваємо відповідну форму залежно від типу контролю доступу
                    if (accessControlType == "Discretionary")
                    {
                        MainWind_Discretionary mainWindDiscretionary = new MainWind_Discretionary(textBox1.Text, accessControlType); // Передаємо ім'я користувача
                        mainWindDiscretionary.Show();
                    }
                    else if (accessControlType == "Role-Based")
                    {
                        MainWind_RoleBased mainWindRoleBased = new MainWind_RoleBased(textBox1.Text); // Передаємо ім'я користувача
                        mainWindRoleBased.Show();
                    }
                    else
                    {
                        MainWind mainWind = new MainWind(textBox1.Text); // Стандартна форма для Mandatory
                        mainWind.Show();
                    }

                    this.Hide(); // Приховуємо поточну форму (Form1)
                }
                else
                {
                    MessageBox.Show("Authorization error. Incorrect login or password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

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

                    // Отримуємо вибраний тип розмежування з comboBoxAccessControl
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
                MessageBox.Show($"Помилка бази даних: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

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
                using (var cmd = new SqlCommand("UPDATE LOGIN_TBL SET PASS = @newPass WHERE USERNAME = @user", con))
                {
                    cmd.Parameters.Add(new SqlParameter("@user", SqlDbType.VarChar, 50)).Value = textBox1.Text;
                    cmd.Parameters.Add(new SqlParameter("@newPass", SqlDbType.VarChar, 100)).Value = textBox2.Text;
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
            catch (SqlException ex)
            {
                MessageBox.Show($"Помилка бази даних: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

       
    }
}