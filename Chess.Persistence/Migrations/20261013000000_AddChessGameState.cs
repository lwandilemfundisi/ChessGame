using Chess.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Chess.Persistence.Migrations
{
    [DbContext(typeof(ChessContext))]
    [Migration("20261013000000_AddChessGameState")]
    public partial class AddChessGameState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(name: "WhiteToMove", table: "Board", type: "bit", nullable: false, defaultValue: true);
            migrationBuilder.AddColumn<long>(name: "EnPassantTargetX", table: "Board", type: "bigint", nullable: false, defaultValue: 0L);
            migrationBuilder.AddColumn<long>(name: "EnPassantTargetY", table: "Board", type: "bigint", nullable: false, defaultValue: 0L);
            migrationBuilder.AddColumn<string>(name: "GameStatus", table: "Board", type: "nvarchar(max)", nullable: false, defaultValue: "InProgress");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "WhiteToMove", table: "Board");
            migrationBuilder.DropColumn(name: "EnPassantTargetX", table: "Board");
            migrationBuilder.DropColumn(name: "EnPassantTargetY", table: "Board");
            migrationBuilder.DropColumn(name: "GameStatus", table: "Board");
        }

        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.8")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Chess.Domain.DomianModel.ChessModel.Board", b =>
            {
                b.Property<string>("Id").HasColumnType("nvarchar(450)");
                b.Property<long>("EnPassantTargetX").HasColumnType("bigint");
                b.Property<long>("EnPassantTargetY").HasColumnType("bigint");
                b.Property<string>("GameStatus").IsRequired().HasColumnType("nvarchar(max)");
                b.Property<bool>("WhiteToMove").HasColumnType("bit");
                b.Property<int>("Version").HasColumnType("int");
                b.HasKey("Id");
                b.ToTable("Board");
            });

            modelBuilder.Entity("Chess.Domain.DomianModel.ChessModel.Entities.Block", b =>
            {
                b.Property<string>("Id").HasColumnType("nvarchar(450)");
                b.Property<string>("BlockColor").HasColumnType("nvarchar(max)");
                b.Property<string>("BoardId").HasColumnType("nvarchar(450)");
                b.Property<string>("ChessPieceId").HasColumnType("nvarchar(450)");
                b.Property<long>("XCoordinate").HasColumnType("bigint");
                b.Property<long>("YCoordinate").HasColumnType("bigint");
                b.HasKey("Id");
                b.HasIndex("BoardId");
                b.HasIndex("ChessPieceId");
                b.ToTable("Block");
            });

            modelBuilder.Entity("Chess.Domain.DomianModel.ChessModel.Entities.ChessPiece", b =>
            {
                b.Property<string>("Id").HasColumnType("nvarchar(450)");
                b.Property<string>("BlockId").HasColumnType("nvarchar(450)");
                b.Property<bool>("HasMovedSinceStart").HasColumnType("bit");
                b.Property<string>("PieceColor").HasColumnType("nvarchar(max)");
                b.Property<string>("PieceName").HasColumnType("nvarchar(max)");
                b.Property<long>("XCoordinate").HasColumnType("bigint");
                b.Property<long>("YCoordinate").HasColumnType("bigint");
                b.HasKey("Id");
                b.HasIndex("BlockId");
                b.ToTable("ChessPiece");
            });

            modelBuilder.Entity("Chess.Domain.DomianModel.ChessModel.Entities.Block", b =>
            {
                b.HasOne("Chess.Domain.DomianModel.ChessModel.Board", null)
                    .WithMany("Blocks").HasForeignKey("BoardId");
                b.HasOne("Chess.Domain.DomianModel.ChessModel.Entities.ChessPiece", "ChessPiece")
                    .WithMany().HasForeignKey("ChessPieceId");
                b.Navigation("ChessPiece");
            });

            modelBuilder.Entity("Chess.Domain.DomianModel.ChessModel.Entities.ChessPiece", b =>
                b.HasOne("Chess.Domain.DomianModel.ChessModel.Entities.Block", null).WithMany().HasForeignKey("BlockId"));

            modelBuilder.Entity("Chess.Domain.DomianModel.ChessModel.Board", b => b.Navigation("Blocks"));
#pragma warning restore 612, 618
        }
    }
}
