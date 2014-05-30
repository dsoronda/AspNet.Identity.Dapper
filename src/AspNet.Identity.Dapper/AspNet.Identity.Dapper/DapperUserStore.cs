using System;
using Microsoft.AspNet.Identity;
using System.Data.Common;
using Dapper;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace AspNet.Identity.Dapper {
	/// <summary>
	/// Dapper user store.
	/// </summary>
	public class DapperUserStore <TKey> : IUserStore<DapperUser<TKey>> , IUserPasswordStore<DapperUser<TKey>> {
		// TODO : move messages to resource file
		//error messages
		const string _emsg_ConnectioIsRequired = "Dbconnection is required!";
		const string _emsg_UserIsRequired = "User is required!";
		const string _emsg_UserNameIsRequired = "User name is required!";
		const string _emsg_UserIdIsRequired = "User Id is required!";

		//fields
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
				if ( value == null )
					throw new ArgumentNullException ( _emsg_ConnectioIsRequired );

				_connection = value;
			}
		}

		/// <summary>
		/// DI Constructor
		/// </summary>
		/// <param name="connection">DbConnectioin for User store.</param>
		public DapperUserStore ( DbConnection connection ) {
			Connection = connection;
		}

		#region IUserStore implementation

		public System.Threading.Tasks.Task CreateAsync ( DapperUser<TKey> user ) {
			if ( user == null )
				throw new ArgumentNullException ( _emsg_UserIsRequired );

			return Task.Factory.StartNew ( ( ) => {
				if ( _connection.State != System.Data.ConnectionState.Open )
					_connection.Open ( );
				_connection.Execute ( "insert into Users(UserId, UserName, PasswordHash) values(@userId, @userName, @passwordHash)", user );
			} );
		}

		public System.Threading.Tasks.Task UpdateAsync ( DapperUser<TKey> user ) {
			if ( user == null )
				throw new ArgumentNullException ( _emsg_UserIsRequired );

			return Task.Factory.StartNew ( ( ) => {
				if ( _connection.State != System.Data.ConnectionState.Open )
					_connection.Open ( );
				_connection.Execute ( "update Users set UserName = @userName, PasswordHash = @passwordHash where UserId = @userId", user );
			} );
		}

		public System.Threading.Tasks.Task DeleteAsync ( DapperUser<TKey> user ) {
			if ( user == null )
				throw new ArgumentNullException ( _emsg_UserIsRequired );

			return Task.Factory.StartNew ( ( ) => {
				if ( _connection.State != System.Data.ConnectionState.Open )
					_connection.Open ( );

				_connection.Execute ( "delete from Users where UserId = @userId", new { user.UserId } );
			} );
		}

		public System.Threading.Tasks.Task<DapperUser<TKey>> FindByIdAsync ( string userId ) {
			if ( string.IsNullOrWhiteSpace ( userId ) )
				throw new ArgumentNullException ( _emsg_UserIdIsRequired );

			var query_string = "select * from Users where UserId = @userId";

			if ( _connection.State != System.Data.ConnectionState.Open )
				_connection.Open ( );

			if ( typeof ( TKey ).Equals ( typeof ( Guid ) ) ) {
				Guid parsegGuid;
				if ( !Guid.TryParse ( userId, out parsegGuid ) )
					throw new ArgumentException ( string.Format ( "'{0}' is not a valid GUID.", new { userId } ) );

				return Task.Factory.StartNew ( ( ) => {
					return _connection.Query<DapperUser<TKey>> ( query_string, new { userId = parsegGuid } ).SingleOrDefault ( );
				} );
			}

			return Task.Factory.StartNew ( ( ) => {
				return _connection.Query<DapperUser<TKey>> ( query_string, new { userId  } ).SingleOrDefault ( );
			} );

		}

		public System.Threading.Tasks.Task<DapperUser<TKey>> FindByNameAsync ( string userName ) {
			if ( string.IsNullOrWhiteSpace ( userName ) )
				throw new ArgumentNullException ( _emsg_UserNameIsRequired );

			return Task.Factory.StartNew ( ( ) => {
				if ( _connection.State != System.Data.ConnectionState.Open )
					_connection.Open ( );
				return _connection.Query<DapperUser<TKey>> ( "select * from Users where lower(UserName) = @userName", new { userName = userName.ToLower ( ) } ).SingleOrDefault ( );
			} );
		}

		#endregion

		#region IDisposable implementation

		public void Dispose ( ) {
			if ( _connection.State == System.Data.ConnectionState.Open )
				_connection.Close ( );
			// nothing to dispose yet
		}

		#endregion

		#region IUserPasswordStore<DapperUser<TKey>> implementation

		public Task SetPasswordHashAsync ( DapperUser<TKey> user, string passwordHash ) {
			if ( user == null )
				throw new ArgumentNullException ( _emsg_UserIsRequired );

			user.PasswordHash = passwordHash;

			return Task.FromResult ( 0 );
		}

		public Task<string> GetPasswordHashAsync ( DapperUser<TKey> user ) {
			if ( user == null )
				throw new ArgumentNullException ( _emsg_UserIsRequired );

			return Task.FromResult(user.PasswordHash);
		}

		public Task<bool> HasPasswordAsync ( DapperUser<TKey> user ) {
			return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
		}

		#endregion
	}
}

