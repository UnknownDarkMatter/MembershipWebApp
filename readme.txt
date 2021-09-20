
####################################################################################
#              INSTALLATION DU PROJET
####################################################################################

Créer une base de données MembershipWebApp

cmd : 
	C:\Windows\Microsoft.NET\Framework\v4.0.30319\aspnet_regsql.exe
		--> sélectionner la base de données MembershipWebApp du serveur (normalement c'est ".\SQLEXPRESS")

Dans SQL Server Management Studio, exécuter le script suivant
	USE [MembershipWebApp]
	GO

	INSERT INTO [dbo].[aspnet_Applications]([ApplicationName], [LoweredApplicationName]) VALUES ('MembershipWebApp' ,'membershipwebapp')
	GO

	DECLARE @AppId AS uniqueidentifier
	SELECT @AppId=ApplicationId FROM [dbo].[aspnet_Applications] WHERE ApplicationName='MembershipWebApp'
	INSERT INTO [dbo].[aspnet_Roles](ApplicationId, RoleName, LoweredRoleName) VALUES (@AppId, 'Reader', 'reader')
	GO


####################################################################################
#              EXECUTER LA SOLUTION
####################################################################################
s'il y a une erreur disant que le fichier  bin\roslyn\csc.exe est manquant : 
	Nettoyer la solution et rebuilder la solution plusieurs fois jusqu'à ce que Visual Studio crée le répertoire roslyn

####################################################################################
#              AJOUT UTILISATEUR
####################################################################################

ouvrir /Home/CreateUser et créer un user en choisissant un mot de passe de 6 caractères minimum

ajouter le role Reader en base avec le script SQL suivant en changeant bien le login
	USE [MembershipWebApp]
	GO

	DECLARE @Login AS VARCHAR(100)
	SET @Login = 'toto' -- renseigner ici le login créé

	DECLARE @RoleId AS uniqueidentifier
	SELECT @RoleId=[RoleId] FROM [dbo].[aspnet_Roles] WHERE RoleName='Reader'
	DECLARE @UserId AS uniqueidentifier
	SELECT @UserId=[UserId] FROM [dbo].[aspnet_Users] WHERE [UserName]=@Login

	INSERT INTO [dbo].[aspnet_UsersInRoles]([UserId], [RoleId]) VALUES (@UserId ,@RoleId)
	GO




