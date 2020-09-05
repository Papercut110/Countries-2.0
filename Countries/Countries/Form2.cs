using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Countries
{
    public partial class Form2 : Form
    {
        
        public Form2()
        {
            InitializeComponent();

            if (ServerSettings.Trigger())
            {
                ShowText();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
            if (textBox1.Text != null && textBox2.Text != null && textBox3.Text != null && textBox4.Text != null)
            {
                ServerSettings.UserSettings(textBox1.Text, textBox2.Text, textBox3.Text, textBox4.Text);
                MessageBox.Show("Настройки применены");
                Form.ActiveForm.Close();
            }
            else
            {
                MessageBox.Show("Заполните пустые поля и нажмите 'Применить настройки'");
            }
            
        }

        public void ShowText()
        {
            if (textBox1.Text != null && textBox2.Text != null && textBox3.Text != null && textBox4.Text != null)
            {
                textBox1.Text = ServerSettings.DataSource;
                textBox2.Text = ServerSettings.InitialCatalog;
                textBox3.Text = ServerSettings.UserId;
                textBox4.Text = ServerSettings.Password;
            }
        }
    }
}
