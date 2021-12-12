using Microsoft.EntityFrameworkCore.Migrations;

namespace Chess.Persistence.Migrations
{
    public partial class AddProperty_HasMovedSinceStart_ChessPiece : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasMovedSinceStart",
                table: "ChessPiece",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasMovedSinceStart",
                table: "ChessPiece");
        }
    }
}
