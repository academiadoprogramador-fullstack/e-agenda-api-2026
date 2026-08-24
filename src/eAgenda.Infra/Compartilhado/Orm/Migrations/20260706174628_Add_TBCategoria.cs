using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eAgenda.Infra.Compartilhado.Orm.Migrations
{
    /// <inheritdoc />
    public partial class Add_TBCategoria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TBCategoria",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBCategoria", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "UQ_TBCategoria_Titulo",
                table: "TBCategoria",
                column: "Titulo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TBCategoria");
        }
    }
}
