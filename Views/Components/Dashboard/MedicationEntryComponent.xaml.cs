using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using OvulaeShared.Models.Shared.Logs;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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
        get
        {
            var result = new List<MedicationModel>();

            foreach (var entry in MedicationEntries)
            {
                // Check if entry has any data
                bool hasName = !string.IsNullOrWhiteSpace(entry.MedicationName);
                bool hasDosage = !string.IsNullOrWhiteSpace(entry.Dosage);

                if (hasName || hasDosage)
                {
                    result.Add(new MedicationModel
                    {
                        Name = entry.MedicationName ?? string.Empty,
                        Dosage = entry.Dosage ?? string.Empty
                    });
                }
            }

            return result;
        }
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
        // Force update of bindings
        if (sender is Entry entry)
        {
            entry.Unfocus(); // This should trigger binding update
        }

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

    private void OnMedicationTextChanged(object sender, TextChangedEventArgs e)
    {
        // Force update when text changes
        if (sender is Entry entry && entry.BindingContext is MedicationEntry medicationEntry)
        {
            // Force property change
            medicationEntry.OnPropertyChanged(nameof(medicationEntry.MedicationName));
            medicationEntry.OnPropertyChanged(nameof(medicationEntry.Dosage));

            // Update the Medications property
            OnPropertyChanged(nameof(Medications));
        }
    }

    public void ForceUpdateBindings()
    {
        // Force property change notifications
        OnPropertyChanged(nameof(MedicationEntries));
        OnPropertyChanged(nameof(Medications));

        // Force each entry to update
        foreach (var entry in MedicationEntries)
        {
            entry.OnPropertyChanged(nameof(entry.MedicationName));
            entry.OnPropertyChanged(nameof(entry.Dosage));
        }
    }

    public void DebugComponent()
    {
        Console.WriteLine("=== MedicationEntryComponent Debug ===");
        Console.WriteLine($"MedicationEntries count: {MedicationEntries?.Count}");

        if (MedicationEntries != null)
        {
            for (int i = 0; i < MedicationEntries.Count; i++)
            {
                var entry = MedicationEntries[i];
                Console.WriteLine($"Entry {i}: Name='{entry.MedicationName ?? "[null]"}', Dosage='{entry.Dosage ?? "[null]"}'");
            }
        }

        var meds = Medications;
        Console.WriteLine($"Medications property count: {meds?.Count}");
        if (meds != null)
        {
            foreach (var med in meds)
            {
                Console.WriteLine($"Med: Name='{med.Name ?? "[null]"}', Dosage='{med.Dosage ?? "[null]"}'");
            }
        }

        // Also serialize to see what JSON would look like
        var json = JsonConvert.SerializeObject(Medications);
        Console.WriteLine($"Medications JSON: {json}");
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

    public void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }


}

// Model for medication data
