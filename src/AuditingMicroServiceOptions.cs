using System.ComponentModel.DataAnnotations;

namespace Firepuma.Auditing.Client
{
    public class AuditingMicroServiceOptions
    {
        [Required]
        public string ServiceUrl { get; set; }
    }
}
