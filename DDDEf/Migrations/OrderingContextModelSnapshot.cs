﻿// <auto-generated />
using System;
using DDDEf.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DDDEf.Migrations
{
    [DbContext(typeof(OrderingContext))]
    partial class OrderingContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.3-rtm-32065")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("Relational:Sequence:.orderitemseq", "'orderitemseq', '', '1', '10', '', '', 'Int64', 'False'")
                .HasAnnotation("Relational:Sequence:ordering.orderseq", "'orderseq', 'ordering', '1', '10', '', '', 'Int64', 'False'")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DDDEf.Domain.Model.OrderAggregate.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:HiLoSequenceName", "orderseq")
                        .HasAnnotation("SqlServer:HiLoSequenceSchema", "ordering")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.SequenceHiLo);

                    b.Property<string>("Description");

                    b.Property<DateTime>("OrderDate");

                    b.Property<int>("OrderStatusId");

                    b.HasKey("Id");

                    b.HasIndex("OrderStatusId");

                    b.ToTable("orders","ordering");
                });

            modelBuilder.Entity("DDDEf.Domain.Model.OrderAggregate.OrderItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:HiLoSequenceName", "orderitemseq")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.SequenceHiLo);

                    b.Property<decimal>("Discount");

                    b.Property<int>("OrderId");

                    b.Property<string>("PictureUrl");

                    b.Property<int>("ProductId");

                    b.Property<string>("ProductName")
                        .IsRequired();

                    b.Property<decimal>("UnitPrice");

                    b.Property<int>("Units");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.ToTable("orderItems","ordering");
                });

            modelBuilder.Entity("DDDEf.Domain.Model.OrderAggregate.OrderStatus", b =>
                {
                    b.Property<int>("Id")
                        .HasDefaultValue(1);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("orderstatus","ordering");
                });

            modelBuilder.Entity("DDDEf.Domain.Model.OrderAggregate.Order", b =>
                {
                    b.HasOne("DDDEf.Domain.Model.OrderAggregate.OrderStatus", "OrderStatus")
                        .WithMany()
                        .HasForeignKey("OrderStatusId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.OwnsOne("DDDEf.Domain.Model.OrderAggregate.Address", "Address", b1 =>
                        {
                            b1.Property<int?>("OrderId");

                            b1.Property<string>("City");

                            b1.Property<string>("Country");

                            b1.Property<string>("State");

                            b1.Property<string>("Street");

                            b1.Property<string>("ZipCode");

                            b1.ToTable("orders","ordering");

                            b1.HasOne("DDDEf.Domain.Model.OrderAggregate.Order")
                                .WithOne("Address")
                                .HasForeignKey("DDDEf.Domain.Model.OrderAggregate.Address", "OrderId")
                                .OnDelete(DeleteBehavior.Cascade);
                        });
                });

            modelBuilder.Entity("DDDEf.Domain.Model.OrderAggregate.OrderItem", b =>
                {
                    b.HasOne("DDDEf.Domain.Model.OrderAggregate.Order")
                        .WithMany("OrderItems")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
