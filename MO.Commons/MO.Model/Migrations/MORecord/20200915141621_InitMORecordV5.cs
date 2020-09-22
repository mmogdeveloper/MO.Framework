using Microsoft.EntityFrameworkCore.Migrations;

namespace MO.Model.Migrations.MORecord
{
    public partial class InitMORecordV5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LoginDevice",
                table: "LoginRecord",
                maxLength: 20,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoginDevice",
                table: "LoginRecord");
        }
    }
}
