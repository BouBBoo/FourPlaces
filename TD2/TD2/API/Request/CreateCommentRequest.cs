using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TD2.API.Request
{
	public class CreateCommentRequest
	{
		[JsonProperty("text")]
		public string Text { get; set; }
	}
}
