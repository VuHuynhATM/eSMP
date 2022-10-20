﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using eSMP.Models;

#nullable disable

namespace eSMP.Migrations
{
    [DbContext(typeof(WebContext))]
    partial class WebContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
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

                    b.Property<string>("Context")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("District")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<double>("Latitude")
                        .HasColumnType("float");

                    b.Property<double>("Longitude")
                        .HasColumnType("float");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Province")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Ward")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("AddressID");

                    b.ToTable("Address");
                });

            modelBuilder.Entity("eSMP.Models.Brand", b =>
                {
                    b.Property<int>("BrandID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("BrandID"), 1L, 1);

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("BrandID");

                    b.ToTable("Brand");
                });

            modelBuilder.Entity("eSMP.Models.Brand_Model", b =>
                {
                    b.Property<int>("Brand_ModelID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Brand_ModelID"), 1L, 1);

                    b.Property<int>("BrandID")
                        .HasColumnType("int");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Brand_ModelID");

                    b.HasIndex("BrandID");

                    b.ToTable("Brand_Model");
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

            modelBuilder.Entity("eSMP.Models.Feedback_Image", b =>
                {
                    b.Property<int>("Feedback_ImageID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Feedback_ImageID"), 1L, 1);

                    b.Property<int>("ImageID")
                        .HasColumnType("int");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<int>("OrderDetailID")
                        .HasColumnType("int");

                    b.HasKey("Feedback_ImageID");

                    b.HasIndex("OrderDetailID");

                    b.HasIndex("ImageID", "OrderDetailID")
                        .IsUnique();

                    b.ToTable("Feedback_Image");
                });

            modelBuilder.Entity("eSMP.Models.Feedback_Status", b =>
                {
                    b.Property<int>("Feedback_StatusID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Feedback_StatusID"), 1L, 1);

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Feedback_StatusID");

                    b.ToTable("Feedback_Status");
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

            modelBuilder.Entity("eSMP.Models.Item", b =>
                {
                    b.Property<int>("ItemID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ItemID"), 1L, 1);

                    b.Property<DateTime>("Create_date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Discount")
                        .HasColumnType("float");

                    b.Property<int>("Item_StatusID")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("Rate")
                        .HasColumnType("real");

                    b.Property<int>("StoreID")
                        .HasColumnType("int");

                    b.Property<int>("Sub_CategoryID")
                        .HasColumnType("int");

                    b.HasKey("ItemID");

                    b.HasIndex("Item_StatusID");

                    b.HasIndex("StoreID");

                    b.HasIndex("Sub_CategoryID");

                    b.ToTable("Item");
                });

            modelBuilder.Entity("eSMP.Models.Item_Image", b =>
                {
                    b.Property<int>("Item_ImageID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Item_ImageID"), 1L, 1);

                    b.Property<int>("ImageID")
                        .HasColumnType("int");

                    b.Property<int>("ItemID")
                        .HasColumnType("int");

                    b.HasKey("Item_ImageID");

                    b.HasIndex("ItemID");

                    b.HasIndex("ImageID", "ItemID")
                        .IsUnique();

                    b.ToTable("Item_Image");
                });

            modelBuilder.Entity("eSMP.Models.Item_Status", b =>
                {
                    b.Property<int>("Item_StatusID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Item_StatusID"), 1L, 1);

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("StatusName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Item_StatusID");

                    b.ToTable("ItemStatus");
                });

            modelBuilder.Entity("eSMP.Models.Model_Item", b =>
                {
                    b.Property<int>("Model_ItemID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Model_ItemID"), 1L, 1);

                    b.Property<int>("Brand_ModelID")
                        .HasColumnType("int");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<int>("ItemID")
                        .HasColumnType("int");

                    b.HasKey("Model_ItemID");

                    b.HasIndex("Brand_ModelID");

                    b.HasIndex("ItemID", "Brand_ModelID")
                        .IsUnique();

                    b.ToTable("Model_Item");
                });

            modelBuilder.Entity("eSMP.Models.Order", b =>
                {
                    b.Property<int>("OrderID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("OrderID"), 1L, 1);

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Create_Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("District")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("FeeShip")
                        .HasColumnType("float");

                    b.Property<bool>("IsPay")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Pick_Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Pick_District")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Pick_Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Pick_Province")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Pick_Tel")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Pick_Ward")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Province")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Tel")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.Property<string>("Ward")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("OrderID");

                    b.HasIndex("UserID");

                    b.ToTable("Order");
                });

            modelBuilder.Entity("eSMP.Models.OrderBuy_Transacsion", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"), 1L, 1);

                    b.Property<DateTime>("Create_Date")
                        .HasColumnType("datetime2");

                    b.Property<long>("MomoTransactionID")
                        .HasColumnType("bigint");

                    b.Property<int>("OrderID")
                        .HasColumnType("int");

                    b.Property<int>("ResultCode")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("OrderID");

                    b.ToTable("OrderBuy_Transacsion");
                });

            modelBuilder.Entity("eSMP.Models.OrderDetail", b =>
                {
                    b.Property<int>("OrderDetailID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("OrderDetailID"), 1L, 1);

                    b.Property<int>("Amount")
                        .HasColumnType("int");

                    b.Property<double>("DiscountPurchase")
                        .HasColumnType("float");

                    b.Property<DateTime?>("FeedBack_Date")
                        .HasColumnType("datetime2");

                    b.Property<double?>("Feedback_Rate")
                        .HasColumnType("float");

                    b.Property<int?>("Feedback_StatusID")
                        .HasColumnType("int");

                    b.Property<string>("Feedback_Title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("OrderID")
                        .HasColumnType("int");

                    b.Property<double>("PricePurchase")
                        .HasColumnType("float");

                    b.Property<int>("Sub_ItemID")
                        .HasColumnType("int");

                    b.HasKey("OrderDetailID");

                    b.HasIndex("Feedback_StatusID");

                    b.HasIndex("OrderID");

                    b.HasIndex("Sub_ItemID");

                    b.ToTable("OrderDetail");
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

            modelBuilder.Entity("eSMP.Models.Specification", b =>
                {
                    b.Property<int>("SpecificationID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SpecificationID"), 1L, 1);

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("SpecificationName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("SpecificationID");

                    b.ToTable("Specification");
                });

            modelBuilder.Entity("eSMP.Models.Specification_Value", b =>
                {
                    b.Property<int>("Specification_ValueID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Specification_ValueID"), 1L, 1);

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<int>("ItemID")
                        .HasColumnType("int");

                    b.Property<int>("SpecificationID")
                        .HasColumnType("int");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Specification_ValueID");

                    b.HasIndex("ItemID");

                    b.HasIndex("SpecificationID", "ItemID")
                        .IsUnique();

                    b.ToTable("Specification_Value");
                });

            modelBuilder.Entity("eSMP.Models.Store", b =>
                {
                    b.Property<int>("StoreID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("StoreID"), 1L, 1);

                    b.Property<int>("AddressID")
                        .HasColumnType("int");

                    b.Property<DateTime>("Create_date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ImageID")
                        .HasColumnType("int");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Pick_date")
                        .HasColumnType("int");

                    b.Property<string>("StoreName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Store_StatusID")
                        .HasColumnType("int");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.HasKey("StoreID");

                    b.HasIndex("AddressID");

                    b.HasIndex("ImageID");

                    b.HasIndex("Store_StatusID");

                    b.HasIndex("UserID")
                        .IsUnique();

                    b.ToTable("Store");
                });

            modelBuilder.Entity("eSMP.Models.Store_Status", b =>
                {
                    b.Property<int>("Store_StatusID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Store_StatusID"), 1L, 1);

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("StatusName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Store_StatusID");

                    b.ToTable("Store_Status");
                });

            modelBuilder.Entity("eSMP.Models.Sub_Category", b =>
                {
                    b.Property<int>("Sub_CategoryID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Sub_CategoryID"), 1L, 1);

                    b.Property<int>("CategoryID")
                        .HasColumnType("int");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Sub_categoryName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Sub_CategoryID");

                    b.HasIndex("CategoryID");

                    b.ToTable("Sub_Category");
                });

            modelBuilder.Entity("eSMP.Models.Sub_Item", b =>
                {
                    b.Property<int>("Sub_ItemID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Sub_ItemID"), 1L, 1);

                    b.Property<int>("Amount")
                        .HasColumnType("int");

                    b.Property<int>("ImageID")
                        .HasColumnType("int");

                    b.Property<int>("ItemID")
                        .HasColumnType("int");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.Property<int>("SubItem_StatusID")
                        .HasColumnType("int");

                    b.Property<string>("Sub_ItemName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Sub_ItemID");

                    b.HasIndex("ImageID");

                    b.HasIndex("ItemID");

                    b.HasIndex("SubItem_StatusID");

                    b.ToTable("Sub_Item");
                });

            modelBuilder.Entity("eSMP.Models.SubCate_Specification", b =>
                {
                    b.Property<int>("SubCate_SpecificationID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SubCate_SpecificationID"), 1L, 1);

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<int>("SpecificationID")
                        .HasColumnType("int");

                    b.Property<int>("Sub_CategoryID")
                        .HasColumnType("int");

                    b.HasKey("SubCate_SpecificationID");

                    b.HasIndex("Sub_CategoryID");

                    b.HasIndex("SpecificationID", "Sub_CategoryID")
                        .IsUnique();

                    b.ToTable("SubCate_Specification");
                });

            modelBuilder.Entity("eSMP.Models.SubItem_Status", b =>
                {
                    b.Property<int>("SubItem_StatusID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SubItem_StatusID"), 1L, 1);

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("SubItem_StatusID");

                    b.ToTable("SubItem_Status");
                });

            modelBuilder.Entity("eSMP.Models.User", b =>
                {
                    b.Property<int>("UserID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserID"), 1L, 1);

                    b.Property<DateTime>("Crete_date")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getdate()");

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ImageID")
                        .HasColumnType("int");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("RoleID")
                        .HasColumnType("int");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("isActive")
                        .HasColumnType("bit");

                    b.HasKey("UserID");

                    b.HasIndex("ImageID");

                    b.HasIndex("RoleID");

                    b.HasIndex("Phone", "RoleID")
                        .IsUnique();

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

            modelBuilder.Entity("eSMP.Models.Brand_Model", b =>
                {
                    b.HasOne("eSMP.Models.Brand", "Brand")
                        .WithMany()
                        .HasForeignKey("BrandID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Brand");
                });

            modelBuilder.Entity("eSMP.Models.Feedback_Image", b =>
                {
                    b.HasOne("eSMP.Models.Image", "Image")
                        .WithMany()
                        .HasForeignKey("ImageID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("eSMP.Models.OrderDetail", "OrderDetail")
                        .WithMany()
                        .HasForeignKey("OrderDetailID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Image");

                    b.Navigation("OrderDetail");
                });

            modelBuilder.Entity("eSMP.Models.Item", b =>
                {
                    b.HasOne("eSMP.Models.Item_Status", "Item_Status")
                        .WithMany()
                        .HasForeignKey("Item_StatusID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("eSMP.Models.Store", "Store")
                        .WithMany()
                        .HasForeignKey("StoreID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("eSMP.Models.Sub_Category", "Sub_Category")
                        .WithMany()
                        .HasForeignKey("Sub_CategoryID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Item_Status");

                    b.Navigation("Store");

                    b.Navigation("Sub_Category");
                });

            modelBuilder.Entity("eSMP.Models.Item_Image", b =>
                {
                    b.HasOne("eSMP.Models.Image", "Image")
                        .WithMany()
                        .HasForeignKey("ImageID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("eSMP.Models.Item", "Item")
                        .WithMany()
                        .HasForeignKey("ItemID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Image");

                    b.Navigation("Item");
                });

            modelBuilder.Entity("eSMP.Models.Model_Item", b =>
                {
                    b.HasOne("eSMP.Models.Brand_Model", "Brand_Model")
                        .WithMany()
                        .HasForeignKey("Brand_ModelID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("eSMP.Models.Item", "Item")
                        .WithMany()
                        .HasForeignKey("ItemID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Brand_Model");

                    b.Navigation("Item");
                });

            modelBuilder.Entity("eSMP.Models.Order", b =>
                {
                    b.HasOne("eSMP.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("eSMP.Models.OrderBuy_Transacsion", b =>
                {
                    b.HasOne("eSMP.Models.Order", "Order")
                        .WithMany()
                        .HasForeignKey("OrderID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");
                });

            modelBuilder.Entity("eSMP.Models.OrderDetail", b =>
                {
                    b.HasOne("eSMP.Models.Feedback_Status", "Feedback_Status")
                        .WithMany()
                        .HasForeignKey("Feedback_StatusID");

                    b.HasOne("eSMP.Models.Order", "Order")
                        .WithMany()
                        .HasForeignKey("OrderID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("eSMP.Models.Sub_Item", "Sub_Item")
                        .WithMany()
                        .HasForeignKey("Sub_ItemID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Feedback_Status");

                    b.Navigation("Order");

                    b.Navigation("Sub_Item");
                });

            modelBuilder.Entity("eSMP.Models.Specification_Value", b =>
                {
                    b.HasOne("eSMP.Models.Item", "Item")
                        .WithMany()
                        .HasForeignKey("ItemID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("eSMP.Models.Specification", "Specification")
                        .WithMany()
                        .HasForeignKey("SpecificationID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Item");

                    b.Navigation("Specification");
                });

            modelBuilder.Entity("eSMP.Models.Store", b =>
                {
                    b.HasOne("eSMP.Models.Address", "Address")
                        .WithMany()
                        .HasForeignKey("AddressID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("eSMP.Models.Image", "Image")
                        .WithMany()
                        .HasForeignKey("ImageID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("eSMP.Models.Store_Status", "Store_Status")
                        .WithMany()
                        .HasForeignKey("Store_StatusID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("eSMP.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Address");

                    b.Navigation("Image");

                    b.Navigation("Store_Status");

                    b.Navigation("User");
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

            modelBuilder.Entity("eSMP.Models.Sub_Item", b =>
                {
                    b.HasOne("eSMP.Models.Image", "Image")
                        .WithMany()
                        .HasForeignKey("ImageID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("eSMP.Models.Item", "Item")
                        .WithMany()
                        .HasForeignKey("ItemID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("eSMP.Models.SubItem_Status", "SubItem_Status")
                        .WithMany()
                        .HasForeignKey("SubItem_StatusID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Image");

                    b.Navigation("Item");

                    b.Navigation("SubItem_Status");
                });

            modelBuilder.Entity("eSMP.Models.SubCate_Specification", b =>
                {
                    b.HasOne("eSMP.Models.Specification", "Specification")
                        .WithMany()
                        .HasForeignKey("SpecificationID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("eSMP.Models.Sub_Category", "Sub_Category")
                        .WithMany()
                        .HasForeignKey("Sub_CategoryID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Specification");

                    b.Navigation("Sub_Category");
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

                    b.Navigation("Image");

                    b.Navigation("Role");
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
