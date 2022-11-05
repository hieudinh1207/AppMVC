using Microsoft.EntityFrameworkCore.Migrations;

namespace MVC_01.Migrations
{
    public partial class Productphoto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "PostCategory",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProductPhoto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductPhoto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductPhoto_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PostCategory_ProductId",
                table: "PostCategory",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPhoto_ProductId",
                table: "ProductPhoto",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_PostCategory_Product_ProductId",
                table: "PostCategory",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostCategory_Product_ProductId",
                table: "PostCategory");

            migrationBuilder.DropTable(
                name: "ProductPhoto");

            migrationBuilder.DropIndex(
                name: "IX_PostCategory_ProductId",
                table: "PostCategory");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "PostCategory");
        }
    }
}
