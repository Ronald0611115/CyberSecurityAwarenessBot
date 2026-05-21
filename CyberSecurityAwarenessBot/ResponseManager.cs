using System;
using System.Collections.Generic;
using System.Linq;

namespace CyberSecurityAwarenessBot
{
    /// <summary>
    /// Manages all chatbot responses using a Dictionary of keyword-response mappings.
    /// Demonstrates generic collections: Dictionary string, List string
    /// </summary>
    public class ResponseManager
    {
        private readonly Random _random = new Random();

        // Generic collection — Dictionary maps each keyword to a List of possible responses
        private readonly Dictionary<string, List<string>> _responses;

        public ResponseManager()
        {
            _responses = new Dictionary<string, List<string>>
            {
                ["password"] = new List<string>
                {
                    "Use a strong password with at least 12 characters, mixing letters, numbers, and symbols.",
                    "Never reuse passwords across different accounts — use a password manager instead.",
                    "Avoid using personal details like your name or birthday in your passwords.",
                    "Consider using a passphrase — a random sequence of words is easier to remember and harder to crack."
                },

                ["phishing"] = new List<string>
                {
                    "Be cautious of emails asking for personal information — legitimate companies won't ask via email.",
                    "Always check the sender's actual email address, not just the display name.",
                    "Hover over links before clicking to see where they really lead.",
                    "Scammers often create urgency — 'Act now or your account will be closed' is a red flag."
                },

                ["scam"] = new List<string>
                {
                    "If something sounds too good to be true online, it almost certainly is.",
                    "Never send money or gift cards to someone you haven't met in person.",
                    "Scammers often pretend to be banks, SARS, or government officials — verify directly.",
                    "Trust your instincts — if a message feels off, don't engage with it."
                },

                ["privacy"] = new List<string>
                {
                    "Review your social media privacy settings regularly — limit who can see your posts.",
                    "Avoid sharing your ID number, address, or financial details online unless absolutely necessary.",
                    "Use a VPN on public Wi-Fi to protect your data from being intercepted.",
                    "Read app permissions carefully — not every app needs access to your contacts or camera."
                },

                ["malware"] = new List<string>
                {
                    "Install reputable antivirus software and keep it updated.",
                    "Never download software from untrusted or unknown websites.",
                    "Malware can hide in email attachments — don't open files from senders you don't know.",
                    "Keep your operating system and apps updated — patches fix security vulnerabilities."
                },

                ["wifi"] = new List<string>
                {
                    "Avoid using public Wi-Fi for banking or anything sensitive.",
                    "Always check that websites use HTTPS before entering any personal information.",
                    "Use a VPN when connecting to public networks for an extra layer of security.",
                    "Forget public Wi-Fi networks on your device after using them."
                },

                ["2fa"] = new List<string>
                {
                    "Enable two-factor authentication on all your important accounts — it adds a critical second layer.",
                    "Use an authenticator app rather than SMS for stronger two-factor protection.",
                    "Even if someone has your password, 2FA stops them from getting in."
                },

                ["safe browsing"] = new List<string>
                {
                    "Look for the padlock icon and HTTPS in the address bar before entering any details.",
                    "Keep your browser updated — updates often include important security fixes.",
                    "Use a reputable ad blocker to reduce exposure to malicious ads.",
                    "Clear your browser cookies and cache regularly."
                }
            };
        }

        /// <summary>
        /// Searches user input for known keywords and returns a matching response.
        /// Returns null if no keyword is matched.
        /// </summary>
        public string GetKeywordResponse(string userInput)
        {
            string input = userInput.ToLower();

            foreach (var entry in _responses)
            {
                if (input.Contains(entry.Key))
                {
                    return GetRandomResponse(entry.Value);
                }
            }

            return null;
        }

        /// <summary>
        /// Returns a random item from a List of strings — implements random response requirement.
        /// </summary>
        private string GetRandomResponse(List<string> options)
        {
            return options[_random.Next(options.Count)];
        }

        /// <summary>
        /// Returns all keywords the bot knows about — used for "what can I ask" responses.
        /// </summary>
        public string GetTopicList()
        {
            var topics = _responses.Keys.Select(k => $"• {k}");
            return "Here are the topics I can help with:\n" + string.Join("\n", topics);
        }

        /// <summary>
        /// Returns a random tip from a random topic — used for "give me a tip" requests.
        /// </summary>
        public string GetRandomTip()
        {
            var keys = _responses.Keys.ToList();
            string randomKey = keys[_random.Next(keys.Count)];
            return $"💡 {randomKey.ToUpper()} TIP: {GetRandomResponse(_responses[randomKey])}";
        }
    }
}
