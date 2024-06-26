﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SpaceTraders.Repositories.DatabaseRepositories.DbContexts;

#nullable disable

namespace SpaceTraders.Migrations
{
    [DbContext(typeof(RepositoryDbContext))]
    [Migration("20240516215806_Add Factions to db")]
    partial class AddFactionstodb
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.4");

            modelBuilder.Entity("SpaceTraders.Repositories.DatabaseRepositories.DbModels.Agent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AccountId")
                        .HasColumnType("TEXT");

                    b.Property<int>("Credits")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Headquarters")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("ShipCount")
                        .HasColumnType("INTEGER");

                    b.Property<string>("StartingFaction")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Symbol")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Agents");
                });

            modelBuilder.Entity("SpaceTraders.Repositories.DatabaseRepositories.DbModels.Contract", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Accepted")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ContractId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("DeadlineToAccept")
                        .HasColumnType("TEXT");

                    b.Property<string>("FactionSymbol")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("Fulfilled")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TermsId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("TermsId");

                    b.ToTable("Contracts");
                });

            modelBuilder.Entity("SpaceTraders.Repositories.DatabaseRepositories.DbModels.ContractDeliverGood", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("ContractTermsId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("DestinationSymbol")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("TradeSymbol")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("UnitsFulfilled")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UnitsRequired")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ContractTermsId");

                    b.ToTable("ContractDeliverGood");
                });

            modelBuilder.Entity("SpaceTraders.Repositories.DatabaseRepositories.DbModels.ContractPayment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("OnAccepted")
                        .HasColumnType("INTEGER");

                    b.Property<int>("OnFulfilled")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("ContractPayment");
                });

            modelBuilder.Entity("SpaceTraders.Repositories.DatabaseRepositories.DbModels.ContractTerms", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Deadline")
                        .HasColumnType("TEXT");

                    b.Property<int>("PaymentId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("PaymentId");

                    b.ToTable("ContractTerms");
                });

            modelBuilder.Entity("SpaceTraders.Repositories.DatabaseRepositories.DbModels.Faction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Headquarters")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsRecruiting")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Symbol")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Factions");
                });

            modelBuilder.Entity("SpaceTraders.Repositories.DatabaseRepositories.DbModels.FactionTrait", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("FactionId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Symbol")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("FactionId");

                    b.ToTable("FactionTrait");
                });

            modelBuilder.Entity("SpaceTraders.Repositories.DatabaseRepositories.DbModels.Token", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("TokenValue")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Tokens");
                });

            modelBuilder.Entity("SpaceTraders.Repositories.DatabaseRepositories.DbModels.Contract", b =>
                {
                    b.HasOne("SpaceTraders.Repositories.DatabaseRepositories.DbModels.ContractTerms", "Terms")
                        .WithMany()
                        .HasForeignKey("TermsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Terms");
                });

            modelBuilder.Entity("SpaceTraders.Repositories.DatabaseRepositories.DbModels.ContractDeliverGood", b =>
                {
                    b.HasOne("SpaceTraders.Repositories.DatabaseRepositories.DbModels.ContractTerms", null)
                        .WithMany("Deliver")
                        .HasForeignKey("ContractTermsId");
                });

            modelBuilder.Entity("SpaceTraders.Repositories.DatabaseRepositories.DbModels.ContractTerms", b =>
                {
                    b.HasOne("SpaceTraders.Repositories.DatabaseRepositories.DbModels.ContractPayment", "Payment")
                        .WithMany()
                        .HasForeignKey("PaymentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Payment");
                });

            modelBuilder.Entity("SpaceTraders.Repositories.DatabaseRepositories.DbModels.FactionTrait", b =>
                {
                    b.HasOne("SpaceTraders.Repositories.DatabaseRepositories.DbModels.Faction", null)
                        .WithMany("Traits")
                        .HasForeignKey("FactionId");
                });

            modelBuilder.Entity("SpaceTraders.Repositories.DatabaseRepositories.DbModels.ContractTerms", b =>
                {
                    b.Navigation("Deliver");
                });

            modelBuilder.Entity("SpaceTraders.Repositories.DatabaseRepositories.DbModels.Faction", b =>
                {
                    b.Navigation("Traits");
                });
#pragma warning restore 612, 618
        }
    }
}
