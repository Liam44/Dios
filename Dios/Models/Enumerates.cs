using System.ComponentModel;

namespace Dios.Models
{
    public enum Status
    {
        undefined = -1,
        [Description("Väntar på åtgärd")]
        waiting,
        [Description("Åtgärd påbörjad")]
        onTheWay,
        [Description("Åtgärd ej behövd")]
        irrelevant,
        [Description("Åtgärd utförd")]
        finished
    }

    public enum Priority
    {
        [Description("Odefinerat")]
        undefined = -1,
        [Description("Hög prioritet")]
        high,
        [Description("Mellanhög prioritet")]
        medium,
        [Description("Låg prioritet")]
        low
    }
}
