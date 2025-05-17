import axios from 'axios';
import { MinecraftEnchantment, MinecraftEnchantmentState, EnchantmentViewModel } from '../types/MinecraftEnchantment';

const API_URL = 'http://localhost:5080/api';

// Create axios instance
const apiClient = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-type': 'application/json'
  }
});

// API Service Methods
export const MinecraftService = {
  
  // Get all enchantments (static data)
  getAllEnchantments: async (): Promise<MinecraftEnchantment[]> => {
    const response = await apiClient.get<MinecraftEnchantment[]>('/enchantments');
    return response.data;
  },
  
  // Get all enchantment states (mutable data)
  getEnchantmentsState: async (): Promise<Record<string, MinecraftEnchantmentState>> => {
    const response = await apiClient.get<Record<string, MinecraftEnchantmentState>>('/enchantments/state');
    return response.data;
  },
  
  // Update enchantment state
  updateEnchantmentState: async (name: string, state: MinecraftEnchantmentState): Promise<void> => {
    await apiClient.put(`/enchantments/state/${encodeURIComponent(name)}`, state);
  },
  
  // Remove enchantment state (reset to default)
  removeEnchantmentState: async (name: string): Promise<void> => {
    await apiClient.delete(`/enchantments/state/${encodeURIComponent(name)}`);
  },
  
  // Enhanced API with minecraft-data
  
  // Get enchantments from the enhanced API
  getEnhancedEnchantments: async (): Promise<MinecraftEnchantment[]> => {
    const response = await apiClient.get<MinecraftEnchantment[]>('/enhanced/enchantments');
    return response.data;
  },
  
  // Get a single enchantment by name from the enhanced API
  getEnhancedEnchantment: async (name: string): Promise<MinecraftEnchantment> => {
    const response = await apiClient.get<MinecraftEnchantment>(`/enhanced/enchantments/${encodeURIComponent(name)}`);
    return response.data;
  },
  
  // Update enchantment state using the enhanced API
  updateEnhancedEnchantmentState: async (name: string, state: MinecraftEnchantmentState): Promise<void> => {
    await apiClient.put(`/enhanced/enchantments/state/${encodeURIComponent(name)}`, state);
  },
  
  // Remove enchantment state using the enhanced API
  removeEnhancedEnchantmentState: async (name: string): Promise<void> => {
    await apiClient.delete(`/enhanced/enchantments/state/${encodeURIComponent(name)}`);
  },
    // Helper method to get combined enchantment data for UI
  getEnchantmentsWithState: async (): Promise<EnchantmentViewModel[]> => {
    const [enchantments, stateMap] = await Promise.all([
      MinecraftService.getAllEnchantments(),
      MinecraftService.getEnchantmentsState()
    ]);
    
    return enchantments.map(enchantment => {
      const state = stateMap[enchantment.name] || {
        hasLibrarianTrade: false
      };
      
      return {
        ...enchantment,
        hasLibrarianTrade: state.hasLibrarianTrade || false,
        level: state.level,
        emeraldCost: state.emeraldCost
      };
    });
  }
};