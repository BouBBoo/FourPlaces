using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TD2.API.Request
{
    public class UpdatePasswordRequest
    {
        [JsonProperty("old_password")]
        public string OldPassword { get; set; }

        [JsonProperty("new_password")]
        public string NewPassword { get; set; }
    }
}
