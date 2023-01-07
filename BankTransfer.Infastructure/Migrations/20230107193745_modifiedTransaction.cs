using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankTransfer.Infastructure.Migrations
{
    /// <inheritdoc />
    public partial class modifiedTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Transaction",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Transaction");
        }
    }
}
