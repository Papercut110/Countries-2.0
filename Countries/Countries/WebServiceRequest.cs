using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Windows.Forms;
using SimpleJSON;
using System.Diagnostics;

namespace Countries
{
    // Класс для получения данных из веб ресура
    class WebServiceRequest : Form1 
    {
        // Поле адреса для поиска информации
        string path = "https://restcountries.eu/rest/v2/name/";
        string page;

        // Метод для проверки доступности ресурса
        public bool CheckSite()
        {
            string url = "https://restcountries.eu/#api-endpoints-all";

            Uri uri = new Uri(url);
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            }
            catch
            {
                MessageBox.Show("Ресурс не доступен");
                return false;
            }
            return true;
        }

        // Метод для получения кода страницы
        private bool Connection(string country)
        {
            string adress = path + country;

            WebClient webClient = new WebClient();
            webClient.Encoding = Encoding.UTF8;
            try
            {
                page = webClient.DownloadString(adress);
                return true;
            }
            catch (WebException exp)
            {
                if (((HttpWebResponse) exp.Response).StatusCode == HttpStatusCode.NotFound)
                {
                    MessageBox.Show("Страна " + country + " не найдена");
                }
                return false;
            }
        }

        /// <summary>
        /// Метод для конвертации в JSONNode и поиска необходимой информации из кода веб ресура
        /// </summary>
        /// <param name="country">Имя страны для поиска по веб ресуру</param>
        /// <returns></returns>
        public JSONNode Parsing(string country)
        {

            if (Connection(country))
            {
                JSONNode js = null;

                try
                {
                    js = JSON.Parse(page);
                }
                catch (Exception exp) { MessageBox.Show(exp.Message); }

                if (js[0]["message"].Value != null && js[0]["message"].Value == "Not found")
                {
                    MessageBox.Show("Страна " + country + " не найдена");
                    return null;
                }
                else
                {
                    return js;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Метод, возвращающий необходимые данные о стране
        /// </summary>
        /// <param name="json">Параметр, содержащий набор json данных о конкретной стране</param>
        /// <returns></returns>
        public CountriesInfo ShowData(JSONNode json)
        {
            if (json != null)
            {
                CountriesInfo info = new CountriesInfo();

                info.Country = json[0]["name"].Value;
                info.Code = json[0]["alpha2Code"].Value;
                info.Capital = json[0]["capital"].Value;
                info.Region = json[0]["region"].Value;
                info.Area = json[0]["area"].AsDouble;
                info.Population = json[0]["population"].AsInt;
                
                return info;
            }
            else
            {
                return null;
            }
        }
    }
}
