# CybersecurityAwarenessBot v3.0 - Final POE
## Project Overview

Parts 1, 2, and 3 are fully integrated into one WPF desktop application.

Final POE submission. New features added on top of Parts 1 and 2:
- Task Assistant with MySQL database integration (add, complete, delete, reminders)
- Cybersecurity Quiz with 12 shuffled questions, instant feedback, and final score
- NLP Simulation — natural language commands routed to correct features
- Activity Log with timestamps — tracks all significant chatbot actions
- All Parts 1, 2, and 3 features integrated into a single tabbed WPF application

The project demonstrates the use of:

- Windows Presentation Foundation (WPF) GUI
- Generic collections — `Dictionary<string, List<string>>`
- Delegates and delegate invocation
- Object-Oriented Programming (OOP)
- String manipulation
- Sentiment detection
- Memory and personalisation
- GitHub Actions CI/CD

---

## Developer Information

| Detail | Info |
|---|---|
| Student Name |Rorisng Ronald Maroga |
| Student Number | ST10476320 |
| Module | PROG6221 — Programming 2A |
| Project | CybersecurityAwarenessBot v2.0 |
| Language | C# |
| Platform | Visual Studio [.NET 8 — WPF Desktop Application] |

---

## Features

### WPF Graphical User Interface
A polished dark-themed chat window with colour-coded message bubbles,
an ASCII art header, and a styled input bar. Replaces the console
interface from v1.0 entirely.

### Voice Greeting
A WAV audio greeting plays automatically when the application launches,
carried forward and integrated into the WPF window load event.

### ASCII Art Header
A cybersecurity-themed ASCII logo is displayed at the top of the
GUI window as a visual header.

### Keyword Recognition
The chatbot recognises 8 cybersecurity keywords and returns
relevant, informative responses using a Dictionary mapping.

### Random Responses
Each keyword maps to a List of multiple responses. A random one
is selected each time to keep conversations varied and engaging.

### Conversation Flow
A state machine guides the conversation naturally:
awaitingName → awaitingTopic → generalChat.
Follow-up commands like "tell me more" and "give me another tip"
work without restarting the conversation.

### Memory and Recall
The chatbot stores the user's name and favourite cybersecurity topic
during the session and refers back to them in personalised responses.

### Sentiment Detection
The chatbot detects emotional tone — worried, curious, frustrated,
or positive — and adjusts responses with empathy before providing tips.

### Input Validation and Error Handling
Empty input, unrecognised queries, and edge cases are all handled
gracefully with helpful fallback messages and no crashes.

### Exit Command
Typing "bye", "exit", or "quit" ends the conversation with a
personalised farewell message and locks the input field cleanly.

 Part 3 Features

| Feature | Description |
|---|---|
| Task Assistant | Add, complete, and delete cybersecurity tasks stored in MySQL |
| Reminders | Optional reminder dates stored with each task via DatePicker |
| Quiz | 12 shuffled cybersecurity questions with instant feedback and scoring |
| NLP Simulation | Natural language commands in Chat tab route to correct features |
| Activity Log | Timestamped record of all significant chatbot actions |
| MySQL Database | Full CRUD via XAMPP — tasks persist across sessions
---

## Project Structure
```
CybersecurityAwarenessBot/
 ├── MainWindow.xaml / .cs         → Tabbed WPF UI (Chat, Tasks, Quiz, Activity Log)
 ├── ChatEngine.cs                  → Conversation engine (Parts 1 + 2, unchanged)
 ├── ResponseManager.cs             → Keyword Dictionary (Part 2, unchanged)
 ├── SentimentDetector.cs           → Delegate sentiment routing (Part 2, unchanged)
 ├── Models/
 │    ├── UserMemory.cs             → User name + topic memory (Part 2)
 │    ├── TaskItem.cs               → Task data model (Part 3)
 │    ├── QuizQuestion.cs           → Quiz question model (Part 3)
 │    └── ActivityLogEntry.cs       → Log entry model (Part 3)
 └── Services/
      ├── DatabaseService.cs        → MySQL CRUD operations (Part 3)
      ├── TaskManager.cs            → Task business logic (Part 3)
      ├── QuizManager.cs            → Quiz session + question bank (Part 3)
      ├── ActivityLogger.cs         → Session action logging (Part 3)
      └── NlpProcessor.cs           → Intent detection via keyword matching (Part 3)
```

---

## Class Descriptions

### MainWindow.xaml
Defines the entire WPF UI layout.

Responsibilities:
- Dark theme with colour variables
- Chat bubble styles for bot and user
- ASCII art header
- Input bar with Send button
- Enter key support via the KeyDown event

---

### MainWindow.xaml.cs
Handles all UI events and connects to ChatEngine.

Responsibilities:
- Send button click handler
- Enter key sends support
- Bot and user chat bubble creation
- Voice greeting playback on window load
- Scroll to the latest message

---

### ChatEngine.cs
The brain of the chatbot — orchestrates everything.

Responsibilities:
- Conversation state machine (awaitingName, awaitingTopic, generalChat)
- Delegate-based routing to correct the handler per state
- Memory recall responses
- Follow-up conversation flow
- Input validation and default fallback
- Name extraction using string manipulation

---

### ResponseManager.cs
Manages all cybersecurity knowledge using generic collections.

Responsibilities:
- `Dictionary<string, List<string>>` keyword-to-response mapping
- Random response selection from each List
- Topic list generation
- Random tip generation across all topics

---

### SentimentDetector.cs
Detects user emotional tone using delegates.

Responsibilities:
- Declares custom `delegate string SentimentHandler.`
- Maps sentiment labels to delegate handler methods
- Detects worried, curious, frustrated, and positive sentiment
- Returns empathetic responses via delegate invocation

---

### Models/UserMemory.cs
Stores and retrieves user details during the session.

Responsibilities:
- Auto-properties: UserName, FavouriteTopic, HasGreetedUser
- Expression-bodied properties: KnowsName, KnowsTopic
- Personalised greeting generation
- Session reset method

---

## Syllabus Concepts Demonstrated

| Concept | File | Detail |
|---|---|---|
| WPF GUI | `MainWindow.xaml` | XAML layout, styles, triggers |
| Generic collections | `ResponseManager.cs` | `Dictionary<string, List<string>>` |
| Delegates | `SentimentDetector.cs` | Custom delegate declaration and invocation |
| Delegate routing | `ChatEngine.cs` | `ConversationRouter` delegate assigned by state |
| Auto-properties | `UserMemory.cs` | `UserName`, `FavouriteTopic`, `HasGreetedUser` |
| String manipulation | `ChatEngine.cs` | `.ToLower()`, `.Contains()`, `.Replace()`, `.Trim()` |
| OOP | All files | Classes, methods, encapsulation, single responsibility |
| Random selection | `ResponseManager.cs` | `new Random()`, `list[rand.Next(list.Count)]` |

---

## Topics You Can Ask About

| Topic | Example input |
|---|---|
| Passwords | `"Tell me about password safety"` |
| Phishing | `"What is phishing?"` |
| Scams | `"How do I avoid scams?"` |
| Privacy | `"Tell me about online privacy"` |
| Malware | `"What is malware?"` |
| Wi-Fi Safety | `"Is public wifi safe?"` |
| Two-Factor Authentication | `"What is 2FA?"` |
| Safe Browsing | `"How do I browse safely?"` |

---

## Conversation Commands

| Command | What it does |
|---|---|
| `tell me more` | Expands on the last topic discussed |
| `give me a tip` | Returns a random cybersecurity tip |
| `what do you remember` | Bot recalls your name and favourite topic |
| `what can I ask` | Lists all available topics |
| `how are you` | Bot responds conversationally |
| `who are you` | Bot explains its purpose |
| `bye` / `exit` / `quit` | Ends the session with a farewell message |


## NLP Commands (type these in the Chat tab)

| What you type | What happens |
|---|---|
| `remind me to enable 2FA` | Creates a task automatically |
| `add task: review privacy settings` | Creates a task |
| `show my tasks` | Lists all your tasks |
| `start quiz` | Launches the quiz |
| `show activity log` | Shows recent actions |
| `what have you done for me` | Shows activity summary |
---

## Technologies Used

- C#
- .NET 8
- WPF (Windows Presentation Foundation)
- XAML
- Visual Studio 2022
- GitHub
- GitHub Actions
- WAV Audio — `System.Media.SoundPlayer`

---

## How to Run

### 1. Clone the Repository
```bash
git clone https://github.com/YOUR-USERNAME/CybersecurityAwarenessBot.git
```

### 2. Open in Visual Studio
Open `CybersecurityAwarenessBot.sln` in Visual Studio 2026.

### 3. Build and Run
Press `Ctrl + F5` or click **Start Without Debugging**.

> Ensure `greeting.wav` is present in the project and set to
> **Copy if newer** so it is available at runtime.

---

## Example Interactions
Bot:  What's your name?
You:  Rorisang
Bot:  Great to meet you, Rorisang! What topic interests you most?

You:  phishing
Bot:  Great choice! Here's something useful:
Be cautious of emails asking for personal information...

You:  Tell me more
Bot:  Sure! Here's another point: Always check the sender's
actual email address, not just the display name.

You:  I'm worried about online scams
Bot:  It's completely understandable to feel that way...
      If something sounds too good to be true online, it almost certainly is.

You:  What do you remember
Bot:  Here's what I remember about you, Rorisang:
• Your name: Rorisang
• Your favourite topic: phishing

---

## Error Handling

The application handles:

- Empty input → validation message displayed, no crash
- Unrecognised input → polite fallback with topic suggestion
- Missing WAV file → silent fail, app continues normally
- All exceptions wrapped in `try-catch` blocks

---
 
## Releases

| Version | Description |
|---|---|
| `v1.0` | Console-based chatbot — Part 1 |
| `v2.0` | WPF GUI chatbot — Part 2, full feature set |
| `v3.0`  | POE Final Submission (Part 3) |

---

## Acknowledgements

- IIE PROG6221 module guidance and brief
- My number 1 lecturer, Mr Lefa Malatxi 
- Microsoft WPF and .NET documentation
- Cybersecurity awareness resources

---

## License

This project is developed for academic purposes only.
The Independent Institute of Education (IIE) © 2026
