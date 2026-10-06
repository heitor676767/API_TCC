using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiTCC.Migrations
{
    /// <inheritdoc />
    public partial class AddDataInicioPasseio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataInicio",
                table: "TB_PASSEIOS",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataInicio",
                table: "TB_PASSEIOS");
        }
    }
}
