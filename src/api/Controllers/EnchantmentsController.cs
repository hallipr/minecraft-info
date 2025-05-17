using Microsoft.AspNetCore.Mvc;
using MinecraftDataApi.Data;
using MinecraftDataApi.Models;

namespace MinecraftDataApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnchantmentsController : ControllerBase
{
    private readonly IMinecraftDataService _dataService;
    private readonly ILogger<EnchantmentsController> _logger;

    public EnchantmentsController(IMinecraftDataService dataService, ILogger<EnchantmentsController> logger)
    {
        _dataService = dataService;
        _logger = logger;
    }

    // GET: api/enchantments
    // Returns all enchantments with default data merged with user saved trade status
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MinecraftEnchantment>>> GetAllEnchantments()
    {
        try
        {
            var enchantments = await _dataService.GetAllEnchantmentsAsync();
            return Ok(enchantments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all enchantments");
            return StatusCode(500, "An error occurred while retrieving enchantments");
        }
    }

    // GET: api/enchantments/state
    // Returns the mutable state of all enchantments
    [HttpGet("state")]
    public async Task<ActionResult<Dictionary<string, MinecraftEnchantmentState>>> GetEnchantmentsState()
    {
        try
        {
            var enchantmentsState = await _dataService.GetEnchantmentsStateAsync();
            return Ok(enchantmentsState);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving enchantment states");
            return StatusCode(500, "An error occurred while retrieving enchantment states");
        }
    }

    // PUT: api/enchantments/state/{name}
    // Only updates the mutable fields
    [HttpPut("state/{name}")]
    public async Task<ActionResult> UpdateEnchantment(string name, [FromBody] MinecraftEnchantmentState enchantment)
    {
        try
        {
            await _dataService.UpdateEnchantmentAsync(name, enchantment);
            
            return Ok();
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Enchantment '{name}' not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating trade status for enchantment '{Name}'", name);
            return StatusCode(500, $"An error occurred while updating trade status for enchantment '{name}'");
        }
    }

    // DELETE: api/enchantments/{name}
    // Removes the mutable state for the named enchantment
    [HttpDelete("state/{name}")]
    public async Task<ActionResult> RemoveEnchantmentAsync(string name)
    {
        try
        {
            await _dataService.RemoveEnchantmentAsync(name);
            return Ok();
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Enchantment '{name}' not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while resetting status for enchantment '{Name}'", name);
            return StatusCode(500, $"An error occurred while resetting status for enchantment '{name}'");
        }
    }
}
