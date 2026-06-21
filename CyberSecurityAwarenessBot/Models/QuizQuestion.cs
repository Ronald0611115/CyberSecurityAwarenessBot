using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Generic;

namespace CyberSecurityAwarenessBot.Models

{
        
        /// Represents a single quiz question — supports both
        /// multiple-choice and true/false formats.
       
        public class QuizQuestion
        {
            public string QuestionText { get; set; } = string.Empty;
            public List<string> Options { get; set; } = new List<string>();
            public int CorrectOptionIndex { get; set; }
            public string Explanation { get; set; } = string.Empty;

            
            /// Checks if the given option index is the correct answer.
            
            public bool IsCorrect(int selectedIndex) => selectedIndex == CorrectOptionIndex;
        }
}


