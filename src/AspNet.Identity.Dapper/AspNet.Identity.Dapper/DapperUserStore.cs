using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNet.Identity;

namespace AspNet.Identity.Dapper {
	/// <summary>
	///     Dapper user store.
	/// </summary>
	public class DapperUserStore<TKey> : IUserStore<DapperUser<TKey>>, IUserPasswordStore<DapperUser<TKey>>,
		IUserLoginStore<DapperUser<TKey>>, IUserSecurityStampStore<DapperUser<TKey>> {

		#region error messages

		private const string _emsg_ConnectioIsRequired = "Dbconnection is required!";
		private const string _emsg_UserIsRequired = "User is required!";
		private const string _emsg_UserNameIsRequired = "User name is required!";
		private const string _emsg_UserIdIsRequired = "User Id is required!";
		private const string _emsg_LoginIsRequired = "Login is required!";

		#endregion

		// TODO : move messages to resource file

		//fields
		private DbConnection _connection;

		/// <summary>
		///     Database connection for User Stopre
		/// </summary>
		/// <value>The connection.</value>
		public DbConnection Connection {
			get { return _connection; }
			set {
				if ( value == null )
					throw new ArgumentNullException( _emsg_ConnectioIsRequired );

				_connection = value;
			}
		}

		/// <summary>
		///     DI Constructor
		/// </summary>
		/// <param name="connection">DbConnectioin for User store.</param>
		public DapperUserStore( DbConnection connection ) {
			Connection = connection;
		}

		#region IDisposable implementation

		public void Dispose() {
			if ( _connection.State == ConnectionState.Open )
				_connection.Close();
			_connection = null;	// remove reference
		}

		#endregion

		#region IUserStore implementation

		public Task CreateAsync( DapperUser<TKey> user ) {
			if ( user == null )
				throw new ArgumentNullException( _emsg_UserIsRequired );

			return Task.Factory.StartNew( () => {
				if ( _connection.State != ConnectionState.Open )
					_connection.Open();
				_connection.Execute( "insert into Users(UserId, UserName, PasswordHash, SecurityStamp) values(@userId, @userName, @passwordHash, @SecurityStamp)",
					user );
			} );
		}

		public Task UpdateAsync( DapperUser<TKey> user ) {
			if ( user == null )
				throw new ArgumentNullException( _emsg_UserIsRequired );

			return Task.Factory.StartNew( () => {
				if ( _connection.State != ConnectionState.Open )
					_connection.Open();
				_connection.Execute( "update Users set UserName = @userName, PasswordHash = @passwordHash , SecurityStamp= @SecurityStamp where UserId = @userId",
					user );
			} );
		}

		public Task DeleteAsync( DapperUser<TKey> user ) {
			if ( user == null )
				throw new ArgumentNullException( _emsg_UserIsRequired );

			return Task.Factory.StartNew( () => {
				if ( _connection.State != ConnectionState.Open )
					_connection.Open();

				_connection.Execute( "delete from Users where UserId = @userId", new { user.UserId } );
			} );
		}

		public Task<DapperUser<TKey>> FindByIdAsync( string userId ) {
			if ( string.IsNullOrWhiteSpace( userId ) )
				throw new ArgumentNullException( _emsg_UserIdIsRequired );

			string query_string = "select * from Users where UserId = @userId";

			if ( _connection.State != ConnectionState.Open )
				_connection.Open();

			if ( typeof( TKey ).Equals( typeof( Guid ) ) ) {
				Guid parsegGuid;
				if ( !Guid.TryParse( userId, out parsegGuid ) )
					throw new ArgumentException( string.Format( "'{0}' is not a valid GUID.", new { userId } ) );

				return
					Task.Factory.StartNew(
						() => { return _connection.Query<DapperUser<TKey>>( query_string, new { userId = parsegGuid } ).SingleOrDefault(); } );
			}

			return
				Task.Factory.StartNew(
					() => { return _connection.Query<DapperUser<TKey>>( query_string, new { userId } ).SingleOrDefault(); } );
		}

		/// <summary>
		/// Custom implementation for TKey (if TKey is Guid, int etc....
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public Task<DapperUser<TKey>> FindByIdAsync( TKey userId ) {
			if ( userId == null ) throw new ArgumentNullException( _emsg_UserIdIsRequired );

			const string query_string = "select * from Users where UserId = @userId";

			if ( _connection.State != ConnectionState.Open )
				_connection.Open();

			if ( typeof( TKey ).Equals( typeof( String ) ) ) {
				return FindByIdAsync( userId.ToString() );
			}

			return
				Task.Factory.StartNew(
					() => { return _connection.Query<DapperUser<TKey>>( query_string, new { userId } ).SingleOrDefault(); } );
		}


		public Task<DapperUser<TKey>> FindByNameAsync( string userName ) {
			if ( string.IsNullOrWhiteSpace( userName ) )
				throw new ArgumentNullException( _emsg_UserNameIsRequired );

			return Task.Factory.StartNew( () => {
				if ( _connection.State != ConnectionState.Open )
					_connection.Open();
				return
					_connection.Query<DapperUser<TKey>>( "select * from Users where lower(UserName) = @userName",
						new { userName = userName.ToLower() } ).SingleOrDefault();
			} );
		}

		#endregion


		#region IUserPasswordStore<DapperUser<TKey>> implementation

		public Task SetPasswordHashAsync( DapperUser<TKey> user, string passwordHash ) {
			if ( user == null )
				throw new ArgumentNullException( _emsg_UserIsRequired );

			user.PasswordHash = passwordHash;

			return Task.FromResult( 0 );
		}

		public Task<string> GetPasswordHashAsync( DapperUser<TKey> user ) {
			if ( user == null )
				throw new ArgumentNullException( _emsg_UserIsRequired );

			return Task.FromResult( user.PasswordHash );
		}

		public Task<bool> HasPasswordAsync( DapperUser<TKey> user ) {
			return Task.FromResult( !string.IsNullOrEmpty( user.PasswordHash ) );
		}

		#endregion

		#region IUserLoginStore<DapperUser<TKey>> implementation

		public Task AddLoginAsync( DapperUser<TKey> user, UserLoginInfo login ) {
			if ( user == null )
				throw new ArgumentNullException( _emsg_UserIsRequired );

			if ( login == null )
				throw new ArgumentNullException( _emsg_LoginIsRequired );

			return Task.Factory.StartNew( () => {
				_connection.Execute(
					"insert into ExternalLogins(ExternalLoginId, UserId, LoginProvider, ProviderKey) values(@externalLoginId, @userId, @loginProvider, @providerKey)",
					new {
						externalLoginId = Guid.NewGuid(),
						userId = user.UserId,
						loginProvider = login.LoginProvider,
						providerKey = login.ProviderKey
					} );
			} );
		}

		public Task RemoveLoginAsync( DapperUser<TKey> user, UserLoginInfo login ) {
			if ( user == null )
				throw new ArgumentNullException( _emsg_UserIsRequired );

			if ( login == null )
				throw new ArgumentNullException( _emsg_LoginIsRequired );

			return Task.Factory.StartNew( () => {
				_connection.Execute(
					"delete from ExternalLogins where UserId = @userId and LoginProvider = @loginProvider and ProviderKey = @providerKey",
					new { user.UserId, login.LoginProvider, login.ProviderKey } );
			} );
		}

		public Task<IList<UserLoginInfo>> GetLoginsAsync( DapperUser<TKey> user ) {
			if ( user == null )
				throw new ArgumentNullException( _emsg_UserIsRequired );

			return
				Task.Factory.StartNew(
					() => {
						return
							( IList<UserLoginInfo> )
								_connection.Query<UserLoginInfo>(
									"select LoginProvider, ProviderKey from ExternalLogins where UserId = @userId", new { user.UserId } ).ToList();
					} );
		}

		public Task<DapperUser<TKey>> FindAsync( UserLoginInfo login ) {
			if ( login == null )
				throw new ArgumentNullException( _emsg_LoginIsRequired );

			return Task.Factory.StartNew( () => {
				return _connection.Query<DapperUser<TKey>>(
					"select u.* from Users u inner join ExternalLogins el on el.UserId = u.UserId where el.LoginProvider = @loginProvider and el.ProviderKey = @providerKey",
					login )
					.SingleOrDefault();
			} );
		}

		#endregion

		#region IUserSecurityStampStore<DapperUser<TKey>> implementation

		public Task SetSecurityStampAsync( DapperUser<TKey> user, string stamp ) {
			if ( user == null )
				throw new ArgumentNullException( _emsg_UserIsRequired );

			user.SecurityStamp = stamp;

			return Task.FromResult( 0 );
		}

		public Task<string> GetSecurityStampAsync( DapperUser<TKey> user ) {
			if ( user == null )
				throw new ArgumentNullException( _emsg_UserIsRequired );

			return Task.FromResult( user.SecurityStamp );
		}

		#endregion


	}
}