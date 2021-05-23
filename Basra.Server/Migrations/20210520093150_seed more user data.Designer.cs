﻿// <auto-generated />
using System;
using Basra.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Basra.Server.Migrations
{
    [DbContext(typeof(MasterContext))]
    [Migration("20210520093150_seed more user data")]
    partial class seedmoreuserdata
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
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

                    b.Property<DateTime?>("LastMoneyAimRequestTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("Level")
                        .HasColumnType("int");

                    b.Property<int>("Money")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OwnedBackgroundIds")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OwnedCardBackIds")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OwnedTitleIds")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PictureUrl")
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
                            Draws = 3,
                            Fbid = "0",
                            IsMoneyAidClaimable = false,
                            Level = 13,
                            Money = 850,
                            Name = "hany",
                            OwnedBackgroundIds = "[1,3]",
                            OwnedCardBackIds = "[0,2]",
                            OwnedTitleIds = "[2,4]",
                            PictureUrl = "https://pbs.twimg.com/profile_images/592734306725933057/s4-h_LQC.jpg",
                            PlayedGames = 3,
                            RequestedMoneyAidToday = 2,
                            Wins = 4,
                            XP = 806
                        },
                        new
                        {
                            Id = "1",
                            Draws = 1,
                            Fbid = "1",
                            IsMoneyAidClaimable = false,
                            Level = 43,
                            Money = 2000,
                            Name = "samy",
                            OwnedBackgroundIds = "[0,9]",
                            OwnedCardBackIds = "[0,1,2]",
                            OwnedTitleIds = "[11,6]",
                            PictureUrl = "https://d3g9pb5nvr3u7.cloudfront.net/authors/57ea8955d8de1e1602f67ca0/1902081322/256.jpg",
                            PlayedGames = 7,
                            RequestedMoneyAidToday = 0,
                            Wins = 11,
                            XP = 1983
                        },
                        new
                        {
                            Id = "2",
                            Draws = 37,
                            Fbid = "2",
                            IsMoneyAidClaimable = false,
                            Level = 139,
                            Money = 8500,
                            Name = "anni",
                            OwnedBackgroundIds = "[10,8]",
                            OwnedCardBackIds = "[4,9]",
                            OwnedTitleIds = "[1,3]",
                            PictureUrl = "https://pbs.twimg.com/profile_images/633661532350623745/8U1sJUc8_400x400.png",
                            PlayedGames = 973,
                            RequestedMoneyAidToday = 4,
                            Wins = 192,
                            XP = 8062
                        },
                        new
                        {
                            Id = "3",
                            Draws = 1,
                            Fbid = "3",
                            IsMoneyAidClaimable = true,
                            Level = 4,
                            Money = 3,
                            Name = "ali",
                            OwnedBackgroundIds = "[10,8]",
                            OwnedCardBackIds = "[4,9]",
                            OwnedTitleIds = "[1,3]",
                            PictureUrl = "https://pbs.twimg.com/profile_images/723902674970750978/p8JWhWxP_400x400.jpg",
                            PlayedGames = 6,
                            RequestedMoneyAidToday = 3,
                            Wins = 2,
                            XP = 12
                        });
                });

            modelBuilder.Entity("Basra.Server.UserRelation", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("OtherUserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("RelationType")
                        .HasColumnType("int");

                    b.HasKey("UserId", "OtherUserId");

                    b.HasIndex("OtherUserId");

                    b.ToTable("UserRelation");
                });

            modelBuilder.Entity("Basra.Server.UserRelation", b =>
                {
                    b.HasOne("Basra.Server.User", "OtherUser")
                        .WithMany()
                        .HasForeignKey("OtherUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Basra.Server.User", "User")
                        .WithMany("Relations")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("OtherUser");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Basra.Server.User", b =>
                {
                    b.Navigation("Relations");
                });
#pragma warning restore 612, 618
        }
    }
}
