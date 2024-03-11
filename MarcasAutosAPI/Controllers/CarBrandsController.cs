using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarcasAutosAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CarBrandsController(ApplicationDbContext db) : ControllerBase
{
    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<string[]>> GetCarBrandsAsync(CancellationToken cancellationToken = default)
        => Ok(await db.CarBrands.Select(c => c.Name).ToArrayAsync(cancellationToken));
}
