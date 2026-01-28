using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using Shared;

namespace Client
{
    public partial class PatientsPage : UserControl
    {
        public ObservableCollection<Patient> Patients { get; set; }
        private ICollectionView _patientsView;

        public event Action RequestNewPatientTab;

        private IPatientService _patientService;

        public PatientsPage()
        {
            InitializeComponent();
        }

        public void SetPatientService(IPatientService patientService)
        {
            _patientService = patientService;
            LoadPatientsAsync();
        }



        private async Task LoadPatientsAsync()
        {
            if (_patientService == null) return;

            ApiResponse<List<Patient>> response = await _patientService.GetPatientsAsync(60);

            if (response.Success)
            {
                Patients = new ObservableCollection<Patient>(response.Data!);

                // Создаем view для фильтрации
                _patientsView = CollectionViewSource.GetDefaultView(Patients);
                _patientsView.Filter = FilterPatients;

                // Привязываем к DataGrid
                MainPatientsDataGrid.ItemsSource = _patientsView;
            }
            else
            {
                //обработка ошибки
            }

        }

        private bool FilterPatients(object obj)
        {
            /*if (obj is not Patient p) return false;

            var from = DateFromPicker.SelectedDate;
            var to = DateToPicker.SelectedDate;

            if (from.HasValue && p.AppointmentDate < from.Value) return false;
            if (to.HasValue && p.AppointmentDate > to.Value) return false;*/

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
}
