﻿// <auto-generated />
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
    [Migration("20241025213551_AddNameToFieldsTable")]
    partial class AddNameToFieldsTable
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

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Type")
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

            modelBuilder.Entity("CentaureaTest.Models.MultipleChoiceTable", b =>
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

                    b.ToTable("multiple_choice");
                });

            modelBuilder.Entity("CentaureaTest.Models.SingleChoiceTable", b =>
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

                    b.ToTable("single_choice");
                });

            modelBuilder.Entity("CentaureaTest.Models.ValuesTable", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("FieldId")
                        .HasColumnType("integer");

                    b.Property<int>("GridId")
                        .HasColumnType("integer");

                    b.Property<int>("RowId")
                        .HasColumnType("integer");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("jsonb")
                        .HasColumnName("Value");

                    b.HasKey("Id");

                    b.ToTable("values");

                    b.HasDiscriminator<string>("Discriminator").HasValue("ValuesTable");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("CentaureaTest.Models.NumericValuesTable", b =>
                {
                    b.HasBaseType("CentaureaTest.Models.ValuesTable");

                    b.Property<decimal>("NumericValue")
                        .HasColumnType("numeric");

                    b.ToTable("values");

                    b.HasDiscriminator().HasValue("Numeric");
                });

            modelBuilder.Entity("CentaureaTest.Models.RegexValuesTable", b =>
                {
                    b.HasBaseType("CentaureaTest.Models.ValuesTable");

                    b.Property<string>("RegexValue")
                        .IsRequired()
                        .HasColumnType("text");

                    b.ToTable("values");

                    b.HasDiscriminator().HasValue("Regex");
                });

            modelBuilder.Entity("CentaureaTest.Models.StringValuesTable", b =>
                {
                    b.HasBaseType("CentaureaTest.Models.ValuesTable");

                    b.Property<string>("StringValue")
                        .IsRequired()
                        .HasColumnType("text");

                    b.ToTable("values");

                    b.HasDiscriminator().HasValue("String");
                });
#pragma warning restore 612, 618
        }
    }
}
