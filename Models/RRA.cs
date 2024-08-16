using System;
using System.Collections.Generic;

namespace AdresApi.Models
{
    public class RData : DefaultResponse
    {
        public RRRAData resultData { get; set; }
    }

    public class RFilterData : DefaultResponse
    {
        public List<RRRAData> resultData { get; set; }
    }

    public class DefaultResponse
    {
        public bool isValid { get; set; } 
        public int numberValue { get; set; }
        public string stringValue { get; set; }
    }

    public class RRAData
    {
        public double presupuesto { get; set; }
        public string unidad { get; set; }
        public string tipo { get; set; }
        public double cantidad { get; set; }
        public double valorUnitario { get; set; }
        public double valorTotal { get; set; }
        public DateTime fecha { get; set; }
        public string proveedor { get; set; }
        public string documentacion { get; set; }
    }

    public class RRRAData : RRAData
    {
        public int id { get; set; }
    }

    public class FilterData
    {
        public string unidad { get; set; }
        public string tipo { get; set; }
        public DateTime fecha { get; set; }
        public string proveedor { get; set; }
    }


}
