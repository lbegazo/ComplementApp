﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// Este código fuente fue generado automáticamente por xsd, Versión=4.8.3928.0.
// 
namespace ComplementApp.API.Dtos.Serializacion.OrdenPago {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.minhacienda.gov.co/mhcp/siifnacion/transferenciaxml/pag")]
    [System.Xml.Serialization.XmlRootAttribute("pagoTercero", Namespace="http://www.minhacienda.gov.co/mhcp/siifnacion/transferenciaxml/pag", IsNullable=false)]
    public partial class tipoPagoTercero {
        
        private tipoListaFiltroConsulta listaFiltroConsultaField;
        
        private tipoItemPagoTercero[] listaItemPagoTerceroField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public tipoListaFiltroConsulta listaFiltroConsulta {
            get {
                return this.listaFiltroConsultaField;
            }
            set {
                this.listaFiltroConsultaField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        [System.Xml.Serialization.XmlArrayItemAttribute("itemPagoTercero", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
        public tipoItemPagoTercero[] listaItemPagoTercero {
            get {
                return this.listaItemPagoTerceroField;
            }
            set {
                this.listaItemPagoTerceroField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.minhacienda.gov.co/mhcp/siifnacion/transferenciaxml/pag")]
    public partial class tipoListaFiltroConsulta {
        
        private string codEntidadField;
        
        private string codPCIConsultaField;
        
        private string areaField;
        
        private string numDocIdentidadField;
        
        private string numDiaAtrasoField;
        
        private System.DateTime fechaInicioField;
        
        private System.DateTime fechaFinField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string codEntidad {
            get {
                return this.codEntidadField;
            }
            set {
                this.codEntidadField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string codPCIConsulta {
            get {
                return this.codPCIConsultaField;
            }
            set {
                this.codPCIConsultaField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public string area {
            get {
                return this.areaField;
            }
            set {
                this.areaField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string numDocIdentidad {
            get {
                return this.numDocIdentidadField;
            }
            set {
                this.numDocIdentidadField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType="integer", IsNullable=true)]
        public string numDiaAtraso {
            get {
                return this.numDiaAtrasoField;
            }
            set {
                this.numDiaAtrasoField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType="date")]
        public System.DateTime fechaInicio {
            get {
                return this.fechaInicioField;
            }
            set {
                this.fechaInicioField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType="date")]
        public System.DateTime fechaFin {
            get {
                return this.fechaFinField;
            }
            set {
                this.fechaFinField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.minhacienda.gov.co/mhcp/siifnacion/transferenciaxml/pag")]
    public partial class tipoCuentaBancariaBasica {
        
        private System.Nullable<enumTipoCuentaBancaria> codTipoCuentaField;
        
        private string numCuentaField;
        
        private string nomEntidadField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public System.Nullable<enumTipoCuentaBancaria> codTipoCuenta {
            get {
                return this.codTipoCuentaField;
            }
            set {
                this.codTipoCuentaField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public string numCuenta {
            get {
                return this.numCuentaField;
            }
            set {
                this.numCuentaField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public string nomEntidad {
            get {
                return this.nomEntidadField;
            }
            set {
                this.nomEntidadField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.minhacienda.gov.co/mhcp/siifnacion/transferenciaxml/pag")]
    public enum enumTipoCuentaBancaria {
        
        /// <remarks/>
        AHR,
        
        /// <remarks/>
        CRR,
        
        /// <remarks/>
        DEL,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.minhacienda.gov.co/mhcp/siifnacion/transferenciaxml/pag")]
    public partial class tipoTerceroBasico {
        
        private string nomTerceroField;
        
        private string codNaturalezaField;
        
        private string nomNaturalezaField;
        
        private string codTipoDocumentoField;
        
        private string nomTipoDocumentoField;
        
        private string numDocIdentidadField;
        
        private string digVerificacionField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string nomTercero {
            get {
                return this.nomTerceroField;
            }
            set {
                this.nomTerceroField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string codNaturaleza {
            get {
                return this.codNaturalezaField;
            }
            set {
                this.codNaturalezaField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string nomNaturaleza {
            get {
                return this.nomNaturalezaField;
            }
            set {
                this.nomNaturalezaField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string codTipoDocumento {
            get {
                return this.codTipoDocumentoField;
            }
            set {
                this.codTipoDocumentoField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string nomTipoDocumento {
            get {
                return this.nomTipoDocumentoField;
            }
            set {
                this.nomTipoDocumentoField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string numDocIdentidad {
            get {
                return this.numDocIdentidadField;
            }
            set {
                this.numDocIdentidadField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public string digVerificacion {
            get {
                return this.digVerificacionField;
            }
            set {
                this.digVerificacionField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.minhacienda.gov.co/mhcp/siifnacion/transferenciaxml/pag")]
    public partial class tipoItemPagoTercero {
        
        private string codPCIField;
        
        private string nomPCIField;
        
        private string nitPCIField;
        
        private string digVerificaField;
        
        private long codOrdenPagoField;
        
        private decimal valorNetoField;
        
        private System.DateTime fechaPagoField;
        
        private string codTesoreriaField;
        
        private enumTipoMedioPago codMedioPagoField;
        
        private string nomMedioPagoField;
        
        private tipoTerceroBasico terceroBeneficiarioPagoField;
        
        private tipoCuentaBancariaBasica cuentaBancariaField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string codPCI {
            get {
                return this.codPCIField;
            }
            set {
                this.codPCIField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string nomPCI {
            get {
                return this.nomPCIField;
            }
            set {
                this.nomPCIField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string nitPCI {
            get {
                return this.nitPCIField;
            }
            set {
                this.nitPCIField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string digVerifica {
            get {
                return this.digVerificaField;
            }
            set {
                this.digVerificaField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public long codOrdenPago {
            get {
                return this.codOrdenPagoField;
            }
            set {
                this.codOrdenPagoField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public decimal valorNeto {
            get {
                return this.valorNetoField;
            }
            set {
                this.valorNetoField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType="date")]
        public System.DateTime fechaPago {
            get {
                return this.fechaPagoField;
            }
            set {
                this.fechaPagoField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string codTesoreria {
            get {
                return this.codTesoreriaField;
            }
            set {
                this.codTesoreriaField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public enumTipoMedioPago codMedioPago {
            get {
                return this.codMedioPagoField;
            }
            set {
                this.codMedioPagoField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string nomMedioPago {
            get {
                return this.nomMedioPagoField;
            }
            set {
                this.nomMedioPagoField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public tipoTerceroBasico terceroBeneficiarioPago {
            get {
                return this.terceroBeneficiarioPagoField;
            }
            set {
                this.terceroBeneficiarioPagoField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public tipoCuentaBancariaBasica cuentaBancaria {
            get {
                return this.cuentaBancariaField;
            }
            set {
                this.cuentaBancariaField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.minhacienda.gov.co/mhcp/siifnacion/transferenciaxml/pag")]
    public enum enumTipoMedioPago {
        
        /// <remarks/>
        AC,
        
        /// <remarks/>
        CH,
        
        /// <remarks/>
        GR,
        
        /// <remarks/>
        TT,
    }
}
