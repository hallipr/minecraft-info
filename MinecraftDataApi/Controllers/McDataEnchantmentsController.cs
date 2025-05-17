using Microsoft.AspNetCore.Mvc;
using MinecraftDataApi.Models;
using MinecraftDataWrapper;
using MinecraftDataWrapper.Models;

namespace MinecraftDataApi.Controllers;

[ApiController]
[Route("api/mcdata/enchantments")]
public class McDataEnchantmentsController : ControllerBase
{
    private readonly MinecraftDataService _minecraftDataService;
    private readonly ILogger<McDataEnchantmentsController> _logger;

    public McDataEnchantmentsController(
        MinecraftDataService minecraftDataService,
        ILogger<McDataEnchantmentsController> logger)
    {
        _minecraftDataService = minecraftDataService;
        _logger = logger;
    }

    // GET: api/mcdata/enchantments
    [HttpGet]
    public ActionResult<IEnumerable<Enchantment>> GetAllEnchantments()
    {
        try
        {
            var enchantments = _minecraftDataService.GetAllEnchantments();
            return Ok(enchantments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving enchantments from minecraft-data");
            return StatusCode(500, "An error occurred while retrieving enchantments");
        }
    }

    // GET: api/mcdata/enchantments/tradeable
    [HttpGet("tradeable")]
    public ActionResult<IEnumerable<Enchantment>> GetTradeableEnchantments()
    {
        try
        {
            var enchantments = _minecraftDataService.GetTradeableEnchantments();
            return Ok(enchantments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tradeable enchantments from minecraft-data");
            return StatusCode(500, "An error occurred while retrieving tradeable enchantments");
        }
    }

    // GET: api/mcdata/enchantments/id/{id}
    [HttpGet("id/{id:int}")]
    public ActionResult<Enchantment> GetEnchantmentById(int id)
    {
        try
        {
            var enchantment = _minecraftDataService.GetEnchantmentById(id);
            if (enchantment == null)
            {
                return NotFound($"Enchantment with ID {id} not found");
            }
            
            return Ok(enchantment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving enchantment with ID {Id}", id);
            return StatusCode(500, "An error occurred while retrieving the enchantment");
        }
    }

    // GET: api/mcdata/enchantments/name/{name}
    [HttpGet("name/{name}")]
    public ActionResult<Enchantment> GetEnchantmentByName(string name)
    {
        try
        {
            var enchantment = _minecraftDataService.GetEnchantmentByName(name);
            if (enchantment == null)
            {
                return NotFound($"Enchantment with name '{name}' not found");
            }
            
            return Ok(enchantment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving enchantment with name {Name}", name);
            return StatusCode(500, "An error occurred while retrieving the enchantment");
        }
    }
    
    // GET: api/mcdata/enchantments/convert-to-api-model
    [HttpGet("convert-to-api-model")]
    public ActionResult<IEnumerable<MinecraftEnchantment>> ConvertToApiModel()
    {
        try
        {
            var enchantments = _minecraftDataService.GetAllEnchantments();
            
            // Convert from Minecraft-data format to our API format
            var apiEnchantments = enchantments
                .Where(e => e.Tradeable)  // Only include tradeable enchantments
                .Select(e => new MinecraftEnchantment
                {
                    Name = e.DisplayName,
                    MaxLevel = e.MaxLevel,
                    Description = $"{e.DisplayName} (Max Level: {e.MaxLevel})",
                    ApplicableItems = GetApplicableItemsForCategory(e.Category),
                    LibrariansCanTrade = e.Tradeable
                })
                .ToList();
                
            return Ok(apiEnchantments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error converting enchantments to API model");
            return StatusCode(500, "An error occurred while converting enchantments");
        }
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
