﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using powerLabel;

namespace powerLabel.Migrations
{
    [DbContext(typeof(ComputerSystemContext))]
    [Migration("20220613080041_AddEvents")]
    partial class AddEvents
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.25")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("powerLabel.Bios", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("date")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("version")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("id");

                    b.ToTable("Bioses");
                });

            modelBuilder.Entity("powerLabel.ComputerSystem", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int?>("biosid")
                        .HasColumnType("int");

                    b.Property<int?>("motherboardid")
                        .HasColumnType("int");

                    b.Property<int?>("operatingSystemid")
                        .HasColumnType("int");

                    b.Property<int>("processorAmount")
                        .HasColumnType("int");

                    b.Property<int?>("processorid")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.HasIndex("biosid");

                    b.HasIndex("motherboardid");

                    b.HasIndex("operatingSystemid");

                    b.HasIndex("processorid");

                    b.ToTable("ComputerSystems");
                });

            modelBuilder.Entity("powerLabel.Disk", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("mediaType")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("model")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("serialNumber")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<ulong>("size")
                        .HasColumnType("bigint unsigned");

                    b.HasKey("id");

                    b.ToTable("Disks");
                });

            modelBuilder.Entity("powerLabel.DiskConfig", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("busType")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int?>("computerSystemid")
                        .HasColumnType("int");

                    b.Property<int?>("diskid")
                        .HasColumnType("int");

                    b.Property<bool>("systemDisk")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("id");

                    b.HasIndex("computerSystemid");

                    b.HasIndex("diskid");

                    b.ToTable("DiskConfigs");
                });

            modelBuilder.Entity("powerLabel.Event", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int?>("computerSystemid")
                        .HasColumnType("int");

                    b.Property<DateTime>("date")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("description")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("employee")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("note")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("id");

                    b.HasIndex("computerSystemid");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("powerLabel.MemoryConfig", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<uint>("currentClockspeed")
                        .HasColumnType("int unsigned");

                    b.Property<int?>("moduleid")
                        .HasColumnType("int");

                    b.Property<int?>("systemid")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.HasIndex("moduleid");

                    b.HasIndex("systemid");

                    b.ToTable("MemoryConfigs");
                });

            modelBuilder.Entity("powerLabel.MemoryModule", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<ulong>("capacity")
                        .HasColumnType("bigint unsigned");

                    b.Property<uint>("formFactor")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("maxClockspeed")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("memoryType")
                        .HasColumnType("int unsigned");

                    b.Property<string>("partNubmer")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("serialNubmer")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("id");

                    b.ToTable("MemoryModules");
                });

            modelBuilder.Entity("powerLabel.Motherboard", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("manufacturer")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("model")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("serialNumber")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("id");

                    b.ToTable("Motherboards");
                });

            modelBuilder.Entity("powerLabel.OS", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("caption")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("language")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("id");

                    b.ToTable("OSes");
                });

            modelBuilder.Entity("powerLabel.Processor", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<uint>("cores")
                        .HasColumnType("int unsigned");

                    b.Property<string>("name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<uint>("threads")
                        .HasColumnType("int unsigned");

                    b.HasKey("id");

                    b.ToTable("Processors");
                });

            modelBuilder.Entity("powerLabel.VideoController", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("manufacturer")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<uint>("vram")
                        .HasColumnType("int unsigned");

                    b.HasKey("id");

                    b.ToTable("VideoControllers");
                });

            modelBuilder.Entity("powerLabel.VideoControllerConfig", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int?>("computerSystemid")
                        .HasColumnType("int");

                    b.Property<int?>("videoControllerid")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.HasIndex("computerSystemid");

                    b.HasIndex("videoControllerid");

                    b.ToTable("VideoControllerConfigs");
                });

            modelBuilder.Entity("powerLabel.ComputerSystem", b =>
                {
                    b.HasOne("powerLabel.Bios", "bios")
                        .WithMany()
                        .HasForeignKey("biosid");

                    b.HasOne("powerLabel.Motherboard", "motherboard")
                        .WithMany()
                        .HasForeignKey("motherboardid");

                    b.HasOne("powerLabel.OS", "operatingSystem")
                        .WithMany()
                        .HasForeignKey("operatingSystemid");

                    b.HasOne("powerLabel.Processor", "processor")
                        .WithMany()
                        .HasForeignKey("processorid");
                });

            modelBuilder.Entity("powerLabel.DiskConfig", b =>
                {
                    b.HasOne("powerLabel.ComputerSystem", "computerSystem")
                        .WithMany("diskConfigs")
                        .HasForeignKey("computerSystemid");

                    b.HasOne("powerLabel.Disk", "disk")
                        .WithMany()
                        .HasForeignKey("diskid");
                });

            modelBuilder.Entity("powerLabel.Event", b =>
                {
                    b.HasOne("powerLabel.ComputerSystem", "computerSystem")
                        .WithMany()
                        .HasForeignKey("computerSystemid");
                });

            modelBuilder.Entity("powerLabel.MemoryConfig", b =>
                {
                    b.HasOne("powerLabel.MemoryModule", "module")
                        .WithMany()
                        .HasForeignKey("moduleid");

                    b.HasOne("powerLabel.ComputerSystem", "system")
                        .WithMany("memoryModules")
                        .HasForeignKey("systemid");
                });

            modelBuilder.Entity("powerLabel.VideoControllerConfig", b =>
                {
                    b.HasOne("powerLabel.ComputerSystem", "computerSystem")
                        .WithMany("videoControllerConfigs")
                        .HasForeignKey("computerSystemid");

                    b.HasOne("powerLabel.VideoController", "videoController")
                        .WithMany()
                        .HasForeignKey("videoControllerid");
                });
#pragma warning restore 612, 618
        }
    }
}
