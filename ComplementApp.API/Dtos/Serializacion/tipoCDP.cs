namespace ComplementApp.API.Dtos.Serializacion.CDP
{
    #region Xml CDP Clases

    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.minhacienda.gov.co/mhcp/siifnacion/transferenciaxml/epg")]
    [System.Xml.Serialization.XmlRootAttribute("cdp", Namespace = "http://www.minhacienda.gov.co/mhcp/siifnacion/transferenciaxml/epg", IsNullable = false)]
    public partial class tipoCDP
    {   

        private tipoListaFiltroConsulta listaFiltroConsultaField;

        private tipoItemCDP[] listaItemCDPField;

        private string numRegistroField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public tipoListaFiltroConsulta listaFiltroConsulta
        {
            get
            {
                return this.listaFiltroConsultaField;
            }
            set
            {
                this.listaFiltroConsultaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)]
        [System.Xml.Serialization.XmlArrayItemAttribute("itemCDP", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
        public tipoItemCDP[] listaItemCDP
        {
            get
            {
                return this.listaItemCDPField;
            }
            set
            {
                this.listaItemCDPField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "integer")]
        public string numRegistro
        {
            get
            {
                return this.numRegistroField;
            }
            set
            {
                this.numRegistroField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.minhacienda.gov.co/mhcp/siifnacion/transferenciaxml/epg")]
    public partial class tipoListaFiltroConsulta
    {

        private string codPCIField;

        private string codPCIConsultaField;

        private string areaField;

        private enumCodTipoGasto codTipoGastoField;

        private enumCodTipoSaldo saldoField;

        private System.DateTime fechaInicioField;

        private System.DateTime fechaFinField;

        private enumSINO continuidadField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string codPCI
        {
            get
            {
                return this.codPCIField;
            }
            set
            {
                this.codPCIField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string codPCIConsulta
        {
            get
            {
                return this.codPCIConsultaField;
            }
            set
            {
                this.codPCIConsultaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)]
        public string area
        {
            get
            {
                return this.areaField;
            }
            set
            {
                this.areaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public enumCodTipoGasto codTipoGasto
        {
            get
            {
                return this.codTipoGastoField;
            }
            set
            {
                this.codTipoGastoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public enumCodTipoSaldo saldo
        {
            get
            {
                return this.saldoField;
            }
            set
            {
                this.saldoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "date")]
        public System.DateTime fechaInicio
        {
            get
            {
                return this.fechaInicioField;
            }
            set
            {
                this.fechaInicioField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "date")]
        public System.DateTime fechaFin
        {
            get
            {
                return this.fechaFinField;
            }
            set
            {
                this.fechaFinField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public enumSINO continuidad
        {
            get
            {
                return this.continuidadField;
            }
            set
            {
                this.continuidadField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.minhacienda.gov.co/mhcp/siifnacion/transferenciaxml/epg")]
    public enum enumCodTipoGasto
    {

        /// <remarks/>
        GASTOS_PERSONAL,

        /// <remarks/>
        GASTOS_GENERALES,

        /// <remarks/>
        TRANSFERENCIAS,

        /// <remarks/>
        COMERCIALIZACION,

        /// <remarks/>
        DEUDA,

        /// <remarks/>
        INVERSION,

        /// <remarks/>
        TODOS,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.minhacienda.gov.co/mhcp/siifnacion/transferenciaxml/epg")]
    public enum enumCodTipoSaldo
    {

        /// <remarks/>
        CON_SALDO,

        /// <remarks/>
        TODOS,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.minhacienda.gov.co/mhcp/siifnacion/transferenciaxml/epg")]
    public enum enumSINO
    {

        /// <remarks/>
        SI,

        /// <remarks/>
        NO,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.minhacienda.gov.co/mhcp/siifnacion/transferenciaxml/epg")]
    public partial class tipoItemEjecuCDP
    {

        private string codCompromisoField;

        private string codCuentaPagarField;

        private string codObligacionField;

        private string codOrdenPagoField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)]
        public string codCompromiso
        {
            get
            {
                return this.codCompromisoField;
            }
            set
            {
                this.codCompromisoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)]
        public string codCuentaPagar
        {
            get
            {
                return this.codCuentaPagarField;
            }
            set
            {
                this.codCuentaPagarField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)]
        public string codObligacion
        {
            get
            {
                return this.codObligacionField;
            }
            set
            {
                this.codObligacionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)]
        public string codOrdenPago
        {
            get
            {
                return this.codOrdenPagoField;
            }
            set
            {
                this.codOrdenPagoField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.minhacienda.gov.co/mhcp/siifnacion/transferenciaxml/epg")]
    public partial class tipoItemAfectacion
    {

        private string codDepAfectacionField;

        private string nomDepAfectacionField;

        private string codPosicionGastoField;

        private string nomPosicionGastoField;

        private string codFuenteFinanciacionField;

        private enumFuenteFinanciacion nomFuenteFinanciacionField;

        private string codRecursoPptalField;

        private string nomRecursoPptalField;

        private string codSituacionFondosField;

        private enumSituacionFondos nomSituacionFondosField;

        private decimal valorInicialField;

        private decimal valorActualField;

        private decimal valorOperacionField;

        private decimal saldoCompField;

        private decimal valorBloqueadoField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string codDepAfectacion
        {
            get
            {
                return this.codDepAfectacionField;
            }
            set
            {
                this.codDepAfectacionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string nomDepAfectacion
        {
            get
            {
                return this.nomDepAfectacionField;
            }
            set
            {
                this.nomDepAfectacionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string codPosicionGasto
        {
            get
            {
                return this.codPosicionGastoField;
            }
            set
            {
                this.codPosicionGastoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string nomPosicionGasto
        {
            get
            {
                return this.nomPosicionGastoField;
            }
            set
            {
                this.nomPosicionGastoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string codFuenteFinanciacion
        {
            get
            {
                return this.codFuenteFinanciacionField;
            }
            set
            {
                this.codFuenteFinanciacionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public enumFuenteFinanciacion nomFuenteFinanciacion
        {
            get
            {
                return this.nomFuenteFinanciacionField;
            }
            set
            {
                this.nomFuenteFinanciacionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string codRecursoPptal
        {
            get
            {
                return this.codRecursoPptalField;
            }
            set
            {
                this.codRecursoPptalField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string nomRecursoPptal
        {
            get
            {
                return this.nomRecursoPptalField;
            }
            set
            {
                this.nomRecursoPptalField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string codSituacionFondos
        {
            get
            {
                return this.codSituacionFondosField;
            }
            set
            {
                this.codSituacionFondosField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public enumSituacionFondos nomSituacionFondos
        {
            get
            {
                return this.nomSituacionFondosField;
            }
            set
            {
                this.nomSituacionFondosField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public decimal valorInicial
        {
            get
            {
                return this.valorInicialField;
            }
            set
            {
                this.valorInicialField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public decimal valorActual
        {
            get
            {
                return this.valorActualField;
            }
            set
            {
                this.valorActualField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public decimal valorOperacion
        {
            get
            {
                return this.valorOperacionField;
            }
            set
            {
                this.valorOperacionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public decimal saldoComp
        {
            get
            {
                return this.saldoCompField;
            }
            set
            {
                this.saldoCompField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public decimal valorBloqueado
        {
            get
            {
                return this.valorBloqueadoField;
            }
            set
            {
                this.valorBloqueadoField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.minhacienda.gov.co/mhcp/siifnacion/transferenciaxml/epg")]
    public enum enumFuenteFinanciacion
    {

        /// <remarks/>
        NACION,

        /// <remarks/>
        PROPIOS,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.minhacienda.gov.co/mhcp/siifnacion/transferenciaxml/epg")]
    public enum enumSituacionFondos
    {

        /// <remarks/>
        CSF,

        /// <remarks/>
        SSF,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.minhacienda.gov.co/mhcp/siifnacion/transferenciaxml/epg")]
    public partial class tipoItemCDP
    {

        private string codPCIField;

        private string nomPCIField;

        private System.Nullable<long> codSolicitudCDPField;

        private long codCDPField;

        private System.DateTime fechaRegistroField;

        private System.DateTime fechaCreacionField;

        private enumCodTipoCDP codTipoCDPField;

        private string estadoField;

        private string objetoField;

        private tipoItemAfectacion[] listaItemAfectacionField;

        private tipoItemEjecuCDP[] listaItemEjecuCDPField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string codPCI
        {
            get
            {
                return this.codPCIField;
            }
            set
            {
                this.codPCIField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string nomPCI
        {
            get
            {
                return this.nomPCIField;
            }
            set
            {
                this.nomPCIField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)]
        public System.Nullable<long> codSolicitudCDP
        {
            get
            {
                return this.codSolicitudCDPField;
            }
            set
            {
                this.codSolicitudCDPField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public long codCDP
        {
            get
            {
                return this.codCDPField;
            }
            set
            {
                this.codCDPField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "date")]
        public System.DateTime fechaRegistro
        {
            get
            {
                return this.fechaRegistroField;
            }
            set
            {
                this.fechaRegistroField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "date")]
        public System.DateTime fechaCreacion
        {
            get
            {
                return this.fechaCreacionField;
            }
            set
            {
                this.fechaCreacionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public enumCodTipoCDP codTipoCDP
        {
            get
            {
                return this.codTipoCDPField;
            }
            set
            {
                this.codTipoCDPField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string estado
        {
            get
            {
                return this.estadoField;
            }
            set
            {
                this.estadoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)]
        public string objeto
        {
            get
            {
                return this.objetoField;
            }
            set
            {
                this.objetoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)]
        [System.Xml.Serialization.XmlArrayItemAttribute("itemAfectacion", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
        public tipoItemAfectacion[] listaItemAfectacion
        {
            get
            {
                return this.listaItemAfectacionField;
            }
            set
            {
                this.listaItemAfectacionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)]
        [System.Xml.Serialization.XmlArrayItemAttribute("itemEjecuCDP", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
        public tipoItemEjecuCDP[] listaItemEjecuCDP
        {
            get
            {
                return this.listaItemEjecuCDPField;
            }
            set
            {
                this.listaItemEjecuCDPField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.minhacienda.gov.co/mhcp/siifnacion/transferenciaxml/epg")]
    public enum enumCodTipoCDP
    {

        /// <remarks/>
        GASTO,

        /// <remarks/>
        MODIFICACION,
    }

    #endregion XML CDP Clases


}