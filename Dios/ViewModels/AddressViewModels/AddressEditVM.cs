namespace Dios.ViewModels
{
    public sealed class AddressEditVM
    {
        public int ID { get; set; }
        public AddressCreateVM Address { get; set; }
        public string[] HostsId { get; set; }
    }
}
