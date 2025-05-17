import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { ThemeProvider, CssBaseline, Container } from '@mui/material';
import { createTheme } from '@mui/material/styles';

// Components
import Header from './components/Header';
import Footer from './components/Footer';
import EnchantmentTracker from './components/EnchantmentTracker';

// Create a theme
const theme = createTheme({
  palette: {
    mode: 'light',
    primary: {
      main: '#43a047', // Minecraft green
    },
    secondary: {
      main: '#5d4037', // Minecraft brown
    },
    background: {
      default: '#f5f5f5',
      paper: '#fff',
    },
  },
});

function App() {
  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <Router>
        <Header />
        <Container sx={{ mt: 4, mb: 4, minHeight: 'calc(100vh - 160px)' }}>
          <Routes>
            <Route path="/" element={<EnchantmentTracker />} />
          </Routes>
        </Container>
        <Footer />
      </Router>
    </ThemeProvider>
  );
}

export default App;
