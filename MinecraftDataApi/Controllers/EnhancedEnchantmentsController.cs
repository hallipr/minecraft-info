using Microsoft.AspNetCore.Mvc;
using MinecraftDataApi.Data;
using MinecraftDataApi.Models;

namespace MinecraftDataApi.Controllers;

[ApiController]
[Route("api/enhanced/enchantments")]
public class EnhancedEnchantmentsController : ControllerBase
{
    private readonly MinecraftDataWrapperAdapter _adapter;
    private readonly IMinecraftDataService _dataService;
    private readonly ILogger<EnhancedEnchantmentsController> _logger;

    public EnhancedEnchantmentsController(
        MinecraftDataWrapperAdapter adapter,
        IMinecraftDataService dataService,
        ILogger<EnhancedEnchantmentsController> logger)
    {
        _adapter = adapter;
        _dataService = dataService;
        _logger = logger;
    }

    // GET: api/enhanced/enchantments
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MinecraftEnchantment>>> GetAllEnchantments()
    {
        try
        {
            var enchantments = await _adapter.GetMergedEnchantmentsAsync();
            return Ok(enchantments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving enhanced enchantments");
            return StatusCode(500, "An error occurred while retrieving enchantments");
        }
    }

    // GET: api/enhanced/enchantments/{name}
    [HttpGet("{name}")]
    public async Task<ActionResult<MinecraftEnchantment>> GetEnchantmentByName(string name)
    {
        try
        {
            var enchantment = await _adapter.GetEnchantmentByNameAsync(name);
            if (enchantment == null)
            {
                return NotFound($"Enchantment '{name}' not found");
            }
            
            return Ok(enchantment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving enhanced enchantment '{Name}'", name);
            return StatusCode(500, $"An error occurred while retrieving enchantment '{name}'");
        }
    }

    // PUT: api/enhanced/enchantments/state/{name}
    [HttpPut("state/{name}")]
    public async Task<ActionResult> UpdateEnchantment(string name, [FromBody] MinecraftEnchantmentState enchantmentState)
    {
        try
        {
            // First check that the enchantment exists in the minecraft-data
            var enchantment = await _adapter.GetEnchantmentByNameAsync(name);
            if (enchantment == null)
            {
                return NotFound($"Enchantment '{name}' not found");
            }
            
            // Then update the state using the existing service
            await _dataService.UpdateEnchantmentAsync(name, enchantmentState);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating state for enchantment '{Name}'", name);
            return StatusCode(500, $"An error occurred while updating state for enchantment '{name}'");
        }
    }

    // DELETE: api/enhanced/enchantments/state/{name}
    [HttpDelete("state/{name}")]
    public async Task<ActionResult> RemoveEnchantment(string name)
    {
        try
        {
            // Check that the enchantment exists in the minecraft-data
            var enchantment = await _adapter.GetEnchantmentByNameAsync(name);
            if (enchantment == null)
            {
                return NotFound($"Enchantment '{name}' not found");
            }
            
            // Remove the state using the existing service
            await _dataService.RemoveEnchantmentAsync(name);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing state for enchantment '{Name}'", name);
            return StatusCode(500, $"An error occurred while removing state for enchantment '{name}'");
        }
    }
}
