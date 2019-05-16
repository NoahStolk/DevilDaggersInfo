﻿// <auto-generated />
using System;
using DevilDaggersWebsite.Models.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DevilDaggersWebsite.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20190515111757_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.11-servicing-32099")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("DevilDaggersWebsite.Models.Database.CustomLeaderboards.CustomEntry", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<float>("Accuracy");

                    b.Property<int?>("CustomLeaderboardID");

                    b.Property<int>("DeathType");

                    b.Property<int>("EnemiesAlive");

                    b.Property<int>("Gems");

                    b.Property<int>("Homing");

                    b.Property<int>("Kills");

                    b.Property<float>("LevelUpTime2");

                    b.Property<float>("LevelUpTime3");

                    b.Property<float>("LevelUpTime4");

                    b.Property<float>("Time");

                    b.HasKey("ID");

                    b.HasIndex("CustomLeaderboardID");

                    b.ToTable("CustomEntry");
                });

            modelBuilder.Entity("DevilDaggersWebsite.Models.Database.CustomLeaderboards.CustomLeaderboard", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("SpawnsetFileName");

                    b.Property<string>("SpawnsetHash");

                    b.HasKey("ID");

                    b.ToTable("CustomLeaderboards");
                });

            modelBuilder.Entity("DevilDaggersWebsite.Models.Database.CustomLeaderboards.CustomEntry", b =>
                {
                    b.HasOne("DevilDaggersWebsite.Models.Database.CustomLeaderboards.CustomLeaderboard")
                        .WithMany("Entries")
                        .HasForeignKey("CustomLeaderboardID");
                });
#pragma warning restore 612, 618
        }
    }
}
