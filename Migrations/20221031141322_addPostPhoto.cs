using Microsoft.EntityFrameworkCore.Migrations;

namespace MVC_01.Migrations
{
    public partial class addPostPhoto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductPhoto_Product_ProductId",
                table: "ProductPhoto");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "ProductPhoto",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "PostPhotos",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostPhotos", x => x.id);
                    table.ForeignKey(
                        name: "FK_PostPhotos_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "PostId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PostPhotos_PostId",
                table: "PostPhotos",
                column: "PostId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductPhoto_Product_ProductId",
                table: "ProductPhoto",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductPhoto_Product_ProductId",
                table: "ProductPhoto");

            migrationBuilder.DropTable(
                name: "PostPhotos");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "ProductPhoto",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductPhoto_Product_ProductId",
                table: "ProductPhoto",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
