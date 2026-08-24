using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eAgenda.Infra.Compartilhado.Orm.Migrations
{
    /// <inheritdoc />
    public partial class Add_TBDespesa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TBDespesa",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DataOcorrencia = table.Column<DateTime>(type: "date", nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FormaPagamento = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBDespesa", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TBDespesaCategoria",
                columns: table => new
                {
                    CategoriasId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DespesasId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBDespesaCategoria", x => new { x.CategoriasId, x.DespesasId });
                    table.ForeignKey(
                        name: "FK_TBDespesaCategoria_TBCategoria_CategoriasId",
                        column: x => x.CategoriasId,
                        principalTable: "TBCategoria",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TBDespesaCategoria_TBDespesa_DespesasId",
                        column: x => x.DespesasId,
                        principalTable: "TBDespesa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TBDespesaCategoria_DespesasId",
                table: "TBDespesaCategoria",
                column: "DespesasId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TBDespesaCategoria");

            migrationBuilder.DropTable(
                name: "TBDespesa");
        }
    }
}
