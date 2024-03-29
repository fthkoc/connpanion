﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using connpanion.API.Data;

namespace connpanion.API.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20190611090252_MessageEntityAdded")]
    partial class MessageEntityAdded
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024");

            modelBuilder.Entity("connpanion.API.Models.Like", b =>
                {
                    b.Property<int>("LikerID");

                    b.Property<int>("LikeeID");

                    b.HasKey("LikerID", "LikeeID");

                    b.HasIndex("LikeeID");

                    b.ToTable("Likes");
                });

            modelBuilder.Entity("connpanion.API.Models.Message", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Content");

                    b.Property<DateTime?>("DateRead");

                    b.Property<DateTime>("MessageSent");

                    b.Property<bool>("ReceiverDeleted");

                    b.Property<int>("ReceiverID");

                    b.Property<bool>("SenderDeleted");

                    b.Property<int>("SenderID");

                    b.Property<bool>("isRead");

                    b.HasKey("ID");

                    b.HasIndex("ReceiverID");

                    b.HasIndex("SenderID");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("connpanion.API.Models.Photograph", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateAdded");

                    b.Property<string>("Description");

                    b.Property<bool>("IsMainPhotograph");

                    b.Property<string>("PublicID");

                    b.Property<string>("URL");

                    b.Property<int>("UserID");

                    b.HasKey("ID");

                    b.HasIndex("UserID");

                    b.ToTable("Photographs");
                });

            modelBuilder.Entity("connpanion.API.Models.User", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("City");

                    b.Property<string>("Country");

                    b.Property<DateTime>("Created");

                    b.Property<DateTime>("DateOfBirth");

                    b.Property<string>("Gender");

                    b.Property<string>("Interests");

                    b.Property<string>("Introduction");

                    b.Property<string>("KnownAs");

                    b.Property<DateTime>("LastActive");

                    b.Property<string>("LookingFor");

                    b.Property<string>("Nationality");

                    b.Property<byte[]>("Password");

                    b.Property<byte[]>("Salt");

                    b.Property<string>("UserName");

                    b.HasKey("ID");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("connpanion.API.Models.Value", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("ID");

                    b.ToTable("Values");
                });

            modelBuilder.Entity("connpanion.API.Models.Like", b =>
                {
                    b.HasOne("connpanion.API.Models.User", "Likee")
                        .WithMany("Likers")
                        .HasForeignKey("LikeeID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("connpanion.API.Models.User", "Liker")
                        .WithMany("Likees")
                        .HasForeignKey("LikerID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("connpanion.API.Models.Message", b =>
                {
                    b.HasOne("connpanion.API.Models.User", "Receiver")
                        .WithMany("MessagesReceived")
                        .HasForeignKey("ReceiverID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("connpanion.API.Models.User", "Sender")
                        .WithMany("MessagesSent")
                        .HasForeignKey("SenderID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("connpanion.API.Models.Photograph", b =>
                {
                    b.HasOne("connpanion.API.Models.User", "User")
                        .WithMany("Photos")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
