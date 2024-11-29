using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonAssets.EventDtos
{
    /// <summary>
    /// Nuværende state for en notifikation. Dette er til at gemme i state store.
    /// </summary>
    public class NotificationState
    {
        public string Status { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
