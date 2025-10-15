using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ESTop1.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AtualizarEnumsDisponibilidade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "Times",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "Times");
        }
    }
}
