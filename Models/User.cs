using System;
using System.Collections.Generic;

namespace ToDo.Models;

public partial class User
{
    public uint Id { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Salt { get; set; } = null!;

    public virtual ICollection<Activity> Activity { get; set; } = new List<Activity>();
}
