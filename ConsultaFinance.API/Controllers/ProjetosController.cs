using ConsultaFinance.API.Data;
using ConsultaFinance.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Projeto>>> GetProjetos()
    {
        return await _context.Projetos
            .Include(p => p.Usuario)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Projeto>> GetProjeto(int id)
    {
        var projeto = await _context.Projetos
            .Include(p => p.Usuario)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (projeto == null)
            return NotFound();

        return projeto;
    }

    [HttpPost]
    public async Task<ActionResult<Projeto>> PostProjeto(Projeto projeto)
    {
        _context.Projetos.Add(projeto);

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetProjeto),
            new { id = projeto.Id },
            projeto
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutProjeto(int id, Projeto projeto)
    {
        if (id != projeto.Id)
            return BadRequest();

        _context.Entry(projeto).State = EntityState.Modified;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProjeto(int id)
    {
        var projeto = await _context.Projetos.FindAsync(id);

        if (projeto == null)
            return NotFound();

        _context.Projetos.Remove(projeto);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}