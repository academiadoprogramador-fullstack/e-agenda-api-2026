using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eAgenda.Infra.Compartilhado.Orm.Migrations
{
    /// <inheritdoc />
    public partial class Add_TBCompromisso : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TBCompromisso",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Assunto = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DataOcorrencia = table.Column<DateTime>(type: "date", nullable: false),
                    HoraInicio = table.Column<TimeSpan>(type: "time", nullable: false),
                    HoraTermino = table.Column<TimeSpan>(type: "time", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Local = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Link = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ContatoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBCompromisso", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBCompromisso_TBContato",
                        column: x => x.ContatoId,
                        principalTable: "TBContato",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TBCompromisso_ContatoId",
                table: "TBCompromisso",
                column: "ContatoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TBCompromisso");
        }
    }
}
