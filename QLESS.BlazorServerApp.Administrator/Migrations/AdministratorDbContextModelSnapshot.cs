﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using QLESS.Core.Data.EntityFramework;
using System;

namespace QLESS.BlazorServerApp.Administrator.Migrations
{
    [DbContext(typeof(AdministratorDbContext))]
    partial class AdministratorDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.17")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("QLESS.Core.Entities.Card", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("Balance")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Expiry")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("Number")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("PrivilegeCardId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("TypeId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("PrivilegeCardId");

                    b.HasIndex("TypeId");

                    b.ToTable("Card");
                });

            modelBuilder.Entity("QLESS.Core.Entities.CardType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("BaseFare")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Description")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<Guid>("DiscountStrategyId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("FareStrategyId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("InitialBalance")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("MaximumBalance")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("MaximumReloadAmount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("MinimumBalance")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("MinimumReloadAmount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Name")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<long>("Validity")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("CardType");
                });

            modelBuilder.Entity("QLESS.Core.Entities.CardTypePrivilege", b =>
                {
                    b.Property<Guid>("CardTypeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("PrivilegeId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("CardTypeId", "PrivilegeId");

                    b.HasIndex("PrivilegeId");

                    b.ToTable("CardTypePrivilege");
                });

            modelBuilder.Entity("QLESS.Core.Entities.Privilege", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("IdentificationNumberPattern")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Name")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Privilege");
                });

            modelBuilder.Entity("QLESS.Core.Entities.PrivilegeCard", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("IdentificationNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("TypeId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("TypeId");

                    b.ToTable("PrivilegeCard");
                });

            modelBuilder.Entity("QLESS.Core.Entities.Trip", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("CardId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Entry")
                        .HasColumnType("datetime2");

                    b.Property<int>("EntryStationNumber")
                        .HasColumnType("int");

                    b.Property<DateTime?>("Exit")
                        .HasColumnType("datetime2");

                    b.Property<int>("ExitStationNumber")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CardId");

                    b.ToTable("Trip");
                });

            modelBuilder.Entity("QLESS.Core.Entities.Card", b =>
                {
                    b.HasOne("QLESS.Core.Entities.PrivilegeCard", "PrivilegeCard")
                        .WithMany()
                        .HasForeignKey("PrivilegeCardId");

                    b.HasOne("QLESS.Core.Entities.CardType", "Type")
                        .WithMany("Cards")
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PrivilegeCard");

                    b.Navigation("Type");
                });

            modelBuilder.Entity("QLESS.Core.Entities.CardTypePrivilege", b =>
                {
                    b.HasOne("QLESS.Core.Entities.CardType", "CardType")
                        .WithMany("CardTypePrivileges")
                        .HasForeignKey("CardTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("QLESS.Core.Entities.Privilege", "Privilege")
                        .WithMany("CardTypePrivileges")
                        .HasForeignKey("PrivilegeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CardType");

                    b.Navigation("Privilege");
                });

            modelBuilder.Entity("QLESS.Core.Entities.PrivilegeCard", b =>
                {
                    b.HasOne("QLESS.Core.Entities.Privilege", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId");

                    b.Navigation("Type");
                });

            modelBuilder.Entity("QLESS.Core.Entities.Trip", b =>
                {
                    b.HasOne("QLESS.Core.Entities.Card", null)
                        .WithMany("Trips")
                        .HasForeignKey("CardId");
                });

            modelBuilder.Entity("QLESS.Core.Entities.Card", b =>
                {
                    b.Navigation("Trips");
                });

            modelBuilder.Entity("QLESS.Core.Entities.CardType", b =>
                {
                    b.Navigation("Cards");

                    b.Navigation("CardTypePrivileges");
                });

            modelBuilder.Entity("QLESS.Core.Entities.Privilege", b =>
                {
                    b.Navigation("CardTypePrivileges");
                });
#pragma warning restore 612, 618
        }
    }
}
