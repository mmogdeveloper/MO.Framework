using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MO.Model.Migrations.MOData
{
    public partial class InitMODataV7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServerConfig",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ServerLevel = table.Column<int>(maxLength: 2, nullable: false),
                    LoginIP = table.Column<string>(maxLength: 20, nullable: true),
                    LoginPort = table.Column<ushort>(maxLength: 6, nullable: false),
                    ApiIP = table.Column<string>(maxLength: 20, nullable: true),
                    ApiPort = table.Column<ushort>(maxLength: 6, nullable: false),
                    GateIP = table.Column<string>(maxLength: 20, nullable: true),
                    GatePort = table.Column<ushort>(maxLength: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerConfig", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServerConfig");
        }
    }
}
