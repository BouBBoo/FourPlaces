using Newtonsoft.Json;
using System;

namespace TD2.Items
{
	public class CommentItem
	{
		[JsonProperty("date")]
		public DateTime Date { get; set; }

		[JsonProperty("author")]
		public UserItem Author { get; set; }

		[JsonProperty("text")]
		public string Text { get; set; }
	}
}
