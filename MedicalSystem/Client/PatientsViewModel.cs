using Shared;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace Client
{
    public class PatientsViewModel : INotifyPropertyChanged
    {
        private readonly IPatientService _patientService;
        private ICollectionView _patientsView;

        public ICommand NewPatientCommand { get; }

        public ObservableCollection<Patient> Patients { get; set; } = new ObservableCollection<Patient>();

        public ICollectionView PatientsView
        {
            get => _patientsView;
            private set { _patientsView = value; OnPropertyChanged(nameof(PatientsView)); }
        }

        private DateTime? _dateFrom;
        public DateTime? DateFrom
        {
            get => _dateFrom;
            set { _dateFrom = value; OnPropertyChanged(nameof(DateFrom)); PatientsView?.Refresh(); }
        }

        private DateTime? _dateTo;
        public DateTime? DateTo
        {
            get => _dateTo;
            set { _dateTo = value; OnPropertyChanged(nameof(DateTo)); PatientsView?.Refresh(); }
        }

        public event Action RequestNewPatientTab;

        public PatientsViewModel(IPatientService patientService)
        {
            _patientService = patientService;

            // Создаем view для фильтрации
            PatientsView = CollectionViewSource.GetDefaultView(Patients);
            PatientsView.Filter = FilterPatients;

            NewPatientCommand = new RelayCommand(NewPatientCommandExecute);
        }

        private bool FilterPatients(object obj)
        {
            if (obj is not Patient p) return false;

            if (DateFrom.HasValue && p.CreatedDate < DateFrom.Value) return false;
            if (DateTo.HasValue && p.CreatedDate > DateTo.Value) return false;

            return true;
        }

        public async void LoadPatientsAsync()
        {
            if (_patientService == null) return;

            var response = await _patientService.GetPatientsAsync(60);

            if (response.Success)
            {
                Patients.Clear();
                foreach (var p in response.Data)
                    Patients.Add(p);

                PatientsView.Refresh();
            }
            else
            {
                // Обработка ошибки
            }
        }

        public void NewPatientCommandExecute()
        {
            RequestNewPatientTab?.Invoke();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
