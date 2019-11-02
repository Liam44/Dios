namespace Dios.Models
{
    public sealed class AddressHostDTO
    {
        public AddressDTO Address { get; set; } = new AddressDTO();
        public UserDTO Host { get; set; } = new UserDTO();

        public AddressHostDTO(AddressDTO address, UserDTO host)
        {
            Address = address;
            Host = host;
        }
    }
}
