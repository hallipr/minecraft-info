namespace MinecraftDataApi.Models;

public class MinecraftEnchantment
{
    public string Name { get; set; } = string.Empty;
    public int MaxLevel { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<string> ApplicableItems { get; set; } = new List<string>();
    public bool LibrariansCanTrade { get; set; }
}

public class MinecraftEnchantmentState
{
    public bool HasLibrarianTrade { get; set; }
    public int? Level { get; set; }
    public int? EmeraldCost { get; set; }
}