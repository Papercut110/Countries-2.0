using Microsoft.SqlServer.Server;
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
    public partial class Form1 : Form
    {
        CountriesInfo info; // Переменная для хранения информации о стране
        public Form1 form;
        public Form2 _form;
        string[] parametrs = new string[4];
        public Form1()
        {
            InitializeComponent();
            info = new CountriesInfo();
            ServerSettings.LoadValues();

            if (ServerSettings.Trigger())
                AddSource();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox3.Text != null)
            {
                WebServiceRequest request = new WebServiceRequest();

                if (request.CheckSite())
                {
                    // Заполнение массива параметров подключения к ДБ в случае, если пришлось вводить их самому в полях настроек
                    if (ServerSettings.Trigger())
                        AddSource();

                    // Получение от ресурса json страницы
                    //request.Connection(textBox3.Text);  

                    // С помощью метода Parsing ищем страну и используя метод ShowData получаем экземпляр CountriesInfo, содержащий необходимые данные
                    info = request.ShowData(request.Parsing(textBox3.Text));

                    //Заполняем окна информацией о стране
                    TextFilling(info);

                    if (info != null)
                    {   // Выводим диалоговое окно с предложением сохранить данные в БД
                        DialogResult dialogResult = MessageBox.Show("Сохранить информацию в базе данных?", "Страна найдена", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            if (parametrs.Length == 4)
                            {
                                DataBase data = new DataBase(parametrs[0], parametrs[1], parametrs[2], parametrs[3]); //.SearchUpdate(info.Code, info.Capital, info.Region, info.Country, info.Area, info.Population);
                                data.SearchCity(info.Capital);
                                data.SearchRegion(info.Region);
                                data.SearchCountry(info.Code);
                                data.AddOrUpdateCountry(info.Code, info.Country, info.Area, info.Population);
                                TextClear();
                            }
                            else
                            {
                                MessageBox.Show("Настройте подключение");
                            }
                        }
                        else if (dialogResult == DialogResult.No)
                        {
                            //похоже не пригодилось
                        }
                    }
                }
            }
        }

        // Обработчик кнопки поиска страны
        private void button2_Click(object sender, EventArgs e)
        {   
            // Очищаем таблицу перед выводом информации
            dataGridView1.Rows.Clear();
            
            DataBase data = new DataBase(parametrs[0], parametrs[1], parametrs[2], parametrs[3]);
            List<CountriesInfo> countries = new List<CountriesInfo>();
            
            // Получаем данные о стране
            countries = data.GetAllValues();
            
            // Заполняем таблицу
            foreach (CountriesInfo i in countries)
            {
                dataGridView1.Rows.Add(FieldMassive(i));
            }
        }

        // Обработчик кнопки очистки БД
        private void button3_Click(object sender, EventArgs e)
        {
            // Очищаем таблицу (DataGrid)
            dataGridView1.Rows.Clear();

            // Очищаем все таблицы БД
            DataBase.ClearBase();
        }

        // Обработчки события окна ввода названия страны
        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            bool flag = false;
            char[] symbols = textBox3.Text.ToCharArray();

            foreach (char i in symbols)
            {
                if (!char.IsLetter(i) && !char.IsWhiteSpace(i))
                {
                    textBox3.Clear();
                    flag = true;
                }
            }
            if (flag)
            {
                MessageBox.Show("Некорректный символ");
            }
        }

        /// <summary>
        /// Вспомогательные методы
        /// </summary>
        /// <param name="info"> Экземпляр класса, содержащего данные о стране</param>
        #region MyMethods

        private void TextFilling(CountriesInfo info)
        {
            if (info != null)
            {
                textBox1.Text = info.Code;
                textBox2.Text = info.Capital;
                textBox4.Text = Convert.ToString(info.Area);
                textBox5.Text = Convert.ToString(info.Population);
                textBox6.Text = info.Region;
            }
        }

        // Метод для очистки всех текстовых полей (наверняка можно было очистить как-то эффективнее всё разом, но торопился, поэтому пока так)
        private void TextClear()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
        }

        /// <summary>
        /// Метод формирования массива элементов для заполнения таблицы DataGrid
        /// </summary>
        /// <param name="countries">Экземпляр класса, содержащего данные о стране</param>
        /// <returns></returns>
        private string[] FieldMassive(CountriesInfo countries)
        {
            string[] elements = new string[6];
            elements[0] = countries.Country;
            elements[1] = countries.Code;
            elements[2] = countries.Capital;
            elements[3] = Convert.ToString(countries.Area);
            elements[4] = Convert.ToString(countries.Population);
            elements[5] = countries.Region;

            return elements;
        }

        public string CorrectText(string text)
        {
            string sometext = null;
            char[] symbols = text.ToCharArray();

            foreach (char i in symbols)
            {
                if (!char.IsWhiteSpace(i))
                {
                    sometext += i;
                }
            }

            return sometext;
        }

        public void AddSource()
        {
            parametrs[0] = CorrectText(ServerSettings.DataSource);
            parametrs[1] = CorrectText(ServerSettings.InitialCatalog);
            parametrs[2] = CorrectText(ServerSettings.UserId);
            parametrs[3] = CorrectText(ServerSettings.Password);
        }


        #endregion

        private void button4_Click(object sender, EventArgs e)
        {
            form = new Form1();
            _form = new Form2();
            _form.Show();
        }
    }
}
