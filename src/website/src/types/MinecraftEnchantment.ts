// Static enchantment data
export interface MinecraftEnchantment {
  name: string;
  maxLevel: number;
  description: string;
  applicableItems: string[];
  librariansCanTrade: boolean;
}

// Mutable enchantment state
export interface MinecraftEnchantmentState {
  hasLibrarianTrade: boolean;
  level?: number;
  emeraldCost?: number;
}

// Combined type for UI display
export interface EnchantmentViewModel extends MinecraftEnchantment {
  hasLibrarianTrade: boolean;
  level?: number;
  emeraldCost?: number;
}