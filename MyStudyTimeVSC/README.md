# MyStudyTime

A modern study management application built with WPF and .NET 8 to help you organize your subjects, notes, and flashcards.

## Features

- **Subject Management**: Create and organize different study subjects
- **Note Taking**: Add and manage notes for each subject
- **Flashcards**: Create and review flashcards with difficulty levels
- **Search Functionality**: Search across all your notes and flashcards
- **Data Persistence**: Dual-backend support:
  - **Database**: SQL Server with Entity Framework 6
  - **Fallback**: XML-based storage when database is unavailable

## Architecture

- **UI Framework**: WPF (Windows Presentation Foundation)
- **Framework**: .NET 8 (modern, long-term support)
- **ORM**: Entity Framework 6 with SQL Server
- **Pattern**: MVVM (Model-View-ViewModel)

## Project Structure

```
MyStudyTimeVSC/
├── src/
│   └── MyStudyTime/
│       ├── Core/                 # Helper classes (ObservableObject, RelayCommand, Converters)
│       ├── Database/             # EF6 DbContext
│       ├── MVVM/
│       │   ├── Model/            # Data models (Subject, Note, FlashCard, StudyGoal)
│       │   ├── View/             # XAML UI views
│       │   └── ViewModel/        # Binding logic
│       ├── Services/             # Data access (IDataService, EF6DataService, XmlDataService)
│       ├── Theme/                # XAML resource dictionaries
│       ├── Assets/               # Fonts and images
│       ├── App.xaml              # Application root
│       ├── MainWindow.xaml       # Main window
│       └── MyStudyTime.csproj    # Project file
├── .vscode/
│   ├── launch.json              # Debug configuration
│   └── tasks.json               # Build tasks
├── .gitignore
├── MyStudyTime.sln              # Solution file
└── README.md
```

## Prerequisites

- **.NET 8 SDK** or later (download from [dotnet.microsoft.com](https://dotnet.microsoft.com/download))
- **Visual Studio Code** or any .NET IDE
- **SQL Server Express** (for database persistence) - optional, falls back to XML storage

## Getting Started

### Build

```bash
# Build the project
dotnet build src/MyStudyTime/MyStudyTime.csproj

# Or use the VSCode build task
Ctrl+Shift+B
```

### Run

```bash
# Run the application
dotnet run --project src/MyStudyTime/MyStudyTime.csproj

# Or use the VSCode run task
Ctrl+Shift+B > select "run"
```

### Debug

- Set breakpoints in VSCode
- Press `F5` to start debugging (uses launch.json configuration)
- The app will launch with debugger attached

## Data Persistence

### Database (Recommended)

By default, the application attempts to connect to SQL Server Express:

- **Connection String**: `Server=.\SQLEXPRESS;Database=StudyTime;Integrated Security=true;`
- Modify in `appsettings.json` if needed

### Fallback Storage

If SQL Server is unavailable, data automatically persists to XML files in:

```
%APPDATA%\MyStudyTime\
├── subjects.xml
├── flashcards.xml
└── studygoals.xml
```

## Development

### Key Technologies

- **WPF**: Modern desktop UI with data binding
- **.NET 8**: Current LTS release with performance improvements
- **Entity Framework 6**: ORM for database operations
- **MVVM Pattern**: Separation of concerns for maintainability

### Project File Format

The project uses **SDK-style .csproj** format (modern, clean, and VSCode-friendly):

- No `packages.config` — uses `PackageReference` in `.csproj`
- Automatic resource discovery
- .NET 8 with Windows Desktop support

## Troubleshooting

### Database Connection Fails

If you see warnings about database unavailability:

1. Ensure SQL Server Express is installed and running
2. Check the connection string in `appsettings.json`
3. App will automatically fall back to XML storage

### Build Issues

```bash
# Clean and rebuild
dotnet clean src/MyStudyTime/MyStudyTime.csproj
dotnet build src/MyStudyTime/MyStudyTime.csproj
```

### Restore Dependencies

```bash
dotnet restore src/MyStudyTime/MyStudyTime.csproj
```

## License

This project is for personal study use.

---

**Happy studying!** 📚✨
