using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GFinancas.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    /// <summary>
    /// Acrescenta o armazenamento do hash da senha ao cadastro de usuários.
    /// </summary>
    public partial class AdicionarSenhaUsuario : Migration
    {
        /// <inheritdoc />
        /// <summary>
        /// Adiciona a coluna obrigatória SenhaHash à tabela Usuarios.
        /// </summary>
        /// <param name="migrationBuilder">Construtor das operações de alteração do banco de dados.</param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SenhaHash",
                table: "Usuarios",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        /// <summary>
        /// Remove a coluna SenhaHash da tabela Usuarios.
        /// </summary>
        /// <param name="migrationBuilder">Construtor das operações de alteração do banco de dados.</param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SenhaHash",
                table: "Usuarios");
        }
    }
}
