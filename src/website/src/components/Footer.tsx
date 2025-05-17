import React from 'react';
import { Box, Typography, Link } from '@mui/material';

const Footer: React.FC = () => {
  return (
    <Box sx={{
      py: 3,
      px: 2, 
      mt: 'auto', 
      backgroundColor: (theme) => theme.palette.grey[200],
      textAlign: 'center'
    }}>
      <Typography variant="body2" color="text.secondary">        {'© '}
        <Link color="inherit" href="#">
          Minecraft Enchantment Tracker
        </Link>{' '}
        {new Date().getFullYear()}
      </Typography>
    </Box>
  );
};

export default Footer;