namespace ConsultaFinance.API.DTOs;

public class DashboardDTO
{
    public int TotalProjetos { get; set; }

    public int TotalDespesas { get; set; }

    public decimal TotalOrcado { get; set; }

    public decimal TotalRealizado { get; set; }

    public decimal Saldo { get; set; }
}