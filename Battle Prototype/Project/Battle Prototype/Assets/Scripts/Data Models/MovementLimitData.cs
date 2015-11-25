namespace Scripts.Data_Models
{
    public class MovementLimitData
    {
        float Endpoint { get; set; }
        Direction Side { get; set; }
        float ActivatesAt { get; set; }
        bool HasBeenActivated { get; set; }
    }
}
