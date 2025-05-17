import React, { useState, useEffect, useCallback } from 'react';
import {
  Typography,
  Box,
  Paper,
  CircularProgress,
  Alert,
  TextField,
  InputAdornment,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Checkbox,
  Chip,
  Tooltip,
  TableSortLabel
} from '@mui/material';
import SearchIcon from '@mui/icons-material/Search';
import { EnchantmentViewModel } from '../types/MinecraftEnchantment';
import { MinecraftService } from '../services/api';

// Define type for sort fields and direction
type SortField = 'name' | 'maxLevel' | 'hasLibrarianTrade' | 'level' | 'emeraldCost';
type SortDirection = 'asc' | 'desc';

const EnchantmentTracker: React.FC = () => {
  const [enchantments, setEnchantments] = useState<EnchantmentViewModel[]>([]);
  const [filteredEnchantments, setFilteredEnchantments] = useState<EnchantmentViewModel[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string>('');
  const [searchTerm, setSearchTerm] = useState<string>('');
  const [sortField, setSortField] = useState<SortField>('name');
  const [sortDirection, setSortDirection] = useState<SortDirection>('asc');

  // Function to handle sorting
  const handleSort = (field: SortField) => {
    const isAsc = sortField === field && sortDirection === 'asc';
    setSortDirection(isAsc ? 'desc' : 'asc');
    setSortField(field);
  };

  // Function to sort enchantments - using useCallback to avoid recreating this function on each render
  const sortEnchantments = useCallback((enchants: EnchantmentViewModel[]) => {
    return [...enchants].sort((a, b) => {
      let compareA: any;
      let compareB: any;

      // Extract the values to compare based on the sort field
      switch (sortField) {
        case 'name':
          compareA = a.name.toLowerCase();
          compareB = b.name.toLowerCase();
          break;
        case 'maxLevel':
          compareA = a.maxLevel;
          compareB = b.maxLevel;
          break;
        case 'hasLibrarianTrade':
          compareA = a.hasLibrarianTrade ? 1 : 0;
          compareB = b.hasLibrarianTrade ? 1 : 0;
          break;
        case 'level':
          compareA = a.level || 0;
          compareB = b.level || 0;
          break;
        case 'emeraldCost':
          compareA = a.emeraldCost || 0;
          compareB = b.emeraldCost || 0;
          break;
        default:
          compareA = a.name.toLowerCase();
          compareB = b.name.toLowerCase();
      }

      // Apply direction to the comparison
      if (sortDirection === 'asc') {
        return compareA < compareB ? -1 : compareA > compareB ? 1 : 0;
      } else {
        return compareB < compareA ? -1 : compareB > compareA ? 1 : 0;
      }
    });
  }, [sortField, sortDirection]);

  // Helper function to update local state and filtered state
  const updateLocalEnchantments = useCallback((updatedEnchantments: EnchantmentViewModel[]) => {
    setEnchantments(updatedEnchantments);
    
    // Filter and then sort
    const filtered = updatedEnchantments.filter(e => 
      searchTerm.trim() === '' || 
      e.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
      e.description.toLowerCase().includes(searchTerm.toLowerCase()) ||
      e.applicableItems.some((item: string) => 
        item.toLowerCase().includes(searchTerm.toLowerCase())
      )
    );
    
    setFilteredEnchantments(sortEnchantments(filtered));
  }, [searchTerm, sortEnchantments]);

  // Use useCallback for fetchEnchantments to avoid recreating this function on each render
  const fetchEnchantments = useCallback(async () => {
    try {
      setLoading(true);
      // Use the helper method that combines enchantments and their states
      const data = await MinecraftService.getEnchantmentsWithState();
      setEnchantments(data);
      // Apply initial sorting
      setFilteredEnchantments(sortEnchantments(data));
      setError('');
    } catch (err) {
      console.error('Failed to fetch enchantments:', err);
      setError('Failed to load enchantment data. Please try again later.');
    } finally {
      setLoading(false);
    }
  }, [sortEnchantments]);

  // Fetch enchantments on initial load
  useEffect(() => {
    fetchEnchantments();
  }, [fetchEnchantments]);

  // Update filtered enchantments when search term, enchantments, or sorting changes
  useEffect(() => {
    let filtered = enchantments;

    // First filter
    if (searchTerm.trim() !== '') {
      filtered = enchantments.filter(enchant => 
        enchant.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
        enchant.description.toLowerCase().includes(searchTerm.toLowerCase()) ||
        enchant.applicableItems.some((item: string) => 
          item.toLowerCase().includes(searchTerm.toLowerCase())
        )
      );
    }

    // Then sort
    setFilteredEnchantments(sortEnchantments(filtered));
  }, [searchTerm, enchantments, sortEnchantments]);

  const handleTradeStatusChange = async (enchantment: EnchantmentViewModel, checked: boolean) => {
    try {
      // If unchecking the trade status, also clear level and cost
      const updatedState = {
        hasLibrarianTrade: checked,
        level: checked ? enchantment.level : undefined,
        emeraldCost: checked ? enchantment.emeraldCost : undefined
      };
      
      // Update state on the server
      await MinecraftService.updateEnchantmentState(enchantment.name, updatedState);
      
      // Update the local state
      const updatedEnchantments = enchantments.map(e => 
        e.name === enchantment.name ? { ...e, ...updatedState } : e
      );
      
      updateLocalEnchantments(updatedEnchantments);
    } catch (err) {
      console.error('Failed to update trade status:', err);
      setError('Failed to update trade status. Please try again.');
    }
  };
  
  const handleEnchantmentStateChange = async (enchantment: EnchantmentViewModel, updatedEnchantment: EnchantmentViewModel) => {
    try {
      // Only update if the enchantment has a librarian trade
      if (!updatedEnchantment.hasLibrarianTrade) {
        return;
      }
      
      // Extract only the mutable state fields
      const updatedState = {
        hasLibrarianTrade: updatedEnchantment.hasLibrarianTrade,
        level: updatedEnchantment.level,
        emeraldCost: updatedEnchantment.emeraldCost
      };
      
      // Update state on the server
      await MinecraftService.updateEnchantmentState(enchantment.name, updatedState);
      
      // Update the local state
      const updatedEnchantments = enchantments.map(e => 
        e.name === enchantment.name ? { ...e, ...updatedState } : e
      );
      
      updateLocalEnchantments(updatedEnchantments);
    } catch (err) {
      console.error('Failed to update enchantment details:', err);
      setError('Failed to update enchantment details. Please try again.');
    }
  };

  // Helper function to group applicable items into common sets
  const getGroupedApplicableItems = (items: string[]): string => {
    const armorPieces = ["Helmet", "Chestplate", "Leggings", "Boots"];
    const tools = ["Pickaxe", "Axe", "Shovel", "Hoe"];
    const weapons = ["Sword", "Bow", "Trident", "Crossbow"];
    
    // Check if all armor pieces are included
    const hasAllArmor = armorPieces.every(piece => items.includes(piece));
    
    // Check if all tools are included
    const hasAllTools = tools.every(tool => items.includes(tool));
    
    // Check if all weapons are included
    const hasAllWeapons = weapons.every(weapon => items.includes(weapon));

    // Build the result string
    let result: string[] = [];
    
    if (hasAllArmor) {
      result.push("All armor");
      // Remove individual armor pieces from further processing
      items = items.filter(item => !armorPieces.includes(item));
    }
    
    if (hasAllTools) {
      result.push("All tools");
      // Remove individual tools from further processing
      items = items.filter(item => !tools.includes(item));
    }
    
    if (hasAllWeapons) {
      result.push("All weapons");
      // Remove individual weapons from further processing
      items = items.filter(item => !weapons.includes(item));
    }
    
    // Add any remaining items
    result.push(...items);
    
    return result.join(", ");
  };

  // Original function to handle display of applicable items
  const getApplicableItemsText = (items: string[]) => {
    const groupedText = getGroupedApplicableItems(items);
    return groupedText;
  };

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" height="50vh">
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box>
      <Typography variant="h4" component="h1" gutterBottom>
        Enchantment Librarian Tracker
      </Typography>
      
      <Paper sx={{ p: 3, mb: 4 }}>
        <Typography variant="body1" paragraph>
          Track which enchantments you have available through librarian trades in your Minecraft world. 
          For each enchantment you've found, you can record its level and emerald cost.
          Click on column headers to sort the table.
        </Typography>
        
        <Box mb={3}>
          <TextField
            fullWidth
            placeholder="Search by name, description, or applicable items"
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            InputProps={{
              startAdornment: (
                <InputAdornment position="start">
                  <SearchIcon />
                </InputAdornment>
              ),
            }}
          />
        </Box>
        
        {error && <Alert severity="error" sx={{ mb: 3 }}>{error}</Alert>}

        {filteredEnchantments.length === 0 && !loading ? (
          <Alert severity="info">
            No enchantments found. {searchTerm ? 'Try a different search term.' : ''}
          </Alert>
        ) : (
          <TableContainer component={Paper} elevation={2}>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell>
                    <TableSortLabel
                      active={sortField === 'name'}
                      direction={sortField === 'name' ? sortDirection : 'asc'}
                      onClick={() => handleSort('name')}
                    >
                      Name
                    </TableSortLabel>
                  </TableCell>
                  <TableCell>
                    <TableSortLabel
                      active={sortField === 'maxLevel'}
                      direction={sortField === 'maxLevel' ? sortDirection : 'asc'}
                      onClick={() => handleSort('maxLevel')}
                    >
                      Max Level
                    </TableSortLabel>
                  </TableCell>
                  <TableCell>
                    Applicable Items
                  </TableCell>
                  <TableCell align="center">
                    <TableSortLabel
                      active={sortField === 'hasLibrarianTrade'}
                      direction={sortField === 'hasLibrarianTrade' ? sortDirection : 'asc'}
                      onClick={() => handleSort('hasLibrarianTrade')}
                    >
                      Have Librarian Trade
                    </TableSortLabel>
                  </TableCell>
                  <TableCell align="center">
                    <TableSortLabel
                      active={sortField === 'level'}
                      direction={sortField === 'level' ? sortDirection : 'asc'}
                      onClick={() => handleSort('level')}
                    >
                      Level
                    </TableSortLabel>
                  </TableCell>
                  <TableCell align="center">
                    <TableSortLabel
                      active={sortField === 'emeraldCost'}
                      direction={sortField === 'emeraldCost' ? sortDirection : 'asc'}
                      onClick={() => handleSort('emeraldCost')}
                    >
                      Emerald Cost
                    </TableSortLabel>
                  </TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {filteredEnchantments.map((enchantment) => (
                  <TableRow 
                    key={enchantment.name}
                    sx={{ 
                      backgroundColor: enchantment.hasLibrarianTrade ? 'rgba(76, 175, 80, 0.08)' : 'inherit'
                    }}
                  >
                    <TableCell component="th" scope="row">
                      <Tooltip 
                        title={enchantment.description}
                        arrow
                        placement="right"
                      >
                        <Typography variant="body1" fontWeight="medium">
                          {enchantment.name}
                        </Typography>
                      </Tooltip>
                    </TableCell>
                    <TableCell>
                      <Chip 
                        label={enchantment.maxLevel} 
                        size="small" 
                        color="primary" 
                      />
                    </TableCell>
                    <TableCell>
                      <Tooltip 
                        title={enchantment.applicableItems.join(', ')}
                        arrow
                        placement="top"
                      >
                        <Typography variant="body2">
                          {getApplicableItemsText(enchantment.applicableItems)}
                        </Typography>
                      </Tooltip>
                    </TableCell>
                    <TableCell align="center">
                      <Checkbox
                        checked={enchantment.hasLibrarianTrade}
                        onChange={(e) => handleTradeStatusChange(enchantment, e.target.checked)}
                        color="primary"
                        disabled={!enchantment.librariansCanTrade}
                      />
                    </TableCell>
                    <TableCell align="center">
                      <TextField
                        type="number"
                        variant="outlined"
                        size="small"
                        value={enchantment.level || ''}
                        onChange={(e) => {
                          const value = e.target.value === '' ? undefined : parseInt(e.target.value, 10);
                          handleEnchantmentStateChange(enchantment, {
                            ...enchantment,
                            level: value && !isNaN(value) ? Math.min(value, enchantment.maxLevel) : undefined
                          });
                        }}
                        disabled={!enchantment.hasLibrarianTrade}
                        inputProps={{
                          min: 1,
                          max: enchantment.maxLevel,
                          style: { textAlign: 'center' }
                        }}
                        sx={{ width: '80px' }}
                      />
                    </TableCell>
                    <TableCell align="center">
                      <TextField
                        type="number"
                        variant="outlined"
                        size="small"
                        value={enchantment.emeraldCost || ''}
                        onChange={(e) => {
                          const value = e.target.value === '' ? undefined : parseInt(e.target.value, 10);
                          handleEnchantmentStateChange(enchantment, {
                            ...enchantment,
                            emeraldCost: value && !isNaN(value) ? value : undefined
                          });
                        }}
                        disabled={!enchantment.hasLibrarianTrade}
                        inputProps={{
                          min: 1,
                          style: { textAlign: 'center' }
                        }}
                        sx={{ width: '80px' }}
                      />
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </TableContainer>
        )}
      </Paper>
    </Box>
  );
};

export default EnchantmentTracker;
