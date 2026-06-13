using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultaFinance.API.Migrations
{
    /// <inheritdoc />
    public partial class AjusteModeloFinanceiro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataFim",
                table: "Projetos",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DataInicio",
                table: "Projetos",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Categoria",
                table: "Despesas",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataFim",
                table: "Projetos");

            migrationBuilder.DropColumn(
                name: "DataInicio",
                table: "Projetos");

            migrationBuilder.DropColumn(
                name: "Categoria",
                table: "Despesas");
        }
    }
}
