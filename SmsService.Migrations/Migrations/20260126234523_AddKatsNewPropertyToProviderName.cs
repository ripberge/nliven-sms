using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmsService.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddKatsNewPropertyToProviderName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "KatsNewProperty",
                table: "ProviderNames",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KatsNewProperty",
                table: "ProviderNames");
        }
    }
}
