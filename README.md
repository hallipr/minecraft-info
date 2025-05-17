# Minecraft Enchantment Tracker

A full-stack application for tracking Minecraft enchantments and librarian trades, built with ASP.NET Core REST API backend and React frontend.

## Project Structure

- **MinecraftDataApi/** - ASP.NET Core REST API backend with JSON file storage
- **minecraft-tracker-ui/** - React TypeScript frontend

## Backend Features

- RESTful API for managing Minecraft enchantments
- JSON file-based data storage with Git versioning
- Operations for reading enchantments and updating trade status
- Static enchantment details with mutable trade status

## Frontend Features

- Modern UI built with React and Material-UI
- List view of all Minecraft enchantments
- Search functionality for enchantments
- Tracking librarian trades for enchantments
- Grouping by applicable item types

## How to Run

### Backend API

1. Navigate to the MinecraftDataApi folder:
   ```
   cd MinecraftDataApi
   ```
2. Run the API:
   ```
   dotnet run
   ```
3. The API will be available at https://localhost:7113 and http://localhost:5027

### Frontend UI

1. Navigate to the minecraft-tracker-ui folder:
   ```
   cd minecraft-tracker-ui
   ```
2. Run the React development server:
   ```
   npm start
   ```
3. The UI will be available at http://localhost:3000

## Data Storage

The application stores data in JSON files located in `/MinecraftDataApi/Data/Json/`.
These files can be versioned with Git for tracking changes over time.