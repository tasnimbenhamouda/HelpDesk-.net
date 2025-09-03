using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HD.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModificationEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProcesseddDate",
                table: "Complaints",
                newName: "ProcessedDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProcessedDate",
                table: "Complaints",
                newName: "ProcesseddDate");
        }
    }
}
