using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace OvulaeApp.ViewModels.Chat
{
    public class ChatMessage : BaseViewModel
    {
        public string Text { get; set; }

        public string Answer { get; set; }

        public bool IsUser { get; set; }

        private bool _isFAQVisible;
        public bool IsFAQVisible 
        {
            get => _isFAQVisible;
            set
            {
                _isFAQVisible = value;
                OnPropertyChanged();
            }
        }
        
        public bool IsTyping { get; set; }

        public bool IsFAQResponse { get; set; }

        private bool _isFAQContainer;
        public bool IsFAQContainer
        {
            get => _isFAQContainer;
            set
            {
                if (_isFAQContainer != value)
                {
                    _isFAQContainer = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _showShowMoreButton;
        public bool ShowShowMoreButton
        {
            get => _showShowMoreButton;
            set
            {
                if (_showShowMoreButton != value)
                {
                    _showShowMoreButton = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _showingRowsCount;
        public string ShowingRowsCount
        {
            get => _showingRowsCount;
            set => SetProperty(ref _showingRowsCount, value);
        }

        private bool _isFAQActionContainer;
        public bool IsFAQActionContainer
        {
            get => _isFAQActionContainer;
            set => SetProperty(ref _isFAQActionContainer, value);
        }

        private string _showingModuleFAQ;
        public string ShowingModuleFAQ
        {
            get => _showingModuleFAQ;
            set => SetProperty(ref _showingModuleFAQ, value);
        }

        private string _fAQModuleType;
        public string FAQModuleType
        {
            get => _fAQModuleType;
            set => SetProperty(ref _fAQModuleType, value);
        }
    }
}
