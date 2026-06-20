using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace CyberSecurityAwarenessBot
{
    
    /// UI code-behind — handles user interaction and connects to ChatEngine.
    /// Keeps all logic OUT of this file — only UI event handling lives here.
    
    public partial class MainWindow : Window
    {
        //   Engine  
        private readonly ChatEngine _chatEngine;

        //   Colours for chat bubbles  
        private readonly SolidColorBrush _botBubbleColour = new SolidColorBrush(Color.FromRgb(42, 42, 62));
        private readonly SolidColorBrush _userBubbleColour = new SolidColorBrush(Color.FromRgb(58, 58, 92));
        private readonly SolidColorBrush _botTextColour = new SolidColorBrush(Color.FromRgb(0, 255, 156));
        private readonly SolidColorBrush _userTextColour = new SolidColorBrush(Color.FromRgb(224, 224, 224));
        private readonly SolidColorBrush _labelColour = new SolidColorBrush(Color.FromRgb(136, 136, 153));

        public MainWindow()
        {
        
            InitializeComponent();
            _chatEngine = new ChatEngine();
            Loaded += MainWindow_Loaded;

            // TEMPORARY — remove after confirming DB connects
            var dbTest = new CyberSecurityAwarenessBot.Services.DatabaseService();
            bool connected = dbTest.TestConnection();
            MessageBox.Show(connected ? "✅ Database connected!" : "❌ Database connection failed.");
        }
        

         
        /// Fires when the window first loads — plays voice greeting and shows welcome message.
        
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            PlayVoiceGreeting();
            ShowBotMessage(_chatEngine.GetWelcomeMessage());
        }

        // Event handlers  

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        private void UserInputBox_KeyDown(object sender, KeyEventArgs e)
        {
            // Press Enter to send
            if (e.Key == Key.Enter)
                SendMessage();
        }

        //   Core send logic  


        /// Reads input, shows user bubble, gets bot response, shows bot bubble.

        private void SendMessage()
        {
            string userInput = UserInputBox.Text;

            if (!string.IsNullOrWhiteSpace(userInput))
                ShowUserMessage(userInput);

            // Check for exit command before processing
            if (_chatEngine.IsExitCommand(userInput))
            {
                ShowBotMessage(_chatEngine.GetFarewellMessage());
                UserInputBox.Clear();

                // Disable input so user knows chat has ended
                UserInputBox.IsEnabled = false;
                SendButton.IsEnabled = false;
                UserInputBox.Text = "Chat ended. Close the window to exit.";
                return;
            }

            string response = _chatEngine.ProcessInput(userInput);
            ShowBotMessage(response);

            UserInputBox.Clear();
            UserInputBox.Focus();
            ChatScrollViewer.ScrollToBottom();
        }

        //   Chat bubble builders  


        /// Adds a bot message bubble to the chat panel.

        private void ShowBotMessage(string message)
        {
            var container = new StackPanel { Margin = new Thickness(0, 6, 0, 6) };

            // Label
            var label = new TextBlock
            {
                Text = "CyberBot",
                Foreground = _labelColour,
                FontSize = 11,
                Margin = new Thickness(4, 0, 0, 2)
            };

            // Bubble
            var bubble = new Border
            {
                Background = _botBubbleColour,
                CornerRadius = new CornerRadius(4, 12, 12, 12),
                Padding = new Thickness(14, 10, 14, 10),
                MaxWidth = 580,
                HorizontalAlignment = HorizontalAlignment.Left
            };

            var text = new TextBlock
            {
                Text = message,
                Foreground = _botTextColour,
                FontSize = 13,
                TextWrapping = TextWrapping.Wrap,
                LineHeight = 20
            };

            bubble.Child = text;
            container.Children.Add(label);
            container.Children.Add(bubble);
            ChatPanel.Children.Add(container);
        }

        
        /// Adds a user message bubble to the chat panel.
         
        private void ShowUserMessage(string message)
        {
            var container = new StackPanel
            {
                Margin = new Thickness(0, 6, 0, 6),
                HorizontalAlignment = HorizontalAlignment.Right
            };

            // Label
            var label = new TextBlock
            {
                Text = "You",
                Foreground = _labelColour,
                FontSize = 11,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, 4, 2)
            };

            // Bubble
            var bubble = new Border
            {
                Background = _userBubbleColour,
                CornerRadius = new CornerRadius(12, 4, 12, 12),
                Padding = new Thickness(14, 10, 14, 10),
                MaxWidth = 580,
                HorizontalAlignment = HorizontalAlignment.Right
            };

            var text = new TextBlock
            {
                Text = message,
                Foreground = _userTextColour,
                FontSize = 13,
                TextWrapping = TextWrapping.Wrap,
                LineHeight = 20
            };

            bubble.Child = text;
            container.Children.Add(label);
            container.Children.Add(bubble);
            ChatPanel.Children.Add(container);
        }

        //   Voice greeting  

         
        /// Plays the WAV voice greeting if the file exists.
        /// Carried forward from Part 1 requirement.
         
        private void PlayVoiceGreeting()
        {
            try
            {
                string audioPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "greeting.wav");

                if (File.Exists(audioPath))
                {
                    var player = new System.Media.SoundPlayer(audioPath);
                    player.Play();
                }
            }
            catch (Exception ex)
            {
                // Fail silently — voice greeting is enhancement, not core functionality
                Console.WriteLine($"Voice greeting error: {ex.Message}");
            }
        }
    }
}