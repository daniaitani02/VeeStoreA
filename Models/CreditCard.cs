//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VeeStoreA.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class CreditCard
    {
        public int Id { get; set; }
        public string CustomerEmail { get; set; }
        public int Number { get; set; }
        public string Name { get; set; }
        public string Expiry { get; set; }
        public int CVV { get; set; }
    
        public virtual Customer Customer { get; set; }
    }
}
