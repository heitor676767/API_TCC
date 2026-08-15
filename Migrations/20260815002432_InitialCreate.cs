using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiTCC.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TB_USUARIOS",
                columns: table => new
                {
                    Cpf = table.Column<string>(type: "nchar(11)", fixedLength: true, maxLength: 11, nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Cep = table.Column<string>(type: "nchar(8)", fixedLength: true, maxLength: 8, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    Senha = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TipoUsuario = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    StatusUser = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    Telefone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Genero = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Nao informado"),
                    Foto = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UltimoLogin = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()"),
                    DataCadastro = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_USUARIOS", x => x.Cpf);
                    table.CheckConstraint("CK_Usuario_StatusUser", "StatusUser IN ('Disponivel','Ausente','Nao Perturbar','Invisivel')");
                    table.CheckConstraint("CK_Usuario_TipoUsuario", "TipoUsuario IN ('Dono','Petwalker','Ambos')");
                });

            migrationBuilder.CreateTable(
                name: "TB_PETS",
                columns: table => new
                {
                    Rga = table.Column<string>(type: "nchar(7)", fixedLength: true, maxLength: 7, nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Especie = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Foto = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Raca = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Peso = table.Column<int>(type: "int", nullable: false),
                    Porte = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Sexo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CpfDono = table.Column<string>(type: "nchar(11)", fixedLength: true, maxLength: 11, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_PETS", x => x.Rga);
                    table.CheckConstraint("CK_Pet_Porte", "Porte IN ('Grande','Medio','Pequeno')");
                    table.CheckConstraint("CK_Pet_Sexo", "Sexo IN ('Macho','Femea')");
                    table.ForeignKey(
                        name: "FK_TB_PETS_TB_USUARIOS_CpfDono",
                        column: x => x.CpfDono,
                        principalTable: "TB_USUARIOS",
                        principalColumn: "Cpf",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TB_PETWALKER_PERFIL",
                columns: table => new
                {
                    Cpf = table.Column<string>(type: "nchar(11)", fixedLength: true, maxLength: 11, nullable: false),
                    Disponibilidade = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    AreaAtendimento = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_PETWALKER_PERFIL", x => x.Cpf);
                    table.ForeignKey(
                        name: "FK_TB_PETWALKER_PERFIL_TB_USUARIOS_Cpf",
                        column: x => x.Cpf,
                        principalTable: "TB_USUARIOS",
                        principalColumn: "Cpf",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TB_AVALIACOES",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Comentario = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Nota = table.Column<int>(type: "int", nullable: false),
                    DataPublicacao = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "GETDATE()"),
                    Rga = table.Column<string>(type: "nchar(7)", fixedLength: true, maxLength: 7, nullable: false),
                    CpfPetwalker = table.Column<string>(type: "nchar(11)", fixedLength: true, maxLength: 11, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_AVALIACOES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TB_AVALIACOES_TB_PETS_Rga",
                        column: x => x.Rga,
                        principalTable: "TB_PETS",
                        principalColumn: "Rga",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TB_AVALIACOES_TB_PETWALKER_PERFIL_CpfPetwalker",
                        column: x => x.CpfPetwalker,
                        principalTable: "TB_PETWALKER_PERFIL",
                        principalColumn: "Cpf",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TB_PASSEIOS",
                columns: table => new
                {
                    IdPasseio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusPass = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    DataPass = table.Column<DateTime>(type: "date", nullable: false),
                    Duracao = table.Column<int>(type: "int", nullable: false),
                    Rga = table.Column<string>(type: "nchar(7)", fixedLength: true, maxLength: 7, nullable: false),
                    CpfPetwalker = table.Column<string>(type: "nchar(11)", fixedLength: true, maxLength: 11, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_PASSEIOS", x => x.IdPasseio);
                    table.ForeignKey(
                        name: "FK_TB_PASSEIOS_TB_PETS_Rga",
                        column: x => x.Rga,
                        principalTable: "TB_PETS",
                        principalColumn: "Rga",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TB_PASSEIOS_TB_PETWALKER_PERFIL_CpfPetwalker",
                        column: x => x.CpfPetwalker,
                        principalTable: "TB_PETWALKER_PERFIL",
                        principalColumn: "Cpf",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TB_LOCALIZACAO_PASSEIO",
                columns: table => new
                {
                    IdPasseio = table.Column<int>(type: "int", nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(9,6)", nullable: false),
                    Longitude = table.Column<decimal>(type: "decimal(9,6)", nullable: false),
                    Cep = table.Column<string>(type: "nchar(8)", fixedLength: true, maxLength: 8, nullable: false),
                    Numero = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_LOCALIZACAO_PASSEIO", x => x.IdPasseio);
                    table.ForeignKey(
                        name: "FK_TB_LOCALIZACAO_PASSEIO_TB_PASSEIOS_IdPasseio",
                        column: x => x.IdPasseio,
                        principalTable: "TB_PASSEIOS",
                        principalColumn: "IdPasseio",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TB_TRANSACOES",
                columns: table => new
                {
                    IdTransacao = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MtdPgmt = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StatusPgmt = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    DataPgmt = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "GETDATE()"),
                    IdPasseio = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_TRANSACOES", x => x.IdTransacao);
                    table.ForeignKey(
                        name: "FK_TB_TRANSACOES_TB_PASSEIOS_IdPasseio",
                        column: x => x.IdPasseio,
                        principalTable: "TB_PASSEIOS",
                        principalColumn: "IdPasseio",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TB_AVALIACOES_CpfPetwalker",
                table: "TB_AVALIACOES",
                column: "CpfPetwalker");

            migrationBuilder.CreateIndex(
                name: "IX_TB_AVALIACOES_Rga",
                table: "TB_AVALIACOES",
                column: "Rga");

            migrationBuilder.CreateIndex(
                name: "IX_TB_PASSEIOS_CpfPetwalker",
                table: "TB_PASSEIOS",
                column: "CpfPetwalker");

            migrationBuilder.CreateIndex(
                name: "IX_TB_PASSEIOS_Rga",
                table: "TB_PASSEIOS",
                column: "Rga");

            migrationBuilder.CreateIndex(
                name: "IX_TB_PETS_CpfDono",
                table: "TB_PETS",
                column: "CpfDono");

            migrationBuilder.CreateIndex(
                name: "IX_TB_TRANSACOES_IdPasseio",
                table: "TB_TRANSACOES",
                column: "IdPasseio");

            migrationBuilder.CreateIndex(
                name: "IX_TB_USUARIOS_Email",
                table: "TB_USUARIOS",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TB_AVALIACOES");

            migrationBuilder.DropTable(
                name: "TB_LOCALIZACAO_PASSEIO");

            migrationBuilder.DropTable(
                name: "TB_TRANSACOES");

            migrationBuilder.DropTable(
                name: "TB_PASSEIOS");

            migrationBuilder.DropTable(
                name: "TB_PETS");

            migrationBuilder.DropTable(
                name: "TB_PETWALKER_PERFIL");

            migrationBuilder.DropTable(
                name: "TB_USUARIOS");
        }
    }
}
