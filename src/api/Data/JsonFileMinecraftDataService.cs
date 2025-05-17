using MinecraftDataApi.Models;
using System.Text.Json;

namespace MinecraftDataApi.Data;

public class JsonFileMinecraftDataService : IMinecraftDataService
{
    private readonly string _dataFolder;
    private readonly string _enchantmentsFilePath;
    private readonly string _defaultEnchantmentsFilePath;
    private readonly ILogger<JsonFileMinecraftDataService> _logger;
    private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };
    
    private List<MinecraftEnchantment>? _enchantments;

    public JsonFileMinecraftDataService(IConfiguration configuration, ILogger<JsonFileMinecraftDataService> logger)
    {
        _dataFolder = configuration["DataSettings:JsonFilePath"] ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Json");
        _enchantmentsFilePath = Path.Combine(_dataFolder, "minecraft-enchantments.json");
        _defaultEnchantmentsFilePath = Path.Combine(_dataFolder, "default-enchantments.json");
        _logger = logger;

        // Ensure the data directory exists
        if (!Directory.Exists(_dataFolder))
        {
            Directory.CreateDirectory(_dataFolder);
        }

        // Create the enchantments file if it doesn't exist
        // We'll create an empty array since we only want to store non-default values
        if (!File.Exists(_enchantmentsFilePath))
        {
            File.WriteAllText(_enchantmentsFilePath, JsonSerializer.Serialize(new List<object>()));
            _logger.LogInformation("Created an empty enchantments file since it didn't exist");
        }
    }
    
    #region Enchantment operations

    public async Task<List<MinecraftEnchantment>> GetAllEnchantmentsAsync()
    {
        if (_enchantments != null)
        {
            return _enchantments;
        }

        if (!File.Exists(_defaultEnchantmentsFilePath))
        {
            throw new Exception("Default enchantments file not found");
        }

        var json = await File.ReadAllTextAsync(_defaultEnchantmentsFilePath);
        _enchantments = JsonSerializer.Deserialize<List<MinecraftEnchantment>>(json) ?? new List<MinecraftEnchantment>();

        return _enchantments;
    }

    public async Task<Dictionary<string, MinecraftEnchantmentState>> GetEnchantmentsStateAsync()
    {
        if (!File.Exists(_enchantmentsFilePath))
        {
            return [];
        }

        var json = await File.ReadAllTextAsync(_enchantmentsFilePath);

        var state = JsonSerializer.Deserialize<Dictionary<string, MinecraftEnchantmentState>>(json) ?? [];

        return state;
    }

    public async Task UpdateEnchantmentAsync(string name, MinecraftEnchantmentState enchantment)
    {
        await CheckEnchantmentNameAsync(name);

        var state = await GetEnchantmentsStateAsync();
        state[name] = enchantment;
        await SaveEnchantmentsAsync(state);
    }

    public async Task RemoveEnchantmentAsync(string name)
    {
        var state = await GetEnchantmentsStateAsync();
        state.Remove(name);
        await SaveEnchantmentsAsync(state);
    }

    private async Task SaveEnchantmentsAsync(Dictionary<string, MinecraftEnchantmentState> state)
    {
        try
        {
            var json = JsonSerializer.Serialize(state, _jsonOptions);
            await File.WriteAllTextAsync(_enchantmentsFilePath, json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error writing Minecraft enchantments to JSON file");
            throw;
        }
    }

    private async Task CheckEnchantmentNameAsync(string name)
    {
        var allEnchantments = await GetAllEnchantmentsAsync();
        if (!allEnchantments.Any(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new KeyNotFoundException($"Enchantment '{name}' not found");
        }
    }
    
    #endregion
}