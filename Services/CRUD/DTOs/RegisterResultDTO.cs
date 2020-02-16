using System.Collections.Generic;

namespace Services.CRUD.DTOs
{
    public class RegisterResultDTO
    {
        public bool IsSucceed { get; set; }
        public List<string> ErrorMessages { get; set; }

        public RegisterResultDTO()
        {
            this.ErrorMessages = new List<string>();
        }
    }
}
