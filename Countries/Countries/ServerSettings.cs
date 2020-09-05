using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Countries
{
    // Класс, ошибочно названный сервером, в котором мы реализуем настройки подключения к БД
    class ServerSettings
    {
        private static List<string> settings = new List<string>();
        public static string DataSource { get; set; }
        public static string InitialCatalog { get; set; }
        public static string UserId { get; set; }
        public static string Password { get; set; }

        
        // Поле с путём к файлу с настройками
        static string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\settings.txt";

        // Метод для загрузки настроек из файла
        public static void LoadValues()
        {
            if (File.Exists(path))
            {
                try
                {
                    using (StreamReader reader = new StreamReader(path))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            settings.Add(line);
                        }
                    }
                    if (settings.Count == 4)
                    {
                        DataSource = settings[0];
                        InitialCatalog = settings[1];
                        UserId = settings[2];
                        Password = settings[3];

                        settings.Clear();
                    }
                    else
                    {
                        MessageBox.Show("Не все поля файла настроек содержат значения");
                        File.Delete(path);
                    }
                }
                catch (Exception exp)
                {
                    MessageBox.Show(exp.Message);
                }
            }
            else
            {
                MessageBox.Show("Файл конфигурации БД не найден, заполните пустые поля в меню настройки и нажмите 'Применить настройки'");
            }
        }

        // Метод для проверки заполнения автосвойств настроек БД
        public static bool Trigger()
        {
            if (DataSource != null && InitialCatalog != null && UserId != null && Password != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Метод для формирования своего файла настроек в случае, если такового нет или он некорректно заполнен
        /// </summary>
        /// <param name="source">DataSource</param>
        /// <param name="catalog">InitialCatalog</param>
        /// <param name="userId">UserId</param>
        /// <param name="password">Password</param>
        public static void UserSettings(string source, string catalog, string userId, string password)
        {
            DataSource = source;
            InitialCatalog = catalog;
            UserId = userId;
            Password = password;

            if (!File.Exists(path))
            {
                try
                {
                    using (FileStream file = File.Create(path))
                    {
                        
                    }
                }
                catch (Exception exp)
                {
                    MessageBox.Show(exp.Message);
                }
            }

            try
            {
                using (StreamWriter writer = new StreamWriter(path))
                {
                    writer.Write(source + "\n" + catalog + "\n" + userId + "\n" + password);
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }
    }
}
