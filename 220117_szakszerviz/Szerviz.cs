//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace _220117_szakszerviz
{
    using System;
    using System.Collections.Generic;
    
    public partial class Szerviz
    {
        public int Id { get; set; }
        public string Rendszam { get; set; }
        public string Marka { get; set; }
        public string Tipus { get; set; }
        public System.DateTime Forgalomban { get; set; }
        public int FK_Szolgaltatas_Id { get; set; }
        public System.DateTime FelvetelDatuma { get; set; }
    
        public virtual Szolgaltatas Szolgaltatas { get; set; }
    }
}