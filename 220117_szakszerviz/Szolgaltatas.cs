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
    
    public partial class Szolgaltatas
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Szolgaltatas()
        {
            this.Szerviz = new HashSet<Szerviz>();
        }
    
        public int Id { get; set; }
        public string Nev { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Szerviz> Szerviz { get; set; }
    }
}
