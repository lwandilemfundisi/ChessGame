using Chess.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Chess.Persistence.Migrations
{
    [DbContext(typeof(ChessContext))]
    [Migration("20261013010000_AddDrawTracking")]
    public partial class AddDrawTracking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(name: "HalfMoveClock", table: "Board", type: "int", nullable: false, defaultValue: 0);
            migrationBuilder.AddColumn<string>(name: "PositionHistory", table: "Board", type: "nvarchar(max)", nullable: false, defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "HalfMoveClock", table: "Board");
            migrationBuilder.DropColumn(name: "PositionHistory", table: "Board");
        }
    }
}
