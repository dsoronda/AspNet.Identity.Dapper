using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using NUnit.Framework;
using System.Runtime.InteropServices;

namespace AspNet.Identity.Dapper.Test.nUnit {
	[TestFixture]
	public class DapperUserStoreTest {
		private Random randeomGen = new Random( Guid.NewGuid().GetHashCode() );

		private DapperUser<string> TestDapperUser {
			get {
				var random = randeomGen.Next().ToString();
				return new DapperUser<string>() {
					UserName = "_testUser_" + random,
					PasswordHash = "_test_",
					SecurityStamp = "_test_",
					UserId = random
				};
			}
		}


		[Test]
		public void CanInjectConnectionString() {
			using ( var dbConnection = DbHelper.GetInMemoryDbConnection() ) {
				var store = new DapperUserStore<string>( dbConnection );
				Assert.IsNotNull( store.Connection );
				Assert.AreEqual( store.Connection.ConnectionString, dbConnection.ConnectionString, "Connection string should be equal!" );
			}
		}

		[Test]
		[ExpectedException( typeof( ArgumentNullException ) )]
		public void GetExceptionOnNullDbConnection() {
			var store = new DapperUserStore<int>( null );
		}

		#region IUserStore implementation tests
		[Test]
		public void CanFindById() {
			// user is created thru SQL script so we can test GetById 
			using ( var dbConnection = DbHelper.GetTestDatabaseConnection() ) {
				DbHelper.UserInsert( dbConnection );

				var store = new DapperUserStore<string>( dbConnection );

				var getTestUser = store.FindByIdAsync( "_test_" ).Result;

				Assert.IsNotNull( getTestUser );
				//Assert.AreEqual( getTestUser.UserId, "_test_" );
				Assert.AreEqual( getTestUser.Id, "_test_" );
				Assert.AreEqual( getTestUser.UserName, "_test_" );
				Assert.AreEqual( getTestUser.PasswordHash, "_test_" );
				Assert.AreEqual( getTestUser.SecurityStamp, "_test_" );
			}
		}

		[Test]
		public void CanCreateAsync() {
			var dapperUser = TestDapperUser;
			using ( var dbConnection = DbHelper.GetTestDatabaseConnection() ) {
				var store = new DapperUserStore<string>( dbConnection );
				store.CreateAsync( dapperUser ).Wait();

				var getTestUser = store.FindByIdAsync( dapperUser.UserId ).Result;

				Assert.IsNotNull( getTestUser );
				Assert.AreEqual( getTestUser.UserId, dapperUser.UserId );
				Assert.AreEqual( getTestUser.Id, dapperUser.Id );
				Assert.AreEqual( getTestUser.UserName, dapperUser.UserName );
				Assert.AreEqual( getTestUser.PasswordHash, dapperUser.PasswordHash );
				Assert.AreEqual( getTestUser.SecurityStamp, dapperUser.SecurityStamp );

			}
		}

		[Test]
		public void CanUpdateAsync() {
			var dapperUser = TestDapperUser;
			using ( var dbConnection = DbHelper.GetTestDatabaseConnection() ) {
				var store = new DapperUserStore<string>( dbConnection );
				store.CreateAsync( dapperUser ).Wait();

				dapperUser.UserName = "a";
				dapperUser.PasswordHash = "a";
				dapperUser.SecurityStamp = "a";

				store.UpdateAsync( dapperUser ).Wait();

				var updatedUser = store.FindByIdAsync( dapperUser.UserId ).Result;
				Assert.AreEqual( updatedUser.UserId, dapperUser.UserId );
				Assert.AreEqual( updatedUser.UserName, dapperUser.UserName );
				Assert.AreEqual( updatedUser.PasswordHash, dapperUser.PasswordHash );
				Assert.AreEqual( updatedUser.SecurityStamp, dapperUser.SecurityStamp );
			}
		}

		[Test]
		public void CanDeleteAsync() {
			var dapperUser = TestDapperUser;
			using ( var dbConnection = DbHelper.GetTestDatabaseConnection() ) {
				var store = new DapperUserStore<string>( dbConnection );
				store.CreateAsync( dapperUser ).Wait();

				var getTestUser = store.FindByIdAsync( dapperUser.UserId ).Result;

				Assert.IsNotNull( getTestUser );
				Assert.AreEqual( getTestUser.UserId, dapperUser.UserId );
				Assert.AreEqual( getTestUser.Id, dapperUser.Id );
				Assert.AreEqual( getTestUser.UserName, dapperUser.UserName );
				Assert.AreEqual( getTestUser.PasswordHash, dapperUser.PasswordHash );
				Assert.AreEqual( getTestUser.SecurityStamp, dapperUser.SecurityStamp );

				store.DeleteAsync( getTestUser ).Wait();
				var deletedUser = store.FindByIdAsync( dapperUser.UserId ).Result;
				Assert.IsNull( deletedUser );
			}
		}


		#endregion

		#region SetPasswordHashAsync implementation tests

		[Test]
		public void CanGetPasswordHashAsync() {
			var dapperUser = TestDapperUser;
			using ( var dbConnection = DbHelper.GetTestDatabaseConnection() ) {
				var store = new DapperUserStore<string>( dbConnection );
				store.CreateAsync( dapperUser ).Wait();
				var passwordHash = store.GetPasswordHashAsync( dapperUser ).Result;
				Assert.IsNotNullOrEmpty( passwordHash );
				Assert.AreEqual( passwordHash, dapperUser.PasswordHash );
			}
		}

		[Test]
		public void CanSetPasswordHashAsync() {
			var dapperUser = TestDapperUser;
			using ( var dbConnection = DbHelper.GetTestDatabaseConnection() ) {
				var store = new DapperUserStore<string>( dbConnection );
				store.CreateAsync( dapperUser ).Wait();
				const string passwordHash = "x";
				store.SetPasswordHashAsync( dapperUser, passwordHash ).Wait();

				Assert.AreEqual( passwordHash, dapperUser.PasswordHash );
			}
		}

		[Test]
		public void HasPasswordAsyncTest() {
			var dapperUser = TestDapperUser;
			using ( var dbConnection = DbHelper.GetTestDatabaseConnection() ) {
				var store = new DapperUserStore<string>( dbConnection );
				store.CreateAsync( dapperUser ).Wait();

				Assert.IsTrue( store.HasPasswordAsync( dapperUser ).Result );
			}
		}


		#endregion

		#region IUserLoginStore<DapperUser<TKey>> implementation tests

		[Test]
		public void CanAddAndGetLoginAsync() {
			var dapperUser = TestDapperUser;
			using ( var dbConnection = DbHelper.GetTestDatabaseConnection() ) {
				var store = new DapperUserStore<string>( dbConnection );
				store.CreateAsync( dapperUser ).Wait();

				const string testValue = "_test_";
				var userLoginInfo = new UserLoginInfo( testValue, testValue );
				store.AddLoginAsync( dapperUser, userLoginInfo ).Wait();

				IList<UserLoginInfo> list = store.GetLoginsAsync( dapperUser ).Result.ToList();
				Assert.IsNotNull( list );
				Assert.IsTrue( list.Count() == 1 );
				Assert.IsNotNull( list.First() );

				var testLoginInfo = list.First();
				Assert.AreEqual( testLoginInfo.LoginProvider, testValue );
				Assert.AreEqual( testLoginInfo.ProviderKey, testValue );
			}
		}

		[Test]
		public void CanRemoveLoginAsync() {
			var dapperUser = TestDapperUser;
			using ( var dbConnection = DbHelper.GetTestDatabaseConnection() ) {
				var store = new DapperUserStore<string>( dbConnection );
				store.CreateAsync( dapperUser ).Wait();

				const string testValue = "_test_";
				var userLoginInfo = new UserLoginInfo( testValue, testValue );
				store.AddLoginAsync( dapperUser, userLoginInfo ).Wait();

				IList<UserLoginInfo> list = store.GetLoginsAsync( dapperUser ).Result.ToList();
				Assert.IsNotNull( list );
				Assert.IsTrue( list.Count() == 1 );
				Assert.IsNotNull( list.First() );

				store.RemoveLoginAsync( dapperUser, userLoginInfo ).Wait();

				IList<UserLoginInfo> listDeleted = store.GetLoginsAsync( dapperUser ).Result.ToList();
				Assert.IsNotNull( listDeleted );
				Assert.IsTrue( listDeleted.Count == 0 );
			}
		}

		[Test]
		public void CanFindAsync() {
			var dapperUser = TestDapperUser;
			using ( var dbConnection = DbHelper.GetTestDatabaseConnection() ) {
				var store = new DapperUserStore<string>( dbConnection );
				store.CreateAsync( dapperUser ).Wait();

				const string testValue = "_test_";
				var userLoginInfo = new UserLoginInfo( testValue, testValue );
				store.AddLoginAsync( dapperUser, userLoginInfo ).Wait();

				DapperUser<string> user = store.FindAsync( userLoginInfo ).Result;
				Assert.IsNotNull( user );
				Assert.AreEqual( user.UserId, dapperUser.UserId );

			}
		}

		[Test]
		public void CanNotFindNonExistent() {
			var dapperUser = TestDapperUser;
			using ( var dbConnection = DbHelper.GetTestDatabaseConnection() ) {
				var store = new DapperUserStore<string>( dbConnection );
				store.CreateAsync( dapperUser ).Wait();

				const string testValue = "_test_";
				var userLoginInfo = new UserLoginInfo( testValue, testValue );
				store.AddLoginAsync( dapperUser, userLoginInfo ).Wait();

				DapperUser<string> user = store.FindAsync( new UserLoginInfo( "a", "a" ) ).Result;
				Assert.IsNull( user );

			}
		}
		#endregion

	}
}

