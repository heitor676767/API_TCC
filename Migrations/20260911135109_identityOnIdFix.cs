using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiTCC.Migrations
{
    /// <inheritdoc />
    public partial class identityOnIdFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Id",
                table: "TB_USUARIOS");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "TB_USUARIOS",
                type: "int",
                nullable: false)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "TB_PETS");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "TB_PETS",
                type: "int",
                nullable: false)
                .Annotation("SqlServer:Identity", "1, 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Id",
                table: "TB_USUARIOS");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "TB_USUARIOS",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.DropColumn(
                name: "Id",
                table: "TB_PETS");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "TB_PETS",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
