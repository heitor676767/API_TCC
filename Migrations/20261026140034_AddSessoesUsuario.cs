using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiTCC.Migrations
{
    /// <inheritdoc />
    public partial class AddSessoesUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TB_SESSOES_USUARIO",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CpfUsuario = table.Column<string>(type: "nchar(11)", fixedLength: true, maxLength: 11, nullable: false),
                    RefreshTokenHash = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NomeDispositivo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EnderecoIp = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UltimoAcesso = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataExpiracao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataRevogacao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_SESSOES_USUARIO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TB_SESSOES_USUARIO_TB_USUARIOS_CpfUsuario",
                        column: x => x.CpfUsuario,
                        principalTable: "TB_USUARIOS",
                        principalColumn: "Cpf",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TB_SESSOES_USUARIO_CpfUsuario_DataRevogacao",
                table: "TB_SESSOES_USUARIO",
                columns: new[] { "CpfUsuario", "DataRevogacao" });

            migrationBuilder.CreateIndex(
                name: "IX_TB_SESSOES_USUARIO_RefreshTokenHash",
                table: "TB_SESSOES_USUARIO",
                column: "RefreshTokenHash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TB_SESSOES_USUARIO");
        }
    }
}
