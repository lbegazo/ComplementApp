﻿// <auto-generated />
using System;
using ComplementApp.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ComplementApp.API.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20200705010040_CreateTipoDetalleModificacionTable")]
    partial class CreateTipoDetalleModificacionTable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.4");

            modelBuilder.Entity("ComplementApp.API.Models.Area", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Codigo")
                        .HasColumnType("TEXT");

                    b.Property<string>("Descripcion")
                        .HasColumnType("TEXT");

                    b.Property<bool>("EsAdministrable")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Estado")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("TB_Area");
                });

            modelBuilder.Entity("ComplementApp.API.Models.CDP", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Cdp")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Dependencia")
                        .HasColumnType("TEXT");

                    b.Property<string>("Estado")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Fecha")
                        .HasColumnType("TEXT");

                    b.Property<int>("Pro")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Proy")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Rubro")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Saldo")
                        .HasColumnType("TEXT");

                    b.Property<string>("Tipo")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Valor")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("TB_CDP");
                });

            modelBuilder.Entity("ComplementApp.API.Models.Cargo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Codigo")
                        .HasColumnType("TEXT");

                    b.Property<string>("Descripcion")
                        .HasColumnType("TEXT");

                    b.Property<bool>("EsAdministrable")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Estado")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("TB_Cargo");
                });

            modelBuilder.Entity("ComplementApp.API.Models.DetalleCDP", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ActividadBpin")
                        .HasColumnType("TEXT");

                    b.Property<string>("Area")
                        .HasColumnType("TEXT");

                    b.Property<int>("Cdp")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Contrato")
                        .HasColumnType("TEXT");

                    b.Property<int>("Crp")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Dependencia")
                        .HasColumnType("TEXT");

                    b.Property<int>("IdArchivo")
                        .HasColumnType("INTEGER");

                    b.Property<int>("IdSofi")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Paa")
                        .HasColumnType("INTEGER");

                    b.Property<string>("PlanDeCompras")
                        .HasColumnType("TEXT");

                    b.Property<int>("Prod")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Proy")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Proyecto")
                        .HasColumnType("TEXT");

                    b.Property<string>("Responsable")
                        .HasColumnType("TEXT");

                    b.Property<string>("Rubro")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("SaldoAct")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("SaldoDisponible")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("SaldoTotal")
                        .HasColumnType("TEXT");

                    b.Property<string>("Tipo")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("ValorAct")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("ValorCDP")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("ValorOB")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("ValorOP")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("ValorRP")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("TB_DetalleCDP");
                });

            modelBuilder.Entity("ComplementApp.API.Models.Photo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsMain")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Url")
                        .HasColumnType("TEXT");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Photos");
                });

            modelBuilder.Entity("ComplementApp.API.Models.RubroPresupuestal", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Descripcion")
                        .HasColumnType("TEXT");

                    b.Property<string>("Identificacion")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("TB_RubroPresupuestal");
                });

            modelBuilder.Entity("ComplementApp.API.Models.TipoDetalleModificacion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Descripcion")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("TB_TipoDetalleModificacion");
                });

            modelBuilder.Entity("ComplementApp.API.Models.TipoOperacion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Codigo")
                        .HasColumnType("TEXT");

                    b.Property<string>("Descripcion")
                        .HasColumnType("TEXT");

                    b.Property<bool>("EsAdministrable")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Estado")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("TB_TipoOperacion");
                });

            modelBuilder.Entity("ComplementApp.API.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("City")
                        .HasColumnType("TEXT");

                    b.Property<string>("Country")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Created")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DayOfBirth")
                        .HasColumnType("TEXT");

                    b.Property<string>("Gender")
                        .HasColumnType("TEXT");

                    b.Property<string>("Interests")
                        .HasColumnType("TEXT");

                    b.Property<string>("Introduction")
                        .HasColumnType("TEXT");

                    b.Property<string>("KnownAs")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LastActive")
                        .HasColumnType("TEXT");

                    b.Property<string>("LookingFor")
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("PasswordHash")
                        .HasColumnType("BLOB");

                    b.Property<byte[]>("PasswordSalt")
                        .HasColumnType("BLOB");

                    b.Property<string>("Username")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ComplementApp.API.Models.Usuario", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Apellidos")
                        .HasColumnType("TEXT");

                    b.Property<int>("AreaId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("CargoId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("FechaCreacion")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("FechaUltimoAcceso")
                        .HasColumnType("TEXT");

                    b.Property<string>("Nombres")
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("PasswordHash")
                        .HasColumnType("BLOB");

                    b.Property<byte[]>("PasswordSalt")
                        .HasColumnType("BLOB");

                    b.Property<string>("Username")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AreaId");

                    b.HasIndex("CargoId");

                    b.ToTable("TB_Usuario");
                });

            modelBuilder.Entity("ComplementApp.API.Models.Value", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Values");
                });

            modelBuilder.Entity("ComplementApp.API.Models.Photo", b =>
                {
                    b.HasOne("ComplementApp.API.Models.User", "User")
                        .WithMany("Photos")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ComplementApp.API.Models.Usuario", b =>
                {
                    b.HasOne("ComplementApp.API.Models.Area", "Area")
                        .WithMany()
                        .HasForeignKey("AreaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ComplementApp.API.Models.Cargo", "Cargo")
                        .WithMany()
                        .HasForeignKey("CargoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
