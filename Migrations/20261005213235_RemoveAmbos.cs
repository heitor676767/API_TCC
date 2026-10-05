using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiTCC.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAmbos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TB_PASSEIOS_TB_AVALIACOES_AvaliacaoId",
                table: "TB_PASSEIOS");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Usuario_TipoUsuario",
                table: "TB_USUARIOS");

            migrationBuilder.DropIndex(
                name: "IX_TB_PASSEIOS_AvaliacaoId",
                table: "TB_PASSEIOS");

            migrationBuilder.DropColumn(
                name: "AvaliacaoId",
                table: "TB_PASSEIOS");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Usuario_TipoUsuario",
                table: "TB_USUARIOS",
                sql: "TipoUsuario IN ('Dono','Petwalker')");

            migrationBuilder.AddForeignKey(
                name: "FK_TB_AVALIACOES_TB_PASSEIOS_IdPasseio",
                table: "TB_AVALIACOES",
                column: "IdPasseio",
                principalTable: "TB_PASSEIOS",
                principalColumn: "IdPasseio",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TB_AVALIACOES_TB_PASSEIOS_IdPasseio",
                table: "TB_AVALIACOES");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Usuario_TipoUsuario",
                table: "TB_USUARIOS");

            migrationBuilder.AddColumn<int>(
                name: "AvaliacaoId",
                table: "TB_PASSEIOS",
                type: "int",
                nullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Usuario_TipoUsuario",
                table: "TB_USUARIOS",
                sql: "TipoUsuario IN ('Dono','Petwalker','Ambos')");

            migrationBuilder.CreateIndex(
                name: "IX_TB_PASSEIOS_AvaliacaoId",
                table: "TB_PASSEIOS",
                column: "AvaliacaoId");

            migrationBuilder.AddForeignKey(
                name: "FK_TB_PASSEIOS_TB_AVALIACOES_AvaliacaoId",
                table: "TB_PASSEIOS",
                column: "AvaliacaoId",
                principalTable: "TB_AVALIACOES",
                principalColumn: "Id");
        }
    }
}
