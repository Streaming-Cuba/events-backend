﻿// <auto-generated />
using System;
using Events.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Events.API.Migrations.Event
{
    [DbContext(typeof(EventContext))]
    [Migration("20210425155719_GroupArg")]
    partial class GroupArg
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.5")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("Events.API.Models.Event", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int?>("CategoryId")
                        .HasColumnType("integer");

                    b.Property<string>("CoverPath")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Identifier")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("ModifiedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Organizer")
                        .HasColumnType("text");

                    b.Property<string>("ShortCoverPath")
                        .HasColumnType("text");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("StatusId")
                        .HasColumnType("integer");

                    b.Property<string>("Subtitle")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("StatusId");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("Events.API.Models.EventSocial", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("EventId")
                        .HasColumnType("integer");

                    b.Property<int>("SocialId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("SocialId");

                    b.HasIndex("EventId", "SocialId")
                        .IsUnique();

                    b.ToTable("EventSocials");
                });

            modelBuilder.Entity("Events.API.Models.EventTag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("EventId")
                        .HasColumnType("integer");

                    b.Property<int>("TagId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("TagId");

                    b.HasIndex("EventId", "TagId")
                        .IsUnique();

                    b.ToTable("EventTags");
                });

            modelBuilder.Entity("Events.API.Models.Group", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<int?>("EventId")
                        .HasColumnType("integer");

                    b.Property<string>("GroupArg")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("EventId");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("Events.API.Models.GroupItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("CoverPath")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<int?>("GroupId")
                        .HasColumnType("integer");

                    b.Property<int?>("MetadataId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("Number")
                        .HasColumnType("integer");

                    b.Property<int?>("TypeId")
                        .HasColumnType("integer");

                    b.Property<long>("Votes")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.HasIndex("MetadataId");

                    b.HasIndex("TypeId");

                    b.ToTable("GroupItems");
                });

            modelBuilder.Entity("Events.API.Models.GroupItemMetadata", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Interpreter")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Productor")
                        .HasColumnType("text");

                    b.Property<string>("Support")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("GroupItemMetadatas");
                });

            modelBuilder.Entity("Events.API.Models.GroupItemSocialSocial", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("ItemId")
                        .HasColumnType("integer");

                    b.Property<int>("SocialId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("SocialId");

                    b.HasIndex("ItemId", "SocialId")
                        .IsUnique();

                    b.ToTable("GroupItemSocialSocials");
                });

            modelBuilder.Entity("Events.API.Models.GroupItemType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("GroupItemTypes");
                });

            modelBuilder.Entity("Events.API.Models.Interaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int?>("EventId")
                        .HasColumnType("integer");

                    b.Property<bool>("Like")
                        .HasColumnType("boolean");

                    b.Property<bool>("Love")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("EventId");

                    b.ToTable("Interactions");
                });

            modelBuilder.Entity("Events.API.Models.NCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("Events.API.Models.NEventStatus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("EventStatuses");
                });

            modelBuilder.Entity("Events.API.Models.NTag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("Events.API.Models.Social", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int?>("PlatformTypeId")
                        .HasColumnType("integer");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("PlatformTypeId");

                    b.ToTable("Socials");
                });

            modelBuilder.Entity("Events.API.Models.SocialPlatformType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("SocialPlatformTypes");
                });

            modelBuilder.Entity("Events.API.Models.Event", b =>
                {
                    b.HasOne("Events.API.Models.NCategory", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId");

                    b.HasOne("Events.API.Models.NEventStatus", "Status")
                        .WithMany()
                        .HasForeignKey("StatusId");

                    b.Navigation("Category");

                    b.Navigation("Status");
                });

            modelBuilder.Entity("Events.API.Models.EventSocial", b =>
                {
                    b.HasOne("Events.API.Models.Event", "Event")
                        .WithMany("Socials")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Events.API.Models.Social", "Social")
                        .WithMany()
                        .HasForeignKey("SocialId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Event");

                    b.Navigation("Social");
                });

            modelBuilder.Entity("Events.API.Models.EventTag", b =>
                {
                    b.HasOne("Events.API.Models.Event", "Event")
                        .WithMany("Tags")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Events.API.Models.NTag", "Tag")
                        .WithMany()
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Event");

                    b.Navigation("Tag");
                });

            modelBuilder.Entity("Events.API.Models.Group", b =>
                {
                    b.HasOne("Events.API.Models.Event", null)
                        .WithMany("Groups")
                        .HasForeignKey("EventId");
                });

            modelBuilder.Entity("Events.API.Models.GroupItem", b =>
                {
                    b.HasOne("Events.API.Models.Group", null)
                        .WithMany("Items")
                        .HasForeignKey("GroupId");

                    b.HasOne("Events.API.Models.GroupItemMetadata", "Metadata")
                        .WithMany()
                        .HasForeignKey("MetadataId");

                    b.HasOne("Events.API.Models.GroupItemType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId");

                    b.Navigation("Metadata");

                    b.Navigation("Type");
                });

            modelBuilder.Entity("Events.API.Models.GroupItemSocialSocial", b =>
                {
                    b.HasOne("Events.API.Models.GroupItem", "Item")
                        .WithMany("Socials")
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Events.API.Models.Social", "Social")
                        .WithMany()
                        .HasForeignKey("SocialId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Item");

                    b.Navigation("Social");
                });

            modelBuilder.Entity("Events.API.Models.Interaction", b =>
                {
                    b.HasOne("Events.API.Models.Event", null)
                        .WithMany("Interactions")
                        .HasForeignKey("EventId");
                });

            modelBuilder.Entity("Events.API.Models.Social", b =>
                {
                    b.HasOne("Events.API.Models.SocialPlatformType", "PlatformType")
                        .WithMany()
                        .HasForeignKey("PlatformTypeId");

                    b.Navigation("PlatformType");
                });

            modelBuilder.Entity("Events.API.Models.Event", b =>
                {
                    b.Navigation("Groups");

                    b.Navigation("Interactions");

                    b.Navigation("Socials");

                    b.Navigation("Tags");
                });

            modelBuilder.Entity("Events.API.Models.Group", b =>
                {
                    b.Navigation("Items");
                });

            modelBuilder.Entity("Events.API.Models.GroupItem", b =>
                {
                    b.Navigation("Socials");
                });
#pragma warning restore 612, 618
        }
    }
}
