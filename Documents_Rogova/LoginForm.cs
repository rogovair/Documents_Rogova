using Documents_Rogova;
using Npgsql;
using System;
using System.Windows.Forms;

namespace Documents_Rogova
{
    public partial class LoginForm : Form
    {
        private PostgresConnector bd = new PostgresConnector();
        private int loginAttempts = 0;

        public LoginForm()
        {
            InitializeComponent();
        }

        private void buttonEnter_Click(object sender, EventArgs e)
        {
            string login = textBoxLog.Text;
            string password = textBoxPass.Text;

            NpgsqlCommand cmd_ex = new NpgsqlCommand("SELECT COUNT(*) FROM users " +
                "WHERE login = @login and password = @password", bd.getConnection());

            cmd_ex.Parameters.AddWithValue("@login", login);
            cmd_ex.Parameters.AddWithValue("@password", password);

            bd.openConnection();

            int user = Convert.ToInt32(cmd_ex.ExecuteScalar());

            bd.closeConnection();

            if (user > 0)
            {
                this.Hide();
                MenuForm f1 = new MenuForm(login);
                f1.ShowDialog();
            }
            else
            {
                loginAttempts++;

                if (loginAttempts >= 3)
                {
                    DialogResult result = MessageBox.Show("Вы ввели неверный пароль 3 раза. Желаете сбросить пароль?", "Сброс пароля", MessageBoxButtons.YesNo);

                    if (result == DialogResult.Yes)
                    {
                        using (ResetForm resetForm = new ResetForm(login))
                        {
                            DialogResult resetResult = resetForm.ShowDialog();
                        }
                        loginAttempts = 0;
                    }
                    else
                    {
                        ResetForm resetForm = new ResetForm(login);
                        resetForm.ShowDialog();
                    }
                }
                else
                {
                    MessageBox.Show("Неправильно введён логин или пароль.");
                }
            }
        }
    }
}