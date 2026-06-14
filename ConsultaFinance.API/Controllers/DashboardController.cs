using ConsultaFinance.API.Data;
using ConsultaFinance.API.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace ConsultaFinance.API.Controllers;

[ApiController]
[Route("api/[controller]")]

public class DashboardController : ControllerBase
{
    private readonly AppDbContext _context;

    public DashboardController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<DashboardDTO>> GetDashboard()
    {
        var totalProjetos = await _context.Projetos.CountAsync();
        var totalDespesas = await _context.Despesas.CountAsync();

        var totalOrcado = await _context.Despesas
        .Select(d => d.ValorOrcado)
        .DefaultIfEmpty(0)
        .SumAsync();

        var totalRealizado = await _context.Despesas
        .Select(d => d.ValorRealizado)
        .DefaultIfEmpty(0)
        .SumAsync();

        var dashboard = new DashboardDTO
        {
            TotalProjetos = totalProjetos,
            TotalDespesas = totalDespesas,
            TotalOrcado = totalOrcado,
            TotalRealizado = totalRealizado,
            Saldo = totalOrcado - totalRealizado
        };

        return Ok(dashboard);
    }
}