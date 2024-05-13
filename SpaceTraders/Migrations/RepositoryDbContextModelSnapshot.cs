﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SpaceTraders.Repositories.DatabaseRepositories.DbContexts;

#nullable disable

namespace SpaceTraders.Migrations
{
    [DbContext(typeof(RepositoryDbContext))]
    partial class RepositoryDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
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
#pragma warning restore 612, 618
        }
    }
}