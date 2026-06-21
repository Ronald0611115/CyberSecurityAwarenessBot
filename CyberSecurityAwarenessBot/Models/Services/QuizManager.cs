using System;
using System.Collections.Generic;
using System.Text;
using CyberSecurityAwarenessBot.Models;

namespace CyberSecurityAwarenessBot.Services
{
        
        /// Manages the cybersecurity quiz: question bank, scoring, and session state.
        
        public class QuizManager
        {
            private readonly List<QuizQuestion> _allQuestions;
            private readonly Random _random = new Random();

            private List<QuizQuestion> _sessionQuestions = new List<QuizQuestion>();
            private int _currentIndex;
            private int _score;

            public int CurrentQuestionNumber => _currentIndex + 1;
            public int TotalQuestions => _sessionQuestions.Count;
            public int Score => _score;
            public bool IsFinished => _currentIndex >= _sessionQuestions.Count;

            public QuizManager()
            {
                _allQuestions = BuildQuestionBank();
            }

            
            /// Starts a new quiz session — shuffles questions for variety.
            
            public void StartNewQuiz()
            {
                _sessionQuestions = new List<QuizQuestion>(_allQuestions);

                // Simple shuffle so the order isn't identical every attempt
                for (int i = _sessionQuestions.Count - 1; i > 0; i--)
                {
                    int j = _random.Next(i + 1);
                    (_sessionQuestions[i], _sessionQuestions[j]) = (_sessionQuestions[j], _sessionQuestions[i]);
                }

                _currentIndex = 0;
                _score = 0;
            }

            public QuizQuestion GetCurrentQuestion() =>
                IsFinished ? null : _sessionQuestions[_currentIndex];

            
            /// Submits an answer for the current question, updates score, and advances.
            /// Returns true if the answer was correct.
            
            public bool SubmitAnswer(int selectedIndex)
            {
                var question = GetCurrentQuestion();
                if (question == null) return false;

                bool correct = question.IsCorrect(selectedIndex);
                if (correct) _score++;

                _currentIndex++;
                return correct;
            }

            
            /// Returns an encouraging message based on the final score percentage.
            
            public string GetFinalFeedback()
            {
                double percentage = (double)_score / _sessionQuestions.Count * 100;

                if (percentage >= 80)
                    return " Great job! You're a cybersecurity pro!";
                if (percentage >= 50)
                    return " Good effort! Keep learning to sharpen your skills.";

                return " Keep practising — review the topics and try again!";
            }

            
            /// 12 cybersecurity questions — mix of multiple-choice and true/false.
            
            private List<QuizQuestion> BuildQuestionBank()
            {
                return new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    QuestionText = "What should you do if you receive an email asking for your password?",
                    Options = new List<string> { "Reply with your password", "Delete the email", "Report the email as phishing", "Ignore it" },
                    CorrectOptionIndex = 2,
                    Explanation = "Reporting phishing emails helps prevent scams and protects others too."
                },
                new QuizQuestion
                {
                    QuestionText = "Using the same password for multiple accounts is safe.",
                    Options = new List<string> { "True", "False" },
                    CorrectOptionIndex = 1,
                    Explanation = "Reusing passwords means one breach can compromise all your accounts."
                },
                new QuizQuestion
                {
                    QuestionText = "Which of these is the strongest password?",
                    Options = new List<string> { "password123", "MyName2024", "Tr@il#Moon9Spark!", "12345678" },
                    CorrectOptionIndex = 2,
                    Explanation = "Long, random passwords with symbols and mixed case are hardest to crack."
                },
                new QuizQuestion
                {
                    QuestionText = "Public Wi-Fi is always safe for online banking.",
                    Options = new List<string> { "True", "False" },
                    CorrectOptionIndex = 1,
                    Explanation = "Public Wi-Fi can be intercepted — avoid sensitive transactions on it."
                },
                new QuizQuestion
                {
                    QuestionText = "What is 'social engineering' in cybersecurity?",
                    Options = new List<string> { "Building secure networks", "Manipulating people into revealing info", "A type of firewall", "Coding social media apps" },
                    CorrectOptionIndex = 1,
                    Explanation = "Social engineering tricks people psychologically rather than hacking systems directly."
                },
                new QuizQuestion
                {
                    QuestionText = "Two-factor authentication adds an extra layer of security.",
                    Options = new List<string> { "True", "False" },
                    CorrectOptionIndex = 0,
                    Explanation = "2FA requires a second verification step, making accounts much harder to breach."
                },
                new QuizQuestion
                {
                    QuestionText = "What does HTTPS in a website address indicate?",
                    Options = new List<string> { "The site is encrypted and more secure", "The site is free to use", "The site is government-owned", "The site has no ads" },
                    CorrectOptionIndex = 0,
                    Explanation = "HTTPS means data between you and the site is encrypted in transit."
                },
                new QuizQuestion
                {
                    QuestionText = "You should click links in unexpected emails to verify if they're real.",
                    Options = new List<string> { "True", "False" },
                    CorrectOptionIndex = 1,
                    Explanation = "Never click suspicious links — verify by contacting the sender directly instead."
                },
                new QuizQuestion
                {
                    QuestionText = "What is malware?",
                    Options = new List<string> { "A type of antivirus", "Software designed to harm or exploit systems", "A password manager", "A firewall setting" },
                    CorrectOptionIndex = 1,
                    Explanation = "Malware includes viruses, ransomware, spyware, and other harmful software."
                },
                new QuizQuestion
                {
                    QuestionText = "A strong password should include personal details like your birthday.",
                    Options = new List<string> { "True", "False" },
                    CorrectOptionIndex = 1,
                    Explanation = "Personal details are easy for attackers to guess or find online."
                },
                new QuizQuestion
                {
                    QuestionText = "What's the safest way to store multiple passwords?",
                    Options = new List<string> { "Write them on paper at your desk", "Use a password manager", "Use the same password everywhere", "Save them in a text file" },
                    CorrectOptionIndex = 1,
                    Explanation = "Password managers encrypt and securely store unique passwords for every account."
                },
                new QuizQuestion
                {
                    QuestionText = "Updating your software regularly helps protect against security vulnerabilities.",
                    Options = new List<string> { "True", "False" },
                    CorrectOptionIndex = 0,
                    Explanation = "Updates often patch security holes that attackers could otherwise exploit."
                }
            };
            }
        }
}

