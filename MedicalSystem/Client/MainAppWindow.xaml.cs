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
            LoadDummyPatients();
            this.DataContext = this;
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

        private void LoadDummyPatients()
        {
            var patients = new List<Patient>();

            for (int i = 1; i <= 50; i++)
            {
                patients.Add(new Patient
                {
                    CardNumber = $"CARD{i:000}",
                    AppointmentDate = $"13.0{i % 12 + 1}.2026",
                    FullName = $"Пациент {i}",
                    BirthDate = $"01.0{i % 12 + 1}.198{i % 10}",
                    Phone = $"+7 900 000 0{i:00}{i:00}",
                    Gender = i % 2 == 0 ? "М" : "Ж",
                    Department = i % 3 == 0 ? "Терапия" : "Хирургия",
                    Diagnosis = i % 2 == 0 ? "ОРВИ" : "Грипп"
                });
            }

            MainPatientsDataGrid.ItemsSource = patients; // <-- Имя DataGrid
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
