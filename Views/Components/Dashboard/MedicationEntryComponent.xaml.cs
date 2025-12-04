using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Controls;
using OvulaeShared.Models.Shared.Logs;

namespace OvulaeApp.Views.Components.Dashboard;

public partial class MedicationEntryComponent : ContentView, INotifyPropertyChanged
{
    public static readonly BindableProperty AddButtonTextProperty =
        BindableProperty.Create(
            nameof(AddButtonText),
            typeof(string),
            typeof(MedicationEntryComponent),
            "Add Medication",
            propertyChanged: OnAddButtonTextChanged);

    public string AddButtonText
    {
        get => (string)GetValue(AddButtonTextProperty);
        set => SetValue(AddButtonTextProperty, value);
    }

    private ObservableCollection<MedicationEntry> _medicationEntries;
    public ObservableCollection<MedicationEntry> MedicationEntries
    {
        get => _medicationEntries;
        set
        {
            _medicationEntries = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    // Public property to get all medication entries
    public List<MedicationModel> Medications
    {
        get => MedicationEntries
            .Where(m => !string.IsNullOrWhiteSpace(m.MedicationName) || !string.IsNullOrWhiteSpace(m.Dosage))
            .Select(m => new MedicationModel
            {
                Name = m.MedicationName,
                Dosage = m.Dosage
            })
            .ToList();
    }

    public MedicationEntryComponent()
    {
        InitializeComponent();
        BindingContext = this;

        MedicationEntries = new ObservableCollection<MedicationEntry>();

        // Add initial empty entry
        AddNewEntry();
    }

    private static void OnAddButtonTextChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is MedicationEntryComponent control)
        {
            control.AddButtonLabel.Text = newValue?.ToString();
        }
    }

    private void AddMedication_Tapped(object sender, EventArgs e)
    {
        AddNewEntry();
    }

    private void AddNewEntry()
    {
        var newEntry = new MedicationEntry();
        newEntry.RemoveCommand = new Command(() =>
        {
            // Only remove if it's not the last entry or if it's empty
            if (MedicationEntries.Count > 1 ||
                (string.IsNullOrWhiteSpace(newEntry.MedicationName) &&
                 string.IsNullOrWhiteSpace(newEntry.Dosage)))
            {
                MedicationEntries.Remove(newEntry);
            }
        });

        MedicationEntries.Add(newEntry);

        // Notify that medications have changed
        OnPropertyChanged(nameof(Medications));
    }

    private void MedicationEntry_Completed(object sender, EventArgs e)
    {
        // Notify that medications have changed when user finishes editing
        OnPropertyChanged(nameof(Medications));
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // Method to clear all entries
    public void ClearEntries()
    {
        MedicationEntries.Clear();
        AddNewEntry(); // Add one empty entry
    }

    // Method to load existing medications
    public void LoadMedications(List<MedicationModel> medications)
    {
        MedicationEntries.Clear();

        if (medications != null && medications.Any())
        {
            foreach (var med in medications)
            {
                var entry = new MedicationEntry
                {
                    MedicationName = med.Name,
                    Dosage = med.Dosage
                };

                entry.RemoveCommand = new Command(() =>
                {
                    MedicationEntries.Remove(entry);
                    OnPropertyChanged(nameof(Medications));
                });

                MedicationEntries.Add(entry);
            }
        }

        // Always add one empty entry at the end
        AddNewEntry();
    }
}

// ViewModel for individual medication entry
public class MedicationEntry : INotifyPropertyChanged
{
    private string _medicationName;
    private string _dosage;

    public string MedicationName
    {
        get => _medicationName;
        set
        {
            _medicationName = value;
            OnPropertyChanged();
        }
    }

    public string Dosage
    {
        get => _dosage;
        set
        {
            _dosage = value;
            OnPropertyChanged();
        }
    }

    public Command RemoveCommand { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

// Model for medication data
