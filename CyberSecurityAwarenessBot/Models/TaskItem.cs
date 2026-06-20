using System;
using System.Collections.Generic;
using System.Text;

namespace CyberSecurityAwarenessBot.Models
{
        /// <summary>
        /// Represents a single cybersecurity task stored in the database.
        /// </summary>
        public class TaskItem
        {
            public int Id { get; set; }
            public string Title { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public DateTime? ReminderDate { get; set; }
            public bool IsCompleted { get; set; }
            public DateTime CreatedAt { get; set; }

            /// <summary>
            /// Friendly display text for the reminder, used in the UI.
            /// </summary>
            public string ReminderDisplay =>
                ReminderDate.HasValue
                    ? $"Reminder: {ReminderDate.Value:dd MMM yyyy}"
                    : "No reminder set";
        }
}


