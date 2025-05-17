import React from 'react';
import { Link as RouterLink } from 'react-router-dom';
import { AppBar, Toolbar, Typography, Button, Box } from '@mui/material';
import EnchantmentIcon from '@mui/icons-material/AutoFixHigh';

const Header: React.FC = () => {
  return (
    <AppBar position="static">
      <Toolbar>        <Typography 
          variant="h6" 
          component={RouterLink} 
          to="/" 
          sx={{ 
            flexGrow: 1, 
            textDecoration: 'none', 
            color: 'white', 
            fontWeight: 'bold' 
          }}
        >
          Minecraft Enchantment Tracker
        </Typography><Box>
          <Button 
            color="inherit" 
            component={RouterLink} 
            to="/" 
            sx={{ mr: 2 }}
            startIcon={<EnchantmentIcon />}
          >
            Enchantments
          </Button>
        </Box>
      </Toolbar>
    </AppBar>
  );
};

export default Header;