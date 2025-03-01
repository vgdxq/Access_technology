using System;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Diagnostics;

namespace TBD_Yaroshenko
{
    public partial class Form1 : Form
    {   //����� ���������� �� ���� �����, ���� �������� � ���������������� �����.
        string cs = ConfigurationManager.ConnectionStrings["dbtbdyaroshenko"].ConnectionString;

        //���������� ����� ��� �������� ��������� ������
        string complexPattern = "(?=^.{8,}$)((?=.*\\d)|(?=.*\\W+))(?![.\\n])(?=.*[A-Z])(?=.*[a-z]).*$";

        public Form1()
        {
            InitializeComponent();
        }

        //�������� ���������� ��� ���������
        private void ������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Performed by: Yaroshenko Iryna, group B-125-21-3-B");
        }

        //��������, �� ���� ��� ����� �� ������. ���� ������, ���� �������
        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                textBox1.Focus();
                errorProvider1.SetError(textBox1, "Enter your login");
            }
            else
            {
                errorProvider1.Clear();
            }
        }
        // ����� ��� ���������� ��������� ������
        private string GetPasswordComplexity(string password)
        {
            if (password.Length >= 8 && Regex.IsMatch(password, @"[A-Z]") && Regex.IsMatch(password, @"[a-z]") && Regex.IsMatch(password, @"\d") && Regex.IsMatch(password, @"\W"))
            {
                return "High";
            }
            else
            {
                return "Low";
            }
        }


        private void InsertPasswordHistory(string username, string password)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                string complexity = GetPasswordComplexity(password);

                string insertQuery = "INSERT INTO PASSWORD_HISTORY (username, pass, password_complexity, CHANGE_DATE) VALUES (@username, @pass, @complexity, @date)";
                SqlCommand cmd = new SqlCommand(insertQuery, con);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@pass", password);
                cmd.Parameters.AddWithValue("@complexity", complexity);
                cmd.Parameters.AddWithValue("@date", DateTime.Now);

                cmd.ExecuteNonQuery();
            }
        }

        // ������� ��������� ������ ��� �������� ��� �������� ������
        private void SavePasswordComplexity(string username, string password)
        {
            string complexity = GetPasswordComplexity(password);

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                string updateComplexityQuery = "UPDATE login_tbl SET password_complexity = @complexity WHERE username = @username";
                SqlCommand cmd = new SqlCommand(updateComplexityQuery, con);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@complexity", complexity);
                cmd.ExecuteNonQuery();
            }
        }

        // ��������� ������ ������ � �����
        private void SavePasswordHistory(string username, string password)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                string insertHistoryQuery = "INSERT INTO PASSWORD_HISTORY (username, pass) VALUES (@username, @pass)";
                SqlCommand cmd = new SqlCommand(insertHistoryQuery, con);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@pass", password);
                cmd.ExecuteNonQuery();
            }
        }

        // �������� �� ������ �� ������� � �������� ������ ��������
        private bool IsPasswordInHistory(string username, string password)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                string checkHistoryQuery = "SELECT COUNT(*) FROM (SELECT TOP 3 * FROM PASSWORD_HISTORY WHERE username = @username ORDER BY CHANGE_DATE DESC) AS LastPasswords WHERE pass = @pass";
                SqlCommand cmd = new SqlCommand(checkHistoryQuery, con);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@pass", password);
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }



        //��������, �� ������ ������� ������� ���������
        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (checkBox2.Checked && !Regex.IsMatch(textBox2.Text, complexPattern))
            {
                textBox2.Focus();
                errorProvider2.SetError(textBox2, "Create a more complex password");
            }
            else
            {
                errorProvider2.Clear();
            }
        }



        //���� ������ ����������� � ���� �����
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

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                // �������� ���������� �����
                string checkUser = "SELECT COUNT(*) FROM login_tbl WHERE username = @user";
                SqlCommand checkCmd = new SqlCommand(checkUser, con);
                checkCmd.Parameters.AddWithValue("@user", textBox1.Text);
                int userExists = (int)checkCmd.ExecuteScalar();

                if (userExists > 0)
                {
                    MessageBox.Show("A user with this login already exists", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // ��������� ������ �����������
                string insertQuery = "INSERT INTO login_tbl (username, pass) VALUES (@user, @pass)";
                SqlCommand cmd = new SqlCommand(insertQuery, con);
                cmd.Parameters.AddWithValue("@user", textBox1.Text);
                cmd.Parameters.AddWithValue("@pass", textBox2.Text);

                int result = cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    // ��������� ��������� ������
                    string complexity = GetPasswordComplexity(textBox2.Text);
                    string updateComplexityQuery = "UPDATE login_tbl SET password_complexity = @complexity WHERE username = @user";
                    SqlCommand updateCmd = new SqlCommand(updateComplexityQuery, con);
                    updateCmd.Parameters.AddWithValue("@user", textBox1.Text);
                    updateCmd.Parameters.AddWithValue("@complexity", complexity);
                    updateCmd.ExecuteNonQuery();

                    MessageBox.Show("User added successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to add user", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        //������� ������ �����������
        private void button3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Fill in all fields", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (checkBox2.Checked && !Regex.IsMatch(textBox2.Text, complexPattern))
            {
                MessageBox.Show("The new password does not meet the complexity requirements", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ��������, �� �� ������� � �������� ������ ��������
            if (IsPasswordInHistory(textBox1.Text, textBox2.Text))
            {
                MessageBox.Show("It is not possible to use one of the last three passwords", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                string updateQuery = "UPDATE login_tbl SET pass = @newPass WHERE username = @user";
                SqlCommand cmd = new SqlCommand(updateQuery, con);
                cmd.Parameters.AddWithValue("@user", textBox1.Text);
                cmd.Parameters.AddWithValue("@newPass", textBox2.Text);

                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    SavePasswordHistory(textBox1.Text, textBox2.Text); // �������� ����� ������ � �����
                    SavePasswordComplexity(textBox1.Text, textBox2.Text); // �������� ��������� ������
                    MessageBox.Show("Password successfully updated", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("User not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        //³������� ��� ������� ������
        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            textBox2.UseSystemPasswordChar = !checkBox1.Checked;
        }

        //��������, �� �������� ������ ������� ������� ���������
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

            if (checkBox2.Checked && !Regex.IsMatch(textBox2.Text, complexPattern))
            {
                MessageBox.Show("The current password does not meet the new complexity requirements", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            // ��������, �� �������� ���� ��� ����� �� ������
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Fill in all fields for authorization", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // �������� ����������� ���������� �� ���� �����
            using (SqlConnection con = new SqlConnection(cs))
            {
                try
                {
                    con.Open(); // ³�������� �'������� � �����

                    // ��������� SQL ������ ��� �������� ����� �� ������
                    string query = "SELECT * FROM login_tbl WHERE username = @user AND pass = @pass";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@user", textBox1.Text); // �������� ��� �����
                    cmd.Parameters.AddWithValue("@pass", textBox2.Text); // �������� ��� ������

                    SqlDataReader dr = cmd.ExecuteReader(); // �������� �����

                    // ��������, �� �������� ����������� � ����� ������ �� �������
                    if (dr.HasRows)
                    {
                        dr.Read(); // ������ ���������
                        string username = dr["username"].ToString(); // �������� ���� � ����������
                        MessageBox.Show($"Login {username}, successfully authorized!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        {
                            Form2 form2 = new Form2();
                            form2.Show();
                            this.Hide(); // ������ ������� �����
                        }
                    }
                    else
                    {
                        MessageBox.Show("Authorization error. Incorrect login or password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    // ������� ������� ��� ��������� ��� �������� ������
                    MessageBox.Show("Database connection error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }

}