using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ET_Vest.Models
{
    public class PrintedEditionProvider
    {
        [Key]
            public int PrintedEditionId { get; set; }
            public PrintedEdition PrintedEdition { get; set; }

            public int ProviderId { get; set; }
            public Provider Provider { get; set; }
    }

}



