using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace powerLabel.Migrations
{
    public partial class Createpowerlabeldb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bioses",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    version = table.Column<string>(nullable: true),
                    date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bioses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Disks",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    model = table.Column<string>(nullable: true),
                    size = table.Column<ulong>(nullable: false),
                    mediaType = table.Column<string>(nullable: true),
                    serialNumber = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Disks", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "MemoryModules",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    capacity = table.Column<ulong>(nullable: false),
                    maxClockspeed = table.Column<uint>(nullable: false),
                    formFactor = table.Column<uint>(nullable: false),
                    memoryType = table.Column<uint>(nullable: false),
                    partNubmer = table.Column<string>(nullable: true),
                    serialNubmer = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemoryModules", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Motherboards",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    model = table.Column<string>(nullable: true),
                    manufacturer = table.Column<string>(nullable: true),
                    serialNumber = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Motherboards", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "OSes",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    caption = table.Column<string>(nullable: true),
                    language = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OSes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Processors",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(nullable: true),
                    cores = table.Column<uint>(nullable: false),
                    threads = table.Column<uint>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Processors", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "VideoControllers",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    manufacturer = table.Column<string>(nullable: true),
                    name = table.Column<string>(nullable: true),
                    vram = table.Column<uint>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoControllers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ComputerSystems",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    motherboardid = table.Column<int>(nullable: true),
                    biosid = table.Column<int>(nullable: true),
                    processorAmount = table.Column<int>(nullable: false),
                    processorid = table.Column<int>(nullable: true),
                    operatingSystemid = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComputerSystems", x => x.id);
                    table.ForeignKey(
                        name: "FK_ComputerSystems_Bioses_biosid",
                        column: x => x.biosid,
                        principalTable: "Bioses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComputerSystems_Motherboards_motherboardid",
                        column: x => x.motherboardid,
                        principalTable: "Motherboards",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComputerSystems_OSes_operatingSystemid",
                        column: x => x.operatingSystemid,
                        principalTable: "OSes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComputerSystems_Processors_processorid",
                        column: x => x.processorid,
                        principalTable: "Processors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DiskConfigs",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    diskid = table.Column<int>(nullable: true),
                    computerSystemid = table.Column<int>(nullable: true),
                    systemDisk = table.Column<bool>(nullable: false),
                    busType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiskConfigs", x => x.id);
                    table.ForeignKey(
                        name: "FK_DiskConfigs_ComputerSystems_computerSystemid",
                        column: x => x.computerSystemid,
                        principalTable: "ComputerSystems",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DiskConfigs_Disks_diskid",
                        column: x => x.diskid,
                        principalTable: "Disks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MemoryConfigs",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    moduleid = table.Column<int>(nullable: true),
                    systemid = table.Column<int>(nullable: true),
                    currentClockspeed = table.Column<uint>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemoryConfigs", x => x.id);
                    table.ForeignKey(
                        name: "FK_MemoryConfigs_MemoryModules_moduleid",
                        column: x => x.moduleid,
                        principalTable: "MemoryModules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MemoryConfigs_ComputerSystems_systemid",
                        column: x => x.systemid,
                        principalTable: "ComputerSystems",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VideoControllerConfigs",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    videoControllerid = table.Column<int>(nullable: true),
                    computerSystemid = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoControllerConfigs", x => x.id);
                    table.ForeignKey(
                        name: "FK_VideoControllerConfigs_ComputerSystems_computerSystemid",
                        column: x => x.computerSystemid,
                        principalTable: "ComputerSystems",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VideoControllerConfigs_VideoControllers_videoControllerid",
                        column: x => x.videoControllerid,
                        principalTable: "VideoControllers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComputerSystems_biosid",
                table: "ComputerSystems",
                column: "biosid");

            migrationBuilder.CreateIndex(
                name: "IX_ComputerSystems_motherboardid",
                table: "ComputerSystems",
                column: "motherboardid");

            migrationBuilder.CreateIndex(
                name: "IX_ComputerSystems_operatingSystemid",
                table: "ComputerSystems",
                column: "operatingSystemid");

            migrationBuilder.CreateIndex(
                name: "IX_ComputerSystems_processorid",
                table: "ComputerSystems",
                column: "processorid");

            migrationBuilder.CreateIndex(
                name: "IX_DiskConfigs_computerSystemid",
                table: "DiskConfigs",
                column: "computerSystemid");

            migrationBuilder.CreateIndex(
                name: "IX_DiskConfigs_diskid",
                table: "DiskConfigs",
                column: "diskid");

            migrationBuilder.CreateIndex(
                name: "IX_MemoryConfigs_moduleid",
                table: "MemoryConfigs",
                column: "moduleid");

            migrationBuilder.CreateIndex(
                name: "IX_MemoryConfigs_systemid",
                table: "MemoryConfigs",
                column: "systemid");

            migrationBuilder.CreateIndex(
                name: "IX_VideoControllerConfigs_computerSystemid",
                table: "VideoControllerConfigs",
                column: "computerSystemid");

            migrationBuilder.CreateIndex(
                name: "IX_VideoControllerConfigs_videoControllerid",
                table: "VideoControllerConfigs",
                column: "videoControllerid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiskConfigs");

            migrationBuilder.DropTable(
                name: "MemoryConfigs");

            migrationBuilder.DropTable(
                name: "VideoControllerConfigs");

            migrationBuilder.DropTable(
                name: "Disks");

            migrationBuilder.DropTable(
                name: "MemoryModules");

            migrationBuilder.DropTable(
                name: "ComputerSystems");

            migrationBuilder.DropTable(
                name: "VideoControllers");

            migrationBuilder.DropTable(
                name: "Bioses");

            migrationBuilder.DropTable(
                name: "Motherboards");

            migrationBuilder.DropTable(
                name: "OSes");

            migrationBuilder.DropTable(
                name: "Processors");
        }
    }
}
