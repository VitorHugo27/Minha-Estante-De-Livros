using Microsoft.AspNetCore.Mvc;
using MinhaEstante.Application.Dtos;
using MinhaEstante.Application.Services;

namespace MinhaEstante.Api.Controllers;

[ApiController]
[Route("api/estatisticas")]
public class EstatisticasController(EstatisticasService estatisticasService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<EstatisticasResponse>> Obter(CancellationToken cancellationToken)
    {
        var estatisticas = await estatisticasService.ObterAsync(cancellationToken);
        return Ok(estatisticas);
    }
}
