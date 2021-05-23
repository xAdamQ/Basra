﻿// <auto-generated />
using System;
using Basra.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Basra.Server.Migrations
{
    [DbContext(typeof(MasterContext))]
    partial class MasterContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.6");

            modelBuilder.Entity("Basra.Server.User", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Draws")
                        .HasColumnType("int");

                    b.Property<string>("Fbid")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsMoneyAidClaimable")
                        .HasColumnType("bit");

                    b.Property<bool>("IsMoneyAimProcessing")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastMoneyAimRequestTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("Level")
                        .HasColumnType("int");

                    b.Property<int>("Money")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PlayedGames")
                        .HasColumnType("int");

                    b.Property<int>("RequestedMoneyAidToday")
                        .HasColumnType("int");

                    b.Property<int>("Wins")
                        .HasColumnType("int");

                    b.Property<int>("XP")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = "0",
                            Draws = 0,
                            Fbid = "0",
                            IsMoneyAidClaimable = false,
                            IsMoneyAimProcessing = false,
                            LastMoneyAimRequestTime = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Level = 0,
                            Money = 0,
                            Name = "hany",
                            PlayedGames = 3,
                            RequestedMoneyAidToday = 0,
                            Wins = 4,
                            XP = 0
                        },
                        new
                        {
                            Id = "1",
                            Draws = 0,
                            Fbid = "1",
                            IsMoneyAidClaimable = false,
                            IsMoneyAimProcessing = false,
                            LastMoneyAimRequestTime = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Level = 0,
                            Money = 0,
                            Name = "samy",
                            PlayedGames = 7,
                            RequestedMoneyAidToday = 0,
                            Wins = 11,
                            XP = 0
                        },
                        new
                        {
                            Id = "2",
                            Draws = 0,
                            Fbid = "2",
                            IsMoneyAidClaimable = false,
                            IsMoneyAimProcessing = false,
                            LastMoneyAimRequestTime = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Level = 0,
                            Money = 0,
                            Name = "anni",
                            PlayedGames = 9,
                            RequestedMoneyAidToday = 0,
                            Wins = 1,
                            XP = 0
                        },
                        new
                        {
                            Id = "3",
                            Draws = 0,
                            Fbid = "3",
                            IsMoneyAidClaimable = false,
                            IsMoneyAimProcessing = false,
                            LastMoneyAimRequestTime = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Level = 0,
                            Money = 0,
                            Name = "ali",
                            PlayedGames = 2,
                            RequestedMoneyAidToday = 0,
                            Wins = 0,
                            XP = 0
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
