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

         
        /// Returns true if we know the user's name.
         
        public bool KnowsName => !string.IsNullOrWhiteSpace(UserName);

        
        /// Returns true if we know the user's favourite topic.
         
        public bool KnowsTopic => !string.IsNullOrWhiteSpace(FavouriteTopic);

         
        /// Builds a personalised greeting using stored details.
         
        public string GetPersonalisedGreeting()
        {
            if (KnowsName && KnowsTopic)
                return $"Welcome back, {UserName}! Ready to talk more about {FavouriteTopic}?";

            if (KnowsName)
                return $"Good to hear from you, {UserName}!";

            return "Hello! I don't think we've met. What's your name?";
        }

         
        /// Clears all stored memory — fresh conversation.
         
        public void Reset()
        {
            UserName = string.Empty;
            FavouriteTopic = string.Empty;
            HasGreetedUser = false;
        }
    }
}