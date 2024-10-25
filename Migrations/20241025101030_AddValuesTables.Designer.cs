﻿// <auto-generated />
using System;
using CentaureaTest.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace centaureatest.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20241025101030_AddValuesTables")]
    partial class AddValuesTables
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("CentaureaTest.Models.FieldsTable", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("GridId")
                        .HasColumnType("integer");

                    b.Property<int>("ValueType")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("fields");
                });

            modelBuilder.Entity("CentaureaTest.Models.GridsTable", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("grids");
                });

            modelBuilder.Entity("CentaureaTest.Models.ValuesTable", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string[]>("ArrayValue")
                        .HasColumnType("text[]");

                    b.Property<int>("FieldId")
                        .HasColumnType("integer");

                    b.Property<int>("GridId")
                        .HasColumnType("integer");

                    b.Property<decimal?>("NumericValue")
                        .HasColumnType("numeric");

                    b.Property<int>("RowId")
                        .HasColumnType("integer");

                    b.Property<string>("TextValue")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("values");
                });
#pragma warning restore 612, 618
        }
    }
}
