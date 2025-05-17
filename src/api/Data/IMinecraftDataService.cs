using MinecraftDataApi.Controllers;
using MinecraftDataApi.Models;

namespace MinecraftDataApi.Data;

public interface IMinecraftDataService
{
    // Enchantment operations
    Task<List<MinecraftEnchantment>> GetAllEnchantmentsAsync();
    Task<Dictionary<string, MinecraftEnchantmentState>> GetEnchantmentsStateAsync();
    Task UpdateEnchantmentAsync(string name, MinecraftEnchantmentState enchantmentState);
    Task RemoveEnchantmentAsync(string name);
}