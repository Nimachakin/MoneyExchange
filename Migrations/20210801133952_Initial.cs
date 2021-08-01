using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ExchangeApi.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExchangeLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserInput = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExchangeResult = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BeforeExchangeData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AfterExchangeData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExchangeDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeLogs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExchangeLogs");
        }
    }
}
