USE [master]
GO
CREATE DATABASE [NlsyLinks97]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'NlsyLinks97', FILENAME = N'D:\database\nlsy-links\nlsy_links_97.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'NlsyLinks97_log', FILENAME = N'D:\database\nlsy-links\nlsy_links_97_log.ldf' , SIZE = 23808KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [NlsyLinks97] SET COMPATIBILITY_LEVEL = 130
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [NlsyLinks97].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [NlsyLinks97] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [NlsyLinks97] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [NlsyLinks97] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [NlsyLinks97] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [NlsyLinks97] SET ARITHABORT OFF 
GO
ALTER DATABASE [NlsyLinks97] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [NlsyLinks97] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [NlsyLinks97] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [NlsyLinks97] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [NlsyLinks97] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [NlsyLinks97] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [NlsyLinks97] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [NlsyLinks97] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [NlsyLinks97] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [NlsyLinks97] SET  ENABLE_BROKER 
GO
ALTER DATABASE [NlsyLinks97] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [NlsyLinks97] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [NlsyLinks97] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [NlsyLinks97] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [NlsyLinks97] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [NlsyLinks97] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [NlsyLinks97] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [NlsyLinks97] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [NlsyLinks97] SET  MULTI_USER 
GO
ALTER DATABASE [NlsyLinks97] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [NlsyLinks97] SET DB_CHAINING OFF 
GO
ALTER DATABASE [NlsyLinks97] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [NlsyLinks97] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [NlsyLinks97] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [NlsyLinks97] SET QUERY_STORE = OFF
GO
USE [NlsyLinks97]
GO
ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET LEGACY_CARDINALITY_ESTIMATION = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET MAXDOP = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = ON;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET PARAMETER_SNIFFING = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET QUERY_OPTIMIZER_HOTFIXES = PRIMARY;
GO
USE [NlsyLinks97]
GO
CREATE SCHEMA [Archive]
GO
CREATE SCHEMA [Enum]
GO
CREATE SCHEMA [Extract]
GO
CREATE SCHEMA [Metadata]
GO
CREATE SCHEMA [Outcome]
GO
CREATE SCHEMA [Process]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Archive].[tblArchiveDescription](
	[ID] [smallint] IDENTITY(1,1) NOT NULL,
	[AlgorithmVersion] [smallint] NOT NULL,
	[Description] [text] NOT NULL,
	[Date] [date] NULL,
 CONSTRAINT [PK_tblArchiveDescription] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Archive].[tblRelatedValuesArchive](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[AlgorithmVersion] [smallint] NOT NULL,
	[SubjectTag_S1] [int] NOT NULL,
	[SubjectTag_S2] [int] NOT NULL,
	[MultipleBirthIfSameSex] [tinyint] NOT NULL,
	[IsMz] [tinyint] NOT NULL,
	[RosterAssignmentID] [tinyint] NULL,
	[RRoster] [float] NULL,
	[LastSurvey_S1] [smallint] NULL,
	[LastSurvey_S2] [smallint] NULL,
	[RImplicitPass1] [float] NULL,
	[RImplicit] [float] NULL,
	[RImplicitSubject] [float] NULL,
	[RImplicitMother] [float] NULL,
	[RExplicitOldestSibVersion] [float] NULL,
	[RExplicitYoungestSibVersion] [float] NULL,
	[RExplicitPass1] [float] NULL,
	[RExplicit] [float] NULL,
	[RPass1] [float] NULL,
	[R] [float] NULL,
	[RFull] [float] NULL,
	[RPeek] [float] NULL
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Enum].[tblLUExtractSource](
	[ID] [tinyint] NOT NULL,
	[Label] [char](20) NOT NULL,
	[Active] [varchar](5) NOT NULL,
	[Notes] [varchar](255) NULL,
 CONSTRAINT [PK_tblLUExtractSource] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Enum].[tblLUGender](
	[ID] [tinyint] NOT NULL,
	[Label] [char](15) NOT NULL,
	[Active] [varchar](5) NOT NULL,
	[Notes] [varchar](255) NULL,
 CONSTRAINT [PK_tblLUGender] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Enum].[tblLUMarkerEvidence](
	[ID] [tinyint] NOT NULL,
	[Label] [char](20) NOT NULL,
	[Active] [varchar](5) NOT NULL,
	[Notes] [varchar](255) NULL,
 CONSTRAINT [PK_tblLUMarkerEvidence] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Enum].[tblLUMarkerType](
	[ID] [tinyint] NOT NULL,
	[Label] [char](40) NOT NULL,
	[Explicit] [bit] NOT NULL,
	[Active] [varchar](5) NOT NULL,
	[Notes] [varchar](255) NULL,
 CONSTRAINT [PK_tblLUMarkerType] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Enum].[tblLUMultipleBirth](
	[ID] [tinyint] NOT NULL,
	[Label] [char](10) NOT NULL,
	[Active] [varchar](5) NOT NULL,
	[Notes] [varchar](255) NULL,
 CONSTRAINT [PK_tblLUMultipleBirth] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Enum].[tblLURaceCohort](
	[ID] [tinyint] NOT NULL,
	[Label] [varchar](15) NOT NULL,
	[Active] [varchar](5) NOT NULL,
	[Notes] [varchar](255) NULL,
 CONSTRAINT [PK_tblLURaceCohort] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Enum].[tblLURelationshipPath](
	[ID] [tinyint] NOT NULL,
	[Label] [char](20) NOT NULL,
	[Active] [varchar](5) NOT NULL,
	[Notes] [varchar](255) NULL,
 CONSTRAINT [PK_tblLURelationshipPath] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Enum].[tblLURoster](
	[ID] [smallint] NOT NULL,
	[Label] [varchar](255) NOT NULL,
	[Active] [varchar](5) NOT NULL,
	[Notes] [varchar](255) NULL,
 CONSTRAINT [PK_tblLURoster] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Enum].[tblLUTristate](
	[ID] [tinyint] NOT NULL,
	[Label] [char](10) NOT NULL,
	[Active] [varchar](5) NOT NULL,
	[Notes] [varchar](255) NULL,
 CONSTRAINT [PK_tblLUIsMZ] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Enum].[tblLUYesNo](
	[ID] [smallint] NOT NULL,
	[Label] [char](40) NOT NULL,
	[Active] [varchar](5) NOT NULL,
	[Notes] [varchar](255) NULL,
 CONSTRAINT [PK_[tblLUYesNoGen1] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Metadata].[tblItem](
	[ID] [smallint] NOT NULL,
	[Label] [varchar](50) NOT NULL,
	[MinValue] [int] NOT NULL,
	[MinNonnegative] [int] NULL,
	[MaxValue] [int] NOT NULL,
	[Active] [varchar](5) NOT NULL,
	[Notes] [varchar](255) NULL,
 CONSTRAINT [PK_tblLUItem] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Metadata].[tblRosterAssignment](
	[ID] [tinyint] NOT NULL,
	[ResponseLower] [smallint] NOT NULL,
	[ResponseUpper] [smallint] NOT NULL,
	[Freq] [smallint] NOT NULL,
	[Resolved] [bit] NOT NULL,
	[R] [float] NULL,
	[RBoundLower] [float] NOT NULL,
	[RBoundUpper] [float] NOT NULL,
	[SameGeneration] [tinyint] NOT NULL,
	[ShareBiodad] [tinyint] NOT NULL,
	[ShareBiomom] [tinyint] NOT NULL,
	[ShareBiograndparent] [tinyint] NOT NULL,
	[Inconsistent] [bit] NOT NULL,
	[Notes] [varchar](255) NULL,
	[ResponseLowerLabel] [varchar](50) NULL,
	[ResponseUpperLabel] [varchar](50) NULL,
 CONSTRAINT [PK_tblroster_assignment_gen1] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Metadata].[tblVariable](
	[VariableCode] [char](8) NOT NULL,
	[Item] [smallint] NOT NULL,
	[Generation] [tinyint] NOT NULL,
	[ExtractSource] [tinyint] NOT NULL,
	[SurveyYear] [smallint] NOT NULL,
	[LoopIndex] [tinyint] NOT NULL,
	[Translate] [bit] NOT NULL,
	[Active] [bit] NOT NULL,
	[Notes] [varchar](255) NULL,
 CONSTRAINT [PK_tblVariable_79] PRIMARY KEY CLUSTERED 
(
	[VariableCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Process].[tblOutcome](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SubjectTag] [int] NOT NULL,
	[Item] [smallint] NOT NULL,
	[SurveyYear] [smallint] NOT NULL,
	[Value] [int] NOT NULL
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Process].[tblRelatedStructure](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ExtendedID] [smallint] NOT NULL,
	[SubjectTag_S1] [int] NOT NULL,
	[SubjectTag_S2] [int] NOT NULL,
	[RelationshipPath] [tinyint] NOT NULL,
	[EverSharedHouse] [bit] NOT NULL,
 CONSTRAINT [PK_tblRelatednessStructure] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Process].[tblRelatedValues](
	[ID] [int] NOT NULL,
	[MultipleBirthIfSameSex] [tinyint] NOT NULL,
	[IsMz] [tinyint] NOT NULL,
	[LastSurvey_S1] [smallint] NULL,
	[LastSurvey_S2] [smallint] NULL,
	[ImplicitShareBiomomPass1] [tinyint] NULL,
	[ImplicitShareBiodadPass1] [tinyint] NULL,
	[ExplicitShareBiomomPass1] [tinyint] NULL,
	[ExplicitShareBiodadPass1] [tinyint] NULL,
	[ShareBiomomPass1] [tinyint] NULL,
	[ShareBiodadPass1] [tinyint] NULL,
	[RImplicitPass1] [float] NULL,
	[RImplicit] [float] NULL,
	[RImplicitSubject] [float] NULL,
	[RImplicitMother] [float] NULL,
	[RExplicitOlderSibVersion] [float] NULL,
	[RExplicitYoungerSibVersion] [float] NULL,
	[RExplicitPass1] [float] NULL,
	[RExplicit] [float] NULL,
	[RPass1] [float] NULL,
	[R] [float] NULL,
	[RFull] [float] NULL,
	[RPeek] [float] NULL
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Process].[tblResponse](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SubjectTag] [int] NOT NULL,
	[ExtendedID] [smallint] NOT NULL,
	[Generation] [tinyint] NOT NULL,
	[SurveySource] [tinyint] NOT NULL,
	[SurveyYear] [smallint] NOT NULL,
	[Item] [smallint] NOT NULL,
	[Value] [int] NOT NULL,
	[LoopIndex] [tinyint] NOT NULL
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Process].[tblRoster](
	[RelatedID] [int] NOT NULL,
	[RosterAssignmentID] [tinyint] NOT NULL,
	[ResponseLower] [smallint] NOT NULL,
	[ResponseUpper] [smallint] NOT NULL,
	[Resolved] [bit] NOT NULL,
	[R] [float] NULL,
	[RBoundLower] [float] NOT NULL,
	[RBoundUpper] [float] NOT NULL,
	[SameGeneration] [tinyint] NOT NULL,
	[ShareBiodad] [tinyint] NOT NULL,
	[ShareBiomom] [tinyint] NOT NULL,
	[ShareBiograndparent] [tinyint] NOT NULL,
	[Inconsistent] [bit] NOT NULL
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Process].[tblSubject](
	[SubjectTag] [int] NOT NULL,
	[ExtendedID] [smallint] NOT NULL,
	[SubjectID] [int] NOT NULL,
	[Gender] [tinyint] NOT NULL,
 CONSTRAINT [PK_Process.tblSubject] PRIMARY KEY CLUSTERED 
(
	[SubjectTag] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Process].[tblSubjectDetails](
	[SubjectTag] [int] NOT NULL,
	[RaceCohort] [tinyint] NOT NULL,
	[SiblingCountInNls] [tinyint] NOT NULL,
	[BirthOrderInNls] [tinyint] NOT NULL,
	[SimilarAgeCount] [tinyint] NOT NULL,
	[HasMzPossibly] [bit] NOT NULL,
	[KidCountBio] [tinyint] NULL,
	[KidCountInNls] [tinyint] NULL,
	[Mob] [date] NULL,
	[LastSurveyYearCompleted] [smallint] NULL,
	[AgeAtLastSurvey] [float] NULL,
	[IsDead] [bit] NOT NULL,
	[DeathDate] [date] NULL,
	[IsBiodadDead] [bit] NULL,
	[BiodadDeathDate] [date] NULL
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Process].[tblSurveyTime](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SubjectTag] [int] NOT NULL,
	[SurveySource] [tinyint] NOT NULL,
	[SurveyYear] [smallint] NOT NULL,
	[SurveyDate] [date] NULL,
	[AgeSelfReportYears] [float] NULL,
	[AgeCalculateYears] [float] NULL
) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_tblRelatedValuesArchive_Unique] ON [Archive].[tblRelatedValuesArchive]
(
	[AlgorithmVersion] ASC,
	[SubjectTag_S1] ASC,
	[SubjectTag_S2] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_tblVariable_79] ON [Metadata].[tblVariable]
(
	[VariableCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_tblOutcome_Unique] ON [Process].[tblOutcome]
(
	[Item] ASC,
	[SubjectTag] ASC,
	[SurveyYear] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_tblRelatedStructure_Unique] ON [Process].[tblRelatedStructure]
(
	[SubjectTag_S1] ASC,
	[SubjectTag_S2] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_tblRelatedValues] ON [Process].[tblRelatedValues]
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_tblSubject_Unique] ON [Process].[tblSubject]
(
	[SubjectID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_tblSubjectDetails] ON [Process].[tblSubjectDetails]
(
	[SubjectTag] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_tblSurveyTime_Unique] ON [Process].[tblSurveyTime]
(
	[SubjectTag] ASC,
	[SurveyYear] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE CLUSTERED COLUMNSTORE INDEX [ICC_tblRelatedValuesArchive] ON [Archive].[tblRelatedValuesArchive] WITH (DROP_EXISTING = OFF, COMPRESSION_DELAY = 0) ON [PRIMARY]
GO
CREATE CLUSTERED COLUMNSTORE INDEX [ICC_tblOutcome] ON [Process].[tblOutcome] WITH (DROP_EXISTING = OFF, COMPRESSION_DELAY = 0) ON [PRIMARY]
GO
CREATE CLUSTERED COLUMNSTORE INDEX [ICC_tblRelatedValues] ON [Process].[tblRelatedValues] WITH (DROP_EXISTING = OFF, COMPRESSION_DELAY = 0) ON [PRIMARY]
GO
CREATE CLUSTERED COLUMNSTORE INDEX [ICC_tblResponse] ON [Process].[tblResponse] WITH (DROP_EXISTING = OFF, COMPRESSION_DELAY = 0) ON [PRIMARY]
GO
CREATE CLUSTERED COLUMNSTORE INDEX [ICC_tblSubjectDetails] ON [Process].[tblSubjectDetails] WITH (DROP_EXISTING = OFF, COMPRESSION_DELAY = 0) ON [PRIMARY]
GO
CREATE CLUSTERED COLUMNSTORE INDEX [ICC_tblSurveyTime] ON [Process].[tblSurveyTime] WITH (DROP_EXISTING = OFF, COMPRESSION_DELAY = 0) ON [PRIMARY]
GO
ALTER TABLE [Metadata].[tblRosterAssignment]  WITH CHECK ADD  CONSTRAINT [FK_tblLURosterAssignment_tblLURoster1] FOREIGN KEY([ResponseUpper])
REFERENCES [Enum].[tblLURoster] ([ID])
GO
ALTER TABLE [Metadata].[tblRosterAssignment] CHECK CONSTRAINT [FK_tblLURosterAssignment_tblLURoster1]
GO
ALTER TABLE [Metadata].[tblRosterAssignment]  WITH CHECK ADD  CONSTRAINT [FK_tblLURosterAssignment_tblLUTristate] FOREIGN KEY([SameGeneration])
REFERENCES [Enum].[tblLUTristate] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [Metadata].[tblRosterAssignment] CHECK CONSTRAINT [FK_tblLURosterAssignment_tblLUTristate]
GO
ALTER TABLE [Metadata].[tblRosterAssignment]  WITH CHECK ADD  CONSTRAINT [FK_tblLURosterAssignment_tblLUTristate1] FOREIGN KEY([ShareBiodad])
REFERENCES [Enum].[tblLUTristate] ([ID])
GO
ALTER TABLE [Metadata].[tblRosterAssignment] CHECK CONSTRAINT [FK_tblLURosterAssignment_tblLUTristate1]
GO
ALTER TABLE [Metadata].[tblRosterAssignment]  WITH CHECK ADD  CONSTRAINT [FK_tblLURosterAssignment_tblLUTristate2] FOREIGN KEY([ShareBiomom])
REFERENCES [Enum].[tblLUTristate] ([ID])
GO
ALTER TABLE [Metadata].[tblRosterAssignment] CHECK CONSTRAINT [FK_tblLURosterAssignment_tblLUTristate2]
GO
ALTER TABLE [Metadata].[tblRosterAssignment]  WITH CHECK ADD  CONSTRAINT [FK_tblLURosterAssignment_tblLUTristate3] FOREIGN KEY([ShareBiograndparent])
REFERENCES [Enum].[tblLUTristate] ([ID])
GO
ALTER TABLE [Metadata].[tblRosterAssignment] CHECK CONSTRAINT [FK_tblLURosterAssignment_tblLUTristate3]
GO
ALTER TABLE [Metadata].[tblRosterAssignment]  WITH CHECK ADD  CONSTRAINT [FK_tblLURosterGen1Assignment_tblLURosterGen1] FOREIGN KEY([ResponseLower])
REFERENCES [Enum].[tblLURoster] ([ID])
GO
ALTER TABLE [Metadata].[tblRosterAssignment] CHECK CONSTRAINT [FK_tblLURosterGen1Assignment_tblLURosterGen1]
GO
ALTER TABLE [Metadata].[tblVariable]  WITH CHECK ADD  CONSTRAINT [FK_tblVariable_tblItem] FOREIGN KEY([Item])
REFERENCES [Metadata].[tblItem] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [Metadata].[tblVariable] CHECK CONSTRAINT [FK_tblVariable_tblItem]
GO
ALTER TABLE [Metadata].[tblVariable]  WITH CHECK ADD  CONSTRAINT [FK_tblVariable_tblLUExtractSource] FOREIGN KEY([ExtractSource])
REFERENCES [Enum].[tblLUExtractSource] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [Metadata].[tblVariable] CHECK CONSTRAINT [FK_tblVariable_tblLUExtractSource]
GO
ALTER TABLE [Process].[tblOutcome]  WITH CHECK ADD  CONSTRAINT [FK_tblOutcome_tblItem] FOREIGN KEY([Item])
REFERENCES [Metadata].[tblItem] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [Process].[tblOutcome] CHECK CONSTRAINT [FK_tblOutcome_tblItem]
GO
ALTER TABLE [Process].[tblOutcome]  WITH CHECK ADD  CONSTRAINT [FK_tblOutcome_tblSubject] FOREIGN KEY([SubjectTag])
REFERENCES [Process].[tblSubject] ([SubjectTag])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [Process].[tblOutcome] CHECK CONSTRAINT [FK_tblOutcome_tblSubject]
GO
ALTER TABLE [Process].[tblRelatedStructure]  WITH CHECK ADD  CONSTRAINT [FK_tblRelatedStructure_tblLURelationshipPath] FOREIGN KEY([RelationshipPath])
REFERENCES [Enum].[tblLURelationshipPath] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [Process].[tblRelatedStructure] CHECK CONSTRAINT [FK_tblRelatedStructure_tblLURelationshipPath]
GO
ALTER TABLE [Process].[tblRelatedStructure]  WITH NOCHECK ADD  CONSTRAINT [FK_tblRelatedStructure_tblSubject_Subject1] FOREIGN KEY([SubjectTag_S1])
REFERENCES [Process].[tblSubject] ([SubjectTag])
GO
ALTER TABLE [Process].[tblRelatedStructure] NOCHECK CONSTRAINT [FK_tblRelatedStructure_tblSubject_Subject1]
GO
ALTER TABLE [Process].[tblRelatedStructure]  WITH NOCHECK ADD  CONSTRAINT [FK_tblRelatedStructure_tblSubject_Subject2] FOREIGN KEY([SubjectTag_S2])
REFERENCES [Process].[tblSubject] ([SubjectTag])
GO
ALTER TABLE [Process].[tblRelatedStructure] NOCHECK CONSTRAINT [FK_tblRelatedStructure_tblSubject_Subject2]
GO
ALTER TABLE [Process].[tblRelatedValues]  WITH CHECK ADD  CONSTRAINT [FK_tblRelatedValues_tblLUMultipleBirth] FOREIGN KEY([MultipleBirthIfSameSex])
REFERENCES [Enum].[tblLUMultipleBirth] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [Process].[tblRelatedValues] CHECK CONSTRAINT [FK_tblRelatedValues_tblLUMultipleBirth]
GO
ALTER TABLE [Process].[tblRelatedValues]  WITH CHECK ADD  CONSTRAINT [FK_tblRelatedValues_tblLUTristate] FOREIGN KEY([IsMz])
REFERENCES [Enum].[tblLUTristate] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [Process].[tblRelatedValues] CHECK CONSTRAINT [FK_tblRelatedValues_tblLUTristate]
GO
ALTER TABLE [Process].[tblRelatedValues]  WITH CHECK ADD  CONSTRAINT [FK_tblRelatedValues_tblRelatedStructure] FOREIGN KEY([ID])
REFERENCES [Process].[tblRelatedStructure] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [Process].[tblRelatedValues] CHECK CONSTRAINT [FK_tblRelatedValues_tblRelatedStructure]
GO
ALTER TABLE [Process].[tblResponse]  WITH CHECK ADD  CONSTRAINT [FK_tblResponse_tblItem] FOREIGN KEY([Item])
REFERENCES [Metadata].[tblItem] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [Process].[tblResponse] CHECK CONSTRAINT [FK_tblResponse_tblItem]
GO
ALTER TABLE [Process].[tblResponse]  WITH CHECK ADD  CONSTRAINT [FK_tblResponse_tblSubject] FOREIGN KEY([SubjectTag])
REFERENCES [Process].[tblSubject] ([SubjectTag])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [Process].[tblResponse] CHECK CONSTRAINT [FK_tblResponse_tblSubject]
GO
ALTER TABLE [Process].[tblRoster]  WITH CHECK ADD  CONSTRAINT [FK_tblRoster_tblLURosterAssignment] FOREIGN KEY([RosterAssignmentID])
REFERENCES [Metadata].[tblRosterAssignment] ([ID])
GO
ALTER TABLE [Process].[tblRoster] CHECK CONSTRAINT [FK_tblRoster_tblLURosterAssignment]
GO
ALTER TABLE [Process].[tblRoster]  WITH CHECK ADD  CONSTRAINT [FK_tblRoster_tblLUTristate] FOREIGN KEY([SameGeneration])
REFERENCES [Enum].[tblLUTristate] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [Process].[tblRoster] CHECK CONSTRAINT [FK_tblRoster_tblLUTristate]
GO
ALTER TABLE [Process].[tblRoster]  WITH CHECK ADD  CONSTRAINT [FK_tblRoster_tblRelatedStructure] FOREIGN KEY([RelatedID])
REFERENCES [Process].[tblRelatedStructure] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [Process].[tblRoster] CHECK CONSTRAINT [FK_tblRoster_tblRelatedStructure]
GO
ALTER TABLE [Process].[tblSubject]  WITH NOCHECK ADD  CONSTRAINT [FK_tblSubject_tblLUGender] FOREIGN KEY([Gender])
REFERENCES [Enum].[tblLUGender] ([ID])
ON UPDATE CASCADE
GO
ALTER TABLE [Process].[tblSubject] CHECK CONSTRAINT [FK_tblSubject_tblLUGender]
GO
ALTER TABLE [Process].[tblSubjectDetails]  WITH CHECK ADD  CONSTRAINT [FK_tblSubjectDetails_tblLURaceCohort] FOREIGN KEY([RaceCohort])
REFERENCES [Enum].[tblLURaceCohort] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [Process].[tblSubjectDetails] CHECK CONSTRAINT [FK_tblSubjectDetails_tblLURaceCohort]
GO
ALTER TABLE [Process].[tblSurveyTime]  WITH CHECK ADD  CONSTRAINT [FK_tblSurveyTime_tblSubject] FOREIGN KEY([SubjectTag])
REFERENCES [Process].[tblSubject] ([SubjectTag])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [Process].[tblSurveyTime] CHECK CONSTRAINT [FK_tblSurveyTime_tblSubject]
GO
ALTER TABLE [Metadata].[tblRosterAssignment]  WITH CHECK ADD  CONSTRAINT [CK_tblLURosterAssignment_R] CHECK  (([R] IS NULL OR (0)<=[R] AND [R]<=(1)))
GO
ALTER TABLE [Metadata].[tblRosterAssignment] CHECK CONSTRAINT [CK_tblLURosterAssignment_R]
GO
ALTER TABLE [Metadata].[tblRosterAssignment]  WITH CHECK ADD  CONSTRAINT [CK_tblLURosterAssignment_RBoundLower] CHECK  (((0)<=[RBoundLower] AND [RBoundLower]<=(0.5)))
GO
ALTER TABLE [Metadata].[tblRosterAssignment] CHECK CONSTRAINT [CK_tblLURosterAssignment_RBoundLower]
GO
ALTER TABLE [Metadata].[tblRosterAssignment]  WITH CHECK ADD  CONSTRAINT [CK_tblLURosterAssignment_RBoundUpper] CHECK  (((0)<=[RBoundUpper] AND [RBoundUpper]<=(1)))
GO
ALTER TABLE [Metadata].[tblRosterAssignment] CHECK CONSTRAINT [CK_tblLURosterAssignment_RBoundUpper]
GO
ALTER TABLE [Metadata].[tblVariable]  WITH CHECK ADD  CONSTRAINT [CK_tblVariable_Generation] CHECK  (([Generation]=(2) OR [Generation]=(1)))
GO
ALTER TABLE [Metadata].[tblVariable] CHECK CONSTRAINT [CK_tblVariable_Generation]
GO
ALTER TABLE [Metadata].[tblVariable]  WITH CHECK ADD  CONSTRAINT [CK_tblVariable_SurveyYear] CHECK  (((0)<=[SurveyYear] AND [SurveyYear]<=(2030)))
GO
ALTER TABLE [Metadata].[tblVariable] CHECK CONSTRAINT [CK_tblVariable_SurveyYear]
GO
ALTER TABLE [Metadata].[tblVariable]  WITH CHECK ADD  CONSTRAINT [CK_tblVariable_VariableCodeLength] CHECK  ((len([VariableCode])=(8)))
GO
ALTER TABLE [Metadata].[tblVariable] CHECK CONSTRAINT [CK_tblVariable_VariableCodeLength]
GO
ALTER TABLE [Process].[tblResponse]  WITH CHECK ADD  CONSTRAINT [CK_tblResponse_SurveyYear] CHECK  (((0)<=[SurveyYear] AND [SurveyYear]<=(2016)))
GO
ALTER TABLE [Process].[tblResponse] CHECK CONSTRAINT [CK_tblResponse_SurveyYear]
GO
ALTER TABLE [Process].[tblRoster]  WITH CHECK ADD  CONSTRAINT [CK_tblRoster_R] CHECK  (((0)<=[R] AND [R]<=(1) OR [R] IS NULL))
GO
ALTER TABLE [Process].[tblRoster] CHECK CONSTRAINT [CK_tblRoster_R]
GO
ALTER TABLE [Process].[tblRoster]  WITH CHECK ADD  CONSTRAINT [CK_tblRoster_RBoundLower] CHECK  (((0)<=[RBoundLower] AND [RBoundLower]<=(1)))
GO
ALTER TABLE [Process].[tblRoster] CHECK CONSTRAINT [CK_tblRoster_RBoundLower]
GO
ALTER TABLE [Process].[tblRoster]  WITH CHECK ADD  CONSTRAINT [CK_tblRoster_RBoundUpper] CHECK  (((0)<=[RBoundUpper] AND [RBoundUpper]<=(1)))
GO
ALTER TABLE [Process].[tblRoster] CHECK CONSTRAINT [CK_tblRoster_RBoundUpper]
GO
ALTER TABLE [Process].[tblSubjectDetails]  WITH CHECK ADD  CONSTRAINT [CK_tblSubjectDetails_AgeAtLastSurvey] CHECK  (([AgeAtLastSurvey]>=(0)))
GO
ALTER TABLE [Process].[tblSubjectDetails] CHECK CONSTRAINT [CK_tblSubjectDetails_AgeAtLastSurvey]
GO
ALTER TABLE [Process].[tblSubjectDetails]  WITH CHECK ADD  CONSTRAINT [CK_tblSubjectDetails_BirthOrderInNls] CHECK  (((1)<=[BirthOrderInNls] AND [BirthOrderInNls]<=(14)))
GO
ALTER TABLE [Process].[tblSubjectDetails] CHECK CONSTRAINT [CK_tblSubjectDetails_BirthOrderInNls]
GO
ALTER TABLE [Process].[tblSubjectDetails]  WITH CHECK ADD  CONSTRAINT [CK_tblSubjectDetails_KidCountBio] CHECK  (((0)<=[KidCountBio] AND [KidCountBio]<=(11)))
GO
ALTER TABLE [Process].[tblSubjectDetails] CHECK CONSTRAINT [CK_tblSubjectDetails_KidCountBio]
GO
ALTER TABLE [Process].[tblSubjectDetails]  WITH CHECK ADD  CONSTRAINT [CK_tblSubjectDetails_LastSurveyYear] CHECK  (((1979)<=[LastSurveyYearCompleted] AND [LastSurveyYearCompleted]<=(2016)))
GO
ALTER TABLE [Process].[tblSubjectDetails] CHECK CONSTRAINT [CK_tblSubjectDetails_LastSurveyYear]
GO
ALTER TABLE [Process].[tblSubjectDetails]  WITH CHECK ADD  CONSTRAINT [CK_tblSubjectDetails_Mob] CHECK  (('1/1/1955'<=[Mob]))
GO
ALTER TABLE [Process].[tblSubjectDetails] CHECK CONSTRAINT [CK_tblSubjectDetails_Mob]
GO
ALTER TABLE [Process].[tblSubjectDetails]  WITH CHECK ADD  CONSTRAINT [CK_tblSubjectDetails_SiblingCountInNls] CHECK  (((0)<=[SiblingCountInNls] AND [SiblingCountInNls]<=(13)))
GO
ALTER TABLE [Process].[tblSubjectDetails] CHECK CONSTRAINT [CK_tblSubjectDetails_SiblingCountInNls]
GO
ALTER TABLE [Process].[tblSurveyTime]  WITH CHECK ADD  CONSTRAINT [CK_tblSurveyTime_AgeCalculateYears] CHECK  (((0)<=[AgeCalculateYears] AND [AgeCalculateYears]<=(70)))
GO
ALTER TABLE [Process].[tblSurveyTime] CHECK CONSTRAINT [CK_tblSurveyTime_AgeCalculateYears]
GO
ALTER TABLE [Process].[tblSurveyTime]  WITH CHECK ADD  CONSTRAINT [CK_tblSurveyTime_AgeSelfReportYears] CHECK  (((0)<=[AgeSelfReportYears] AND [AgeSelfReportYears]<=(70)))
GO
ALTER TABLE [Process].[tblSurveyTime] CHECK CONSTRAINT [CK_tblSurveyTime_AgeSelfReportYears]
GO
ALTER TABLE [Process].[tblSurveyTime]  WITH CHECK ADD  CONSTRAINT [CK_tblSurveyTime_SurveyYear] CHECK  (((0)<=[SurveyYear] AND [SurveyYear]<=(2016)))
GO
ALTER TABLE [Process].[tblSurveyTime] CHECK CONSTRAINT [CK_tblSurveyTime_SurveyYear]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Name
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [Process].[prcArchiveMaxVersion] 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
SELECT Max([AlgorithmVersion]) as MaxVersion
  FROM Archive.[tblRelatedValuesArchive]

END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Process].[prcResponseRetrieveSubset]
AS
BEGIN
	SET NOCOUNT ON;-- SET NOCOUNT ON added to prevent extra result sets from interfering with SELECT statements.
SELECT     ID, SubjectTag, ExtendedID, Generation, SurveySource, SurveyYear, Item, Value, LoopIndex
FROM         Process.tblResponse
WHERE Item in (0) --For RelatedValues

OR Item in (13,14,15,16,17,20, 21,22,100)                                                                  --For SurveyTime: Birthday Values, SelfReported Age at Interview, and the SubjectID
OR Item in (1,2)                                                                                         --For Roster
OR Item in (13, 14, 306, 326, 340)                                                                       --For ParentsOfGen1Retro
OR Item in (300, 301, 302, 305, 307, 308,  310, 311, 320, 321, 322, 325, 327, 330, 331, 340)             --For ParentsOfGen1Current 309, 329,
OR Item in ( 49, 81,82,83,84,85,86,87,88,89,90, 91, 92 )                                                 --For BabyDaddy
OR Item in (121, 122, 123, 124, 125)                                                                     --For Gen2CFather
OR Item in (11, 13,14,15, 48, 49, 60, 64, 82, 86, 87, 88, 103)                                           --For SubjectDetails
OR Item in (1,2,4,5,6)                                                                                   --For MarkerGen1
OR Item in (9,10)                                                                                        --For MarkerGen2
OR Item in (                                                                                             --Outcomes
	200,201,203,                                                                                           --Gen1HeightInches, Gen1WeightPounds, Gen1AfqtScaled3Decimals
	500,501,502,503,                                                                                       --Gen2HeightInchesTotal, Gen2HeightFeetOnly, Gen2HeightInchesRemainder, Gen2HeightInchesTotalMotherSupplement
	504,512,513,                                                                                           --Gen2WeightPoundsYA, Gen2PiatMathPercentile, Gen2PiatMathStandard
	122                                                                                                    --Gen2CFatherAlive
  )                

END
GO
USE [master]
GO
ALTER DATABASE [NlsyLinks97] SET  READ_WRITE 
GO
