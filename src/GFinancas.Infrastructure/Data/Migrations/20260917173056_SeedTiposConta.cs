using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GFinancas.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    /// <summary>
    /// Insere os quatro tipos de conta iniciais no banco de dados.
    /// </summary>
    public partial class SeedTiposConta : Migration
    {
        /// <inheritdoc />
        /// <summary>
        /// Insere os tipos Conta Corrente, Poupança, Carteira e Conta de Investimento.
        /// </summary>
        /// <param name="migrationBuilder">Construtor das operações de alteração do banco de dados.</param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TiposConta",
                columns: new[] { "Id", "Nome" },
                values: new object[,]
                {
                    { 1, "Conta Corrente" },
                    { 2, "Poupança" },
                    { 3, "Carteira" },
                    { 4, "Conta de Investimento" }
                });
        }

        /// <inheritdoc />
        /// <summary>
        /// Remove os quatro tipos de conta inseridos por esta migração.
        /// </summary>
        /// <param name="migrationBuilder">Construtor das operações de alteração do banco de dados.</param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TiposConta",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TiposConta",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "TiposConta",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "TiposConta",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
