using System;
using NUnit.Framework;
using System.Runtime.InteropServices;

namespace AspNet.Identity.Dapper.Test.nUnit {
	[TestFixture]
	public class DapperUserStoreTest {
		private static DapperUser<string> TestDapperUser {
			get {
				var ticks = DateTime.Now.Ticks.ToString();
				return new DapperUser<string>() {

					UserName = "_testUser_" + ticks,
					PasswordHash = "_test_",
					SecurityStamp = "_test_",
					UserId = ticks
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
			// var dapperUser = TestDapperUser;
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




		#endregion
	}
}

