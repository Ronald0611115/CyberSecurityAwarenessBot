using System;
using CyberSecurityAwarenessBot.Models;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace CyberSecurityAwarenessBot
{
    /// <summary>
    /// Orchestrates the chatbot conversation.
    /// Connects ResponseManager, SentimentDetector, and UserMemory.
    /// Demonstrates: OOP, delegates, string manipulation, conversation flow, memory recall.
    /// </summary>
    public class ChatEngine
    {
        // ── Dependencies ──────────────────────────────────────────────────────
        private readonly ResponseManager _responseManager;
        private readonly SentimentDetector _sentimentDetector;
        private readonly UserMemory _memory;

        // ── Delegate — used for conversation flow routing ─────────────────────
        // Func takes the user input string and returns the bot response string
        private delegate string ConversationRouter(string input);

        // Tracks what the bot is waiting for next (name, topic, general chat)
        private string _conversationState = "awaitingName";

        // Stores the last cybersecurity topic discussed — for "tell me more" flow
        private string _lastTopic = string.Empty;

        public ChatEngine()
        {
            _responseManager = new ResponseManager();
            _sentimentDetector = new SentimentDetector();
            _memory = new UserMemory();
        }

        /// <summary>
        /// Main entry point — receives raw user input, returns bot response.
        /// Routes through the correct conversation state using a delegate.
        /// </summary>
        public string ProcessInput(string userInput)
        {
            // Input validation — handle empty or whitespace input
            if (string.IsNullOrWhiteSpace(userInput))
                return "Please type a message before sending.";

            string trimmed = userInput.Trim();

            // Wire the correct handler to the delegate based on conversation state
            ConversationRouter router = _conversationState switch
            {
                "awaitingName" => HandleAwaitingName,
                "awaitingTopic" => HandleAwaitingTopic,
                _ => HandleGeneralChat
            };

            // Invoke the delegate
            return router(trimmed);
        }

        // ── Conversation state handlers (delegate targets) ────────────────────

        /// <summary>
        /// State: bot is waiting for the user to provide their name.
        /// </summary>
        private string HandleAwaitingName(string input)
        {
            // Extract name using string manipulation
            string name = ExtractName(input);

            if (string.IsNullOrWhiteSpace(name))
                return "I didn't quite catch your name. What should I call you?";

            // Store in memory
            _memory.UserName = name;
            _memory.HasGreetedUser = true;

            // Move conversation forward
            _conversationState = "awaitingTopic";

            return $"Great to meet you, {_memory.UserName}! \n\n" +
                   $"I'm your Cybersecurity Awareness Bot — I'm here to help you stay safe online.\n\n" +
                   $"What cybersecurity topic are you most interested in? " +
                   $"(e.g. passwords, phishing, privacy, scams, malware, Wi-Fi, 2FA)";
        }

        /// <summary>
        /// State: bot is waiting for the user's favourite topic.
        /// </summary>
        private string HandleAwaitingTopic(string input)
        {
            _memory.FavouriteTopic = input.Trim();
            _conversationState = "generalChat";

            string response = _responseManager.GetKeywordResponse(input);

            if (response != null)
            {
                _lastTopic = input;
                return $"Great choice! I'll remember that you're interested in {_memory.FavouriteTopic}.\n\n" +
                       $"Here's something useful to start with:\n\n{response}\n\n" +
                       $"Feel free to ask me anything else, {_memory.UserName}!";
            }

            return $"Got it — I'll remember that you're interested in {_memory.FavouriteTopic}. " +
                   $"Ask me anything about cybersecurity, {_memory.UserName}!";
        }

        /// <summary>
        /// State: general conversation — handles all normal chat.
        /// </summary>
        private string HandleGeneralChat(string input)
        {
            string lower = input.ToLower();

            // ── Conversation flow: follow-up commands ─────────────────────────
            if (lower.Contains("tell me more") || lower.Contains("explain more") ||
                lower.Contains("give me more") || lower.Contains("more info"))
            {
                return HandleFollowUp();
            }

            if (lower.Contains("another tip") || lower.Contains("give me a tip") ||
                lower.Contains("tip please"))
            {
                return _responseManager.GetRandomTip();
            }

            // ── Memory recall ─────────────────────────────────────────────────
            if (lower.Contains("what do you remember") || lower.Contains("what do you know about me"))
            {
                return HandleMemoryRecall();
            }

            // ── Topic list ────────────────────────────────────────────────────
            if (lower.Contains("what can i ask") || lower.Contains("help") ||
                lower.Contains("topics") || lower.Contains("what can you do"))
            {
                return _responseManager.GetTopicList();
            }

            // ── Basic conversational responses ────────────────────────────────
            if (lower.Contains("how are you") || lower.Contains("how r u"))
            {
                return $"I'm running smoothly and ready to help, {GetNameOrFallback()}! " +
                       $"What cybersecurity topic can I assist you with today?";
            }

            if (lower.Contains("your purpose") || lower.Contains("what are you") ||
                lower.Contains("who are you"))
            {
                return "I'm the Cybersecurity Awareness Bot — designed to help you understand " +
                       "online threats and stay safe. Ask me about passwords, phishing, scams, " +
                       "privacy, malware, Wi-Fi safety, or two-factor authentication.";
            }

            // ── Sentiment detection — runs before keyword matching ────────────
            string sentimentResponse = _sentimentDetector.Analyse(input);
            if (sentimentResponse != null)
            {
                // After empathetic response, also provide a relevant tip if possible
                string keywordResponse = _responseManager.GetKeywordResponse(input);
                if (keywordResponse != null)
                {
                    _lastTopic = input;
                    return $"{sentimentResponse}\n\n {keywordResponse}";
                }
                return sentimentResponse;
            }

            // ── Keyword recognition — main response engine ────────────────────
            string response = _responseManager.GetKeywordResponse(input);
            if (response != null)
            {
                _lastTopic = input;
                return $"{response}\n\nWould you like to know more about this topic, " +
                       $"{GetNameOrFallback()}? Just say 'tell me more'.";
            }

            // ── Default fallback — input validation ───────────────────────────
            return $"I'm not sure I understand that, {GetNameOrFallback()}. " +
                   $"Could you rephrase? You can also ask me 'what can I ask?' " +
                   $"to see all available topics.";
        }

        // ── Helper methods ────────────────────────────────────────────────────

        /// <summary>
        /// Handles follow-up requests by expanding on the last topic discussed.
        /// </summary>
        private string HandleFollowUp()
        {
            if (string.IsNullOrWhiteSpace(_lastTopic))
                return "We haven't discussed a specific topic yet. What would you like to know about?";

            string response = _responseManager.GetKeywordResponse(_lastTopic);
            return response != null
                ? $"Sure! Here's another point on that topic:\n\n{response}"
                : "I don't have more details on that specific topic. Try asking about something else!";
        }

        /// <summary>
        /// Returns stored user memory in a conversational way — memory recall requirement.
        /// </summary>
        private string HandleMemoryRecall()
        {
            if (!_memory.KnowsName)
                return "I don't know much about you yet — we've only just started chatting!";

            string recall = $"Here's what I remember about you, {_memory.UserName}:\n\n";
            recall += $" Your name: {_memory.UserName}\n";

            if (_memory.KnowsTopic)
                recall += $" Your favourite cybersecurity topic: {_memory.FavouriteTopic}\n";

            if (!string.IsNullOrWhiteSpace(_lastTopic))
                recall += $" Last topic we discussed: {_lastTopic}\n";

            return recall;
        }

        /// <summary>
        /// Extracts a clean name from input like "I'm Rorisang" or "my name is Rorisang" or just "Rorisang".
        /// Uses string manipulation — ToLower, Replace, Trim, Split.
        /// </summary>
        private string ExtractName(string input)
        {
            string cleaned = input.ToLower()
                                  .Replace("my name is", "")
                                  .Replace("i'm", "")
                                  .Replace("i am", "")
                                  .Replace("call me", "")
                                  .Trim();

            // Capitalise first letter
            if (string.IsNullOrWhiteSpace(cleaned)) return string.Empty;
            return char.ToUpper(cleaned[0]) + cleaned.Substring(1);
        }

        /// <summary>
        /// Returns the user's name if known, otherwise a friendly fallback.
        /// </summary>
        private string GetNameOrFallback()
        {
            return _memory.KnowsName ? _memory.UserName : "there";
        }

        /// <summary>
        /// Returns the opening message when the bot first launches.
        /// </summary>
        public string GetWelcomeMessage()
        {
            return "Welcome to the Cybersecurity Awareness Bot!\n\n" +
                   "I'm here to help you stay safe online.\n\n" +
                   "Before we start — what's your name?";
        }
    }
}