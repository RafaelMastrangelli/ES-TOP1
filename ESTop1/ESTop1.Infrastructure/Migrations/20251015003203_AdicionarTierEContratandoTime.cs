using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ESTop1.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarTierEContratandoTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Contratando",
                table: "Times",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Tier",
                table: "Times",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Contratando",
                table: "Times");

            migrationBuilder.DropColumn(
                name: "Tier",
                table: "Times");
        }
    }
}
