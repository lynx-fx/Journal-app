# Journal App

A Personal Journaling Application built with **.NET MAUI Blazor Hybrid**.

## 📖 Overview
Journal App is a cross-platform application that helps you track your daily thoughts, emotions, and personal growth. Combining the power of .NET MAUI with the flexibility of Blazor, it runs seamlessly on Windows, interactively tracking your moods and providing insightful analytics.

## ✨ Features
- **📝 Rich Journaling**: Create and edit journal entries with ease.
- **🎭 Mood Tracking**: Detailed mood selection system (Positive, Neutral, Negative) to capture how you feel.
- **📊 Analytics Dashboard**: Visualize your data with:
  - Mood Distribution Charts
  - Daily/Weekly Streaks
  - Word Count Analysis
  - Most Used Tags
- **📅 Calendar View**: Navigate through your past entries via an intuitive calendar interface.
- **🏷️ Tagging System**: Organize entries with custom tags.
- **🌗 Theme Support**: Switch between Dark and Light modes for comfortable writing.
- **💾 Local Data**: All data is stored locally using SQLite for privacy and offline access.

## 🛠️ Tech Stack
- **Framework**: [.NET MAUI](https://dotnet.microsoft.com/en-us/apps/maui) with [Blazor Hybrid](https://learn.microsoft.com/en-us/aspnet/core/blazor/hybrid/maui)
- **Language**: C#, Razor (HTML/CSS)
- **Database**: SQLite (via `sqlite-net-pcl`)
- **UI Components**: Native Blazor components with custom CSS styling

## 🚀 Getting Started

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (with .NET Multi-platform App UI development workload installed)

### Installation
1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd Journal-app
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Run the application**
   You can run it directly from Visual Studio or via CLI:
   ```bash
   dotnet build -t:Run -f net8.0-windows10.0.19041.0
   ```
   *(Note: Target framework may vary based on your setup)*

## 📂 Project Structure
- **/Components**: Blazor UI components and pages (Dashboard, JournalEntry, MoodSelection, etc.)
- **/Services**: Core business logic and data access services (JournalServices, MoodServices, etc.)
- **/Data**: Database context and connection handling.
- **/Models**: Data entities.