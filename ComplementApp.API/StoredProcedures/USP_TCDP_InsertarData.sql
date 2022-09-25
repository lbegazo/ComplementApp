CREATE OR ALTER PROCEDURE [dbo].[USP_TCDP_InsertarData]    
(  @TipoDocumentoId INT)   
AS   
BEGIN    
    
   
  IF(@TipoDocumentoId=1)  
  BEGIN  
  DELETE FROM TCDP  
  WHERE Instancia=3  
    
  INSERT INTO TCDP(  
   Instancia,  
   Cdp,  
   Crp,  
   Obligacion,  
   OrdenPago,  
   Fecha,  
   Detalle1,  
   ValorInicial,  
   Operacion,  
   ValorTotal,  
   SaldoActual,  
   Detalle2,  
   Detalle3,  
   Detalle4,  
   Detalle5,  
   Detalle6,  
   Detalle7,  
   Detalle8,  
   Detalle9,  
   Detalle10,  
   RubroPresupuestalId,  
   TerceroId,  
   PciId)  
   SELECT '3' AS Instancia,  
     NumeroDocumento AS Cdp,  
     0 AS Crp,  
     0 AS Obligacion,  
     0 AS OrdenPago,  
     FechaRegistro,  
     Estado,  
     ValorInicial,  
     ValorOperacion,  
     ValorActual,  
     SaldoPorComprometer AS SaldoActual,  
     Dependencia AS Detalle2,  
     DependenciaDescripcion AS Detalle3,  
     Objeto,''AS Detalle5,  
     '' AS Detalle6,  
     '' AS Detalle7,  
     '' AS Detalle8,  
     '' AS Detalle9 ,  
     '' AS Detalle10,  
     RubroPresupuestalId,  
     null AS TerceroId ,  
     PciId  
   FROM TDocumentoCdp  
   INNER JOIN TRubroPresupuestal ON IdentificacionRubroPresupuestal = Identificacion  
  END   
  ELSE IF(@TipoDocumentoId=2)  
  BEGIN  
   DELETE FROM TCDP  
   WHERE Instancia=4  
  
   INSERT INTO TCDP(  
   Instancia,  
   Cdp,  
   Crp,  
   Obligacion,  
   OrdenPago,  
   Fecha,  
   Detalle1,  
   ValorInicial,  
   Operacion,  
   ValorTotal,  
   SaldoActual,  
   Detalle2,  
   Detalle3,  
   Detalle4,  
   Detalle5,  
   Detalle6,  
   Detalle7,  
   Detalle8,  
   Detalle9,  
   Detalle10,  
   RubroPresupuestalId,  
   TerceroId,  
   PciId)  
    SELECT '4' AS Instancia,  
      CDP,  
      NumeroDocumento AS Crp,  
      0 AS Obligacion,  
      0 AS OrdenPago,  
      C.FechaRegistro,  
      Estado,  
      ValorInicial,  
      ValorOperacion,  
      ValorActual,  
      SaldoPorUtilizar,  
      Dependencia AS Detalle2,  
      DependenciaDescripcion AS Detalle3,  
      Observaciones,  
      '' AS Detalle5,  
      NumeroDocumentoSoporte  AS Detalle6,  
      TipoDocumentoSoporte Detalle7,  
      FuenteFinanciacion AS Detalle8,  
      SituacionFondo AS Detalle9,   
      TRecursoPresupuestal.Codigo AS Detalle10,   
      C.RubroPresupuestalId,  
      T.TerceroId,  
      PciId  
    FROM TDocumentoCompromiso C  
    INNER JOIN TRecursoPresupuestal ON Nombre = RecursoPresupuestal  
    INNER JOIN TRubroPresupuestal ON IdentificacionRubroPresupuestal = Identificacion  
    INNER JOIN TTipoDocumentoIdentidad TD ON UPPER(C.TipoIdentificacion) = UPPER(TD.Nombre)  
    INNER JOIN TTercero T ON TD.TipoDocumentoIdentidadId = T.TipoIdentificacion AND T.NumeroIdentificacion=C.NumeroIdentificacion  
  END  
  ELSE IF(@TipoDocumentoId=3)  
  BEGIN  
   DELETE FROM TCDP  
   WHERE Instancia=5  
  
   INSERT INTO TCDP(  
   Instancia,  
   Cdp,  
   Crp,  
   Obligacion,  
   OrdenPago,  
   Fecha,  
   Detalle1,  
   ValorInicial,  
   Operacion,  
   ValorTotal,  
   SaldoActual,  
   Detalle2,  
   Detalle3,  
   Detalle4,  
   Detalle5,  
   Detalle6,  
   Detalle7,  
   Detalle8,  
   Detalle9,  
   Detalle10,  
   RubroPresupuestalId,  
   TerceroId,  
   PciId)  
    SELECT '5' AS Instancia,  
      Cdp,  
      Compromisos AS Crp,  
      NumeroDocumento AS Obligacón,  
      0 AS Ordenpago,  
      O.FechaRegistro,  
      Estado AS Detalle1,  
      ValorInicial,  
      ValorOperacion,  
      ValorActual2,  
      SaldoPorUtilizar AS SaldoActual,  
      Dependencia AS Detalle2,  
      DependenciaDescripcion AS Detalle3,  
      Concepto AS Detalle4,  
      '' AS Detalle5,  
      NumeroDocumentoSoporte AS Detalle6,  
      TipoDocumentoSoporte AS Detalle7,  
      FuenteFinanciacion,  
      SituacionFondo,  
      RecursoPresupuestal AS Detalle10,   
      RubroPresupuestalId,  
      T.TerceroId,  
      PciId  
    FROM TDocumentoObligacion O  
    INNER JOIN TRubroPresupuestal ON IdentificacionRubroPresupuestal = Identificacion  
    INNER JOIN TTipoDocumentoIdentidad TD ON UPPER(O.TipoIdentificacion) = UPPER(TD.Nombre)  
    INNER JOIN TTercero T ON TD.TipoDocumentoIdentidadId = T.TipoIdentificacion AND T.NumeroIdentificacion=O.NumeroIdentificacion  
 END  
 ELSE IF(@TipoDocumentoId=4)  
  BEGIN  
   DELETE FROM TCDP  
   WHERE Instancia=6  
  
   INSERT INTO TCDP(  
   Instancia,  
   Cdp,  
   Crp,  
   Obligacion,  
   OrdenPago,  
   Fecha,  
   Detalle1,  
   ValorInicial,  
   Operacion,  
   ValorTotal,  
   SaldoActual,  
   Detalle2,  
   Detalle3,  
   Detalle4,  
   Detalle5,  
   Detalle6,  
   Detalle7,  
   Detalle8,  
   Detalle9,  
   Detalle10,  
   RubroPresupuestalId,  
   TerceroId,  
   PciId)  
    SELECT '6' AS Instancia,  
      Cdp,  
      Compromisos AS Crp,  
      Obligaciones,  
      NumeroDocumento AS Ordenpago,  
      OP.FechaRegistro,  
      Estado AS Detalle1,  
      ValorBruto,   
      ValorDeduccion,   
      ValorNeto,  
      ValorBruto,  
      Dependencia AS Detalle2,  
      DependenciaDescripcion AS Detalle3,  
      ConceptoPago AS Detalle4,  
      '' AS Detalle5,  
      NumeroDocumentoSoporteCompromiso AS Detalle6,  
      TipoDocumentoSoporteCompromiso AS Detalle7,  
      FuenteFinanciacion,  
      SituacionFondo,  
      RecursoPresupuestal AS Detalle10,  
      RubroPresupuestalId,  
      T.TerceroId,  
      PciId  
    FROM TDocumentoOrdenPago OP  
    INNER JOIN TRubroPresupuestal ON IdentificacionRubroPresupuestal = Identificacion  
    INNER JOIN TTipoDocumentoIdentidad TD ON UPPER(OP.TipoIdentificacion) = UPPER(TD.Nombre)  
    INNER JOIN TTercero T ON TD.TipoDocumentoIdentidadId = T.TipoIdentificacion AND T.NumeroIdentificacion=OP.NumeroIdentificacion  
 END  
   
 END