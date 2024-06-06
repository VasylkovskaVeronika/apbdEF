using EF_Example.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace EF_Example.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ClientsController: ControllerBase
{
    private readonly ScaffoldContext _context;

    public ClientsController(ScaffoldContext context)
    {
        _context = context;
    }
    private bool ClientExists(int id)
    {
        return _context.Clients.Any(e => e.IdClient == id);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteClientById(int id)
    {
        var client = await _context.Clients.FindAsync(id);
        if (!ClientExists(client.IdClient))
        {
            return NotFound("Client not found.");
        }

        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();

        return NoContent();
    }

}