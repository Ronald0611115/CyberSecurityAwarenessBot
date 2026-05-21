using System;
using System.Collections.Generic;
using System.Text;

namespace CyberSecurityAwarenessBot
{
    /// <summary>
    /// Detects sentiment in user input and returns empathetic responses.
    /// Demonstrates delegates: Func and Action used for response routing.
    /// </summary>
    public class SentimentDetector
    {
        // DELEGATE — Func takes a string (input) and returns a string (response)
        // This is the delegate type we use to map each sentiment to a handler method
        public delegate string SentimentHandler(string userInput);

        // Dictionary mapping sentiment labels to their delegate handlers
        private readonly Dictionary<string, SentimentHandler> _sentimentHandlers;

        // Keywords that signal each sentiment — string manipulation used for matching
        private readonly Dictionary<string, List<string>> _sentimentKeywords;

        public SentimentDetector()
        {
            // Register sentiment keywords
            _sentimentKeywords = new Dictionary<string, List<string>>
            {
                ["worried"] = new List<string>
                {
                    "worried", "scared", "afraid", "nervous", "anxious",
                    "frightened", "concerned", "stressed", "panic"
                },

                ["curious"] = new List<string>
                {
                    "curious", "wondering", "interested", "want to know",
                    "tell me more", "explain", "how does", "what is", "what are"
                },

                ["frustrated"] = new List<string>
                {
                    "frustrated", "annoyed", "confused", "don't understand",
                    "doesn't make sense", "this is hard", "complicated", "lost"
                },

                ["positive"] = new List<string>
                {
                    "great", "awesome", "thanks", "thank you", "helpful",
                    "amazing", "love it", "perfect", "excellent", "good"
                }
            };

            // Wire each sentiment label to a handler method using delegates
            _sentimentHandlers = new Dictionary<string, SentimentHandler>
            {
                ["worried"] = HandleWorried,
                ["curious"] = HandleCurious,
                ["frustrated"] = HandleFrustrated,
                ["positive"] = HandlePositive
            };
        }

        /// <summary>
        /// Analyses the input, detects sentiment, and invokes the correct delegate handler.
        /// Returns null if no sentiment is detected.
        /// </summary>
        public string Analyse(string userInput)
        {
            string input = userInput.ToLower();

            foreach (var sentiment in _sentimentKeywords)
            {
                foreach (string keyword in sentiment.Value)
                {
                    if (input.Contains(keyword))
                    {
                        // Invoke the delegate for the matched sentiment
                        SentimentHandler handler = _sentimentHandlers[sentiment.Key];
                        return handler(userInput);
                    }
                }
            }

            return null;
        }

        // ── Delegate handler methods ──────────────────────────────────────────

        private string HandleWorried(string input)
        {
            return "It's completely understandable to feel that way — cybersecurity can feel overwhelming. " +
                   "You're already doing the right thing by learning about it. " +
                   "Let me share something that might help put your mind at ease.";
        }

        private string HandleCurious(string input)
        {
            return "I love the curiosity! Asking questions is one of the best ways to stay safe online. " +
                   "What specifically would you like to know more about?";
        }

        private string HandleFrustrated(string input)
        {
            return "I hear you — some of this can feel really complicated at first. " +
                   "Let's slow it down. Try asking me about one specific topic, " +
                   "like passwords or phishing, and I'll break it down simply.";
        }

        private string HandlePositive(string input)
        {
            return "Really glad I could help! Staying informed is your best defence online. " +
                   "Feel free to ask me anything else about cybersecurity.";
        }
    }
}