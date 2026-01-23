using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainAppWindow.xaml
    /// </summary>
    public partial class MainAppWindow : Window
    {
        public ObservableCollection<Patient> Patients { get; set; }

        public MainAppWindow()
        {
            InitializeComponent();
            LoadSampleData();
            this.DataContext = this;
        }

        private void LoadSampleData()
        {
            Patients = new ObservableCollection<Patient>
            {
                new Patient
                {
                    CardNumber = "X260000001",
                    AppointmentDate = "15.02.2026",
                    FullName = "Иванов Иван Иванович",
                    BirthDate = "12.11.1999",
                    Phone = "+79961005555",
                    Gender = "М",
                    Department = "Терапия",
                    Diagnosis = "Халязион"
                }
                // Добавьте больше пациентов здесь
            };

            // Привязка DataGrid к коллекции
            if (MainTabControl.Items[0] is TabItem firstTab)
            {
                if (firstTab.Content is Grid grid)
                {
                    foreach (var child in grid.Children)
                    {
                        if (child is DataGrid dataGrid)
                        {
                            dataGrid.ItemsSource = Patients;
                            break;
                        }
                    }
                }
            }
        }

        private void AddNewTab_Click(object sender, RoutedEventArgs e)
        {
            TabItem newTab = new TabItem
            {
                Header = "Новая вкладка",
                Content = new TextBlock
                {
                    Text = "Содержимое новой вкладки",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                },
                Style = (Style)FindResource("BrowserTabStyle")
            };

            MainTabControl.Items.Insert(MainTabControl.Items.Count - 1, newTab);
            MainTabControl.SelectedItem = newTab;
        }
    }

    public class Patient
    {
        public string? CardNumber { get; set; }
        public string? AppointmentDate { get; set; }
        public string? FullName { get; set; }
        public string? BirthDate { get; set; }
        public string? Phone { get; set; }
        public string? Gender { get; set; }
        public string? Department { get; set; }
        public string? Diagnosis { get; set; }
    }
}
