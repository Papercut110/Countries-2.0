using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Countries
{
	// Класс для работы с БД
    class DataBase
    {
		int idCity = int.MinValue;
		int idRegion = int.MinValue;

		// Автосвойства для получения данных о стране, городе и регионе, если такие имеются.
		public string ResponseCountry { get; set; } 
		public string ResponseCity { get; set; }
		public string ResponseRegion { get; set; }
									
		private static string connection;   // = "Data Source=DESKTOP-JKVR2L0\\SQLEXPRESS; Initial Catalog = DataDB; User ID = sa; Password = raw1";
		
		public DataBase(string source, string catalog, string userId, string password)
        {			
			connection = string.Format("Data Source=" + source + "; Initial Catalog = " + catalog + "; User ID = " + userId + "; Password = " + password);
		}

		/// <summary>
		/// Метод для поиска и обновления данных в базе
		/// </summary>
		/// <param name="code">Код страны</param>
		/// <param name="city">Столица страны</param>
		/// <param name="region">Регион страны</param>
		/// <param name="country">Страна</param>
		/// <param name="area">Площадь страны</param>
		/// <param name="population">Население страны</param>
        public void SearchUpdate(string code, string city, string region, string country, double area, int population)
        {
			// Переменные для получения Id ключа в таблицах городов и регионов
			int idCity = int.MinValue;
			int idRegion = int.MinValue;

			// SQL запросы для поиска данных по таблицам
			string valueCountry = "SELECT * FROM [Countries] WHERE @Code = Code";
			string valueCity = "SELECT * FROM [Cities] WHERE @Name = Name";
			string valueRegion = "SELECT * FROM [Regions] WHERE @Name = Name";

			try
            {
				using (SqlConnection connect = new SqlConnection(connection))
				{
					connect.Open();

					// Поиск страны по коду в таблице стран

                    try 
					{
						using (SqlCommand command = new SqlCommand(valueCountry, connect))
						{
							command.Parameters.AddWithValue("Code", code);
							command.ExecuteNonQuery();
                            try 
							{
								using (var read = command.ExecuteReader())
								{
									while (read.Read())
									{
										ResponseCountry = read.GetValue(2).ToString();
									}
								}
							}
							catch (Exception exp)
							{
								MessageBox.Show(exp.Message);
							}
						}
					}
					catch (Exception exp)
					{
						MessageBox.Show(exp.Message);
					}

                    // Поиск столица по названию в таблице городов
                    try 
					{
						using (SqlCommand command = new SqlCommand(valueCity, connect))
						{
							command.Parameters.AddWithValue("Name", city);
							command.ExecuteNonQuery();
                            try 
							{
								using (var read = command.ExecuteReader())
								{
									while (read.Read())
									{
										ResponseCity = read.GetValue(1).ToString();
									}
								}
							}
							catch (Exception exp)
							{
								MessageBox.Show(exp.Message);
							}
						}
					}
					catch (Exception exp)
					{
						MessageBox.Show(exp.Message);
					}

                    // Поиск по региону в таблице регионов
                    try 
					{
						using (SqlCommand command = new SqlCommand(valueRegion, connect))
						{
							command.Parameters.AddWithValue("Name", region);
							command.ExecuteNonQuery();

							try
							{
								using (var read = command.ExecuteReader())
								{
									while (read.Read())
									{
										ResponseRegion = read.GetValue(1).ToString();
									}
								}
							}
							catch (Exception exp) { MessageBox.Show(exp.Message); }
						}
					}
					catch (Exception exp) { MessageBox.Show(exp.Message); }

					// Проверяем, есть ли в БД страна\город\регион, если нет - добавляем + получаем Id 
					if (ResponseCity == null)
					{
						string valuesCities = "INSERT INTO [Cities] VALUES(@Name)";

                        try
                        {
							using (SqlCommand command = new SqlCommand(valuesCities, connect))
							{
								command.Parameters.AddWithValue("@Name", city);
								command.ExecuteNonQuery();
								command.CommandText = "SELECT @@IDENTITY";
								idCity = Convert.ToInt32(command.ExecuteScalar());
							}
						}
						catch (Exception exp) { MessageBox.Show(exp.Message); }
					}
					else
					{
						string City = "SELECT * FROM [Cities] WHERE @Name = Name";
						try
                        {
                            using (SqlCommand command = new SqlCommand(City, connect))
                            {
                                command.Parameters.AddWithValue("@Name", city);
                                command.ExecuteNonQuery();
                                try
                                {
                                    using (var read = command.ExecuteReader())
                                    {
                                        while (read.Read())
                                        {
                                            idCity = Convert.ToInt32(read.GetValue(0));
                                        }
                                    }
                                }
                                catch (Exception exp) { MessageBox.Show(exp.Message); }
                            }
                        }
                        catch (Exception exp) { MessageBox.Show(exp.Message); }
					}

					if (ResponseRegion == null)
					{
						string valuesRegions = "INSERT INTO [Regions] VALUES(@Name)";

						try
                        {
                            using (SqlCommand command = new SqlCommand(valuesRegions, connect))
                            {
                                command.Parameters.AddWithValue("@Name", region);
                                command.ExecuteNonQuery();
                                command.CommandText = "SELECT @@IDENTITY";
                                idRegion = Convert.ToInt32(command.ExecuteScalar());
                            }
                        }
                        catch (Exception exp) { MessageBox.Show(exp.Message); }
					}
					else
					{
						string Region = "SELECT * FROM [Regions] WHERE @Name = Name";
						try
                        {
                            using (SqlCommand command = new SqlCommand(Region, connect))
                            {
                                command.Parameters.AddWithValue("@Name", region);
                                command.ExecuteNonQuery();
                                try
                                {
                                    using (var read = command.ExecuteReader())
                                    {
                                        while (read.Read())
                                        {
                                            idRegion = Convert.ToInt32(read.GetValue(0));
                                        }
                                    }
                                }
                                catch (Exception exp) { MessageBox.Show(exp.Message); }
                            }
                        }
                        catch (Exception exp) { MessageBox.Show(exp.Message); }
					}

					if (ResponseCountry == null)
					{
						string valuesCountry = "INSERT INTO [Countries] VALUES(@CountryName, @Code, @Cities_Id, @Area, @Population, @Region_Id)";

						try
                        {
                            using (SqlCommand command = new SqlCommand(valuesCountry, connect))
                            {
                                command.Parameters.AddWithValue("@CountryName", country);
                                command.Parameters.AddWithValue("@Code", code);
                                command.Parameters.AddWithValue("@Cities_Id", idCity);
                                command.Parameters.AddWithValue("@Area", area);
                                command.Parameters.AddWithValue("@Population", population);
                                command.Parameters.AddWithValue("@Region_Id", idRegion);
                                command.ExecuteNonQuery();
                            }
                        }
                        catch (Exception exp) { MessageBox.Show(exp.Message); }
					}
					else
					{
						string value = "UPDATE [Countries] SET @Code = Code, @Cities_Id = Cities_Id, @Area = Area, @Population = Population, @Region_Id = Region_Id";

						try
                        {
                            using (SqlCommand command = new SqlCommand(value, connect))
                            {
                                command.Parameters.AddWithValue("@Code", code);
                                command.Parameters.AddWithValue("@Cities_Id", idCity);
                                command.Parameters.AddWithValue("@Area", area);
                                command.Parameters.AddWithValue("@Population", population);
                                command.Parameters.AddWithValue("@Region_Id", idRegion);
                                command.ExecuteNonQuery();
                            }
                        }
                        catch (Exception exp) { MessageBox.Show(exp.Message); }
					}
					connect.Close();

				}
			}
			catch (Exception exp)
            {
				MessageBox.Show(exp.Message);
            }
		}

		/// <summary>
		/// Метод для поиска столицы
		/// </summary>
		/// <param name="city">Название столицы</param>
		public void SearchCity(string city)
		{
			string valueCity = "SELECT * FROM [Cities] WHERE @Name = Name";

			try
			{
				using (SqlConnection connect = new SqlConnection(connection))
				{
					connect.Open();
					try
					{
						using (SqlCommand command = new SqlCommand(valueCity, connect))
						{
							command.Parameters.AddWithValue("Name", city);
							command.ExecuteNonQuery();
							try
							{
								using (var read = command.ExecuteReader())
								{
									while (read.Read())
									{
										ResponseCity = read.GetValue(1).ToString();
									}
								}
							}
							catch (Exception exp)
							{
								MessageBox.Show(exp.Message);
							}
						}
					}
					catch (Exception exp)
					{
						MessageBox.Show(exp.Message);
					}
					connect.Close();
				}
			}
			catch (Exception exp)
			{
				MessageBox.Show(exp.Message);
			}
			CheckOrAddCity(city);
		}

		/// <summary>
		/// Метод для поиска региона
		/// </summary>
		/// <param name="region">Название региона</param>
		public void SearchRegion(string region)
        {
			string valueRegion = "SELECT * FROM [Regions] WHERE @Name = Name";

			try
			{
				using (SqlConnection connect = new SqlConnection(connection))
				{
					connect.Open();
					try
					{
						using (SqlCommand command = new SqlCommand(valueRegion, connect))
						{
							command.Parameters.AddWithValue("Name", region);
							command.ExecuteNonQuery();

							try
							{
								using (var read = command.ExecuteReader())
								{
									while (read.Read())
									{
										ResponseRegion = read.GetValue(1).ToString();
									}
								}
							}
							catch (Exception exp) { MessageBox.Show(exp.Message); }
						}
					}
					catch (Exception exp) { MessageBox.Show(exp.Message); }
					connect.Close();
				}
			}
			catch (Exception exp)
			{
				MessageBox.Show(exp.Message);
			}
			CheckOrAddRegion(region);
		}

		/// <summary>
		/// Метод для поиска страны
		/// </summary>
		/// <param name="code">Код страны</param>
		public void SearchCountry(string code)
		{
			string valueCountry = "SELECT * FROM [Countries] WHERE @Code = Code";

			try
			{
				using (SqlConnection connect = new SqlConnection(connection))
				{
					connect.Open();
					try
					{
						using (SqlCommand command = new SqlCommand(valueCountry, connect))
						{
							command.Parameters.AddWithValue("Code", code);
							command.ExecuteNonQuery();
							try
							{
								using (var read = command.ExecuteReader())
								{
									while (read.Read())
									{
										ResponseCountry = read.GetValue(2).ToString();
									}
								}
							}
							catch (Exception exp)
							{
								MessageBox.Show(exp.Message);
							}
						}
					}
					catch (Exception exp)
					{
						MessageBox.Show(exp.Message);
					}
					connect.Close();
				}
			}
			catch (Exception exp)
			{
				MessageBox.Show(exp.Message);
			}
		}

		/// <summary>
		/// Инкапсулированный метод для добавления столицы. Вызывается из метода SearchCity
		/// </summary>
		/// <param name="city">Название столицы</param>
		private void CheckOrAddCity(string city)
        {
			if (ResponseCity == null)
			{
				string valuesCities = "INSERT INTO [Cities] VALUES(@Name)";

				try
				{
					using (SqlConnection connect = new SqlConnection(connection))
					{
						connect.Open();
						try
						{
							using (SqlCommand command = new SqlCommand(valuesCities, connect))
							{
								command.Parameters.AddWithValue("@Name", city);
								command.ExecuteNonQuery();
								command.CommandText = "SELECT @@IDENTITY";
								idCity = Convert.ToInt32(command.ExecuteScalar());
							}
						}
						catch (Exception exp) { MessageBox.Show(exp.Message); }
						connect.Close();
					}
				}
				catch (Exception exp) { MessageBox.Show(exp.Message); }
			}
			else
			{
				string City = "SELECT * FROM [Cities] WHERE @Name = Name";

				try
				{
					using (SqlConnection connect = new SqlConnection(connection))
					{
						connect.Open();
						try
						{
							using (SqlCommand command = new SqlCommand(City, connect))
							{
								command.Parameters.AddWithValue("@Name", city);
								command.ExecuteNonQuery();
								try
								{
									using (var read = command.ExecuteReader())
									{
										while (read.Read())
										{
											idCity = Convert.ToInt32(read.GetValue(0));
										}
									}
								}
								catch (Exception exp) { MessageBox.Show(exp.Message); }
							}
						}
						catch (Exception exp) { MessageBox.Show(exp.Message); }
						connect.Close();
					}
				}
				catch (Exception exp) { MessageBox.Show(exp.Message); }
			}
		}

		/// <summary>
		/// Инкапсулированный метод для добавления региона. Вызывается из метода SearchRegion
		/// </summary>
		/// <param name="region">Название региона</param>
		private void CheckOrAddRegion(string region)
		{
			if (ResponseRegion == null)
			{
				string valuesRegions = "INSERT INTO [Regions] VALUES(@Name)";

				try
				{
					using (SqlConnection connect = new SqlConnection(connection))
					{
						connect.Open();
						try
						{
							using (SqlCommand command = new SqlCommand(valuesRegions, connect))
							{
								command.Parameters.AddWithValue("@Name", region);
								command.ExecuteNonQuery();
								command.CommandText = "SELECT @@IDENTITY";
								idRegion = Convert.ToInt32(command.ExecuteScalar());
							}
						}
						catch (Exception exp) { MessageBox.Show(exp.Message); }
						connect.Close();
					}
				}
				catch (Exception exp) { MessageBox.Show(exp.Message); }
			}
			else
			{
				string Region = "SELECT * FROM [Regions] WHERE @Name = Name";

				try
				{
					using (SqlConnection connect = new SqlConnection(connection))
					{
						connect.Open();
						try
						{
							using (SqlCommand command = new SqlCommand(Region, connect))
							{
								command.Parameters.AddWithValue("@Name", region);
								command.ExecuteNonQuery();
								try
								{
									using (var read = command.ExecuteReader())
									{
										while (read.Read())
										{
											idRegion = Convert.ToInt32(read.GetValue(0));
										}
									}
								}
								catch (Exception exp) { MessageBox.Show(exp.Message); }
							}
						}
						catch (Exception exp) { MessageBox.Show(exp.Message); }
						connect.Close();
					}
				}
				catch (Exception exp) { MessageBox.Show(exp.Message); }
			}
		}

		/// <summary>
		/// Метод для для добавления или обновления данных о стране.
		/// </summary>
		/// <param name="code">Код страны</param>
		/// <param name="country">Название страны</param>
		/// <param name="area">Площадь страны</param>
		/// <param name="population">Размер населения страны</param>
		public void AddOrUpdateCountry(string code, string country, double area, int population)
		{
			if (ResponseCountry == null)
			{
				string valuesCountry = "INSERT INTO [Countries] VALUES(@CountryName, @Code, @Cities_Id, @Area, @Population, @Region_Id)";

				try
				{
					using (SqlConnection connect = new SqlConnection(connection))
					{
						connect.Open();

						try
						{
							using (SqlCommand command = new SqlCommand(valuesCountry, connect))
							{
								command.Parameters.AddWithValue("@CountryName", country);
								command.Parameters.AddWithValue("@Code", code);
								command.Parameters.AddWithValue("@Cities_Id", idCity);
								command.Parameters.AddWithValue("@Area", area);
								command.Parameters.AddWithValue("@Population", population);
								command.Parameters.AddWithValue("@Region_Id", idRegion);
								command.ExecuteNonQuery();
							}
						}
						catch (Exception exp) { MessageBox.Show(exp.Message); }
						connect.Close();
					}
				}
				catch (Exception exp) { MessageBox.Show(exp.Message); }
			}

			else
			{
				string value = "UPDATE [Countries] SET @Code = Code, @Cities_Id = Cities_Id, @Area = Area, @Population = Population, @Region_Id = Region_Id";

				try
				{
					using (SqlConnection connect = new SqlConnection(connection))
					{
						connect.Open();

						try
						{
							using (SqlCommand command = new SqlCommand(value, connect))
							{
								command.Parameters.AddWithValue("@Code", code);
								command.Parameters.AddWithValue("@Cities_Id", idCity);
								command.Parameters.AddWithValue("@Area", area);
								command.Parameters.AddWithValue("@Population", population);
								command.Parameters.AddWithValue("@Region_Id", idRegion);
								command.ExecuteNonQuery();
							}
						}
						catch (Exception exp) { MessageBox.Show(exp.Message); }
						connect.Close();
					}
				}
				catch (Exception exp) { MessageBox.Show(exp.Message); }
			}
		}


			/// <summary>
			/// Метод для получения данных по всем странам в БД. Возвращает список стран, если они вообще есть.
			/// </summary>
			/// <returns></returns>
		public List<CountriesInfo> GetAllValues()
		{
			string value = "SELECT * FROM Countries";
			List<CountriesInfo> countries = new List<CountriesInfo>();
			
			try
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    connect.Open();
                    try
                    {
                        using (SqlCommand command = new SqlCommand(value, connect))
                        {
                            using (var read = command.ExecuteReader())
                            {
                                while (read.Read())
                                {
                                    CountriesInfo country = new CountriesInfo();
                                    country.Country = read.GetValue(1).ToString();
                                    country.Code = read.GetValue(2).ToString();
                                    country.Area = Convert.ToDouble(read.GetValue(4));
                                    country.Population = Convert.ToInt32(read.GetValue(5));
                                    int CityId = Convert.ToInt32(read.GetValue(3));
                                    int RegionId = Convert.ToInt32(read.GetValue(6));
                                    country.Capital = GetById(CityId, RegionId).Capital;
                                    country.Region = GetById(CityId, RegionId).Region;
                                    countries.Add(country);
                                }
                            }
                        }
                    }
                    catch (Exception exp) { MessageBox.Show(exp.Message); }
                    connect.Close();
                }
            }
            catch (Exception exp) { MessageBox.Show(exp.Message); }

			return countries;
		}

		/// <summary>
		/// Метод для поиска столицы и региона по Id. Метод инкапсулирован для внутреннего использования в классе.
		/// </summary>
		/// <param name="CityId">Id столицы</param>
		/// <param name="RegionId">Id региона</param>
		/// <returns></returns>
		private CountriesInfo GetById(int CityId, int RegionId)
        {
			CountriesInfo country = new CountriesInfo();

			try
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    connect.Open();
                    string City = "SELECT * FROM [Cities] WHERE @Id = Id";
                    try
                    {
						using (SqlCommand sqlCommand = new SqlCommand(City, connect))
						{
							sqlCommand.Parameters.AddWithValue("@Id", CityId);
							sqlCommand.ExecuteNonQuery();

                            try
                            {
								using (var sqlRead = sqlCommand.ExecuteReader())
								{
									while (sqlRead.Read())
									{
										country.Capital = sqlRead.GetValue(1).ToString();
									}
								}
							}
							catch (Exception exp) { MessageBox.Show(exp.Message); }
						}
					}
					catch (Exception exp) { MessageBox.Show(exp.Message); }

					string Region = "SELECT * FROM [Regions] WHERE @Id = Id";
                    try
                    {
                        using (SqlCommand sqlCommand = new SqlCommand(Region, connect))
                        {
                            sqlCommand.Parameters.AddWithValue("@Id", RegionId);
                            sqlCommand.ExecuteNonQuery();
                            try
                            {
                                using (var sqlRead = sqlCommand.ExecuteReader())
                                {
                                    while (sqlRead.Read())
                                    {
                                        country.Region = sqlRead.GetValue(1).ToString();
                                    }
                                }
                            }
                            catch (Exception exp) { MessageBox.Show(exp.Message); }
                        }
                    }
                    catch (Exception exp) { MessageBox.Show(exp.Message); }
                    connect.Close();
                }
            }
            catch (Exception exp) { MessageBox.Show(exp.Message); }
			return country;
		}

		/// <summary>
		/// Метод для полной очистки всех таблиц БД
		/// </summary>
		public static void ClearBase()
        {
			string country = "DELETE FROM [Countries]";
			string city = "DELETE FROM [Cities]";
			string region = "DELETE FROM [Regions]";

			Remove(country);
			Remove(city);
			Remove(region);
		}

		/// <summary>
		/// Вспомогательный метод для очистки таблиц, так же инкапсулирован в класс только для внутреннего пользования
		/// </summary>
		/// <param name="value">SQL запрос для очистки конкретной таблицы</param>
		private static void Remove(string value)
        {
            try
            {
				using (SqlConnection connect = new SqlConnection(connection))
				{
					connect.Open();
					try
					{
						using (SqlCommand command = new SqlCommand(value, connect))
						{
							command.ExecuteNonQuery();
						}
					}
					catch (Exception exp) { MessageBox.Show(exp.Message); }
					connect.Close();
				}
			}
			catch (Exception exp) { MessageBox.Show(exp.Message); }
		}
    }
}
