﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ToDo.Models;

#nullable disable

namespace ToDo.Migrations
{
    [DbContext(typeof(ToDoDbContext))]
    [Migration("20231007161939_initial-migration")]
    partial class initialmigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseCollation("utf8mb4_general_ci")
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.HasCharSet(modelBuilder, "utf8mb4");

            modelBuilder.Entity("ToDo.Models.Activity", b =>
                {
                    b.Property<uint>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int(10) unsigned");

                    b.Property<string>("Name")
                        .HasMaxLength(444)
                        .HasColumnType("varchar(444)")
                        .UseCollation("utf8mb4_thai_520_w2");

                    b.Property<uint?>("UserId")
                        .HasColumnType("int(10) unsigned");

                    b.Property<DateTime?>("When")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("current_timestamp()");

                    b.HasKey("Id")
                        .HasName("PRIMARY");

                    b.HasIndex(new[] { "UserId" }, "UserId");

                    b.ToTable("Activity");
                });

            modelBuilder.Entity("ToDo.Models.User", b =>
                {
                    b.Property<uint>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int(10) unsigned");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(44)
                        .HasColumnType("varchar(44)")
                        .UseCollation("utf8mb4_thai_520_w2");

                    b.Property<string>("Salt")
                        .IsRequired()
                        .HasMaxLength(24)
                        .HasColumnType("varchar(24)")
                        .UseCollation("utf8mb4_thai_520_w2");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(44)
                        .HasColumnType("varchar(44)")
                        .UseCollation("utf8mb4_thai_520_w2");

                    b.HasKey("Id")
                        .HasName("PRIMARY");

                    b.ToTable("User");
                });

            modelBuilder.Entity("ToDo.Models.Activity", b =>
                {
                    b.HasOne("ToDo.Models.User", "User")
                        .WithMany("Activity")
                        .HasForeignKey("UserId")
                        .HasConstraintName("Activity_ibfk_1");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ToDo.Models.User", b =>
                {
                    b.Navigation("Activity");
                });
#pragma warning restore 612, 618
        }
    }
}
