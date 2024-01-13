using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystemWeb.Models;

public partial class TblTask
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string? Title { get; set; }

    [Required]
    public string? Description { get; set; }

    [Required]
    [StringLength(50)]
    public string? Assignee { get; set; }

    [Required]
    public DateTime DueDate { get; set; }

    [Required]
    public string? EmailId { get; set; }

    [Required]
    [StringLength(20)]
    public string? StatusId { get; set; }
}
