﻿using System;
using System.Collections.Generic;

namespace Meta.DataTier.Models;

public partial class Account
{
    public Guid Id { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string? Image { get; set; }

    public string? Gender { get; set; }

    public string? Role { get; set; }

    public string? FullName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Status { get; set; }

    public string? Email { get; set; }

    public int? Point { get; set; }

    public Guid? RankId { get; set; }

    public string? Address { get; set; }

    public virtual ICollection<CustomerRequest> CustomerRequests { get; set; } = new List<CustomerRequest>();

    public virtual ICollection<Pet> Pets { get; set; } = new List<Pet>();

    public virtual Rank? Rank { get; set; }

    public virtual ICollection<TaskManager> TaskManagers { get; set; } = new List<TaskManager>();
}
