using System;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Price_Verifier_App_Settings.Services;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace BGC_Prod_Verifier
{
    public partial class settingsForm : Form
    {
        private readonly ConnectionStringService connectionStringService;
        private readonly SecurityService securityService;
        private DatabaseConfig _config;

        public settingsForm()
        {
            InitializeComponent();
            LoadSettings();
            btn_clear.Click += btn_clear_Click;

            rb_ipos.CheckedChanged += RadioButton_CheckedChanged;
            rb_eipos.CheckedChanged += RadioButton_CheckedChanged;

        }
        private void btnBrowseImages_Click_1(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "Select a folder containing images";

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                tb_adpicpath.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void btnBrowseVideos_Click_1(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "Select a folder containing videos";

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                tb_advidpath.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void btnSave_Click_1(object sender, EventArgs e)
        {
            _config = new DatabaseConfig();
            string connString = $"server={_config.Server};port={_config.Port};uid={_config.Uid};pwd={_config.Pwd};database={_config.Database}";

            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                string query = "UPDATE settings SET set_appname = @appName, set_adpictime = @adPicTime, set_adpic = @adPicPath, set_advidtime = @adVidTime, set_advid = @adVidPath, set_disptime = @dispTime";

                MySqlCommand command = new MySqlCommand(query, conn);

                command.Parameters.AddWithValue("@appName", tb_appname.Text);
                command.Parameters.AddWithValue("@adPicTime", tb_adpictime.Text);
                command.Parameters.AddWithValue("@adPicPath", tb_adpicpath.Text);
                command.Parameters.AddWithValue("@adVidTime", tb_advidtime.Text);
                command.Parameters.AddWithValue("@adVidPath", tb_advidpath.Text);
                command.Parameters.AddWithValue("@dispTime", tb_disptime.Text);

                conn.Open();
                command.ExecuteNonQuery();
                conn.Close();
                MessageBox.Show("Settings successfully saved.");
            }
        }

        private void LoadSettings()
        {
            _config = new DatabaseConfig();
            string connString = $"server={_config.Server};port={_config.Port};uid={_config.Uid};pwd={_config.Pwd};database={_config.Database}";

            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                string query = "SELECT * FROM settings";

                MySqlCommand command = new MySqlCommand(query, conn);

                conn.Open();
                MySqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    tb_appname.Text = reader["set_appname"].ToString();
                    tb_adpictime.Text = reader["set_adpictime"].ToString();
                    tb_adpicpath.Text = reader["set_adpic"].ToString();
                    tb_advidtime.Text = reader["set_advidtime"].ToString();
                    tb_advidpath.Text = reader["set_advid"].ToString();
                    tb_disptime.Text = reader["set_disptime"].ToString();
                    // Retrieve the set_code value from the reader
                    int setCode = reader.GetInt32("set_code");

                    // Set the radio button based on the set_code value
                    if (setCode == 1)
                    {
                        rb_ipos.Checked = true;
                        rb_eipos.Checked = false;
                    }
                    else if (setCode == 2)
                    {
                        rb_ipos.Checked = false;
                        rb_eipos.Checked = true;
                    }
                }

                reader.Close();
                conn.Close();
            }
        }

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            _config = new DatabaseConfig();
            string connString = $"server={_config.Server};port={_config.Port};uid={_config.Uid};pwd={_config.Pwd};database={_config.Database}";

            int set_code = 0;

            if (rb_ipos.Checked)
            {
                set_code = 1;
            }
            else if (rb_eipos.Checked)
            {
                set_code = 2;
            }

            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                string updateQuery = "UPDATE settings SET set_code = @setCode";
                MySqlCommand command = new MySqlCommand(updateQuery, conn);
                command.Parameters.AddWithValue("@setCode", set_code);

                conn.Open(); 
                command.ExecuteNonQuery(); 
                conn.Close();
            }
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            Controls.OfType<TextBox>().ToList().ForEach(textBox => textBox.Clear());
        }
    }
}