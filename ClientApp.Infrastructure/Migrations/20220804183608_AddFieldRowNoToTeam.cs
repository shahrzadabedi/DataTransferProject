using Microsoft.EntityFrameworkCore.Migrations;

namespace ClientApp.Infrastructure.Migrations
{
    public partial class AddFieldRowNoToTeam : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RowNo",
                table: "Teams",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowNo",
                table: "Teams");
        }
    }
}
