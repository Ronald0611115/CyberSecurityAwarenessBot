using CyberSecurityAwarenessBot.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using CyberSecurityAwarenessBot.Models;

namespace CyberSecurityAwarenessBot.Services
    {
        
        /// Business logic layer for managing cybersecurity tasks.
        /// Sits between the UI and DatabaseService — never touches SQL directly.
        
        public class TaskManager
        {
            private readonly DatabaseService _db;

            public TaskManager(DatabaseService db)
            {
                _db = db;
            }

            
            /// Creates and saves a new task to the database.
            
            public TaskItem AddTask(string title, string description, DateTime? reminderDate)
            {
                var task = new TaskItem
                {
                    Title = title,
                    Description = description,
                    ReminderDate = reminderDate,
                    IsCompleted = false
                };

                task.Id = _db.InsertTask(task);
                return task;
            }

            public List<TaskItem> GetAllTasks() => _db.GetAllTasks();

            public List<TaskItem> GetActiveTasks() =>
                _db.GetAllTasks().Where(t => !t.IsCompleted).ToList();

            public void CompleteTask(int taskId) => _db.MarkTaskCompleted(taskId);

            public void DeleteTask(int taskId) => _db.DeleteTask(taskId);

            
            /// Parses natural language reminder phrases like "in 3 days" or "tomorrow"
            /// into an actual DateTime. Used later by the NLP simulation in Step 5.
            
            public DateTime? ParseReminderPhrase(string phrase)
            {
                if (string.IsNullOrWhiteSpace(phrase)) return null;

                string lower = phrase.ToLower().Trim();

                if (lower.Contains("tomorrow")) return DateTime.Now.AddDays(1);
                if (lower.Contains("today")) return DateTime.Now;
                if (lower.Contains("week")) return DateTime.Now.AddDays(7);

                var match = Regex.Match(lower, @"(\d+)\s*day");
                if (match.Success && int.TryParse(match.Groups[1].Value, out int days))
                    return DateTime.Now.AddDays(days);

                return null;
            }
        }
}


