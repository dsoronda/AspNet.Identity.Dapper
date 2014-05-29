using System;
using Microsoft.AspNet.Identity;

namespace AspNet.Identity.Dapper
{
	/// <summary>
	/// Dapper user.
	/// </summary>
	public class DapperUser<TKey> : IUser
	{
		public TKey UserId { get; set; }
		public string UserName { get; set; }
		public string PasswordHash { get; set; }

		public string Id {
			get {
				return UserId.ToString ();
			}
		}
	}
}

