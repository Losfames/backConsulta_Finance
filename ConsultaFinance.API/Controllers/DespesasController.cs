using ConsultaFinance.API.Data;
using ConsultaFinance.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ConsultaFinance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DespesasController : ControllerBase
{
    private readonly AppDbContext _context;

    public DespesasController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/Despesas
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Despesa>>> GetDespesas()
    {
        var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        return await _context.Despesas
            .Include(d => d.Projeto)
            .Where(d => d.Projeto.UsuarioId == usuarioId)
            .ToListAsync();
    }

    // GET: api/Despesas/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Despesa>> GetDespesa(int id)
    {
        var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        var despesa = await _context.Despesas
            .Include(d => d.Projeto)
            .FirstOrDefaultAsync(d => d.Id == id && d.Projeto.UsuarioId == usuarioId);

        if (despesa == null) return NotFound();

        return despesa;
    }

    // POST: api/Despesas
    [HttpPost]
    public async Task<ActionResult<Despesa>> PostDespesa(Despesa despesa)
    {
        var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        var projeto = await _context.Projetos.FirstOrDefaultAsync(p => p.Id == despesa.ProjetoId && p.UsuarioId == usuarioId);
        if (projeto == null) return Unauthorized("Você não tem permissão para adicionar despesas neste projeto.");

        _context.Despesas.Add(despesa);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDespesa), new { id = despesa.Id }, despesa);
    }

    // PUT: api/Despesas/5 (MÉTODO ADICIONADO PARA COMPLETAR O CRUD)
    [HttpPut("{id}")]
    public async Task<IActionResult> PutDespesa(int id, Despesa despesa)
    {
        if (id != despesa.Id)
            return BadRequest();

        var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        // Verifica se a despesa existe e se pertence a um projeto do usuário logado antes de alterar
        var despesaExistente = await _context.Despesas
            .Include(d => d.Projeto)
            .FirstOrDefaultAsync(d => d.Id == id && d.Projeto.UsuarioId == usuarioId);

        if (despesaExistente == null)
            return Unauthorized("Você não tem permissão para alterar esta despesa.");

        // Desvincula o rastreamento da entidade antiga para evitar conflitos no EF Core
        _context.Entry(despesaExistente).State = EntityState.Detached;

        _context.Entry(despesa).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Despesas.AnyAsync(e => e.Id == id))
                return NotFound();
            else
                throw;
        }

        return NoContent();
    }

    // DELETE: api/Despesas/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDespesa(int id)
    {
        var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        var despesa = await _context.Despesas
            .Include(d => d.Projeto)
            .FirstOrDefaultAsync(d => d.Id == id && d.Projeto.UsuarioId == usuarioId);

        if (despesa == null) return NotFound();

        _context.Despesas.Remove(despesa);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}