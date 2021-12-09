using Microsoft.EntityFrameworkCore.Migrations;

namespace Chess.Persistence.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Board",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Board", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChessPiece",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PieceName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PieceColor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    XCoordinate = table.Column<long>(type: "bigint", nullable: false),
                    YCoordinate = table.Column<long>(type: "bigint", nullable: false),
                    BlockId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChessPiece", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Block",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ChessPieceId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    BlockColor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    XCoordinate = table.Column<long>(type: "bigint", nullable: false),
                    YCoordinate = table.Column<long>(type: "bigint", nullable: false),
                    BoardId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Block", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Block_Board_BoardId",
                        column: x => x.BoardId,
                        principalTable: "Board",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Block_ChessPiece_ChessPieceId",
                        column: x => x.ChessPieceId,
                        principalTable: "ChessPiece",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Block_BoardId",
                table: "Block",
                column: "BoardId");

            migrationBuilder.CreateIndex(
                name: "IX_Block_ChessPieceId",
                table: "Block",
                column: "ChessPieceId");

            migrationBuilder.CreateIndex(
                name: "IX_ChessPiece_BlockId",
                table: "ChessPiece",
                column: "BlockId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChessPiece_Block_BlockId",
                table: "ChessPiece",
                column: "BlockId",
                principalTable: "Block",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Block_Board_BoardId",
                table: "Block");

            migrationBuilder.DropForeignKey(
                name: "FK_Block_ChessPiece_ChessPieceId",
                table: "Block");

            migrationBuilder.DropTable(
                name: "Board");

            migrationBuilder.DropTable(
                name: "ChessPiece");

            migrationBuilder.DropTable(
                name: "Block");
        }
    }
}
