using Microsoft.EntityFrameworkCore.Migrations;

namespace MO.Model.Migrations.MOData
{
    public partial class InitMODataV3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeviceId",
                table: "GameUser",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNum",
                table: "GameUser",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WeiXinCode",
                table: "GameUser",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeviceId",
                table: "GameUser");

            migrationBuilder.DropColumn(
                name: "PhoneNum",
                table: "GameUser");

            migrationBuilder.DropColumn(
                name: "WeiXinCode",
                table: "GameUser");
        }
    }
}
