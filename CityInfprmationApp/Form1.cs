using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CityInfprmationApp
{
    public partial class CityInformationUI : Form
    {
        public CityInformationUI()
        {
            InitializeComponent();
        }
        string connectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=UniversityStudentDB;Data Source=SHIPLU";
       private bool isUpdateMode = false;
        private string cityNameId ="";
       private bool searchCity = false;
       private bool searchCountry = false;
        private void saveButton_Click(object sender, EventArgs e)
        {
            City city = new City();
          
            city.cityName = cityNameTextBox.Text;
            city.about = aboutTextBox.Text;
            city.country = countryTextBox.Text;

            if (isUpdateMode)
            {
                SqlConnection connection = new SqlConnection(connectionString);
                string query = "update  CityInformation_Table set About='" + city.about + "', Country='" + city.country + "'where [City Name]='"+cityNameId+"'";
                SqlCommand command = new SqlCommand(query,connection);
                connection.Open();
               int rowCount= command.ExecuteNonQuery();
               if (rowCount > 0)
                {
                    MessageBox.Show("Updated Successfully");
                    saveButton.Text = "Save";
                    cityNameId = "";
                    cityNameTextBox.Enabled = true;
                    isUpdateMode = false;
                    ClearAllTextBox();
                    ShowAllInformation();
                }
                else
                {
                    MessageBox.Show("Updated fail");
                }
            }
            else { 

            if (cityNameTextBox.Text == "" || aboutTextBox.Text == "" || countryTextBox.Text=="")
            {
                MessageBox.Show("Enter the value all boxes");
            }
            else if (cityNameTextBox.Text.Length < 4)
            {
                MessageBox.Show("You must input city name 4 carecter above ");
            }
            else
            {
                if (IsCityNameExists(city.cityName))
                {
                    MessageBox.Show("city Name already exists");
                }
                else
                {

                    SqlConnection connection = new SqlConnection(connectionString);
                    string query = "Insert into CityInformation_Table values('" + city.cityName + "','" + city.about + "','" +
                                   city.country + "')";
                    SqlCommand command = new SqlCommand(query, connection);

                    connection.Open();

                    int rowCount = command.ExecuteNonQuery();
                    if (rowCount > 0)
                    {
                        MessageBox.Show("Inserted Successfully");
                    }
                    else
                    {
                        MessageBox.Show("Insert Fail");
                    }

                    ClearAllTextBox();
                    ShowAllInformation();
                    connection.Close();
                }   
                }
            }
        }

        public bool IsCityNameExists(string city)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            string query = "Select * from CityInformation_Table where [City Name]='" + city + "'";
            SqlCommand command = new SqlCommand(query, connection);

            connection.Open();
            bool isCityNameExists = false;
           SqlDataReader reader= command.ExecuteReader();

            while (reader.Read())
            {
                isCityNameExists = true;
                break;
            }
            reader.Close();
            connection.Close();
            return isCityNameExists;
        }

        private void ShowAllInformation()
        {
            SqlConnection connection = new SqlConnection(connectionString);
            string query = "select * from CityInformation_Table";
            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            List<City> cityInfoDbList = new List<City>();
          SqlDataReader reader=  command.ExecuteReader();

          while (reader.Read())
          {
              City city = new City();
              city.cityName = reader["City Name"].ToString();
              city.about = reader["About"].ToString();
              city.country = reader["Country"].ToString();
              cityInfoDbList.Add(city);
          }
          reader.Close();
          connection.Close();
          LoadInformationIntoListview(cityInfoDbList);

        }
        private void LoadInformationIntoListview(List<City> aCity)
        {
            showOutputLlistView.Items.Clear();
          int count = 1;
            foreach (City city in aCity)
            {
                ListViewItem item = new ListViewItem(count.ToString());
                item.SubItems.Add(city.cityName);
                item.SubItems.Add(city.about);
                item.SubItems.Add(city.country);
                showOutputLlistView.Items.Add(item);
                count++;
            }
        }

        private City GetCityInformationBycityName(string cityName)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            string query = "select * from CityInformation_Table where [City Name] = '" + cityName + "'";
            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            List<City> cityInfoDbList = new List<City>();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                City city = new City();
                city.cityName = reader["City Name"].ToString();
                city.about = reader["About"].ToString();
                city.country = reader["Country"].ToString();
                cityInfoDbList.Add(city);

            }
            reader.Close();
            connection.Close();
            return cityInfoDbList.FirstOrDefault();
           //SqlDataReader reader = command.ExecuteReader();
           //while (reader.Read())
           //{
           //    City city = new City();
           //    city.cityName = reader[1].ToString();
           //    city.about = reader[2].ToString();
           //    city.country = reader[3].ToString();
           //    break;

           //}
           //reader.Close();
           //connection.Close();
           //return city;
        }
       
        private void searchButton_Click(object sender, EventArgs e)
        {
          string searchByText=  searchTextBox.Text;
          if (searchCountry)
          {
              SearchByCityOrCountry(searchByText,2);
          }
          else{
              SearchByCityOrCountry(searchByText, 1);
          }
           
        }

        private void SearchByCityOrCountry(string searchByText, int indexOperation)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            string query = "select * from CityInformation_Table ";
            SqlCommand command = new SqlCommand(query, connection);
            int count = 0;
            connection.Open();
            List<City> searchInfoList = new List<City>();
            SqlDataReader reader = command.ExecuteReader();

            if (indexOperation == 1)
            {
                while (reader.Read())
                {
                    City city = new City();
                    if (reader[1].ToString().Contains(searchByText) || reader[1].ToString().Contains(searchByText.ToUpper()) || reader[1].ToString().Contains(searchByText.ToLower()))
                    {
                        city.cityName = reader["City Name"].ToString();
                        city.about = reader["About"].ToString();
                        city.country = reader["Country"].ToString();
                        searchInfoList.Add(city);
                        count++;
                    }
                }
                reader.Close();
                connection.Close();
                if (count > 0)
                {
                    LoadInformationIntoListview(searchInfoList);
                }
                else
                {
                    MessageBox.Show("City not found");
                }
            }
            else
            {
                while (reader.Read())
                {
                    City city = new City();
                    if (reader[3].ToString().Contains(searchByText) || reader[3].ToString().Contains(searchByText.ToUpper()) || reader[3].ToString().Contains(searchByText.ToLower()))
                    {
                        city.cityName = reader["City Name"].ToString();
                        city.about = reader["About"].ToString();
                        city.country = reader["Country"].ToString();
                        searchInfoList.Add(city);
                        count++;
                    }
                }
                reader.Close();
                connection.Close();
                if (count > 0)
                {
                    LoadInformationIntoListview(searchInfoList);
                }
                else
                {
                    MessageBox.Show("Country not found");
                }
                ClearAllTextBox();
            }

        }
        private void showOutputLlistView_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListViewItem item = showOutputLlistView.SelectedItems[0];
           string cityName= item.SubItems[1].Text.ToString();
     
         City cityInformation = GetCityInformationBycityName(cityName);
         if (cityInformation != null)
         {
             isUpdateMode = true;
             cityNameId = cityInformation.cityName;
             cityNameTextBox.Text = cityInformation.cityName.ToString();
             aboutTextBox.Text = cityInformation.about.ToString();
             countryTextBox.Text = cityInformation.country.ToString();
             saveButton.Text = "Update";
             cityNameTextBox.Enabled = false;
            
         }
         
        }

        private void ClearAllTextBox()
        {
            cityNameTextBox.Clear();
            aboutTextBox.Clear();
            countryTextBox.Clear();
            searchTextBox.Clear();
        }

        private void CityInformationUI_Load(object sender, EventArgs e)
        {
            ShowAllInformation();
        }

        private void searchByCityRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            searchCity = true;
            searchCountry = false;
        }

        private void searchByCountryRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            searchCountry = true;
            searchCity = false;
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            string query = "delete from CityInformation_Table  where [City Name]='"+cityNameId+"'";
            SqlCommand command = new SqlCommand(query, connection);

            connection.Open();

            if (command.ExecuteNonQuery() > 0)
            {
                MessageBox.Show("Deleted Successfully");
                ShowAllInformation();
                ClearAllTextBox();
                cityNameId = "";
                saveButton.Text = "Save";
                cityNameTextBox.Enabled = true;
                isUpdateMode = false;
            }
            else
            {
                MessageBox.Show("Not found");
            }

        }


    }
}
