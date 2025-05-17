# Minecraft Enchantment Tracker UI

The frontend for the Minecraft Enchantment Tracker application, built with React and TypeScript.

## Technologies Used

- **React**: JavaScript library for building user interfaces
- **TypeScript**: Static typing for JavaScript
- **Material-UI**: React UI framework with Material Design components
- **React Router**: Navigation for React apps
- **Axios**: Promise-based HTTP client for the browser

## Features

- Track which enchantments you have available through librarian trades
- Search enchantments by name, description, or applicable items
- Toggle trade status for each enchantment
- View detailed information about enchantments
- Material UI design with Minecraft-inspired theme

## Available Scripts

In the project directory, you can run:

### `npm start`

Runs the app in development mode.\
Open [http://localhost:3000](http://localhost:3000) to view it in your browser.

The page will reload if you make edits.\
You will also see any lint errors in the console.

### `npm test`

Launches the test runner in the interactive watch mode.

### `npm run build`

Builds the app for production to the `build` folder.\
It correctly bundles React in production mode and optimizes the build for the best performance.

## Connection to Backend

The frontend is configured to connect to the ASP.NET Core API running at `https://localhost:7113`.
If your API is running on a different URL, update the `API_URL` constant in `src/services/api.ts`.
