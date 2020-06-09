using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CourseworkFinal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_SendQuery(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Query.Text))
            {
                string title = "Empty query";
                string messageText = "You did not enter anything";
                MessageBoxButton buttons = MessageBoxButton.OK;
                MessageBoxImage image = MessageBoxImage.Error;
                MessageBox.Show(messageText, title, buttons, image);
            }
            else
            {
                string result = "";
                try
                {
                    result = await SendQuery(Query.Text);
                    JObject resultObject = JObject.Parse(result);
                    JArray resultArray = resultObject.Value<dynamic>("list");

                    List<Definition> listResult = new List<Definition>();
                    foreach (var definition in resultArray)
                    {
                        string definitionText = definition.Value<string>("definition").Replace("[", String.Empty).Replace("]", String.Empty);
                        string word = definition.Value<string>("word").Replace("[", String.Empty).Replace("]", String.Empty);
                        string author = definition.Value<string>("author").Replace("[", String.Empty).Replace("]", String.Empty);
                        string example = definition.Value<string>("example").Replace("[", String.Empty).Replace("]", String.Empty);
                        int rating = definition.Value<int>("thumbs_up");

                        listResult.Add(new Definition() { example = example, word = word, author = author, thumbs_up = rating, definition = definitionText });

                    }
                    //Set result list as the data table for results
                    ResultTable.ItemsSource = listResult;
                }
                catch (Exception exception)
                {
                    MessageBox.Show($"Something went wrong, please try again \n ${exception.Message}");
                }
            }
        }

        private async Task<string> SendQuery(string term)
        {
            RestClient client = new RestClient($"https://mashape-community-urban-dictionary.p.rapidapi.com/define?term={term}");
            RestRequest request = new RestRequest(Method.GET);
            request.AddHeader("x-rapidapi-host", "mashape-community-urban-dictionary.p.rapidapi.com");
            request.AddHeader("x-rapidapi-key", "d130b288eemshbad0641ebc8d771p147389jsnfb282aebccfb");
            IRestResponse response = await client.ExecuteAsync(request);

            return response.Content;
        }

    }
}
