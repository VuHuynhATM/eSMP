﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using eSMP.Models;

#nullable disable

namespace eSMP.Migrations
{
    [DbContext(typeof(WebContext))]
    [Migration("20220921073801_v1")]
    partial class v1
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("eSMP.Models.Address", b =>
                {
                    b.Property<int>("AddressID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AddressID"), 1L, 1);

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("context")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("latitude")
                        .HasColumnType("float");

                    b.Property<double>("longitude")
                        .HasColumnType("float");

                    b.HasKey("AddressID");

                    b.ToTable("Address");
                });

            modelBuilder.Entity("eSMP.Models.Category", b =>
                {
                    b.Property<int>("CategoryID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CategoryID"), 1L, 1);

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CategoryID");

                    b.ToTable("Category");
                });

            modelBuilder.Entity("eSMP.Models.Image", b =>
                {
                    b.Property<int>("ImageID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ImageID"), 1L, 1);

                    b.Property<DateTime>("Crete_date")
                        .HasColumnType("datetime2");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ImageID");

                    b.ToTable("Image");
                });

            modelBuilder.Entity("eSMP.Models.Role", b =>
                {
                    b.Property<int>("RoleID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RoleID"), 1L, 1);

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("RoleID");

                    b.ToTable("Role");
                });

            modelBuilder.Entity("eSMP.Models.Store", b =>
                {
                    b.Property<int>("StoreID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("StoreID"), 1L, 1);

                    b.Property<DateTime>("Create_date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StoreName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Store_StatusID")
                        .HasColumnType("int");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.HasKey("StoreID");

                    b.HasIndex("Store_StatusID");

                    b.HasIndex("UserID");

                    b.ToTable("Store");
                });

            modelBuilder.Entity("eSMP.Models.Store_Address", b =>
                {
                    b.Property<int>("Store_AddressID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Store_AddressID"), 1L, 1);

                    b.Property<int>("AddressID")
                        .HasColumnType("int");

                    b.Property<int>("StoreID")
                        .HasColumnType("int");

                    b.HasKey("Store_AddressID");

                    b.HasIndex("AddressID");

                    b.HasIndex("StoreID");

                    b.ToTable("Store_Address");
                });

            modelBuilder.Entity("eSMP.Models.Store_Img", b =>
                {
                    b.Property<int>("Store_ImgID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Store_ImgID"), 1L, 1);

                    b.Property<int>("ImageID")
                        .HasColumnType("int");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<int>("StoreID")
                        .HasColumnType("int");

                    b.HasKey("Store_ImgID");

                    b.HasIndex("ImageID");

                    b.HasIndex("StoreID", "ImageID")
                        .IsUnique();

                    b.ToTable("Store_img");
                });

            modelBuilder.Entity("eSMP.Models.Store_Status", b =>
                {
                    b.Property<int>("Store_StatusID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Store_StatusID"), 1L, 1);

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<int>("StatusName")
                        .HasColumnType("int");

                    b.HasKey("Store_StatusID");

                    b.ToTable("Store_Status");
                });

            modelBuilder.Entity("eSMP.Models.Sub_Category", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"), 1L, 1);

                    b.Property<int>("CategoryID")
                        .HasColumnType("int");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.HasKey("ID");

                    b.HasIndex("CategoryID");

                    b.ToTable("Sub_Category");
                });

            modelBuilder.Entity("eSMP.Models.User", b =>
                {
                    b.Property<int>("UserID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserID"), 1L, 1);

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ImageID")
                        .HasColumnType("int");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RoleID")
                        .HasColumnType("int");

                    b.Property<int>("StatusID")
                        .HasColumnType("int");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserID");

                    b.HasIndex("ImageID");

                    b.HasIndex("RoleID");

                    b.HasIndex("StatusID");

                    b.ToTable("User");
                });

            modelBuilder.Entity("eSMP.Models.User_Address", b =>
                {
                    b.Property<int>("User_AddressID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("User_AddressID"), 1L, 1);

                    b.Property<int>("AddressID")
                        .HasColumnType("int");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.HasKey("User_AddressID");

                    b.HasIndex("UserID");

                    b.HasIndex("AddressID", "UserID")
                        .IsUnique();

                    b.ToTable("User_Address");
                });

            modelBuilder.Entity("eSMP.Models.User_Status", b =>
                {
                    b.Property<int>("StatusID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("StatusID"), 1L, 1);

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("StatusName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("StatusID");

                    b.ToTable("User_Status");
                });

            modelBuilder.Entity("eSMP.Models.Store", b =>
                {
                    b.HasOne("eSMP.Models.Store_Status", "Store_Status")
                        .WithMany()
                        .HasForeignKey("Store_StatusID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("eSMP.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Store_Status");

                    b.Navigation("User");
                });

            modelBuilder.Entity("eSMP.Models.Store_Address", b =>
                {
                    b.HasOne("eSMP.Models.Address", "Address")
                        .WithMany()
                        .HasForeignKey("AddressID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("eSMP.Models.Store", "Store")
                        .WithMany()
                        .HasForeignKey("StoreID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Address");

                    b.Navigation("Store");
                });

            modelBuilder.Entity("eSMP.Models.Store_Img", b =>
                {
                    b.HasOne("eSMP.Models.Image", "Image")
                        .WithMany()
                        .HasForeignKey("ImageID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("eSMP.Models.Store", "Store")
                        .WithMany()
                        .HasForeignKey("StoreID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Image");

                    b.Navigation("Store");
                });

            modelBuilder.Entity("eSMP.Models.Sub_Category", b =>
                {
                    b.HasOne("eSMP.Models.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("eSMP.Models.User", b =>
                {
                    b.HasOne("eSMP.Models.Image", "Image")
                        .WithMany()
                        .HasForeignKey("ImageID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("eSMP.Models.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("eSMP.Models.User_Status", "status")
                        .WithMany()
                        .HasForeignKey("StatusID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Image");

                    b.Navigation("Role");

                    b.Navigation("status");
                });

            modelBuilder.Entity("eSMP.Models.User_Address", b =>
                {
                    b.HasOne("eSMP.Models.Address", "Address")
                        .WithMany()
                        .HasForeignKey("AddressID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("eSMP.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Address");

                    b.Navigation("User");
                });
#pragma warning restore 612, 618
        }
    }
}