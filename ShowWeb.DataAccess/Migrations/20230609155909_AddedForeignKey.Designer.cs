﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using ShowWeb.DataAccess.Data;

#nullable disable

namespace ShowWeb.DataAccess.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20230609155909_AddedForeignKey")]
    partial class AddedForeignKey
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ShowWeb.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("DisplayOrder")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)");

                    b.HasKey("Id");

                    b.ToTable("Categories");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            DisplayOrder = 1,
                            Name = "Laptops"
                        },
                        new
                        {
                            Id = 2,
                            DisplayOrder = 2,
                            Name = "PCs"
                        },
                        new
                        {
                            Id = 3,
                            DisplayOrder = 3,
                            Name = "Mobiles"
                        });
                });

            modelBuilder.Entity("ShowWeb.Models.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("CategoryId")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<double>("ListPrice")
                        .HasColumnType("double precision");

                    b.Property<string>("Manufacturer")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Model")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<double>("Price")
                        .HasColumnType("double precision");

                    b.Property<double>("Price100")
                        .HasColumnType("double precision");

                    b.Property<double>("Price50")
                        .HasColumnType("double precision");

                    b.Property<string>("SKU")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Products");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            CategoryId = 1,
                            Description = "Dell XPS 13",
                            ListPrice = 100000.0,
                            Manufacturer = "Dell",
                            Model = "XPS 13",
                            Price = 95000.0,
                            Price100 = 85000.0,
                            Price50 = 90000.0,
                            SKU = "SKU 1",
                            Title = "Dell XPS 13"
                        },
                        new
                        {
                            Id = 2,
                            CategoryId = 1,
                            Description = "Dell XPS 15",
                            ListPrice = 120000.0,
                            Manufacturer = "Dell",
                            Model = "XPS 15",
                            Price = 110000.0,
                            Price100 = 95000.0,
                            Price50 = 100000.0,
                            SKU = "SKU 2",
                            Title = "Dell XPS 15"
                        },
                        new
                        {
                            Id = 3,
                            CategoryId = 2,
                            Description = "Dell XPS 17",
                            ListPrice = 150000.0,
                            Manufacturer = "Dell",
                            Model = "XPS 17",
                            Price = 140000.0,
                            Price100 = 130000.0,
                            Price50 = 135000.0,
                            SKU = "SKU 3",
                            Title = "Dell XPS 17"
                        });
                });

            modelBuilder.Entity("ShowWeb.Models.Product", b =>
                {
                    b.HasOne("ShowWeb.Models.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
                });
#pragma warning restore 612, 618
        }
    }
}