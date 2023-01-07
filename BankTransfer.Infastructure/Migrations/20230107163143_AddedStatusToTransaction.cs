using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankTransfer.Infastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedStatusToTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TransferStatus",
                table: "Transaction",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransferStatus",
                table: "Transaction");
        }
    }
}
