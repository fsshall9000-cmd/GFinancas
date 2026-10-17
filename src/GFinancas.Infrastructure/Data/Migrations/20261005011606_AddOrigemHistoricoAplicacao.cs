using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GFinancas.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOrigemHistoricoAplicacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Origem",
                table: "HistoricosValoresAplicacoes",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Origem",
                table: "HistoricosValoresAplicacoes");
        }
    }
}
