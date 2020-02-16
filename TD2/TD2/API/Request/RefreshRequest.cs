using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TD2.API.Request
{
    public class RefreshRequest
    {
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
    }
