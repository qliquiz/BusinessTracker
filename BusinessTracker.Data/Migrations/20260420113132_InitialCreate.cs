using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BusinessTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Inn = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    LoadOptions = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Organizations_pkey", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Users_pkey", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Categories_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categories_Organizations",
                        column: x => x.OwnerId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Employees_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_Organizations",
                        column: x => x.OwnerId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JournalRows",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<long>(type: "bigint", nullable: false),
                    TypeCode = table.Column<int>(type: "integer", nullable: false),
                    TransTypeName = table.Column<string>(type: "text", nullable: false, defaultValue: ""),
                    ReceiptNumber = table.Column<int>(type: "integer", nullable: false),
                    ProductCode = table.Column<long>(type: "bigint", nullable: true),
                    CategoryCode = table.Column<long>(type: "bigint", nullable: true),
                    EmployeeCode = table.Column<int>(type: "integer", nullable: true),
                    Period = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Quantity = table.Column<double>(type: "double precision", nullable: false),
                    Price = table.Column<double>(type: "double precision", nullable: false),
                    Discount = table.Column<double>(type: "double precision", nullable: false),
                    RawId = table.Column<int>(type: "integer", nullable: false),
                    RawLoginId = table.Column<int>(type: "integer", nullable: false),
                    EmployeeName = table.Column<string>(type: "text", nullable: false, defaultValue: ""),
                    CategoryName = table.Column<string>(type: "text", nullable: false, defaultValue: ""),
                    NomenclatureName = table.Column<string>(type: "text", nullable: false, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("JournalRows_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JournalRows_Organizations",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LinksUserOrganizations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("LinksUserOrganizations_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LinksUserOrganizations_Organizations",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LinksUserOrganizations_Users",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Nomenclatures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Nomenclatures_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Nomenclatures_Categories",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NomenclatureId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(15,2)", precision: 15, scale: 2, nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric(15,2)", precision: 15, scale: 2, nullable: false),
                    Discount = table.Column<decimal>(type: "numeric(15,2)", precision: 15, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Transactions_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Employees",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transactions_Nomenclatures",
                        column: x => x.NomenclatureId,
                        principalTable: "Nomenclatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transactions_Organizations",
                        column: x => x.OwnerId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Organizations",
                columns: new[] { "Id", "Address", "Inn", "LoadOptions", "Name" },
                values: new object[,]
                {
                    { new Guid("a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11"), "190000, Ленинградская обл., Ломоносовский р-н, г. Ломоносов, ул. Советская, д. 12", "1234567890", null, "Главный офис (Спб)" },
                    { new Guid("a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a12"), "230000, Иркутская обл., Октябрьский р-н, г. Иркутск, ул. 3 Июля, д. 3", "0987654321", null, "Филиал (Иркутск)" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Name", "Password" },
                values: new object[] { new Guid("b0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11"), "Администратор", "admin123" });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name", "OwnerId" },
                values: new object[] { new Guid("c1eebc99-9c0b-4ef8-bb6d-6bb9bd380a11"), "Продукты питания", new Guid("a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11") });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "Name", "OwnerId", "PhoneNumber", "Role" },
                values: new object[] { new Guid("e1eebc99-9c0b-4ef8-bb6d-6bb9bd380a11"), "Иван Иванов", new Guid("a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11"), "+79991234567", 1 });

            migrationBuilder.InsertData(
                table: "Nomenclatures",
                columns: new[] { "Id", "CategoryId", "Name" },
                values: new object[] { new Guid("d1eebc99-9c0b-4ef8-bb6d-6bb9bd380a11"), new Guid("c1eebc99-9c0b-4ef8-bb6d-6bb9bd380a11"), "Хлеб пшеничный" });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_OwnerId",
                table: "Categories",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_OwnerId",
                table: "Employees",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalRows_OrganizationId",
                table: "JournalRows",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_LinksUserOrganizations_OrganizationId",
                table: "LinksUserOrganizations",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_LinksUserOrganizations_UserId",
                table: "LinksUserOrganizations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Nomenclatures_CategoryId",
                table: "Nomenclatures",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "OrganizationInnIx",
                table: "Organizations",
                column: "Inn",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_EmployeeId",
                table: "Transactions",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_NomenclatureId",
                table: "Transactions",
                column: "NomenclatureId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_OwnerId",
                table: "Transactions",
                column: "OwnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JournalRows");

            migrationBuilder.DropTable(
                name: "LinksUserOrganizations");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Nomenclatures");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Organizations");
        }
    }
}
