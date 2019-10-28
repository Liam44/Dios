namespace Dios.Models
{
    public class ParameterDTO
    {
        public string UserId { get; set; } = string.Empty;
        public UserDTO User { get; set; } = new UserDTO();

        public FlatDTO Flat { get; set; } = new FlatDTO();

        public bool IsPhoneNumberVisible { get; set; }

        public bool IsEmailVisible { get; set; }

        public bool CanBeContacted { get; set; }

        public ParameterDTO(Flat flat = null,
                            User user = null,
                            Parameter parameter = null)
        {
            Flat = new FlatDTO(flat);
            User = new UserDTO(user);

            if (parameter == null)
            {
                IsPhoneNumberVisible = true;
                IsEmailVisible = true;
                CanBeContacted = true;

                if (user != null)
                {
                    UserId = user.Id;
                }
            }
            else
            {
                UserId = parameter.UserId;
                IsPhoneNumberVisible = parameter.IsPhoneNumberVisible;
                IsEmailVisible = parameter.IsEmailVisible;
                CanBeContacted = parameter.CanBeContacted;
            }
        }
    }
}