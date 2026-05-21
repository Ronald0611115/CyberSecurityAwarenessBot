using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CyberSecurityAwarenessBot.Models
{
    public class UserMemory
    {
        // Auto-properties — syllabus requirement from Part 1
        public string UserName { get; set; } = string.Empty;
        public string FavouriteTopic { get; set; } = string.Empty;

        // Tracks whether we have already asked for the user's name
        public bool HasGreetedUser { get; set; } = false;

        /// <summary>
        /// Returns true if we know the user's name.
        /// </summary>
        public bool KnowsName => !string.IsNullOrWhiteSpace(UserName);

        /// <summary>
        /// Returns true if we know the user's favourite topic.
        /// </summary>
        public bool KnowsTopic => !string.IsNullOrWhiteSpace(FavouriteTopic);

        /// <summary>
        /// Builds a personalised greeting using stored details.
        /// </summary>
        public string GetPersonalisedGreeting()
        {
            if (KnowsName && KnowsTopic)
                return $"Welcome back, {UserName}! Ready to talk more about {FavouriteTopic}?";

            if (KnowsName)
                return $"Good to hear from you, {UserName}!";

            return "Hello! I don't think we've met. What's your name?";
        }

        /// <summary>
        /// Clears all stored memory — fresh conversation.
        /// </summary>
        public void Reset()
        {
            UserName = string.Empty;
            FavouriteTopic = string.Empty;
            HasGreetedUser = false;
        }
    }
}