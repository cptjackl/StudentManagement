using System;
using System.Collections.Generic;

namespace Lab4.DataAccess;

public partial class Role
{
    public int Id { get; set; }

    public string? Role1 { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
