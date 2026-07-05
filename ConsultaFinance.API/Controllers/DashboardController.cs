using ConsultaFinance.API.Data;
using ConsultaFinance.API.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ConsultaFinance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Bloqueio de segurança ativado!
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
        // 1. Identifica o usuário logado pelo Token JWT
        var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        // 2. Filtra os totais apenas para os projetos DESTE usuário
        var totalProjetos = await _context.Projetos.CountAsync(p => p.UsuarioId == usuarioId);

        var projetosIds = await _context.Projetos
            .Where(p => p.UsuarioId == usuarioId)
            .Select(p => p.Id)
            .ToListAsync();

        var totalDespesas = await _context.Despesas.CountAsync(d => projetosIds.Contains(d.ProjetoId));

        var totalOrcado = await _context.Despesas
            .Where(d => projetosIds.Contains(d.ProjetoId))
            .Select(d => d.ValorOrcado)
            .DefaultIfEmpty(0)
            .SumAsync();

        var totalRealizado = await _context.Despesas
            .Where(d => projetosIds.Contains(d.ProjetoId))
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