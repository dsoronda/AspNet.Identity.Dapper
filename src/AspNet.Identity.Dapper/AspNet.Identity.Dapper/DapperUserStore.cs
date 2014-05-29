using System;
using Microsoft.AspNet.Identity;
using System.Data.Common;

namespace AspNet.Identity.Dapper
{
	/// <summary>
	/// Dapper user store.
	/// </summary>
	public class DapperUserStore <TKey> : IUserStore<DapperUser<TKey>>
	{
		// TODO : move messages to resource file
		//error messages
		const string dbconnectionIsRequired = "Dbconnection is required!";


		DbConnection _connection;
		/// <summary>
		/// Database connection for User Stopre
		/// </summary>
		/// <value>The connection.</value>
		public DbConnection Connection {
			get {
				return _connection;
			}
			set {
				if (value == null)
					throw new ArgumentNullException (dbconnectionIsRequired);
				_connection = value;
			}
		}
		/// <summary>
		/// DI Constructor
		/// </summary>
		/// <param name="connection">DbConnectioin for User store.</param>
		public DapperUserStore(DbConnection connection){
			Connection = connection;
		}

		#region IUserStore implementation
		public System.Threading.Tasks.Task CreateAsync (DapperUser<TKey> user)
		{
			throw new NotImplementedException ();
		}
		public System.Threading.Tasks.Task UpdateAsync (DapperUser<TKey> user)
		{
			throw new NotImplementedException ();
		}
		public System.Threading.Tasks.Task DeleteAsync (DapperUser<TKey> user)
		{
			throw new NotImplementedException ();
		}
		public System.Threading.Tasks.Task<DapperUser<TKey>> FindByIdAsync (string userId)
		{
			throw new NotImplementedException ();
		}
		public System.Threading.Tasks.Task<DapperUser<TKey>> FindByNameAsync (string userName)
		{
			throw new NotImplementedException ();
		}
		#endregion

		#region IDisposable implementation
		public void Dispose ()
		{
			// nothing to dispose yet
		}
		#endregion
	 
	}
}

