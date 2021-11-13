using Microsoft.EntityFrameworkCore.Migrations;

namespace WebRecommend.Migrations
{
    public partial class FixNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Desription",
                table: "Articles",
                newName: "Desсription");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Desсription",
                table: "Articles",
                newName: "Desription");
        }
    }
}
