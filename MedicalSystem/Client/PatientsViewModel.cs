using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shared;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Client
{
    public partial class PatientsViewModel : ObservableObject
    {
        private readonly IPatientService _patientService;
        private ICollectionView _filteredPatientsView;

        [ObservableProperty]
        private DateTime? _dateFrom;

        [ObservableProperty]
        private DateTime? _dateTo;

        public ObservableCollection<Patient> Patients { get; } = new ObservableCollection<Patient>();

        public ICollectionView FilteredPatients
        {
            get => _filteredPatientsView;
            private set => SetProperty(ref _filteredPatientsView, value);
        }

        public event Action? RequestNewPatientTab;

        public PatientsViewModel(IPatientService patientService)
        {
            _patientService = patientService;

            // Создаем view для фильтрации
            FilteredPatients = CollectionViewSource.GetDefaultView(Patients);
            FilteredPatients.Filter = FilterPatients;

            // Подписываемся на изменения свойств фильтрации
            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(DateFrom) || e.PropertyName == nameof(DateTo))
                {
                    FilteredPatients?.Refresh();
                }
            };
        }

        private bool FilterPatients(object obj)
        {
            if (obj is not Patient p) return false;

            if (DateFrom.HasValue && p.CreatedDate < DateFrom.Value.Date) return false;
            if (DateTo.HasValue && p.CreatedDate > DateTo.Value.Date) return false;

            return true;
        }

        [RelayCommand]
        private void NewPatient()
        {
            RequestNewPatientTab?.Invoke();
        }

        public async Task LoadPatientsAsync()
        {
            if (_patientService == null) return;

            try
            {
                var response = await _patientService.GetPatientsAsync(60);

                if (response.Success)
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        Patients.Clear();
                        foreach (var p in response.Data)
                            Patients.Add(p);

                        FilteredPatients?.Refresh();
                    });
                }
                else
                {
                    // Обработка ошибки
                    // Можно использовать ILogger или Messenger для показа уведомлений
                }
            }
            catch (Exception ex)
            {
                // Обработка исключений
            }
        }
    }
}