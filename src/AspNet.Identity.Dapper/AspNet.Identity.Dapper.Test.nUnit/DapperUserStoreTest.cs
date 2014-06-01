using System;
using System.Threading.Tasks;
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

	}
}

