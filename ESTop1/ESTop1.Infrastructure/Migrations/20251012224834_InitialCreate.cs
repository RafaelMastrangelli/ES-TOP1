using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ESTop1.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Times",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    Pais = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Times", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Jogadores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Apelido = table.Column<string>(type: "TEXT", nullable: false),
                    Pais = table.Column<string>(type: "TEXT", nullable: false),
                    Idade = table.Column<int>(type: "INTEGER", nullable: false),
                    FuncaoPrincipal = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Disponibilidade = table.Column<int>(type: "INTEGER", nullable: false),
                    TimeAtualId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ValorDeMercado = table.Column<decimal>(type: "TEXT", precision: 14, scale: 2, nullable: false),
                    Visivel = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jogadores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Jogadores_Times_TimeAtualId",
                        column: x => x.TimeAtualId,
                        principalTable: "Times",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Estatisticas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    JogadorId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Periodo = table.Column<string>(type: "TEXT", nullable: false),
                    Rating = table.Column<decimal>(type: "TEXT", nullable: false),
                    KD = table.Column<decimal>(type: "TEXT", nullable: false),
                    PartidasJogadas = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estatisticas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Estatisticas_Jogadores_JogadorId",
                        column: x => x.JogadorId,
                        principalTable: "Jogadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Estatisticas_JogadorId_Periodo",
                table: "Estatisticas",
                columns: new[] { "JogadorId", "Periodo" });

            migrationBuilder.CreateIndex(
                name: "IX_Jogadores_Apelido",
                table: "Jogadores",
                column: "Apelido");

            migrationBuilder.CreateIndex(
                name: "IX_Jogadores_TimeAtualId",
                table: "Jogadores",
                column: "TimeAtualId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Estatisticas");

            migrationBuilder.DropTable(
                name: "Jogadores");

            migrationBuilder.DropTable(
                name: "Times");
        }
    }
}
