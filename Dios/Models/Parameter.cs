namespace Dios.Models
{
    public sealed class Parameter
    {
        public string UserId { get; set; }

        public int FlatID { get; set; }

        public bool IsPhoneNumberVisible { get; set; }

        public bool IsEmailVisible { get; set; }

        public bool CanBeContacted { get; set; }
    }
}
