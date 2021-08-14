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
    [Migration("20210731085338_last")]
    partial class last
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.6")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Basra.Server.User", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("BasraCount")
                        .HasColumnType("int");

                    b.Property<int>("BigBasraCount")
                        .HasColumnType("int");

                    b.Property<int>("Draws")
                        .HasColumnType("int");

                    b.Property<int>("EatenCardsCount")
                        .HasColumnType("int");

                    b.Property<string>("Fbid")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LastMoneyAimRequestTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("Level")
                        .HasColumnType("int");

                    b.Property<int>("MaxWinStreak")
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

                    b.Property<int>("PlayedRoomsCount")
                        .HasColumnType("int");

                    b.Property<int>("RequestedMoneyAidToday")
                        .HasColumnType("int");

                    b.Property<int>("SelectedBackground")
                        .HasColumnType("int");

                    b.Property<int>("SelectedCardback")
                        .HasColumnType("int");

                    b.Property<int>("SelectedTitleId")
                        .HasColumnType("int");

                    b.Property<int>("TotalEarnedMoney")
                        .HasColumnType("int");

                    b.Property<int>("WinStreak")
                        .HasColumnType("int");

                    b.Property<int>("WonRoomsCount")
                        .HasColumnType("int");

                    b.Property<int>("XP")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = "0",
                            BasraCount = 0,
                            BigBasraCount = 0,
                            Draws = 3,
                            EatenCardsCount = 0,
                            Fbid = "0",
                            Level = 13,
                            MaxWinStreak = 0,
                            Money = 22250,
                            Name = "hany",
                            OwnedBackgroundIds = "[1,3]",
                            OwnedCardBackIds = "[0,2]",
                            OwnedTitleIds = "[2,4]",
                            PictureUrl = "https://pbs.twimg.com/profile_images/592734306725933057/s4-h_LQC.jpg",
                            PlayedRoomsCount = 3,
                            RequestedMoneyAidToday = 2,
                            SelectedBackground = 0,
                            SelectedCardback = 2,
                            SelectedTitleId = 0,
                            TotalEarnedMoney = 0,
                            WinStreak = 0,
                            WonRoomsCount = 4,
                            XP = 806
                        },
                        new
                        {
                            Id = "1",
                            BasraCount = 0,
                            BigBasraCount = 0,
                            Draws = 1,
                            EatenCardsCount = 0,
                            Fbid = "1",
                            Level = 43,
                            MaxWinStreak = 0,
                            Money = 89000,
                            Name = "samy",
                            OwnedBackgroundIds = "[0,9]",
                            OwnedCardBackIds = "[0,1,2]",
                            OwnedTitleIds = "[11,6]",
                            PictureUrl = "https://d3g9pb5nvr3u7.cloudfront.net/authors/57ea8955d8de1e1602f67ca0/1902081322/256.jpg",
                            PlayedRoomsCount = 7,
                            RequestedMoneyAidToday = 0,
                            SelectedBackground = 0,
                            SelectedCardback = 1,
                            SelectedTitleId = 0,
                            TotalEarnedMoney = 0,
                            WinStreak = 0,
                            WonRoomsCount = 11,
                            XP = 1983
                        },
                        new
                        {
                            Id = "2",
                            BasraCount = 0,
                            BigBasraCount = 0,
                            Draws = 37,
                            EatenCardsCount = 0,
                            Fbid = "2",
                            Level = 139,
                            MaxWinStreak = 0,
                            Money = 8500,
                            Name = "anni",
                            OwnedBackgroundIds = "[10,8]",
                            OwnedCardBackIds = "[4,9]",
                            OwnedTitleIds = "[1,3]",
                            PictureUrl = "https://pbs.twimg.com/profile_images/633661532350623745/8U1sJUc8_400x400.png",
                            PlayedRoomsCount = 973,
                            RequestedMoneyAidToday = 4,
                            SelectedBackground = 0,
                            SelectedCardback = 4,
                            SelectedTitleId = 0,
                            TotalEarnedMoney = 0,
                            WinStreak = 0,
                            WonRoomsCount = 192,
                            XP = 8062
                        },
                        new
                        {
                            Id = "3",
                            BasraCount = 0,
                            BigBasraCount = 0,
                            Draws = 1,
                            EatenCardsCount = 0,
                            Fbid = "3",
                            Level = 4,
                            MaxWinStreak = 0,
                            Money = 3,
                            Name = "ali",
                            OwnedBackgroundIds = "[10,8]",
                            OwnedCardBackIds = "[2,4,8]",
                            OwnedTitleIds = "[1,3]",
                            PictureUrl = "https://pbs.twimg.com/profile_images/723902674970750978/p8JWhWxP_400x400.jpg",
                            PlayedRoomsCount = 6,
                            RequestedMoneyAidToday = 3,
                            SelectedBackground = 0,
                            SelectedCardback = 2,
                            SelectedTitleId = 0,
                            TotalEarnedMoney = 0,
                            WinStreak = 0,
                            WonRoomsCount = 2,
                            XP = 12
                        },
                        new
                        {
                            Id = "999",
                            BasraCount = 0,
                            BigBasraCount = 0,
                            Draws = 2,
                            EatenCardsCount = 0,
                            Level = 7,
                            MaxWinStreak = 0,
                            Money = 1000,
                            Name = "botA",
                            OwnedBackgroundIds = "[0,3]",
                            OwnedCardBackIds = "[8]",
                            OwnedTitleIds = "[1]",
                            PictureUrl = "https://pbs.twimg.com/profile_images/723902674970750978/p8JWhWxP_400x400.jpg",
                            PlayedRoomsCount = 9,
                            RequestedMoneyAidToday = 0,
                            SelectedBackground = 0,
                            SelectedCardback = 1,
                            SelectedTitleId = 0,
                            TotalEarnedMoney = 0,
                            WinStreak = 0,
                            WonRoomsCount = 2,
                            XP = 34
                        },
                        new
                        {
                            Id = "9999",
                            BasraCount = 0,
                            BigBasraCount = 0,
                            Draws = 2,
                            EatenCardsCount = 0,
                            Level = 8,
                            MaxWinStreak = 0,
                            Money = 1100,
                            Name = "botB",
                            OwnedBackgroundIds = "[3]",
                            OwnedCardBackIds = "[0,8]",
                            OwnedTitleIds = "[0,1]",
                            PictureUrl = "https://pbs.twimg.com/profile_images/723902674970750978/p8JWhWxP_400x400.jpg",
                            PlayedRoomsCount = 11,
                            RequestedMoneyAidToday = 0,
                            SelectedBackground = 0,
                            SelectedCardback = 2,
                            SelectedTitleId = 0,
                            TotalEarnedMoney = 0,
                            WinStreak = 0,
                            WonRoomsCount = 3,
                            XP = 44
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
