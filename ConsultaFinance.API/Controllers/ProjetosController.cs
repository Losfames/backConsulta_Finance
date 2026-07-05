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
public class ProjetosController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProjetosController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/Projetos (Lista apenas os projetos do usuário logado)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Projeto>>> GetProjetos()
    {
        var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        return await _context.Projetos
            .Include(p => p.Usuario)
            .Where(p => p.UsuarioId == usuarioId)
            .ToListAsync();
    }

    // GET: api/Projetos/5 (Busca apenas se o projeto for do usuário logado)
    [HttpGet("{id}")]
    public async Task<ActionResult<Projeto>> GetProjeto(int id)
    {
        var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        var projeto = await _context.Projetos
            .Include(p => p.Usuario)
            .FirstOrDefaultAsync(p => p.Id == id && p.UsuarioId == usuarioId);

        if (projeto == null)
            return NotFound();

        return projeto;
    }

    // POST: api/Projetos
    [HttpPost]
    public async Task<ActionResult<Projeto>> PostProjeto(Projeto projeto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
        {
            return Unauthorized("Usuário não autenticado.");
        }

        var userId = int.Parse(userIdClaim.Value);
        projeto.UsuarioId = userId;

        _context.Projetos.Add(projeto);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetProjeto),
            new { id = projeto.Id },
            projeto
        );
    }

    // PUT: api/Projetos/5 (Altera apenas se o projeto for do usuário logado)
    [HttpPut("{id}")]
    public async Task<IActionResult> PutProjeto(int id, Projeto projeto)
    {
        if (id != projeto.Id)
            return BadRequest();

        var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        // Valida se o projeto realmente pertence ao usuário logado antes de alterar
        var projetoExiste = await _context.Projetos.AnyAsync(p => p.Id == id && p.UsuarioId == usuarioId);
        if (!projetoExiste)
        {
            return Unauthorized("Você não tem permissão para alterar este projeto.");
        }

        // Garante que o vínculo do usuário não seja alterado na requisição
        projeto.UsuarioId = usuarioId;

        _context.Entry(projeto).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Projetos.AnyAsync(p => p.Id == id))
                return NotFound();
            else
                throw;
        }

        return NoContent();
    }

    // DELETE: api/Projetos/5 (Apaga apenas se o projeto for do usuário logado)
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProjeto(int id)
    {
        var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        var projeto = await _context.Projetos.FirstOrDefaultAsync(p => p.Id == id && p.UsuarioId == usuarioId);

        if (projeto == null)
            return NotFound();

        _context.Projetos.Remove(projeto);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}