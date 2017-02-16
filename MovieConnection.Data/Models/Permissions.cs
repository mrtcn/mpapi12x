using System;

namespace MovieConnections.Data.Models
{
    [Flags]
    public enum Permissions {
        None = 0x0,
        View = 0x1,
        Create = 0x2,
        Edit = 0x4,
        Delete = 0x8,
        Export = 0x16
    }
}