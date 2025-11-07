using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OvulaeApp.Services.UserInterface.Components
{
    public interface ISpinnerService
    {
        Task ShowSpinnerAsync();

        Task HideSpinner();
    }
}
