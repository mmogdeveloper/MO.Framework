using Microsoft.EntityFrameworkCore.Migrations;

namespace MO.Model.Migrations.MORecord
{
    public partial class InitMORecordV1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "LoginRecord",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "LoginRecord",
                type: "int",
                nullable: false,
                oldClrType: typeof(long));
        }
    }
}
