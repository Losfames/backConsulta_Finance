using ClosedXML.Excel;
using ConsultaFinance.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ConsultaFinance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RelatorioController : ControllerBase
{
    private readonly AppDbContext _context;

    public RelatorioController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("ExportarExcel")]
public async Task<IActionResult> ExportarExcel()
{
    var usuarioClaim = User.FindFirst(ClaimTypes.NameIdentifier);

    if (usuarioClaim == null)
        return Unauthorized("Usuário não autenticado.");

    var usuarioId = int.Parse(usuarioClaim.Value);

    var despesas = await _context.Despesas
        .Include(d => d.Projeto)
        .Where(d => d.Projeto.UsuarioId == usuarioId)
        .OrderBy(d => d.Data)
        .ToListAsync();

    if (!despesas.Any())
        return BadRequest("Nenhuma despesa encontrada.");

        // Título
        ws.Cell("A1").Value = "RELATÓRIO DE DESPESAS";
        ws.Range("A1:E1").Merge();
        ws.Cell("A1").Style.Font.Bold = true;
        ws.Cell("A1").Style.Font.FontSize = 16;
        ws.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        // Cabeçalho
        ws.Cell("A3").Value = "Projeto";
        ws.Cell("B3").Value = "Categoria";
        ws.Cell("C3").Value = "Descrição";
        ws.Cell("D3").Value = "Data";
        ws.Cell("E3").Value = "Valor";

        var header = ws.Range("A3:E3");
        header.Style.Font.Bold = true;
        header.Style.Fill.BackgroundColor = XLColor.LightGray;

        int linha = 4;
        decimal total = 0;

        foreach (var d in despesas)
        {
            ws.Cell(linha, 1).Value = d.Projeto?.Nome;
            ws.Cell(linha, 2).Value = d.Categoria;
            ws.Cell(linha, 3).Value = d.Descricao;
            ws.Cell(linha, 4).Value = d.Data.ToString("dd/MM/yyyy");
            ws.Cell(linha, 5).Value = d.ValorRealizado;

            ws.Cell(linha, 5).Style.NumberFormat.Format = "R$ #,##0.00";

            total += d.ValorRealizado;
            linha++;
        }

        ws.Cell(linha + 1, 4).Value = "TOTAL:";
        ws.Cell(linha + 1, 4).Style.Font.Bold = true;

        ws.Cell(linha + 1, 5).Value = total;
        ws.Cell(linha + 1, 5).Style.NumberFormat.Format = "R$ #,##0.00";
        ws.Cell(linha + 1, 5).Style.Font.Bold = true;

        ws.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        return File(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "RelatorioFinanceiro.xlsx"
        );
    }
}