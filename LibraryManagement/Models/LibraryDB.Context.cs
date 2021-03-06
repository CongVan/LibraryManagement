﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class LibraryManagementEntities : DbContext
    {
        public LibraryManagementEntities()
            : base("name=LibraryManagementEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Book> Books { get; set; }
        public virtual DbSet<BookBill> BookBills { get; set; }
        public virtual DbSet<BookBillDetail> BookBillDetails { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<LostBook> LostBooks { get; set; }
        public virtual DbSet<Publisher> Publishers { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Setting> Settings { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<Menu> Menus { get; set; }
        public virtual DbSet<Menu_Role> Menu_Role { get; set; }
    }
}
