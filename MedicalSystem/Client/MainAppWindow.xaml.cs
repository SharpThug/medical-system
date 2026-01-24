using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Client
{
    public partial class MainAppWindow : Window
    {
        public ObservableCollection<Patient> Patients { get; set; }

        private ICollectionView _patientsView;

        public MainAppWindow()
        {
            InitializeComponent();

            LoadDummyPatients();
        }


        // =========================================================
        // ЗАГРУЗКА ДАННЫХ
        // =========================================================
        private void LoadDummyPatients()
        {
            Patients = new ObservableCollection<Patient>();

            for (int i = 1; i <= 50; i++)
            {
                Patients.Add(new Patient
                {
                    CardNumber = $"CARD{i:000}",
                    AppointmentDate = new DateTime(2026, i % 12 + 1, 13),
                    BirthDate = new DateTime(1980 + i % 10, i % 12 + 1, 1),
                    FullName = $"Пациент {i}",
                    Phone = $"+7 900 000 0{i:00}{i:00}",
                    Gender = i % 2 == 0 ? "М" : "Ж",
                    Department = i % 3 == 0 ? "Терапия" : "Хирургия",
                    Diagnosis = i % 2 == 0 ? "ОРВИ" : "Грипп"
                });
            }

            _patientsView = CollectionViewSource.GetDefaultView(Patients);
            _patientsView.Filter = FilterPatients;

            MainPatientsDataGrid.ItemsSource = _patientsView;
        }


        // =========================================================
        // ФИЛЬТР
        // =========================================================
        private bool FilterPatients(object obj)
        {
            if (obj is not Patient p)
                return false;

            var from = DateFromPicker.SelectedDate;
            var to = DateToPicker.SelectedDate;

            if (from.HasValue && p.AppointmentDate < from.Value)
                return false;

            if (to.HasValue && p.AppointmentDate > to.Value)
                return false;

            return true;
        }


        // =========================================================
        // СОБЫТИЕ ИЗМЕНЕНИЯ ДАТЫ
        // =========================================================
        private void DateFilter_Changed(object sender, SelectionChangedEventArgs e)
        {
            _patientsView?.Refresh();
        }


        // =========================================================
        // СБРОС ФИЛЬТРА
        // =========================================================
        private void ResetFilter_Click(object sender, RoutedEventArgs e)
        {
            DateFromPicker.SelectedDate = null;
            DateToPicker.SelectedDate = null;
            _patientsView.Refresh();
        }
    }


    // =============================================================
    // МОДЕЛЬ
    // =============================================================
    public class Patient
    {
        public string CardNumber { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string FullName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; }
        public string Department { get; set; }
        public string Diagnosis { get; set; }
    }
}
