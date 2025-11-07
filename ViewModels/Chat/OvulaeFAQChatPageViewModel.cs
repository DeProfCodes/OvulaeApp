using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Services.LocalDataService.ChatBot;
using OvulaeShared.Enums;
using OvulaeShared.Enums.App;
using OvulaeShared.Services.Email;

namespace OvulaeApp.ViewModels.Chat
{
    public partial class OvulaeFAQChatPageViewModel : BaseViewModel
    {
        public ObservableCollection<ChatMessage> ChatMessages { get; set; } = new();

        public ObservableCollection<ChatMessage> _faqQuestions;
        public ObservableCollection<ChatMessage> FaqQuestions 
        {
            get => _faqQuestions;
            set
            {
                _faqQuestions = value;
                OnPropertyChanged();
            }
        }

        private int _displayedFAQCount = 3;
        public int DisplayedFAQCount
        {
            get => _displayedFAQCount;
            set => SetProperty(ref _displayedFAQCount, value);
        }

        private bool _showCustomInputBox;
        public bool ShowCustomInputBox
        {
            get => _showCustomInputBox;
            set => SetProperty(ref _showCustomInputBox, value);
        }

        private double _dotOpacity1;
        public double DotOpacity1
        {
            get => _dotOpacity1;
            set => SetProperty(ref _dotOpacity1, value);
        }

        private double _dotOpacity2;
        public double DotOpacity2
        {
            get => _dotOpacity2;
            set => SetProperty(ref _dotOpacity2, value);
        }

        private double _dotOpacity3;
        public double DotOpacity3
        {
            get => _dotOpacity3;
            set => SetProperty(ref _dotOpacity3, value);
        }

        private string faqModuleType;

        public IRelayCommand ShowMoreFAQsCommand { get; }
        public IRelayCommand ShowCustomInputBoxCommand { get; }
        public IRelayCommand<string> SendCustomUserMessageCommand { get; }
        public IRelayCommand<ChatMessage> SendFAQQuestionCommand { get; }
        public IRelayCommand AskAnotherCommand { get; }
        public Command<string> OnTrackerTypeTappedCommand { get; }

        private CancellationTokenSource typingAnimationCts;

        private CollectionView ChatCollection;

        private readonly IChatBotService _chatBot;
        private readonly IOvulaeEmailService _emailServ;

        private List<ChatFAQItem> AllModulesFAQs;

        public OvulaeFAQChatPageViewModel(CollectionView ChatCollection, IChatBotService chatBot, IOvulaeEmailService emailServ)
        {
            this.ChatCollection = ChatCollection;
            _chatBot = chatBot;
            _emailServ = emailServ;

            SendFAQQuestionCommand = new RelayCommand<ChatMessage>(SendFAQQuestion);
            AskAnotherCommand = new RelayCommand(OnAskAnother);
            ShowMoreFAQsCommand = new RelayCommand(ShowMoreFAQs);
            SendCustomUserMessageCommand = new RelayCommand<string>(SendCustomUserMessage);
            ShowCustomInputBoxCommand = new RelayCommand(() => ShowCustomInputBox = true);
            OnTrackerTypeTappedCommand = new Command<string>(OnTrackerTypeTapped);
        }

        public void LoadInitialWelcome()
        {
            ChatMessages.Clear();

            AllModulesFAQs = _chatBot.GetAllModulesFAQs();

            var primaryModule = LocalStorageService.AppPrimaryGoal;
            faqModuleType = primaryModule.GetDisplayShortName();

            FaqQuestions = new();

            UpdateModuleFAQs(primaryModule);

            var userFirstName = LocalStorageService.UserDetails?.Firstname ?? "there";
            ChatMessages.Add(new ChatMessage
            {
                Text = $"Hi {userFirstName}, welcome to the Ovulae Help Center.\n\nHow can we help you today?",
                IsUser = false,
                IsFAQVisible = true,
                IsFAQContainer = true,
                ShowShowMoreButton = FaqQuestions.Count > DisplayedFAQCount,
                ShowingRowsCount = $"...showing {DisplayedFAQCount} of {FaqQuestions.Count}",
                FAQModuleType = faqModuleType,
            });

            ShowCustomInputBox = false;
        }

        public void UpdateModuleFAQs(ModuleType moduleType)
        {
            var data = AllModulesFAQs.Where(x => (moduleType != ModuleType.All ? x.ModuleType == moduleType : true)).ToList();

            DisplayedFAQCount = 3;

            FaqQuestions = new ObservableCollection<ChatMessage>(
                data.Select(faq => new ChatMessage
                {
                    Text = faq.Question,
                    Answer = faq.Answer,
                    IsUser = true,
                    ShowShowMoreButton = data.Count > DisplayedFAQCount,
                    ShowingRowsCount = $"...showing {DisplayedFAQCount} of {data.Count}",
                    FAQModuleType = faqModuleType,
                })
            );
        }

        private async Task AnimateDots(ChatMessage msg, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await AnimateDotOpacity(msg, 1, token);
                await AnimateDotOpacity(msg, 2, token);
                await AnimateDotOpacity(msg, 3, token);
                await ResetDots(msg);
            }
        }

        private void ShowMoreFAQs()
        {
            if (DisplayedFAQCount < FaqQuestions.Count)
            {
                DisplayedFAQCount += 3;
                if (DisplayedFAQCount >= FaqQuestions.Count)
                {
                    DisplayedFAQCount = FaqQuestions.Count;
                }
            }

            var lastFaqMessage = ChatMessages.LastOrDefault(m => m.IsFAQContainer && m.IsFAQVisible);
            if (lastFaqMessage != null)
            {
                lastFaqMessage.ShowShowMoreButton = DisplayedFAQCount < FaqQuestions.Count;
                lastFaqMessage.ShowingRowsCount = $"...showing {DisplayedFAQCount} of {FaqQuestions.Count}";
            }
        }

        private async void SendFAQQuestion(ChatMessage question)
        {
            if (question == null) return;

            HideActionBtnsInPrevMsg();

            ShowCustomInputBox = false;

            ChatMessages.Add(new ChatMessage
            {
                Text = question.Text,
                IsUser = true
            });

            ForceScrollToLastBottom();

            await StartBotTyping();

            ChatMessages.Add(new ChatMessage
            {
                Text = question.Answer,
                IsUser = false,
                IsFAQResponse = true,
                IsFAQContainer = true,
                IsFAQVisible = false  
            });

            ForceScrollToLastBottom();
        }

        private async Task StartBotTyping(ChatMessage typingMessage = null, bool close = true)
        {
            typingMessage = new ChatMessage
            {
                IsUser = false,
                IsTyping = true
            };

            ChatMessages.Add(typingMessage);

            await Application.Current.Dispatcher.DispatchAsync(() =>
            {
                ChatCollection.ScrollTo(typingMessage);
            });

            var cts = new CancellationTokenSource();
            _ = AnimateDots(typingMessage, cts.Token);

            typingAnimationCts = cts;

            if (close)
            {
                await Task.Delay(1000);

                ChatMessages.Remove(typingMessage);

                typingAnimationCts?.Cancel();
                typingAnimationCts = null;
                typingMessage.IsTyping = false;
            }
        }

        private async Task StopBotTyping(ChatMessage typingMessage)
        {
            ChatMessages.Remove(typingMessage);

            typingAnimationCts?.Cancel();
            typingAnimationCts = null;
            typingMessage.IsTyping = false;
        }

        private void OnTrackerTypeTapped(string trackerModuleType)
        {
            try
            {
                var moduleType = EnumHelper.GetEnumValueFromShortName<ModuleType>(trackerModuleType);
                faqModuleType = trackerModuleType;

                DisplayedFAQCount = 3;

                UpdateModuleFAQs(moduleType);

                var lastFaqMessage = ChatMessages.LastOrDefault(m => m.IsFAQContainer);
                if (lastFaqMessage != null)
                {
                    lastFaqMessage.FAQModuleType = faqModuleType;
                    lastFaqMessage.ShowShowMoreButton = FaqQuestions.Count > DisplayedFAQCount;
                    lastFaqMessage.ShowingRowsCount = $"...showing {DisplayedFAQCount} of {FaqQuestions.Count}";
                }
                ShowCustomInputBox = false;
            }
            catch (Exception ex)
            {
              
            }
        }

        private void OnAskAnother()
        {
            HideActionBtnsInPrevMsg();

            ChatMessages.Add(new ChatMessage
            {
                Text = "Ask another question",
                IsUser = true
            });

            ForceScrollToLastBottom();

            DisplayedFAQCount = 3;

            ChatMessages.Add(new ChatMessage
            {
                Text = "How can we help you today?",
                IsUser = false,
                IsFAQVisible = true,
                IsFAQContainer = true,
                ShowShowMoreButton = FaqQuestions.Count > DisplayedFAQCount,
                ShowingRowsCount = $"...showing {DisplayedFAQCount} of {FaqQuestions.Count}",
                FAQModuleType = faqModuleType
            });

            ForceScrollToLastBottom();

            ShowCustomInputBox = false;
        }

        private async void SendCustomUserMessage(string userInput)
        {
            if (string.IsNullOrWhiteSpace(userInput)) return;

            foreach (var msg in ChatMessages)
            {
                msg.IsFAQContainer = false;
            }

            ChatMessages.Add(new ChatMessage
            {
                Text = userInput.Trim(),
                IsUser = true
            });

            ForceScrollToLastBottom();

            var typingMessage = new ChatMessage
            {
                IsUser = false,
                IsTyping = true
            };

            ChatMessages.Add(typingMessage);

            await Application.Current.Dispatcher.DispatchAsync(() =>
            {
                ChatCollection.ScrollTo(typingMessage);
            });

            var cts = new CancellationTokenSource();
            _ = AnimateDots(typingMessage, cts.Token);

            typingAnimationCts = cts;

            var fullname = $"{LocalStorageService.UserDetails.Firstname} {LocalStorageService.UserDetails.Lastname}";
            var email = LocalStorageService.UserDetails.Email;
            var sendMessageStatus = await _emailServ.SendOvulaeAppUserQueryEmail(fullname, email, userInput);

            ChatMessages.Remove(typingMessage);

            typingAnimationCts?.Cancel();
            typingAnimationCts = null;
            typingMessage.IsTyping = false;


            if (sendMessageStatus)
            {
                ChatMessages.Add(new ChatMessage
                {
                    Text = $"Thank you for reaching out. One of our trusted gynecologists will review your message and reply to you to your email '{email}' as soon as possible.",
                    IsUser = false,
                    IsFAQResponse = true,
                    IsFAQContainer = true,
                    IsFAQVisible = false
                });
            }
            else
            {
                ChatMessages.Add(new ChatMessage
                {
                    Text = "We’re sorry, but it looks like your message couldn’t be sent right now. Please try again later, or email your query directly to us at queries@ovulae.com, and one of our trusted gynecologists will get back to you.",
                    IsUser = false,
                    IsFAQResponse = true,
                    IsFAQContainer = true,
                    IsFAQVisible = false
                });
            }

            ForceScrollToLastBottom();

            ShowCustomInputBox = false;
        }

        private void HideActionBtnsInPrevMsg()
        {
            // Hide FAQ buttons on all previous messages
            for (int i = 0; i < ChatMessages.Count; i++)
            {
                var msg = ChatMessages[i];
                msg.IsFAQContainer = false;
                msg.IsFAQVisible = false;
                msg.ShowShowMoreButton = false;
            }
        }

        private async void ForceScrollToLastBottom()
        {
            await Application.Current.Dispatcher.DispatchAsync(() =>
            {
                ChatCollection.ScrollTo(ChatMessages.Last(), position: ScrollToPosition.End, animate: true);
            });
        }

        private async Task AnimateDotOpacity(ChatMessage msg, int dotNumber, CancellationToken token)
        {
            const int fadeDuration = 300; // milliseconds

            try
            {
                double currentOpacity = 0;
                double targetOpacity = 1;
                int steps = 10;
                int delayPerStep = fadeDuration / steps;

                for (int i = 0; i <= steps && !token.IsCancellationRequested; i++)
                {
                    double opacity = currentOpacity + (targetOpacity - currentOpacity) * i / steps;
                    SetDotOpacity(msg, dotNumber, opacity);
                    await Task.Delay(delayPerStep, token);
                }
            }
            catch (TaskCanceledException)
            {
                // safely ignore cancellation
            }
        }

        private async Task ResetDots(ChatMessage msg)
        {
            DotOpacity1 = 0;
            DotOpacity2 = 0;
            DotOpacity3 = 0;
            await Task.CompletedTask;
        }

        private void SetDotOpacity(ChatMessage msg, int dotNumber, double opacity)
        {
            switch (dotNumber)
            {
                case 1: DotOpacity1 = opacity; break;
                case 2: DotOpacity2 = opacity; break;
                case 3: DotOpacity3 = opacity; break;
            }
        }
    }
}
