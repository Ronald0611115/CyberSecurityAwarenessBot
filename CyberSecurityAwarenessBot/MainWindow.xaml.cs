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
using CyberSecurityAwarenessBot.Services;
using CyberSecurityAwarenessBot.Models;

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
        private readonly DatabaseService _databaseService;
        private readonly TaskManager _taskManager;
        private readonly QuizManager _quizManager;
        private readonly ActivityLogger _activityLogger;
        private readonly NlpProcessor _nlpProcessor;

        public MainWindow()
        {
            InitializeComponent();
            _chatEngine = new ChatEngine();
            _databaseService = new DatabaseService();
            _taskManager = new TaskManager(_databaseService);
            _quizManager = new QuizManager();
            _activityLogger = new ActivityLogger();
            _nlpProcessor = new NlpProcessor();
            Loaded += MainWindow_Loaded;
        }



        /// Fires when the window first loads — plays voice greeting and shows welcome message.

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            PlayVoiceGreeting();
            ShowBotMessage(_chatEngine.GetWelcomeMessage());
            RefreshTaskList();
            ShowQuizStartScreen();

            _activityLogger.Log("Bot launched — voice greeting played");
            RefreshActivityLog();
        }

        // Quiz

        private void ShowQuizStartScreen()
        {
            QuizContentPanel.Children.Clear();

            QuizContentPanel.Children.Add(new TextBlock
            {
                Text = "Test your cybersecurity knowledge with 12 questions covering " +
                       "passwords, phishing, safe browsing, and social engineering.",
                Foreground = _userTextColour,
                FontSize = 13,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 16)
            });

            var startButton = new Button
            {
                Content = "Start Quiz",
                Style = (Style)FindResource("ActionButtonStyle"),
                HorizontalAlignment = HorizontalAlignment.Left
            };
            startButton.Click += (s, e) =>
            {
                _quizManager.StartNewQuiz();
                ShowQuizQuestion();
            };

            QuizContentPanel.Children.Add(startButton);
        }

        private void ShowQuizQuestion()
        {
            QuizContentPanel.Children.Clear();

            var question = _quizManager.GetCurrentQuestion();
            if (question == null)
            {
                ShowQuizResults();
                return;
            }

            QuizContentPanel.Children.Add(new TextBlock
            {
                Text = $"Question {_quizManager.CurrentQuestionNumber} of {_quizManager.TotalQuestions}",
                Foreground = _labelColour,
                FontSize = 12,
                Margin = new Thickness(0, 0, 0, 8)
            });

            QuizContentPanel.Children.Add(new TextBlock
            {
                Text = question.QuestionText,
                Foreground = _botTextColour,
                FontSize = 15,
                FontWeight = FontWeights.Bold,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 16)
            });

            for (int i = 0; i < question.Options.Count; i++)
            {
                int optionIndex = i; // capture for the lambda

                var optionButton = new Button
                {
                    Content = $"{(char)('A' + i)})  {question.Options[i]}",
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    Background = _botBubbleColour,
                    Foreground = _userTextColour,
                    BorderBrush = new SolidColorBrush(Color.FromRgb(58, 58, 92)),
                    BorderThickness = new Thickness(1),
                    Padding = new Thickness(12, 10, 12, 10),
                    Margin = new Thickness(0, 0, 0, 8),
                    Cursor = Cursors.Hand
                };

                optionButton.Click += (s, e) => HandleQuizAnswer(optionIndex, question);
                QuizContentPanel.Children.Add(optionButton);
            }
        }

        private void HandleQuizAnswer(int selectedIndex, QuizQuestion question)
        {
            bool wasCorrect = _quizManager.SubmitAnswer(selectedIndex);

            QuizContentPanel.Children.Clear();

            QuizContentPanel.Children.Add(new TextBlock
            {
                Text = wasCorrect ? "Correct!" : " Not quite.",
                Foreground = wasCorrect ? _botTextColour : new SolidColorBrush(Color.FromRgb(255, 100, 100)),
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 10)
            });

            QuizContentPanel.Children.Add(new TextBlock
            {
                Text = question.Explanation,
                Foreground = _userTextColour,
                FontSize = 13,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 20)
            });

            var nextButton = new Button
            {
                Content = _quizManager.IsFinished ? "See Results" : "Next Question",
                Style = (Style)FindResource("ActionButtonStyle"),
                HorizontalAlignment = HorizontalAlignment.Left
            };
            nextButton.Click += (s, e) =>
            {
                if (_quizManager.IsFinished)
                    ShowQuizResults();
                else
                    ShowQuizQuestion();
            };

            QuizContentPanel.Children.Add(nextButton);
        }

        private void ShowQuizResults()
        {
            // Log the quiz completion
            _activityLogger.Log($"Quiz completed — Score: {_quizManager.Score}/{_quizManager.TotalQuestions}");

            QuizContentPanel.Children.Clear();


            QuizContentPanel.Children.Add(new TextBlock
            {
                Text = $"Final Score: {_quizManager.Score} / {_quizManager.TotalQuestions}",
                Foreground = _botTextColour,
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 10)
            });

            QuizContentPanel.Children.Add(new TextBlock
            {
                Text = _quizManager.GetFinalFeedback(),
                Foreground = _userTextColour,
                FontSize = 14,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 20)
            });

            var retryButton = new Button
            {
                Content = "Try Again",
                Style = (Style)FindResource("ActionButtonStyle"),
                HorizontalAlignment = HorizontalAlignment.Left
            };
            retryButton.Click += (s, e) =>
            {
                _quizManager.StartNewQuiz();
                ShowQuizQuestion();
            };

            QuizContentPanel.Children.Add(retryButton);
        }

        // Task Assistant 

        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            string title = TaskTitleBox.Text.Trim();
            string description = TaskDescriptionBox.Text.Trim();
            DateTime? reminder = TaskReminderPicker.SelectedDate;

            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("Please enter a task title.", "Missing Title",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _taskManager.AddTask(title, description, reminder);

            // Log the action
            string reminderNote = reminder.HasValue
                ? $" (Reminder: {reminder.Value:dd MMM yyyy})" : "";
            _activityLogger.Log($"Task added: '{title}'{reminderNote}");

            TaskTitleBox.Clear();
            TaskDescriptionBox.Clear();
            TaskReminderPicker.SelectedDate = null;

            RefreshTaskList();
            RefreshActivityLog();
        }


        /// Reloads all tasks from the database and rebuilds the task list UI.

        private void RefreshTaskList()
        {
            TaskListPanel.Items.Clear();

            var tasks = _taskManager.GetAllTasks();

            if (tasks.Count == 0)
            {
                TaskListPanel.Items.Add(new TextBlock
                {
                    Text = "No tasks yet — add one above to get started.",
                    Foreground = _labelColour,
                    FontStyle = FontStyles.Italic,
                    Margin = new Thickness(4, 10, 0, 0)
                });
                return;
            }

            foreach (var task in tasks)
                TaskListPanel.Items.Add(BuildTaskCard(task));
        }

        
        /// Builds a visual card for a single task, with complete and delete buttons.
        
        private Border BuildTaskCard(TaskItem task)
        {
            var card = new Border
            {
                Background = task.IsCompleted
                    ? new SolidColorBrush(Color.FromRgb(30, 50, 40))
                    : _botBubbleColour,
                BorderBrush = new SolidColorBrush(Color.FromRgb(58, 58, 92)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(6),
                Padding = new Thickness(14, 10, 14, 10),
                Margin = new Thickness(0, 0, 0, 10)
            };

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var textPanel = new StackPanel();

            var titleText = new TextBlock
            {
                Text = task.Title,
                FontWeight = FontWeights.Bold,
                FontSize = 14,
                Foreground = task.IsCompleted ? _labelColour : _botTextColour,
                TextDecorations = task.IsCompleted ? TextDecorations.Strikethrough : null
            };
            textPanel.Children.Add(titleText);

            if (!string.IsNullOrWhiteSpace(task.Description))
            {
                textPanel.Children.Add(new TextBlock
                {
                    Text = task.Description,
                    FontSize = 12,
                    Foreground = _userTextColour,
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 4, 0, 0)
                });
            }

            textPanel.Children.Add(new TextBlock
            {
                Text = task.ReminderDisplay,
                FontSize = 11,
                Foreground = _labelColour,
                Margin = new Thickness(0, 4, 0, 0)
            });

            Grid.SetColumn(textPanel, 0);
            grid.Children.Add(textPanel);

            var completeButton = new Button
            {
                Content = task.IsCompleted ? "Done" : "Mark Complete",
                Margin = new Thickness(8, 0, 0, 0),
                Padding = new Thickness(10, 6, 10, 6),
                FontSize = 11,
                IsEnabled = !task.IsCompleted,
                Background = new SolidColorBrush(Color.FromRgb(0, 255, 156)),
                Foreground = new SolidColorBrush(Color.FromRgb(30, 30, 46)),
                BorderThickness = new Thickness(0),
                Cursor = Cursors.Hand
            };
            completeButton.Click += (s, e) =>
            {
                _taskManager.CompleteTask(task.Id);
                RefreshTaskList();
            };
            Grid.SetColumn(completeButton, 1);
            grid.Children.Add(completeButton);

            var deleteButton = new Button
            {
                Content = "Delete",
                Margin = new Thickness(8, 0, 0, 0),
                Padding = new Thickness(10, 6, 10, 6),
                FontSize = 11,
                Background = new SolidColorBrush(Color.FromRgb(200, 60, 60)),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Cursor = Cursors.Hand
            };
            deleteButton.Click += (s, e) =>
            {
                _taskManager.DeleteTask(task.Id);
                RefreshTaskList();
            };
            Grid.SetColumn(deleteButton, 2);
            grid.Children.Add(deleteButton);

            card.Child = grid;
            return card;
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

            // Exit command
            if (_chatEngine.IsExitCommand(userInput))
            {
                ShowBotMessage(_chatEngine.GetFarewellMessage());
                UserInputBox.Clear();
                UserInputBox.IsEnabled = false;
                SendButton.IsEnabled = false;
                UserInputBox.Text = "Chat ended. Close the window to exit.";
                return;
            }

            // NLP intercept — runs before ChatEngine 
            string intent = _nlpProcessor.DetectIntent(userInput);
            string nlpResponse = HandleNlpIntent(intent, userInput);

            if (nlpResponse != null)
            {
                ShowBotMessage(nlpResponse);
                UserInputBox.Clear();
                UserInputBox.Focus();
                ChatScrollViewer.ScrollToBottom();
                RefreshActivityLog();
                return;
            }

            // Fall through to ChatEngine for general conversation
            string response = _chatEngine.ProcessInput(userInput);
            ShowBotMessage(response);

            UserInputBox.Clear();
            UserInputBox.Focus();
            ChatScrollViewer.ScrollToBottom();
            RefreshActivityLog();
        }

        // NLP Simulation 

         
        /// Handles intents detected by the NLP processor.
        /// Returns a bot response string, or null to fall through to ChatEngine.
        
        private string HandleNlpIntent(string intent, string userInput)
        {
            switch (intent)
            {
                case "add_task":
                    string taskTitle = _nlpProcessor.ExtractTaskTitle(userInput);
                    _taskManager.AddTask(taskTitle, "Added via chat command", null);
                    RefreshTaskList();
                    _activityLogger.Log($"Task added via NLP: '{taskTitle}'");
                    return $"✅ Task added: '{taskTitle}'\n\n" +
                           $"You can set a reminder for it in the Tasks tab. " +
                           $"Would you like to add a reminder?";

                case "view_tasks":
                    var tasks = _taskManager.GetAllTasks();
                    _activityLogger.Log("User viewed task list via chat");
                    if (tasks.Count == 0)
                        return "You have no tasks yet. Try saying 'add task: enable 2FA' to create one.";

                    string list = string.Join("\n",
                        System.Linq.Enumerable.Select(tasks,
                            t => $"• {t.Title} {(t.IsCompleted ? "✅" : "⏳")} — {t.ReminderDisplay}"));

                    return $"Here are your tasks:\n\n{list}";

                case "start_quiz":
                    _quizManager.StartNewQuiz();
                    _activityLogger.Log("Quiz started via chat command");
                    return "🧠 Quiz started! Head over to the Quiz tab to begin.\n\n" +
                           "12 questions on passwords, phishing, safe browsing, and more!";

                case "activity_log":
                    _activityLogger.Log("User requested activity log summary");
                    return _activityLogger.GetSummary();

                default:
                    return null; // let ChatEngine handle it
            }
        }

        // Activity Log
         
        /// Rebuilds the Activity Log tab UI from the current logger state.
        
        private void RefreshActivityLog()
        {
            ActivityLogPanel.Items.Clear();

            var entries = _activityLogger.GetRecentEntries(10);

            if (entries.Count == 0)
            {
                ActivityLogPanel.Items.Add(new TextBlock
                {
                    Text = "No activity recorded yet.",
                    Foreground = _labelColour,
                    FontStyle = FontStyles.Italic,
                    Margin = new Thickness(4, 10, 0, 0)
                });
                return;
            }

            foreach (var entry in entries)
            {
                var card = new Border
                {
                    Background = _botBubbleColour,
                    BorderBrush = new SolidColorBrush(Color.FromRgb(58, 58, 92)),
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(4),
                    Padding = new Thickness(12, 8, 12, 8),
                    Margin = new Thickness(0, 0, 0, 6)
                };

                card.Child = new TextBlock
                {
                    Text = entry.Display,
                    Foreground = _userTextColour,
                    FontSize = 12,
                    TextWrapping = TextWrapping.Wrap
                };

                ActivityLogPanel.Items.Add(card);
            }

            // Total count footer
            ActivityLogPanel.Items.Add(new TextBlock
            {
                Text = $"Showing last {entries.Count} of {_activityLogger.TotalCount} total actions.",
                Foreground = _labelColour,
                FontSize = 11,
                Margin = new Thickness(4, 8, 0, 0)
            });
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