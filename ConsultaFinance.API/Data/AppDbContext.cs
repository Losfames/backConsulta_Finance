using Microsoft.EntityFrameworkCore;
using ConsultaFinance.API.Models;

namespace ConsultaFinance.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; }

    public DbSet<Projeto> Projetos { get; set; }

    public DbSet<Despesa> Despesas { get; set; }
}