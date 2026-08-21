using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiTCC.Migrations
{
    /// <inheritdoc />
    public partial class Migration21_08 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Senha",
                table: "TB_USUARIOS");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "TB_USUARIOS",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordHash",
                table: "TB_USUARIOS",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordSalt",
                table: "TB_USUARIOS",
                type: "varbinary(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Id",
                table: "TB_USUARIOS");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "TB_USUARIOS");

            migrationBuilder.DropColumn(
                name: "PasswordSalt",
                table: "TB_USUARIOS");

            migrationBuilder.AddColumn<string>(
                name: "Senha",
                table: "TB_USUARIOS",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }
    }
}
