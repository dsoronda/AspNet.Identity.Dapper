using System;
using NUnit.Framework;
using System.Runtime.InteropServices;

namespace AspNet.Identity.Dapper.Test.nUnit {
	[TestFixture]
	public class DapperUserStoreTest {
		private static DapperUser<int> TestDapperUser {
			get {
				return new DapperUser<int>() {
					UserName = "_testUser_" + DateTime.Now.Ticks,
					PasswordHash = "_test_",
					SecurityStamp = "_test_",
					UserId = -1
				};
			}
		}


		[Test]
		public void CanInjectConnectionString() {
			using ( var dbConnection = DbHelper.GetInMemoryDbConnection() ) {
				var store = new DapperUserStore<int>( dbConnection );
				Assert.IsNotNull( store.Connection );
				Assert.AreEqual( store.Connection.ConnectionString, dbConnection.ConnectionString, "Connection string should be equal!" );
			}
		}

		#region IUserStore implementation tests

		[Test]
		public void CreateAsyncTest() {
			var dapperUser = TestDapperUser;
			using ( var dbConnection = DbHelper.GetTestDatabaseConnection() ) {
				var store = new DapperUserStore<int>( dbConnection );
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

