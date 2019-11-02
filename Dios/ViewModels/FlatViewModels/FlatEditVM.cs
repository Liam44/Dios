namespace Dios.ViewModels
{
    public sealed class FlatEditVM
    {
        public int ID { get; set; }
        public FlatCreateVM Flat { get; set; }
        public string[] UsersId { get; set; }
    }
}
