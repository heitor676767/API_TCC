using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiTCC.Migrations
{
    /// <inheritdoc />
    public partial class AddIdPasseioAvaliacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AvaliacaoId",
                table: "TB_PASSEIOS",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdPasseio",
                table: "TB_AVALIACOES",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TB_PASSEIOS_AvaliacaoId",
                table: "TB_PASSEIOS",
                column: "AvaliacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_TB_AVALIACOES_IdPasseio",
                table: "TB_AVALIACOES",
                column: "IdPasseio",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TB_PASSEIOS_TB_AVALIACOES_AvaliacaoId",
                table: "TB_PASSEIOS",
                column: "AvaliacaoId",
                principalTable: "TB_AVALIACOES",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TB_PASSEIOS_TB_AVALIACOES_AvaliacaoId",
                table: "TB_PASSEIOS");

            migrationBuilder.DropIndex(
                name: "IX_TB_PASSEIOS_AvaliacaoId",
                table: "TB_PASSEIOS");

            migrationBuilder.DropIndex(
                name: "IX_TB_AVALIACOES_IdPasseio",
                table: "TB_AVALIACOES");

            migrationBuilder.DropColumn(
                name: "AvaliacaoId",
                table: "TB_PASSEIOS");

            migrationBuilder.DropColumn(
                name: "IdPasseio",
                table: "TB_AVALIACOES");
        }
    }
}
