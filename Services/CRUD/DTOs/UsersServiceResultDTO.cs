using System.Collections.Generic;

namespace Services.CRUD.DTOs
{
    public class UsersServiceResultDTO
    {
        public bool IsSucceed { get; set; }
        public List<string> ErrorMessages { get; set; }

        public UsersServiceResultDTO()
        {
            this.ErrorMessages = new List<string>();
        }
    }
}
