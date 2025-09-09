using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HD.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ajoutdeComplaintFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "Complaints");

            migrationBuilder.CreateTable(
                name: "ComplaintFiles",
                columns: table => new
                {
                    ComplaintFileId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ComplaintFK = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComplaintFiles", x => x.ComplaintFileId);
                    table.ForeignKey(
                        name: "FK_ComplaintFiles_Complaints_ComplaintFK",
                        column: x => x.ComplaintFK,
                        principalTable: "Complaints",
                        principalColumn: "ComplaintId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComplaintFiles_ComplaintFK",
                table: "ComplaintFiles",
                column: "ComplaintFK");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComplaintFiles");

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "Complaints",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
