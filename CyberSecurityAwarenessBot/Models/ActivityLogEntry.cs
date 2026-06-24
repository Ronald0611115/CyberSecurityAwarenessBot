using System;
using System.Collections.Generic;
using System.Text;


namespace CyberSecurityAwarenessBot.Models
{
        /// <summary>
        /// Represents a single logged action in the chatbot's activity history.
        /// </summary>
        public class ActivityLogEntry
        {
            public string Action { get; set; } = string.Empty;
            public DateTime Timestamp { get; set; } = DateTime.Now;

            /// <summary>
            /// Formats the entry for display in the Activity Log tab.
            /// </summary>
            public string Display =>
                $"[{Timestamp:HH:mm:ss}] {Action}";
        }
}
