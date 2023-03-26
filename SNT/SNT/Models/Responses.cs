using System;
using System.Collections.Generic;
using System.Text;

namespace SNT.Models
{
    class LoginResponse
    {
        
        public int userid { get; set; }
        public string token { get; set; }
        public int SntId { get; set; }
        public LoginResponse() { }
        
    }

    class UchastokResponse
    {
        public int SntId { get; set; }
    }
}
