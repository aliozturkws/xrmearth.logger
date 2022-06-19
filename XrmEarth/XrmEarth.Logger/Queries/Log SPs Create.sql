USE [DATABASE_NAME]

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

--Application Insert SP
CREATE PROCEDURE [dbo].[SP_Insert_Application] 
	@ID UNIQUEIDENTIFIER,
	@Name NVARCHAR(300),
	@Namespace NVARCHAR(300),
	@Description NVARCHAR(2000),
	@AssemblyGuid UNIQUEIDENTIFIER,
	@AssemblyVersion NCHAR(20),
	@CreatedAt DATETIME,
	@ValidID UNIQUEIDENTIFIER OUT
AS
BEGIN

	SET NOCOUNT ON;

	SELECT TOP 1 @ValidID=[ID] FROM [Application] WHERE [AssemblyGuid] = @AssemblyGuid AND [AssemblyVersion] = @AssemblyVersion;

	IF @ValidID IS NULL
	BEGIN
		SET @ValidID = @ID;
		INSERT INTO [Application]
				   ([ID]
				   ,[Name]
				   ,[Namespace]
				   ,[Description]
				   ,[AssemblyGuid]
				   ,[AssemblyVersion]
				   ,[CreatedAt])
			 VALUES
				   (@ID,
				   @Name,
				   @Namespace,
				   @Description,
				   @AssemblyGuid,
				   @AssemblyVersion,
				   @CreatedAt);
	END

END

GO


--Application Instance Insert SP
CREATE PROCEDURE [dbo].[SP_Insert_ApplicationInstance] 
	@ID UNIQUEIDENTIFIER,
	@ApplicationID UNIQUEIDENTIFIER,
	@Path NVARCHAR(MAX),
	@Parameters NVARCHAR(MAX),
	@StartAt DATETIME,
	@CreatedAt DATETIME
AS
BEGIN

	SET NOCOUNT ON;

	INSERT INTO [ApplicationInstance]
				([ID]
				,[ApplicationID]
				,[Path]
				,[StartAt]
				,[Parameters]
				,[CreatedAt])
			VALUES
				(@ID,
				@ApplicationID,
				@Path,
				@StartAt,
				@Parameters,
				@CreatedAt)
END

GO


--Application Log Insert SP
CREATE PROCEDURE [dbo].[SP_Insert_ApplicationLog] 
	@ID UNIQUEIDENTIFIER,
	@ApplicationInstanceID UNIQUEIDENTIFIER,
	@ParentCallerMember NVARCHAR(MAX),
	@CallerMember NVARCHAR(MAX),
	@Message NVARCHAR(MAX),
	@Type INT,
	@LogLevel INT,
	@Tag1 NVARCHAR(MAX),
	@Tag2 NVARCHAR(MAX),
	@CreatedAt DATETIME
AS
BEGIN

	SET NOCOUNT ON;

	INSERT INTO [ApplicationLog]
				([ID]
				,[ApplicationInstanceID]
				,[ParentCallerMember]
				,[CallerMember]
				,[Message]
				,[Type]
				,[LogLevel]
				,[Tag1]
				,[Tag2]
				,[CreatedAt])
			VALUES
				(@ID,
				@ApplicationInstanceID,
				@ParentCallerMember,
				@CallerMember,
				@Message,
				@Type,
				@LogLevel,
				@Tag1,
				@Tag2,
				@CreatedAt)

END

GO

--Application Instance Update SP
CREATE PROCEDURE [dbo].[SP_Update_ApplicationInstance] 
	@ID UNIQUEIDENTIFIER,
	@FinishAt DATETIME,
	@Result NVARCHAR(MAX),
	@Summary NVARCHAR(MAX)
AS
BEGIN

	SET NOCOUNT ON;

	UPDATE [ApplicationInstance]
	   SET [FinishAt] = @FinishAt
		  ,[Result] = @Result
		  ,[Summary] = @Summary
	 WHERE [ID] = @ID

END

GO