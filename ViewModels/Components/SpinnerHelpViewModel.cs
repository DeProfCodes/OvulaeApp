using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace OvulaeApp.ViewModels.Components
{
    public class ShowSpinnerMessage : ValueChangedMessage<bool>
    {
        public ShowSpinnerMessage(bool value) : base(value)
        {

        }
    }
}
