using Microsoft.EntityFrameworkCore.Migrations;

namespace MO.Model.Migrations.MOData
{
    public partial class InitMODataV8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<ushort>(
                name: "LoginPort",
                table: "ServerConfig",
                maxLength: 6,
                nullable: false,
                defaultValue: (ushort)8001,
                oldClrType: typeof(ushort),
                oldType: "smallint unsigned",
                oldMaxLength: 6);

            migrationBuilder.AlterColumn<string>(
                name: "LoginIP",
                table: "ServerConfig",
                maxLength: 20,
                nullable: true,
                defaultValue: "127.0.0.1",
                oldClrType: typeof(string),
                oldType: "varchar(20) CHARACTER SET utf8mb4",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<ushort>(
                name: "GatePort",
                table: "ServerConfig",
                maxLength: 6,
                nullable: false,
                defaultValue: (ushort)9001,
                oldClrType: typeof(ushort),
                oldType: "smallint unsigned",
                oldMaxLength: 6);

            migrationBuilder.AlterColumn<string>(
                name: "GateIP",
                table: "ServerConfig",
                maxLength: 20,
                nullable: true,
                defaultValue: "127.0.0.1",
                oldClrType: typeof(string),
                oldType: "varchar(20) CHARACTER SET utf8mb4",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<ushort>(
                name: "ApiPort",
                table: "ServerConfig",
                maxLength: 6,
                nullable: false,
                defaultValue: (ushort)8000,
                oldClrType: typeof(ushort),
                oldType: "smallint unsigned",
                oldMaxLength: 6);

            migrationBuilder.AlterColumn<string>(
                name: "ApiIP",
                table: "ServerConfig",
                maxLength: 20,
                nullable: true,
                defaultValue: "127.0.0.1",
                oldClrType: typeof(string),
                oldType: "varchar(20) CHARACTER SET utf8mb4",
                oldMaxLength: 20,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<ushort>(
                name: "LoginPort",
                table: "ServerConfig",
                type: "smallint unsigned",
                maxLength: 6,
                nullable: false,
                oldClrType: typeof(ushort),
                oldMaxLength: 6,
                oldDefaultValue: (ushort)8001);

            migrationBuilder.AlterColumn<string>(
                name: "LoginIP",
                table: "ServerConfig",
                type: "varchar(20) CHARACTER SET utf8mb4",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 20,
                oldNullable: true,
                oldDefaultValue: "127.0.0.1");

            migrationBuilder.AlterColumn<ushort>(
                name: "GatePort",
                table: "ServerConfig",
                type: "smallint unsigned",
                maxLength: 6,
                nullable: false,
                oldClrType: typeof(ushort),
                oldMaxLength: 6,
                oldDefaultValue: (ushort)9001);

            migrationBuilder.AlterColumn<string>(
                name: "GateIP",
                table: "ServerConfig",
                type: "varchar(20) CHARACTER SET utf8mb4",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 20,
                oldNullable: true,
                oldDefaultValue: "127.0.0.1");

            migrationBuilder.AlterColumn<ushort>(
                name: "ApiPort",
                table: "ServerConfig",
                type: "smallint unsigned",
                maxLength: 6,
                nullable: false,
                oldClrType: typeof(ushort),
                oldMaxLength: 6,
                oldDefaultValue: (ushort)8000);

            migrationBuilder.AlterColumn<string>(
                name: "ApiIP",
                table: "ServerConfig",
                type: "varchar(20) CHARACTER SET utf8mb4",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 20,
                oldNullable: true,
                oldDefaultValue: "127.0.0.1");
        }
    }
}
