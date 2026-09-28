using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiTCC.Migrations
{
    /// <inheritdoc />
    public partial class AddLatLngPetwalker : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                table: "TB_PETWALKER_PERFIL",
                type: "decimal(9,6)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                table: "TB_PETWALKER_PERFIL",
                type: "decimal(9,6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "TB_PETWALKER_PERFIL");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "TB_PETWALKER_PERFIL");
        }
    }
}
