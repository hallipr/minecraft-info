# Minecraft Enchantment Tracker

A full-stack application for tracking Minecraft enchantments and librarian trades, built with ASP.NET Core REST API backend and React frontend.

## Project Structure

- **src/api/** - ASP.NET Core REST API backend with JSON file storage
- **src/website/** - React TypeScript frontend
- **external/minecraft-data-net/** - .NET wrapper library for minecraft-data

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

1. Navigate to the API folder:
   ```
   cd src/api
   ```
2. Run the API:
   ```
   dotnet run
   ```
3. The API will be available at https://localhost:7113 and http://localhost:5027

### Frontend UI

1. Navigate to the website folder:
   ```
   cd src/website
   ```
2. Run the React development server:
   ```
   npm start
   ```
3. The UI will be available at http://localhost:3000

## Data Storage

The application stores data in JSON files located in `src/api/Data/Json/`.
These files can be versioned with Git for tracking changes over time.

## Minecraft Data Library

The project uses a .NET wrapper for the minecraft-data repository, located at `external/minecraft-data-net/`. 
This library provides strongly-typed C# objects for game items, blocks, entities, enchantments, and more.

### Library Usage Example

```csharp
// Initialize with specific edition and version
var minecraft = new MinecraftData("bedrock", "1.19.1");

// Asynchronously load enchantments data
var enchantments = await minecraft.LoadEnchantmentsAsync();

// Access by name (case-insensitive)
Enchantment featherFalling = enchantments["feather_falling"];
Console.WriteLine($"Feather Falling max level: {featherFalling.MaxLevel}");
```