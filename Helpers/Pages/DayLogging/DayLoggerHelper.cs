using OvulaeApp.ViewModels.Dashboard;
using OvulaeApp.Views.Components.Dashboard;
using OvulaeShared.Models.Shared.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OvulaeShared.Helpers.API.OvulaeApiEndPoints;

namespace OvulaeApp.Helpers.Pages.DayLogging
{
    public class DayLoggerHelper
    {
        public static List<string> YesNoOption = new List<string>
        {
            "✅ Yes", "❌ No"
        };

        public static string GetYesNoSelected(bool? value)
        {
            var result = (value != null && value.HasValue) ? (value.Value ? YesNoOption[0] : YesNoOption[1]) : "";

            return result;
        }

        public static bool? GetBooleanFromYesNoSelection(MultiSelectLogItemComponent component, bool? unchanged = null)
        {
            if (component == null || component.SelectedItems.Count() == 0)
                return unchanged;

            return component.SelectedItems.FirstOrDefault().Contains(YesNoOption[0]);
        }

        public static bool GetBooleanFromYesNoSelection2(MultiSelectLogItemComponent component, bool? unchanged = false)
        {
            var answer = GetBooleanFromYesNoSelection(component, unchanged);
            return answer != null ? answer.Value : false;
        }

        public static void PopulateMultiSelectComponent(MultiSelectLogItemComponent component, List<string> allOptions, List<string> selectedOptions)
        {
            if (component == null || allOptions == null)
                return;

            component.Items.Clear();

            foreach (var option in allOptions)
            {
                var item = new SelectableItem(option.Trim(), component, selectedOptions?.Contains(option.Trim()) == true);
                component.Items.Add(item);
            }
        }

        public static void PopulateMultiSelectComponent(MultiSelectLogItemComponent component, List<string> allOptions, string selectedOption)
        {
            if (component == null || allOptions == null)
                return;

            component.Items.Clear();

            foreach (var option in allOptions)
            {
                bool isSelected = selectedOption != null && option == selectedOption;
                var item = new SelectableItem(option, component, isSelected);
                component.Items.Add(item);
            }
        }

        public static bool NoChangeInLogs(List<string> list1, List<string> list2)
        {
            if(list1 == null || list2 == null)
                return false;

            bool sameItems = list1.Count == list2.Count && !list1.Except(list2).Any(); 

            return sameItems;
        }

        public static void PopulateMedicationComponent(MedicationEntryComponent component, List<MedicationModel> medications)
        {
            if (component == null) return;

            component.ClearEntries();

            if (medications != null && medications.Any())
            {
                component.LoadMedications(medications);
            }
            else
            {
                // Ensure there's at least one empty entry
                component.ClearEntries();
            }
        }
    }
}
