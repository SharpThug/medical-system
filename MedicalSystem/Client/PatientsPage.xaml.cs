using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;

namespace Client
{
    public partial class PatientsPage : UserControl
    {
        public ObservableCollection<Patient> Patients { get; set; }
        private ICollectionView _patientsView;

        public event Action RequestNewPatientTab;

        public PatientsPage()
        {
            InitializeComponent();
            LoadDummyPatients();
        }

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

        private bool FilterPatients(object obj)
        {
            if (obj is not Patient p) return false;

            var from = DateFromPicker.SelectedDate;
            var to = DateToPicker.SelectedDate;

            if (from.HasValue && p.AppointmentDate < from.Value) return false;
            if (to.HasValue && p.AppointmentDate > to.Value) return false;

            return true;
        }

        private void DateFilter_Changed(object sender, SelectionChangedEventArgs e)
        {
            _patientsView?.Refresh();
        }

        private void NewPatient_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            RequestNewPatientTab?.Invoke();
        }
    }

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
