//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Exam_Management.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Approval
    {
        [Key]
        public int Id { get; set; }
        public Nullable<int> PaperID { get; set; }
        public Nullable<int> ApproverID { get; set; }
        public Nullable<System.DateTime> ApprovalDate { get; set; }
    
        public virtual User User { get; set; }
        public virtual Paper Paper { get; set; }
    }
}
