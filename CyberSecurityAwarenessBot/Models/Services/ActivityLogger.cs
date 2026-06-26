using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using CyberSecurityAwarenessBot.Models;


namespace CyberSecurityAwarenessBot.Services
    {
        
        /// Records significant chatbot actions during the session.
        /// Uses a List of ActivityLogEntry — generic collection requirement.
        /// Displays the last 5–10 entries as required by the brief.
        
        public class ActivityLogger
        {
            private readonly List<ActivityLogEntry> _log = new List<ActivityLogEntry>();

            // Maximum entries to store before oldest are dropped
            private const int MaxEntries = 50;

            
            /// Adds a new action to the log.
            
            public void Log(string action)
            {
                _log.Add(new ActivityLogEntry { Action = action });

                // Keep the log from growing indefinitely
                if (_log.Count > MaxEntries)
                    _log.RemoveAt(0);
            }

            
            /// Returns the last 10 entries, newest first — shown in the Activity Log tab.
            
            public List<ActivityLogEntry> GetRecentEntries(int count = 10)
            {
                return _log
                    .AsEnumerable()
                    .Reverse()
                    .Take(count)
                    .ToList();
            }

            
            /// Returns a formatted summary string for the Chat tab response.
            
            public string GetSummary()
            {
                var recent = GetRecentEntries(5);

                if (recent.Count == 0)
                    return "No actions recorded yet — start chatting, add a task, or take the quiz!";

                string header = " Here's a summary of recent actions:\n\n";
                string entries = string.Join("\n",
                    recent.Select((e, i) => $"{i + 1}. {e.Action}"));

                return header + entries;
            }

            public int TotalCount => _log.Count;
        }
}
