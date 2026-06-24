using System;
using System.Collections.Generic;
using System.Text;


namespace CyberSecurityAwarenessBot.Services
    {
         
        /// NLP Simulation — detects user intent from varied natural language input.
        /// Uses keyword detection via string.Contains() to route commands
        /// to the correct feature (Task, Quiz, Log, or general chat).
        
        public class NlpProcessor
        {
            // Each intent maps to a set of phrases that signal it
            private readonly Dictionary<string, List<string>> _intentKeywords;

            public NlpProcessor()
            {
                _intentKeywords = new Dictionary<string, List<string>>
                {
                    ["add_task"] = new List<string>
                {
                    "add task", "add a task", "create task", "new task",
                    "remind me to", "set a reminder", "i need to",
                    "don't forget to", "remember to", "schedule"
                },

                    ["view_tasks"] = new List<string>
                {
                    "view tasks", "show tasks", "my tasks", "list tasks",
                    "what tasks", "pending tasks", "task list"
                },

                    ["start_quiz"] = new List<string>
                {
                    "start quiz", "take quiz", "quiz me", "test me",
                    "start the quiz", "begin quiz", "play quiz",
                    "cybersecurity quiz", "trivia"
                },

                    ["activity_log"] = new List<string>
                {
                    "activity log", "show log", "what have you done",
                    "recent actions", "history", "show activity",
                    "what did you do", "log"
                }
                };
            }

             
            /// Detects the intent of the user's input.
            /// Returns the intent label or "general" if no match found.
             
            public string DetectIntent(string input)
            {
                string lower = input.ToLower().Trim();

                foreach (var intent in _intentKeywords)
                {
                    foreach (string phrase in intent.Value)
                    {
                        if (lower.Contains(phrase))
                            return intent.Key;
                    }
                }

                return "general";
            }

             
            /// Extracts a task title from natural language.
            /// e.g. "remind me to update my password" → "Update my password"
             
            public string ExtractTaskTitle(string input)
            {
                string lower = input.ToLower();
                string[] prefixes = new[]
                {
                "remind me to", "add task", "add a task", "create task",
                "new task", "i need to", "don't forget to",
                "remember to", "schedule", "set a reminder to",
                "set a reminder for", "set reminder"
            };

                foreach (string prefix in prefixes)
                {
                    if (lower.Contains(prefix))
                    {
                        int idx = lower.IndexOf(prefix) + prefix.Length;
                        string extracted = input.Substring(idx).Trim();
                        if (!string.IsNullOrWhiteSpace(extracted))
                        {
                            // Capitalise first letter
                            return char.ToUpper(extracted[0]) + extracted.Substring(1);
                        }
                    }
                }

                // Fallback — use the full input as the title
                return input.Trim();
            }

        
        } 
}

