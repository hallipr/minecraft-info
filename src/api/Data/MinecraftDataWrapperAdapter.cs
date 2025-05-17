using MinecraftDataApi.Models;
using MinecraftDataWrapper;

namespace MinecraftDataApi.Data;

/// <summary>
/// Adapter to integrate MinecraftDataWrapper with our existing API
/// </summary>
public class MinecraftDataWrapperAdapter
{
    private readonly MinecraftDataService _minecraftDataService;
    private readonly IMinecraftDataService _existingDataService;
    private readonly ILogger<MinecraftDataWrapperAdapter> _logger;

    public MinecraftDataWrapperAdapter(
        MinecraftDataService minecraftDataService,
        IMinecraftDataService existingDataService,
        ILogger<MinecraftDataWrapperAdapter> logger)
    {
        _minecraftDataService = minecraftDataService;
        _existingDataService = existingDataService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all enchantments from minecraft-data and combines them with saved state
    /// </summary>
    public async Task<List<MinecraftEnchantment>> GetMergedEnchantmentsAsync()
    {
        try
        {
            // Get enchantments from minecraft-data
            var wrapperEnchantments = _minecraftDataService.GetAllEnchantments()
                .Where(e => e.Tradeable) // Only include librarian-tradeable enchantments
                .ToList();
            
            // Get saved state from existing service
            var enchantmentStates = await _existingDataService.GetEnchantmentsStateAsync();
            
            // Map from wrapper format to our API format
            var result = wrapperEnchantments.Select(e => new MinecraftEnchantment
            {
                Name = e.DisplayName,
                MaxLevel = e.MaxLevel,
                Description = $"{e.DisplayName} (Max Level: {e.MaxLevel})",
                ApplicableItems = GetApplicableItemsForCategory(e.Category),
                LibrariansCanTrade = e.Tradeable,
                // Set user-saved state if it exists
                HasLibrarianTrade = enchantmentStates.TryGetValue(e.DisplayName, out var state) && state.HasLibrarianTrade
            }).ToList();
            
            _logger.LogInformation("Combined {WrapperCount} enchantments from minecraft-data with {StateCount} saved states", 
                wrapperEnchantments.Count, enchantmentStates.Count);
                
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting merged enchantments");
            throw;
        }
    }
    
    /// <summary>
    /// Gets the full enchantment data with user state from a name
    /// </summary>
    public async Task<MinecraftEnchantment?> GetEnchantmentByNameAsync(string name)
    {
        // Try to find in minecraft-data by display name (most common case)
        var wrapperEnchantment = _minecraftDataService.GetAllEnchantments()
            .FirstOrDefault(e => string.Equals(e.DisplayName, name, StringComparison.OrdinalIgnoreCase));
            
        if (wrapperEnchantment == null)
        {
            // Try by internal name
            wrapperEnchantment = _minecraftDataService.GetAllEnchantments()
                .FirstOrDefault(e => string.Equals(e.Name, name, StringComparison.OrdinalIgnoreCase));
                
            if (wrapperEnchantment == null)
            {
                return null;
            }
        }
            
        // Get the saved state if available
        var states = await _existingDataService.GetEnchantmentsStateAsync();
        var hasState = states.TryGetValue(wrapperEnchantment.DisplayName, out var state);
        
        return new MinecraftEnchantment
        {
            Name = wrapperEnchantment.DisplayName,
            MaxLevel = wrapperEnchantment.MaxLevel,
            Description = $"{wrapperEnchantment.DisplayName} (Max Level: {wrapperEnchantment.MaxLevel})",
            ApplicableItems = GetApplicableItemsForCategory(wrapperEnchantment.Category),
            LibrariansCanTrade = wrapperEnchantment.Tradeable,
            HasLibrarianTrade = hasState && state!.HasLibrarianTrade
        };
    }
    
    // Helper method to determine applicable items based on category
    private List<string> GetApplicableItemsForCategory(string category)
    {
        return category switch
        {
            "armor" => new List<string> { "Helmet", "Chestplate", "Leggings", "Boots" },
            "armor_head" => new List<string> { "Helmet" },
            "armor_chest" => new List<string> { "Chestplate" },
            "armor_legs" => new List<string> { "Leggings" },
            "armor_feet" => new List<string> { "Boots" },
            "weapon" => new List<string> { "Sword" },
            "digger" => new List<string> { "Pickaxe", "Shovel", "Axe" },
            "breakable" => new List<string> { "Pickaxe", "Shovel", "Axe", "Hoe", "Shears", "Fishing Rod", "Bow", "Trident" },
            "bow" => new List<string> { "Bow" },
            "wearable" => new List<string> { "Elytra" },
            "crossbow" => new List<string> { "Crossbow" },
            "vanishable" => new List<string> { "Compass" },
            "fishing_rod" => new List<string> { "Fishing Rod" },
            "trident" => new List<string> { "Trident" },
            _ => new List<string>()
        };
    }
}
