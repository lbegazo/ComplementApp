﻿// <auto-generated />
using System;
using ComplementApp.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ComplementApp.API.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20200908095124_UpdateTransaccionTable")]
    partial class UpdateTransaccionTable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ComplementApp.API.Models.ActividadEspecifica", b =>
                {
                    b.Property<int>("ActividadEspecificaId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ActividadGeneralId")
                        .HasColumnType("int");

                    b.Property<string>("Nombre")
                        .HasColumnType("VARCHAR(250)");

                    b.Property<int?>("RubroPresupuestalId")
                        .HasColumnType("int");

                    b.Property<decimal>("SaldoPorProgramar")
                        .HasColumnType("decimal(30,8)");

                    b.Property<decimal>("ValorApropiacionVigente")
                        .HasColumnType("decimal(30,8)");

                    b.HasKey("ActividadEspecificaId");

                    b.HasIndex("ActividadGeneralId");

                    b.HasIndex("RubroPresupuestalId");

                    b.ToTable("TActividadEspecifica");
                });

            modelBuilder.Entity("ComplementApp.API.Models.ActividadGeneral", b =>
                {
                    b.Property<int>("ActividadGeneralId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("VARCHAR(250)");

                    b.Property<decimal>("SaldoActual")
                        .HasColumnType("decimal(30,8)");

                    b.Property<decimal>("ValorApropiacion")
                        .HasColumnType("decimal(30,8)");

                    b.HasKey("ActividadGeneralId");

                    b.ToTable("TActividadGeneral");
                });

            modelBuilder.Entity("ComplementApp.API.Models.Area", b =>
                {
                    b.Property<int>("AreaId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Codigo")
                        .IsRequired()
                        .HasColumnType("VARCHAR(10)");

                    b.Property<bool>("Estado")
                        .HasColumnType("bit");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("VARCHAR(100)");

                    b.HasKey("AreaId");

                    b.ToTable("TArea");
                });

            modelBuilder.Entity("ComplementApp.API.Models.CDP", b =>
                {
                    b.Property<int>("CdpId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("Cdp")
                        .HasColumnType("bigint");

                    b.Property<long>("Crp")
                        .HasColumnType("bigint");

                    b.Property<string>("Detalle1")
                        .HasColumnType("VARCHAR(250)");

                    b.Property<string>("Detalle10")
                        .HasColumnType("VARCHAR(250)");

                    b.Property<string>("Detalle2")
                        .HasColumnType("VARCHAR(250)");

                    b.Property<string>("Detalle3")
                        .HasColumnType("VARCHAR(250)");

                    b.Property<string>("Detalle4")
                        .HasColumnType("VARCHAR(250)");

                    b.Property<string>("Detalle5")
                        .HasColumnType("VARCHAR(250)");

                    b.Property<string>("Detalle6")
                        .HasColumnType("VARCHAR(250)");

                    b.Property<string>("Detalle7")
                        .HasColumnType("VARCHAR(250)");

                    b.Property<string>("Detalle8")
                        .HasColumnType("VARCHAR(250)");

                    b.Property<string>("Detalle9")
                        .HasColumnType("VARCHAR(250)");

                    b.Property<DateTime>("Fecha")
                        .HasColumnType("datetime2");

                    b.Property<int>("Instancia")
                        .HasColumnType("int");

                    b.Property<long>("Obligacion")
                        .HasColumnType("bigint");

                    b.Property<decimal>("Operacion")
                        .HasColumnType("decimal(30,8)");

                    b.Property<long>("OrdenPago")
                        .HasColumnType("bigint");

                    b.Property<int>("RubroPresupuestalId")
                        .HasColumnType("int");

                    b.Property<decimal>("SaldoActual")
                        .HasColumnType("decimal(30,8)");

                    b.Property<int>("TerceroId")
                        .HasColumnType("int");

                    b.Property<decimal>("ValorInicial")
                        .HasColumnType("decimal(30,8)");

                    b.Property<decimal>("ValorTotal")
                        .HasColumnType("decimal(30,8)");

                    b.HasKey("CdpId");

                    b.HasIndex("RubroPresupuestalId");

                    b.HasIndex("TerceroId");

                    b.ToTable("TCDP");
                });

            modelBuilder.Entity("ComplementApp.API.Models.Cargo", b =>
                {
                    b.Property<int>("CargoId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Codigo")
                        .IsRequired()
                        .HasColumnType("VARCHAR(10)");

                    b.Property<bool>("Estado")
                        .HasColumnType("bit");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("VARCHAR(100)");

                    b.HasKey("CargoId");

                    b.ToTable("TCargo");
                });

            modelBuilder.Entity("ComplementApp.API.Models.Dependencia", b =>
                {
                    b.Property<int>("DependenciaId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AreaId")
                        .HasColumnType("int");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("VARCHAR(250)");

                    b.HasKey("DependenciaId");

                    b.HasIndex("AreaId");

                    b.ToTable("TDependencia");
                });

            modelBuilder.Entity("ComplementApp.API.Models.DetalleCDP", b =>
                {
                    b.Property<int>("DetalleCdpId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ActividadEspecificaId")
                        .HasColumnType("int");

                    b.Property<int>("ActividadGeneralId")
                        .HasColumnType("int");

                    b.Property<string>("AplicaContrato")
                        .HasColumnType("VARCHAR(10)");

                    b.Property<int>("AreaId")
                        .HasColumnType("int");

                    b.Property<long>("Cdp")
                        .HasColumnType("bigint");

                    b.Property<int>("Convenio")
                        .HasColumnType("int");

                    b.Property<int>("DecretoId")
                        .HasColumnType("int");

                    b.Property<int>("DependenciaId")
                        .HasColumnType("int");

                    b.Property<int>("IdArchivo")
                        .HasColumnType("int");

                    b.Property<string>("PcpId")
                        .HasColumnType("VARCHAR(10)");

                    b.Property<string>("PlanDeCompras")
                        .HasColumnType("VARCHAR(500)");

                    b.Property<int>("Prod")
                        .HasColumnType("int");

                    b.Property<int>("Proy")
                        .HasColumnType("int");

                    b.Property<int>("Rp")
                        .HasColumnType("int");

                    b.Property<int>("RubroPresupuestalId")
                        .HasColumnType("int");

                    b.Property<decimal>("SaldoAct")
                        .HasColumnType("decimal(30,8)");

                    b.Property<decimal>("SaldoDisponible")
                        .HasColumnType("decimal(30,8)");

                    b.Property<decimal>("SaldoTotal")
                        .HasColumnType("decimal(30,8)");

                    b.Property<int>("UsuarioId")
                        .HasColumnType("int");

                    b.Property<decimal>("ValorAct")
                        .HasColumnType("decimal(30,8)");

                    b.Property<decimal>("ValorCDP")
                        .HasColumnType("decimal(30,8)");

                    b.Property<decimal>("ValorOB")
                        .HasColumnType("decimal(30,8)");

                    b.Property<decimal>("ValorOP")
                        .HasColumnType("decimal(30,8)");

                    b.Property<decimal>("ValorRP")
                        .HasColumnType("decimal(30,8)");

                    b.Property<decimal>("Valor_Convenio")
                        .HasColumnType("decimal(30,8)");

                    b.HasKey("DetalleCdpId");

                    b.HasIndex("UsuarioId");

                    b.ToTable("TDetalleCDP");
                });

            modelBuilder.Entity("ComplementApp.API.Models.Estado", b =>
                {
                    b.Property<int>("EstadoId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasColumnType("VARCHAR(250)");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("VARCHAR(100)");

                    b.Property<string>("TipoDocumento")
                        .IsRequired()
                        .HasColumnType("VARCHAR(100)");

                    b.HasKey("EstadoId");

                    b.ToTable("TEstado");
                });

            modelBuilder.Entity("ComplementApp.API.Models.Perfil", b =>
                {
                    b.Property<int>("PerfilId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Codigo")
                        .IsRequired()
                        .HasColumnType("VARCHAR(10)");

                    b.Property<string>("Descripcion")
                        .HasColumnType("VARCHAR(250)");

                    b.Property<bool>("Estado")
                        .HasColumnType("bit");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("VARCHAR(100)");

                    b.HasKey("PerfilId");

                    b.ToTable("TPerfil");
                });

            modelBuilder.Entity("ComplementApp.API.Models.PerfilTransaccion", b =>
                {
                    b.Property<int>("PerfilId")
                        .HasColumnType("int");

                    b.Property<int>("TransaccionId")
                        .HasColumnType("int");

                    b.HasKey("PerfilId", "TransaccionId");

                    b.HasIndex("TransaccionId");

                    b.ToTable("TPerfilTransaccion");
                });

            modelBuilder.Entity("ComplementApp.API.Models.Photo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsMain")
                        .HasColumnType("bit");

                    b.Property<string>("Url")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Photos");
                });

            modelBuilder.Entity("ComplementApp.API.Models.PlanPago", b =>
                {
                    b.Property<int>("PlanPagoId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AnioPago")
                        .HasColumnType("int");

                    b.Property<long>("Cdp")
                        .HasColumnType("bigint");

                    b.Property<long>("Crp")
                        .HasColumnType("bigint");

                    b.Property<int?>("DiasAlPago")
                        .HasColumnType("int");

                    b.Property<int?>("EstadoOrdenPagoId")
                        .HasColumnType("int");

                    b.Property<int?>("EstadoPlanPagoId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("FechaFactura")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("FechaOrdenPago")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("FechaRadicadoProveedor")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("FechaRadicadoSupervisor")
                        .HasColumnType("datetime2");

                    b.Property<int>("MesPago")
                        .HasColumnType("int");

                    b.Property<string>("NumeroFactura")
                        .HasColumnType("VARCHAR(250)");

                    b.Property<int>("NumeroPago")
                        .HasColumnType("int");

                    b.Property<string>("NumeroRadicadoProveedor")
                        .HasColumnType("VARCHAR(250)");

                    b.Property<string>("NumeroRadicadoSupervisor")
                        .HasColumnType("VARCHAR(250)");

                    b.Property<long?>("Obligacion")
                        .HasColumnType("bigint");

                    b.Property<string>("Observaciones")
                        .HasColumnType("VARCHAR(250)");

                    b.Property<long?>("OrdenPago")
                        .HasColumnType("bigint");

                    b.Property<int>("RubroPresupuestalId")
                        .HasColumnType("int");

                    b.Property<int>("TerceroId")
                        .HasColumnType("int");

                    b.Property<int>("UsoPresupuestalId")
                        .HasColumnType("int");

                    b.Property<decimal>("ValorAPagar")
                        .HasColumnType("decimal(30,8)");

                    b.Property<decimal?>("ValorAdicion")
                        .HasColumnType("decimal(30,8)");

                    b.Property<decimal?>("ValorFacturado")
                        .HasColumnType("decimal(30,8)");

                    b.Property<decimal>("ValorInicial")
                        .HasColumnType("decimal(30,8)");

                    b.Property<decimal?>("ValorPagado")
                        .HasColumnType("decimal(30,8)");

                    b.Property<bool>("Viaticos")
                        .HasColumnType("bit");

                    b.HasKey("PlanPagoId");

                    b.HasIndex("RubroPresupuestalId");

                    b.HasIndex("TerceroId");

                    b.HasIndex("UsoPresupuestalId");

                    b.ToTable("TPlanPago");
                });

            modelBuilder.Entity("ComplementApp.API.Models.RubroPresupuestal", b =>
                {
                    b.Property<int>("RubroPresupuestalId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Identificacion")
                        .IsRequired()
                        .HasColumnType("VARCHAR(100)");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("VARCHAR(250)");

                    b.Property<int?>("PadreRubroId")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.HasKey("RubroPresupuestalId");

                    b.ToTable("TRubroPresupuestal");
                });

            modelBuilder.Entity("ComplementApp.API.Models.Tercero", b =>
                {
                    b.Property<int>("TerceroId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("VARCHAR(250)");

                    b.Property<string>("NumeroIdentificacion")
                        .IsRequired()
                        .HasColumnType("VARCHAR(20)");

                    b.Property<int>("TipoIdentificacion")
                        .HasColumnType("int");

                    b.HasKey("TerceroId");

                    b.ToTable("TTercero");
                });

            modelBuilder.Entity("ComplementApp.API.Models.TipoDetalleCDP", b =>
                {
                    b.Property<int>("TipoDetalleCDPId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("VARCHAR(250)");

                    b.HasKey("TipoDetalleCDPId");

                    b.ToTable("TTipoDetalleCDP");
                });

            modelBuilder.Entity("ComplementApp.API.Models.TipoOperacion", b =>
                {
                    b.Property<int>("TipoOperacionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Codigo")
                        .IsRequired()
                        .HasColumnType("VARCHAR(10)");

                    b.Property<bool>("Estado")
                        .HasColumnType("bit");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("VARCHAR(250)");

                    b.HasKey("TipoOperacionId");

                    b.ToTable("TTipoOperacion");
                });

            modelBuilder.Entity("ComplementApp.API.Models.Transaccion", b =>
                {
                    b.Property<int>("TransaccionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Codigo")
                        .IsRequired()
                        .HasColumnType("VARCHAR(50)");

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasColumnType("VARCHAR(250)");

                    b.Property<bool>("Estado")
                        .HasColumnType("bit");

                    b.Property<string>("Icono")
                        .HasColumnType("VARCHAR(50)");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("VARCHAR(100)");

                    b.Property<int?>("PadreTransaccionId")
                        .IsRequired()
                        .HasColumnType("int");

                    b.Property<string>("Ruta")
                        .HasColumnType("VARCHAR(50)");

                    b.HasKey("TransaccionId");

                    b.ToTable("TTransaccion");
                });

            modelBuilder.Entity("ComplementApp.API.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("City")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Country")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DayOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("Gender")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Interests")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Introduction")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("KnownAs")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("LastActive")
                        .HasColumnType("datetime2");

                    b.Property<string>("LookingFor")
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("PasswordHash")
                        .HasColumnType("varbinary(max)");

                    b.Property<byte[]>("PasswordSalt")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("Username")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("TUser");
                });

            modelBuilder.Entity("ComplementApp.API.Models.UsoPresupuestal", b =>
                {
                    b.Property<int>("UsoPresupuestalId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Identificacion")
                        .IsRequired()
                        .HasColumnType("VARCHAR(100)");

                    b.Property<bool>("MarcaAusteridad")
                        .HasColumnType("bit");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("VARCHAR(250)");

                    b.Property<int?>("RubroPresupuestalId")
                        .HasColumnType("int");

                    b.HasKey("UsoPresupuestalId");

                    b.HasIndex("RubroPresupuestalId");

                    b.ToTable("TUsoPresupuestal");
                });

            modelBuilder.Entity("ComplementApp.API.Models.Usuario", b =>
                {
                    b.Property<int>("UsuarioId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Apellidos")
                        .IsRequired()
                        .HasColumnType("VARCHAR(100)");

                    b.Property<int?>("AreaId")
                        .HasColumnType("int");

                    b.Property<int?>("CargoId")
                        .HasColumnType("int");

                    b.Property<bool>("EsAdministrador")
                        .HasColumnType("bit");

                    b.Property<DateTime>("FechaCreacion")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("FechaUltimoAcceso")
                        .HasColumnType("datetime2");

                    b.Property<string>("Nombres")
                        .IsRequired()
                        .HasColumnType("VARCHAR(100)");

                    b.Property<byte[]>("PasswordHash")
                        .HasColumnType("varbinary(max)");

                    b.Property<byte[]>("PasswordSalt")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("VARCHAR(20)");

                    b.HasKey("UsuarioId");

                    b.HasIndex("AreaId");

                    b.HasIndex("CargoId");

                    b.ToTable("TUsuario");
                });

            modelBuilder.Entity("ComplementApp.API.Models.UsuarioPerfil", b =>
                {
                    b.Property<int>("UsuarioId")
                        .HasColumnType("int");

                    b.Property<int>("PerfilId")
                        .HasColumnType("int");

                    b.HasKey("UsuarioId", "PerfilId");

                    b.HasIndex("PerfilId");

                    b.ToTable("TUsuarioPerfil");
                });

            modelBuilder.Entity("ComplementApp.API.Models.Value", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Values");
                });

            modelBuilder.Entity("ComplementApp.API.Models.ActividadEspecifica", b =>
                {
                    b.HasOne("ComplementApp.API.Models.ActividadGeneral", "ActividadGeneral")
                        .WithMany()
                        .HasForeignKey("ActividadGeneralId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ComplementApp.API.Models.RubroPresupuestal", "RubroPresupuestal")
                        .WithMany()
                        .HasForeignKey("RubroPresupuestalId");
                });

            modelBuilder.Entity("ComplementApp.API.Models.CDP", b =>
                {
                    b.HasOne("ComplementApp.API.Models.RubroPresupuestal", "RubroPresupuestal")
                        .WithMany()
                        .HasForeignKey("RubroPresupuestalId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ComplementApp.API.Models.Tercero", "Tercero")
                        .WithMany()
                        .HasForeignKey("TerceroId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ComplementApp.API.Models.Dependencia", b =>
                {
                    b.HasOne("ComplementApp.API.Models.Area", "Area")
                        .WithMany()
                        .HasForeignKey("AreaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ComplementApp.API.Models.DetalleCDP", b =>
                {
                    b.HasOne("ComplementApp.API.Models.Usuario", "Usuario")
                        .WithMany()
                        .HasForeignKey("UsuarioId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ComplementApp.API.Models.PerfilTransaccion", b =>
                {
                    b.HasOne("ComplementApp.API.Models.Perfil", "Perfil")
                        .WithMany("PerfilTransacciones")
                        .HasForeignKey("PerfilId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ComplementApp.API.Models.Transaccion", "Transaccion")
                        .WithMany("PerfilTransacciones")
                        .HasForeignKey("TransaccionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ComplementApp.API.Models.Photo", b =>
                {
                    b.HasOne("ComplementApp.API.Models.User", "User")
                        .WithMany("Photos")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ComplementApp.API.Models.PlanPago", b =>
                {
                    b.HasOne("ComplementApp.API.Models.RubroPresupuestal", "RubroPresupuestal")
                        .WithMany()
                        .HasForeignKey("RubroPresupuestalId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ComplementApp.API.Models.Tercero", "Tercero")
                        .WithMany()
                        .HasForeignKey("TerceroId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ComplementApp.API.Models.UsoPresupuestal", "UsoPresupuestal")
                        .WithMany()
                        .HasForeignKey("UsoPresupuestalId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ComplementApp.API.Models.UsoPresupuestal", b =>
                {
                    b.HasOne("ComplementApp.API.Models.RubroPresupuestal", "RubroPresupuestal")
                        .WithMany()
                        .HasForeignKey("RubroPresupuestalId");
                });

            modelBuilder.Entity("ComplementApp.API.Models.Usuario", b =>
                {
                    b.HasOne("ComplementApp.API.Models.Area", "Area")
                        .WithMany()
                        .HasForeignKey("AreaId");

                    b.HasOne("ComplementApp.API.Models.Cargo", "Cargo")
                        .WithMany()
                        .HasForeignKey("CargoId");
                });

            modelBuilder.Entity("ComplementApp.API.Models.UsuarioPerfil", b =>
                {
                    b.HasOne("ComplementApp.API.Models.Perfil", "Perfil")
                        .WithMany("UsuarioPerfiles")
                        .HasForeignKey("PerfilId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ComplementApp.API.Models.Usuario", "Usuario")
                        .WithMany("UsuarioPerfiles")
                        .HasForeignKey("UsuarioId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
