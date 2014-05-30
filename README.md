AspNet.Identity.Dapper
======================

AspNet.Identity implemented using Dapper micro ORM

UserID is generic type so it can be Guid or int for example.

Implementation requires initialized DbConnection to database with with valid ConnectionString.


Inspired by article on http://blog.markjohnson.io/exorcising-entity-framework-from-asp-net-identity/

Required tables in database : Users and ExternalLogins

MS SQL Example :

CREATE TABLE [dbo].[Users]
(
    [UserId] UNIQUEIDENTIFIER/INT/VARCHAR(MAX) NOT NULL PRIMARY KEY, 
    [UserName] VARCHAR(MAX) NOT NULL, 
    [PasswordHash] VARCHAR(MAX) NULL, 
    [SecurityStamp] VARCHAR(MAX) NULL
)
 
CREATE TABLE [dbo].[ExternalLogins]
(
    [ExternalLoginId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [UserId] UNIQUEIDENTIFIER NOT NULL, 
    [LoginProvider] VARCHAR(MAX) NOT NULL, 
    [ProviderKey] VARCHAR(MAX) NOT NULL, 
    CONSTRAINT [FK_ExternalLogins_Users] FOREIGN KEY ([UserId]) REFERENCES [Users]([UserId])
)