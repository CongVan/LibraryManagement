//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LibraryManagement.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Book
    {
        public int ID { get; set; }
        public string SKU { get; set; }
        public string Title { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<System.DateTime> PublishDate { get; set; }
        public string Author { get; set; }
        public string Image { get; set; }
        public Nullable<int> NumberOfPages { get; set; }
        public string Description { get; set; }
        public Nullable<int> CategoryID { get; set; }
        public Nullable<int> PublisherID { get; set; }
        public Nullable<int> LocationID { get; set; }
        public Nullable<System.DateTime> ImportDate { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<bool> Flag { get; set; }
    }
}
