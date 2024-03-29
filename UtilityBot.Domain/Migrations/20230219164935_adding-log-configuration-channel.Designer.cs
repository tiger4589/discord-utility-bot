﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UtilityBot.Domain.Database;

#nullable disable

namespace UtilityBot.Domain.Migrations
{
    [DbContext(typeof(UtilityBotContext))]
    [Migration("20230219164935_adding-log-configuration-channel")]
    partial class addinglogconfigurationchannel
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("UtilityBot.Domain.DomainObjects.ConfiguredAction", b =>
                {
                    b.Property<decimal>("GuildId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<string>("ConfigurationType")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("GuildId", "ConfigurationType");

                    b.ToTable("ConfiguredActions");
                });

            modelBuilder.Entity("UtilityBot.Domain.DomainObjects.JoinedServer", b =>
                {
                    b.Property<decimal>("GuildId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<bool>("IsActivated")
                        .HasColumnType("bit");

                    b.Property<bool>("IsConnected")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("GuildId");

                    b.ToTable("JoinedServers");
                });

            modelBuilder.Entity("UtilityBot.Domain.DomainObjects.LogConfiguration", b =>
                {
                    b.Property<decimal>("GuildId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<decimal>("ChannelId")
                        .HasColumnType("decimal(20,0)");

                    b.HasKey("GuildId");

                    b.ToTable("LogConfigurations");
                });

            modelBuilder.Entity("UtilityBot.Domain.DomainObjects.UserJoinConfiguration", b =>
                {
                    b.Property<decimal>("GuildId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<string>("Action")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("GuildId", "Action");

                    b.ToTable("UserJoinConfigurations");
                });

            modelBuilder.Entity("UtilityBot.Domain.DomainObjects.UserJoinMessage", b =>
                {
                    b.Property<decimal>("GuildId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<decimal?>("ChannelId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<bool>("IsPrivate")
                        .HasColumnType("bit");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("GuildId");

                    b.ToTable("UserJoinMessages");
                });

            modelBuilder.Entity("UtilityBot.Domain.DomainObjects.UserJoinRole", b =>
                {
                    b.Property<decimal>("GuildId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<decimal>("RoleId")
                        .HasColumnType("decimal(20,0)");

                    b.HasKey("GuildId", "RoleId");

                    b.ToTable("UserJoinRoles");
                });

            modelBuilder.Entity("UtilityBot.Domain.DomainObjects.VerifyConfiguration", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal>("ChannelId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<decimal>("RoleId")
                        .HasColumnType("decimal(20,0)");

                    b.HasKey("Id");

                    b.ToTable("VerifyConfigurations");
                });

            modelBuilder.Entity("UtilityBot.Domain.DomainObjects.ConfiguredAction", b =>
                {
                    b.HasOne("UtilityBot.Domain.DomainObjects.JoinedServer", "JoinedServer")
                        .WithMany()
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("JoinedServer");
                });

            modelBuilder.Entity("UtilityBot.Domain.DomainObjects.LogConfiguration", b =>
                {
                    b.HasOne("UtilityBot.Domain.DomainObjects.JoinedServer", "JoinedServer")
                        .WithMany()
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("JoinedServer");
                });

            modelBuilder.Entity("UtilityBot.Domain.DomainObjects.UserJoinConfiguration", b =>
                {
                    b.HasOne("UtilityBot.Domain.DomainObjects.JoinedServer", "JoinedServer")
                        .WithMany()
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("JoinedServer");
                });

            modelBuilder.Entity("UtilityBot.Domain.DomainObjects.UserJoinMessage", b =>
                {
                    b.HasOne("UtilityBot.Domain.DomainObjects.JoinedServer", "JoinedServer")
                        .WithMany()
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("JoinedServer");
                });

            modelBuilder.Entity("UtilityBot.Domain.DomainObjects.UserJoinRole", b =>
                {
                    b.HasOne("UtilityBot.Domain.DomainObjects.JoinedServer", "JoinedServer")
                        .WithMany()
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("JoinedServer");
                });
#pragma warning restore 612, 618
        }
    }
}
