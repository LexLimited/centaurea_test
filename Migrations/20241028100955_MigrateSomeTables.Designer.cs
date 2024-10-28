﻿// <auto-generated />
using System;
using System.Collections.Generic;
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
    [Migration("20241028100955_MigrateSomeTables")]
    partial class MigrateSomeTables
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("CentaureaTest.Models.DataGridValue", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("FieldId")
                        .HasColumnType("integer");

                    b.Property<int>("RowIndex")
                        .HasColumnType("integer");

                    b.Property<string>("ValueType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("values");

                    b.HasDiscriminator<string>("ValueType").HasValue("Base");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("CentaureaTest.Models.FieldsTable", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("FieldType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("GridId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Order")
                        .HasColumnType("integer");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("fields");

                    b.HasDiscriminator<string>("FieldType").HasValue("Base");

                    b.UseTphMappingStrategy();
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

            modelBuilder.Entity("CentaureaTest.Models.MultiSelectTable", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("FieldId")
                        .HasColumnType("integer");

                    b.Property<string>("Option")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("TableId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("multi_select");
                });

            modelBuilder.Entity("CentaureaTest.Models.SingleSelectTable", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("FieldId")
                        .HasColumnType("integer");

                    b.Property<string>("Option")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("single_select");
                });

            modelBuilder.Entity("CentaureaTest.Models.DataGridMultiSelectValue", b =>
                {
                    b.HasBaseType("CentaureaTest.Models.DataGridValue");

                    b.Property<List<int>>("OptionIds")
                        .IsRequired()
                        .HasColumnType("integer[]");

                    b.ToTable("values");

                    b.HasDiscriminator().HasValue("MultiSelect");
                });

            modelBuilder.Entity("CentaureaTest.Models.DataGridNumericValue", b =>
                {
                    b.HasBaseType("CentaureaTest.Models.DataGridValue");

                    b.Property<decimal>("Value")
                        .HasColumnType("numeric");

                    b.ToTable("values", t =>
                        {
                            t.Property("Value")
                                .HasColumnName("DataGridNumericValue_Value");
                        });

                    b.HasDiscriminator().HasValue("Numeric");
                });

            modelBuilder.Entity("CentaureaTest.Models.DataGridRefValue", b =>
                {
                    b.HasBaseType("CentaureaTest.Models.DataGridValue");

                    b.Property<int>("ReferencedFieldId")
                        .HasColumnType("integer");

                    b.ToTable("values");

                    b.HasDiscriminator().HasValue("Ref");
                });

            modelBuilder.Entity("CentaureaTest.Models.DataGridRegexValue", b =>
                {
                    b.HasBaseType("CentaureaTest.Models.DataGridValue");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("text");

                    b.ToTable("values", t =>
                        {
                            t.Property("Value")
                                .HasColumnName("DataGridRegexValue_Value");
                        });

                    b.HasDiscriminator().HasValue("Regex");
                });

            modelBuilder.Entity("CentaureaTest.Models.DataGridSingleSelectValue", b =>
                {
                    b.HasBaseType("CentaureaTest.Models.DataGridValue");

                    b.Property<int>("OptionId")
                        .HasColumnType("integer");

                    b.ToTable("values");

                    b.HasDiscriminator().HasValue("SingleSelect");
                });

            modelBuilder.Entity("CentaureaTest.Models.DataGridStringValue", b =>
                {
                    b.HasBaseType("CentaureaTest.Models.DataGridValue");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("text");

                    b.ToTable("values");

                    b.HasDiscriminator().HasValue("String");
                });

            modelBuilder.Entity("CentaureaTest.Models.RefFieldsTable", b =>
                {
                    b.HasBaseType("CentaureaTest.Models.FieldsTable");

                    b.Property<int?>("ReferencedGridId")
                        .HasColumnType("integer");

                    b.ToTable("fields");

                    b.HasDiscriminator().HasValue("Ref");
                });

            modelBuilder.Entity("CentaureaTest.Models.RegexFieldsTable", b =>
                {
                    b.HasBaseType("CentaureaTest.Models.FieldsTable");

                    b.Property<string>("RegexPattern")
                        .HasColumnType("text");

                    b.ToTable("fields");

                    b.HasDiscriminator().HasValue("Regex");
                });
#pragma warning restore 612, 618
        }
    }
}
