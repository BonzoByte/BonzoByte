USE [master]
GO
/****** Object:  Database [BonzoByte]    Script Date: 19.7.2025. 11:43:54 ******/
CREATE DATABASE [BonzoByte]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'BonzoByte', FILENAME = N'D:\Development\MSSQL\MSSQL12.MSSQLSERVER\MSSQL\DATA\BonzoByte.mdf' , SIZE = 21504KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'BonzoByte_log', FILENAME = N'D:\Development\MSSQL\MSSQL12.MSSQLSERVER\MSSQL\DATA\BonzoByte_log.ldf' , SIZE = 57664KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [BonzoByte] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [BonzoByte].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [BonzoByte] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [BonzoByte] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [BonzoByte] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [BonzoByte] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [BonzoByte] SET ARITHABORT OFF 
GO
ALTER DATABASE [BonzoByte] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [BonzoByte] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [BonzoByte] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [BonzoByte] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [BonzoByte] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [BonzoByte] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [BonzoByte] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [BonzoByte] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [BonzoByte] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [BonzoByte] SET  DISABLE_BROKER 
GO
ALTER DATABASE [BonzoByte] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [BonzoByte] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [BonzoByte] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [BonzoByte] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [BonzoByte] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [BonzoByte] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [BonzoByte] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [BonzoByte] SET RECOVERY FULL 
GO
ALTER DATABASE [BonzoByte] SET  MULTI_USER 
GO
ALTER DATABASE [BonzoByte] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [BonzoByte] SET DB_CHAINING OFF 
GO
ALTER DATABASE [BonzoByte] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [BonzoByte] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [BonzoByte] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'BonzoByte', N'ON'
GO
USE [BonzoByte]
GO
/****** Object:  Table [dbo].[Bookie]    Script Date: 19.7.2025. 11:43:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Bookie](
	[BookieId] [tinyint] NOT NULL,
	[BookieName] [nvarchar](25) NOT NULL,
	[BookieUrl] [varchar](100) NULL,
 CONSTRAINT [PK_Bookie] PRIMARY KEY CLUSTERED 
(
	[BookieId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Country]    Script Date: 19.7.2025. 11:43:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Country](
	[CountryTPId] [tinyint] NOT NULL,
	[CountryISO3] [nchar](3) NULL,
	[CountryISO2] [nchar](2) NULL,
	[CountryFull] [nvarchar](100) NULL,
 CONSTRAINT [PK_Country] PRIMARY KEY CLUSTERED 
(
	[CountryTPId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Match]    Script Date: 19.7.2025. 11:43:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Match](
	[MatchTPId] [int] NOT NULL,
	[TournamentEventTPId] [int] NULL,
	[DateTime] [datetime] NULL,
	[Player1TPId] [int] NULL,
	[Player2TPId] [int] NULL,
	[Player1Seed] [varchar](10) NULL,
	[Player2Seed] [varchar](10) NULL,
	[Result] [char](2) NULL,
	[ResultDetails] [char](40) NULL,
	[Player1Odds] [decimal](5, 2) NULL,
	[Player2Odds] [decimal](5, 2) NULL,
	[Player1Percentage] [decimal](5, 2) NULL,
	[Player2Percentage] [decimal](5, 2) NULL,
	[SurfaceId] [tinyint] NULL,
	[RoundId] [tinyint] NULL,
	[Player1TrueSkillMeanM] [float] NULL,
	[Player1TrueSkillStandardDeviationM] [float] NULL,
	[Player2TrueSkillMeanM] [float] NULL,
	[Player2TrueSkillStandardDeviationM] [float] NULL,
	[Player1TrueSkillMeanOldM] [float] NULL,
	[Player1TrueSkillStandardDeviationOldM] [float] NULL,
	[Player2TrueSkillMeanOldM] [float] NULL,
	[Player2TrueSkillStandardDeviationOldM] [float] NULL,
	[WinProbabilityPlayer1M] [float] NULL,
	[Player1TrueSkillMeanSM] [float] NULL,
	[Player1TrueSkillStandardDeviationSM] [float] NULL,
	[Player2TrueSkillMeanSM] [float] NULL,
	[Player2TrueSkillStandardDeviationSM] [float] NULL,
	[Player1TrueSkillMeanOldSM] [float] NULL,
	[Player1TrueSkillStandardDeviationOldSM] [float] NULL,
	[Player2TrueSkillMeanOldSM] [float] NULL,
	[Player2TrueSkillStandardDeviationOldSM] [float] NULL,
	[WinProbabilityPlayer1SM] [float] NULL,
	[Player1TrueSkillMeanGSM] [float] NULL,
	[Player1TrueSkillStandardDeviationGSM] [float] NULL,
	[Player2TrueSkillMeanGSM] [float] NULL,
	[Player2TrueSkillStandardDeviationGSM] [float] NULL,
	[Player1TrueSkillMeanOldGSM] [float] NULL,
	[Player1TrueSkillStandardDeviationOldGSM] [float] NULL,
	[Player2TrueSkillMeanOldGSM] [float] NULL,
	[Player2TrueSkillStandardDeviationOldGSM] [float] NULL,
	[WinProbabilityPlayer1GSM] [float] NULL,
	[Player1TrueSkillMeanMS1] [float] NULL,
	[Player1TrueSkillStandardDeviationMS1] [float] NULL,
	[Player2TrueSkillMeanMS1] [float] NULL,
	[Player2TrueSkillStandardDeviationMS1] [float] NULL,
	[Player1TrueSkillMeanOldMS1] [float] NULL,
	[Player1TrueSkillStandardDeviationOldMS1] [float] NULL,
	[Player2TrueSkillMeanOldMS1] [float] NULL,
	[Player2TrueSkillStandardDeviationOldMS1] [float] NULL,
	[WinProbabilityPlayer1MS1] [float] NULL,
	[Player1TrueSkillMeanSMS1] [float] NULL,
	[Player1TrueSkillStandardDeviationSMS1] [float] NULL,
	[Player2TrueSkillMeanSMS1] [float] NULL,
	[Player2TrueSkillStandardDeviationSMS1] [float] NULL,
	[Player1TrueSkillMeanOldSMS1] [float] NULL,
	[Player1TrueSkillStandardDeviationOldSMS1] [float] NULL,
	[Player2TrueSkillMeanOldSMS1] [float] NULL,
	[Player2TrueSkillStandardDeviationOldSMS1] [float] NULL,
	[WinProbabilityPlayer1SMS1] [float] NULL,
	[Player1TrueSkillMeanGSMS1] [float] NULL,
	[Player1TrueSkillStandardDeviationGSMS1] [float] NULL,
	[Player2TrueSkillMeanGSMS1] [float] NULL,
	[Player2TrueSkillStandardDeviationGSMS1] [float] NULL,
	[Player1TrueSkillMeanOldGSMS1] [float] NULL,
	[Player1TrueSkillStandardDeviationOldGSMS1] [float] NULL,
	[Player2TrueSkillMeanOldGSMS1] [float] NULL,
	[Player2TrueSkillStandardDeviationOldGSMS1] [float] NULL,
	[WinProbabilityPlayer1GSMS1] [float] NULL,
	[Player1TrueSkillMeanMS2] [float] NULL,
	[Player1TrueSkillStandardDeviationMS2] [float] NULL,
	[Player2TrueSkillMeanMS2] [float] NULL,
	[Player2TrueSkillStandardDeviationMS2] [float] NULL,
	[Player1TrueSkillMeanOldMS2] [float] NULL,
	[Player1TrueSkillStandardDeviationOldMS2] [float] NULL,
	[Player2TrueSkillMeanOldMS2] [float] NULL,
	[Player2TrueSkillStandardDeviationOldMS2] [float] NULL,
	[WinProbabilityPlayer1MS2] [float] NULL,
	[Player1TrueSkillMeanSMS2] [float] NULL,
	[Player1TrueSkillStandardDeviationSMS2] [float] NULL,
	[Player2TrueSkillMeanSMS2] [float] NULL,
	[Player2TrueSkillStandardDeviationSMS2] [float] NULL,
	[Player1TrueSkillMeanOldSMS2] [float] NULL,
	[Player1TrueSkillStandardDeviationOldSMS2] [float] NULL,
	[Player2TrueSkillMeanOldSMS2] [float] NULL,
	[Player2TrueSkillStandardDeviationOldSMS2] [float] NULL,
	[WinProbabilityPlayer1SMS2] [float] NULL,
	[Player1TrueSkillMeanGSMS2] [float] NULL,
	[Player1TrueSkillStandardDeviationGSMS2] [float] NULL,
	[Player2TrueSkillMeanGSMS2] [float] NULL,
	[Player2TrueSkillStandardDeviationGSMS2] [float] NULL,
	[Player1TrueSkillMeanOldGSMS2] [float] NULL,
	[Player1TrueSkillStandardDeviationOldGSMS2] [float] NULL,
	[Player2TrueSkillMeanOldGSMS2] [float] NULL,
	[Player2TrueSkillStandardDeviationOldGSMS2] [float] NULL,
	[WinProbabilityPlayer1GSMS2] [float] NULL,
	[Player1TrueSkillMeanMS3] [float] NULL,
	[Player1TrueSkillStandardDeviationMS3] [float] NULL,
	[Player2TrueSkillMeanMS3] [float] NULL,
	[Player2TrueSkillStandardDeviationMS3] [float] NULL,
	[Player1TrueSkillMeanOldMS3] [float] NULL,
	[Player1TrueSkillStandardDeviationOldMS3] [float] NULL,
	[Player2TrueSkillMeanOldMS3] [float] NULL,
	[Player2TrueSkillStandardDeviationOldMS3] [float] NULL,
	[WinProbabilityPlayer1MS3] [float] NULL,
	[Player1TrueSkillMeanSMS3] [float] NULL,
	[Player1TrueSkillStandardDeviationSMS3] [float] NULL,
	[Player2TrueSkillMeanSMS3] [float] NULL,
	[Player2TrueSkillStandardDeviationSMS3] [float] NULL,
	[Player1TrueSkillMeanOldSMS3] [float] NULL,
	[Player1TrueSkillStandardDeviationOldSMS3] [float] NULL,
	[Player2TrueSkillMeanOldSMS3] [float] NULL,
	[Player2TrueSkillStandardDeviationOldSMS3] [float] NULL,
	[WinProbabilityPlayer1SMS3] [float] NULL,
	[Player1TrueSkillMeanGSMS3] [float] NULL,
	[Player1TrueSkillStandardDeviationGSMS3] [float] NULL,
	[Player2TrueSkillMeanGSMS3] [float] NULL,
	[Player2TrueSkillStandardDeviationGSMS3] [float] NULL,
	[Player1TrueSkillMeanOldGSMS3] [float] NULL,
	[Player1TrueSkillStandardDeviationOldGSMS3] [float] NULL,
	[Player2TrueSkillMeanOldGSMS3] [float] NULL,
	[Player2TrueSkillStandardDeviationOldGSMS3] [float] NULL,
	[WinProbabilityPlayer1GSMS3] [float] NULL,
	[Player1TrueSkillMeanMS4] [float] NULL,
	[Player1TrueSkillStandardDeviationMS4] [float] NULL,
	[Player2TrueSkillMeanMS4] [float] NULL,
	[Player2TrueSkillStandardDeviationMS4] [float] NULL,
	[Player1TrueSkillMeanOldMS4] [float] NULL,
	[Player1TrueSkillStandardDeviationOldMS4] [float] NULL,
	[Player2TrueSkillMeanOldMS4] [float] NULL,
	[Player2TrueSkillStandardDeviationOldMS4] [float] NULL,
	[WinProbabilityPlayer1MS4] [float] NULL,
	[Player1TrueSkillMeanSMS4] [float] NULL,
	[Player1TrueSkillStandardDeviationSMS4] [float] NULL,
	[Player2TrueSkillMeanSMS4] [float] NULL,
	[Player2TrueSkillStandardDeviationSMS4] [float] NULL,
	[Player1TrueSkillMeanOldSMS4] [float] NULL,
	[Player1TrueSkillStandardDeviationOldSMS4] [float] NULL,
	[Player2TrueSkillMeanOldSMS4] [float] NULL,
	[Player2TrueSkillStandardDeviationOldSMS4] [float] NULL,
	[WinProbabilityPlayer1SMS4] [float] NULL,
	[Player1TrueSkillMeanGSMS4] [float] NULL,
	[Player1TrueSkillStandardDeviationGSMS4] [float] NULL,
	[Player2TrueSkillMeanGSMS4] [float] NULL,
	[Player2TrueSkillStandardDeviationGSMS4] [float] NULL,
	[Player1TrueSkillMeanOldGSMS4] [float] NULL,
	[Player1TrueSkillStandardDeviationOldGSMS4] [float] NULL,
	[Player2TrueSkillMeanOldGSMS4] [float] NULL,
	[Player2TrueSkillStandardDeviationOldGSMS4] [float] NULL,
	[WinProbabilityPlayer1GSMS4] [float] NULL,
	[Player1WinsTotal] [int] NULL,
	[Player1LossesTotal] [int] NULL,
	[Player1WinsLastYear] [int] NULL,
	[Player1LossesLastYear] [int] NULL,
	[Player1WinsLastMonth] [int] NULL,
	[Player1LossesLastMonth] [int] NULL,
	[Player1WinsLastWeek] [int] NULL,
	[Player1LossesLastWeek] [int] NULL,
	[Player2WinsTotal] [int] NULL,
	[Player2LossesTotal] [int] NULL,
	[Player2WinsLastYear] [int] NULL,
	[Player2LossesLastYear] [int] NULL,
	[Player2WinsLastMonth] [int] NULL,
	[Player2LossesLastMonth] [int] NULL,
	[Player2WinsLastWeek] [int] NULL,
	[Player2LossesLastWeek] [int] NULL,
	[Player1WinsTotalS1] [int] NULL,
	[Player1LossesTotalS1] [int] NULL,
	[Player1WinsLastYearS1] [int] NULL,
	[Player1LossesLastYearS1] [int] NULL,
	[Player1WinsLastMonthS1] [int] NULL,
	[Player1LossesLastMonthS1] [int] NULL,
	[Player1WinsLastWeekS1] [int] NULL,
	[Player1LossesLastWeekS1] [int] NULL,
	[Player2WinsTotalS1] [int] NULL,
	[Player2LossesTotalS1] [int] NULL,
	[Player2WinsLastYearS1] [int] NULL,
	[Player2LossesLastYearS1] [int] NULL,
	[Player2WinsLastMonthS1] [int] NULL,
	[Player2LossesLastMonthS1] [int] NULL,
	[Player2WinsLastWeekS1] [int] NULL,
	[Player2LossesLastWeekS1] [int] NULL,
	[Player1WinsTotalS2] [int] NULL,
	[Player1LossesTotalS2] [int] NULL,
	[Player1WinsLastYearS2] [int] NULL,
	[Player1LossesLastYearS2] [int] NULL,
	[Player1WinsLastMonthS2] [int] NULL,
	[Player1LossesLastMonthS2] [int] NULL,
	[Player1WinsLastWeekS2] [int] NULL,
	[Player1LossesLastWeekS2] [int] NULL,
	[Player2WinsTotalS2] [int] NULL,
	[Player2LossesTotalS2] [int] NULL,
	[Player2WinsLastYearS2] [int] NULL,
	[Player2LossesLastYearS2] [int] NULL,
	[Player2WinsLastMonthS2] [int] NULL,
	[Player2LossesLastMonthS2] [int] NULL,
	[Player2WinsLastWeekS2] [int] NULL,
	[Player2LossesLastWeekS2] [int] NULL,
	[Player1WinsTotalS3] [int] NULL,
	[Player1LossesTotalS3] [int] NULL,
	[Player1WinsLastYearS3] [int] NULL,
	[Player1LossesLastYearS3] [int] NULL,
	[Player1WinsLastMonthS3] [int] NULL,
	[Player1LossesLastMonthS3] [int] NULL,
	[Player1WinsLastWeekS3] [int] NULL,
	[Player1LossesLastWeekS3] [int] NULL,
	[Player2WinsTotalS3] [int] NULL,
	[Player2LossesTotalS3] [int] NULL,
	[Player2WinsLastYearS3] [int] NULL,
	[Player2LossesLastYearS3] [int] NULL,
	[Player2WinsLastMonthS3] [int] NULL,
	[Player2LossesLastMonthS3] [int] NULL,
	[Player2WinsLastWeekS3] [int] NULL,
	[Player2LossesLastWeekS3] [int] NULL,
	[Player1WinsTotalS4] [int] NULL,
	[Player1LossesTotalS4] [int] NULL,
	[Player1WinsLastYearS4] [int] NULL,
	[Player1LossesLastYearS4] [int] NULL,
	[Player1WinsLastMonthS4] [int] NULL,
	[Player1LossesLastMonthS4] [int] NULL,
	[Player1WinsLastWeekS4] [int] NULL,
	[Player1LossesLastWeekS4] [int] NULL,
	[Player2WinsTotalS4] [int] NULL,
	[Player2LossesTotalS4] [int] NULL,
	[Player2WinsLastYearS4] [int] NULL,
	[Player2LossesLastYearS4] [int] NULL,
	[Player2WinsLastMonthS4] [int] NULL,
	[Player2LossesLastMonthS4] [int] NULL,
	[Player2WinsLastWeekS4] [int] NULL,
	[Player2LossesLastWeekS4] [int] NULL,
	[Player1WinsSetsTotal] [int] NULL,
	[Player1LossesSetsTotal] [int] NULL,
	[Player1WinsSetsLastYear] [int] NULL,
	[Player1LossesSetsLastYear] [int] NULL,
	[Player1WinsSetsLastMonth] [int] NULL,
	[Player1LossesSetsLastMonth] [int] NULL,
	[Player1WinsSetsLastWeek] [int] NULL,
	[Player1LossesSetsLastWeek] [int] NULL,
	[Player2WinsSetsTotal] [int] NULL,
	[Player2LossesSetsTotal] [int] NULL,
	[Player2WinsSetsLastYear] [int] NULL,
	[Player2LossesSetsLastYear] [int] NULL,
	[Player2WinsSetsLastMonth] [int] NULL,
	[Player2LossesSetsLastMonth] [int] NULL,
	[Player2WinsSetsLastWeek] [int] NULL,
	[Player2LossesSetsLastWeek] [int] NULL,
	[Player1WinsSetsTotalS1] [int] NULL,
	[Player1LossesSetsTotalS1] [int] NULL,
	[Player1WinsSetsLastYearS1] [int] NULL,
	[Player1LossesSetsLastYearS1] [int] NULL,
	[Player1WinsSetsLastMonthS1] [int] NULL,
	[Player1LossesSetsLastMonthS1] [int] NULL,
	[Player1WinsSetsLastWeekS1] [int] NULL,
	[Player1LossesSetsLastWeekS1] [int] NULL,
	[Player2WinsSetsTotalS1] [int] NULL,
	[Player2LossesSetsTotalS1] [int] NULL,
	[Player2WinsSetsLastYearS1] [int] NULL,
	[Player2LossesSetsLastYearS1] [int] NULL,
	[Player2WinsSetsLastMonthS1] [int] NULL,
	[Player2LossesSetsLastMonthS1] [int] NULL,
	[Player2WinsSetsLastWeekS1] [int] NULL,
	[Player2LossesSetsLastWeekS1] [int] NULL,
	[Player1WinsSetsTotalS2] [int] NULL,
	[Player1LossesSetsTotalS2] [int] NULL,
	[Player1WinsSetsLastYearS2] [int] NULL,
	[Player1LossesSetsLastYearS2] [int] NULL,
	[Player1WinsSetsLastMonthS2] [int] NULL,
	[Player1LossesSetsLastMonthS2] [int] NULL,
	[Player1WinsSetsLastWeekS2] [int] NULL,
	[Player1LossesSetsLastWeekS2] [int] NULL,
	[Player2WinsSetsTotalS2] [int] NULL,
	[Player2LossesSetsTotalS2] [int] NULL,
	[Player2WinsSetsLastYearS2] [int] NULL,
	[Player2LossesSetsLastYearS2] [int] NULL,
	[Player2WinsSetsLastMonthS2] [int] NULL,
	[Player2LossesSetsLastMonthS2] [int] NULL,
	[Player2WinsSetsLastWeekS2] [int] NULL,
	[Player2LossesSetsLastWeekS2] [int] NULL,
	[Player1WinsSetsTotalS3] [int] NULL,
	[Player1LossesSetsTotalS3] [int] NULL,
	[Player1WinsSetsLastYearS3] [int] NULL,
	[Player1LossesSetsLastYearS3] [int] NULL,
	[Player1WinsSetsLastMonthS3] [int] NULL,
	[Player1LossesSetsLastMonthS3] [int] NULL,
	[Player1WinsSetsLastWeekS3] [int] NULL,
	[Player1LossesSetsLastWeekS3] [int] NULL,
	[Player2WinsSetsTotalS3] [int] NULL,
	[Player2LossesSetsTotalS3] [int] NULL,
	[Player2WinsSetsLastYearS3] [int] NULL,
	[Player2LossesSetsLastYearS3] [int] NULL,
	[Player2WinsSetsLastMonthS3] [int] NULL,
	[Player2LossesSetsLastMonthS3] [int] NULL,
	[Player2WinsSetsLastWeekS3] [int] NULL,
	[Player2LossesSetsLastWeekS3] [int] NULL,
	[Player1WinsSetsTotalS4] [int] NULL,
	[Player1LossesSetsTotalS4] [int] NULL,
	[Player1WinsSetsLastYearS4] [int] NULL,
	[Player1LossesSetsLastYearS4] [int] NULL,
	[Player1WinsSetsLastMonthS4] [int] NULL,
	[Player1LossesSetsLastMonthS4] [int] NULL,
	[Player1WinsSetsLastWeekS4] [int] NULL,
	[Player1LossesSetsLastWeekS4] [int] NULL,
	[Player2WinsSetsTotalS4] [int] NULL,
	[Player2LossesSetsTotalS4] [int] NULL,
	[Player2WinsSetsLastYearS4] [int] NULL,
	[Player2LossesSetsLastYearS4] [int] NULL,
	[Player2WinsSetsLastMonthS4] [int] NULL,
	[Player2LossesSetsLastMonthS4] [int] NULL,
	[Player2WinsSetsLastWeekS4] [int] NULL,
	[Player2LossesSetsLastWeekS4] [int] NULL,
	[Player1WinsGamesTotal] [int] NULL,
	[Player1LossesGamesTotal] [int] NULL,
	[Player1WinsGamesLastYear] [int] NULL,
	[Player1LossesGamesLastYear] [int] NULL,
	[Player1WinsGamesLastMonth] [int] NULL,
	[Player1LossesGamesLastMonth] [int] NULL,
	[Player1WinsGamesLastWeek] [int] NULL,
	[Player1LossesGamesLastWeek] [int] NULL,
	[Player2WinsGamesTotal] [int] NULL,
	[Player2LossesGamesTotal] [int] NULL,
	[Player2WinsGamesLastYear] [int] NULL,
	[Player2LossesGamesLastYear] [int] NULL,
	[Player2WinsGamesLastMonth] [int] NULL,
	[Player2LossesGamesLastMonth] [int] NULL,
	[Player2WinsGamesLastWeek] [int] NULL,
	[Player2LossesGamesLastWeek] [int] NULL,
	[Player1WinsGamesTotalS1] [int] NULL,
	[Player1LossesGamesTotalS1] [int] NULL,
	[Player1WinsGamesLastYearS1] [int] NULL,
	[Player1LossesGamesLastYearS1] [int] NULL,
	[Player1WinsGamesLastMonthS1] [int] NULL,
	[Player1LossesGamesLastMonthS1] [int] NULL,
	[Player1WinsGamesLastWeekS1] [int] NULL,
	[Player1LossesGamesLastWeekS1] [int] NULL,
	[Player2WinsGamesTotalS1] [int] NULL,
	[Player2LossesGamesTotalS1] [int] NULL,
	[Player2WinsGamesLastYearS1] [int] NULL,
	[Player2LossesGamesLastYearS1] [int] NULL,
	[Player2WinsGamesLastMonthS1] [int] NULL,
	[Player2LossesGamesLastMonthS1] [int] NULL,
	[Player2WinsGamesLastWeekS1] [int] NULL,
	[Player2LossesGamesLastWeekS1] [int] NULL,
	[Player1WinsGamesTotalS2] [int] NULL,
	[Player1LossesGamesTotalS2] [int] NULL,
	[Player1WinsGamesLastYearS2] [int] NULL,
	[Player1LossesGamesLastYearS2] [int] NULL,
	[Player1WinsGamesLastMonthS2] [int] NULL,
	[Player1LossesGamesLastMonthS2] [int] NULL,
	[Player1WinsGamesLastWeekS2] [int] NULL,
	[Player1LossesGamesLastWeekS2] [int] NULL,
	[Player2WinsGamesTotalS2] [int] NULL,
	[Player2LossesGamesTotalS2] [int] NULL,
	[Player2WinsGamesLastYearS2] [int] NULL,
	[Player2LossesGamesLastYearS2] [int] NULL,
	[Player2WinsGamesLastMonthS2] [int] NULL,
	[Player2LossesGamesLastMonthS2] [int] NULL,
	[Player2WinsGamesLastWeekS2] [int] NULL,
	[Player2LossesGamesLastWeekS2] [int] NULL,
	[Player1WinsGamesTotalS3] [int] NULL,
	[Player1LossesGamesTotalS3] [int] NULL,
	[Player1WinsGamesLastYearS3] [int] NULL,
	[Player1LossesGamesLastYearS3] [int] NULL,
	[Player1WinsGamesLastMonthS3] [int] NULL,
	[Player1LossesGamesLastMonthS3] [int] NULL,
	[Player1WinsGamesLastWeekS3] [int] NULL,
	[Player1LossesGamesLastWeekS3] [int] NULL,
	[Player2WinsGamesTotalS3] [int] NULL,
	[Player2LossesGamesTotalS3] [int] NULL,
	[Player2WinsGamesLastYearS3] [int] NULL,
	[Player2LossesGamesLastYearS3] [int] NULL,
	[Player2WinsGamesLastMonthS3] [int] NULL,
	[Player2LossesGamesLastMonthS3] [int] NULL,
	[Player2WinsGamesLastWeekS3] [int] NULL,
	[Player2LossesGamesLastWeekS3] [int] NULL,
	[Player1WinsGamesTotalS4] [int] NULL,
	[Player1LossesGamesTotalS4] [int] NULL,
	[Player1WinsGamesLastYearS4] [int] NULL,
	[Player1LossesGamesLastYearS4] [int] NULL,
	[Player1WinsGamesLastMonthS4] [int] NULL,
	[Player1LossesGamesLastMonthS4] [int] NULL,
	[Player1WinsGamesLastWeekS4] [int] NULL,
	[Player1LossesGamesLastWeekS4] [int] NULL,
	[Player2WinsGamesTotalS4] [int] NULL,
	[Player2LossesGamesTotalS4] [int] NULL,
	[Player2WinsGamesLastYearS4] [int] NULL,
	[Player2LossesGamesLastYearS4] [int] NULL,
	[Player2WinsGamesLastMonthS4] [int] NULL,
	[Player2LossesGamesLastMonthS4] [int] NULL,
	[Player2WinsGamesLastWeekS4] [int] NULL,
	[Player2LossesGamesLastWeekS4] [int] NULL,
	[Player1DaysSinceLastWin] [int] NULL,
	[Player2DaysSinceLastWin] [int] NULL,
	[Player1DaysSinceLastWinS1] [int] NULL,
	[Player2DaysSinceLastWinS1] [int] NULL,
	[Player1DaysSinceLastWinS2] [int] NULL,
	[Player2DaysSinceLastWinS2] [int] NULL,
	[Player1DaysSinceLastWinS3] [int] NULL,
	[Player2DaysSinceLastWinS3] [int] NULL,
	[Player1DaysSinceLastWinS4] [int] NULL,
	[Player2DaysSinceLastWinS4] [int] NULL,
	[Player1DaysSinceLastLoss] [int] NULL,
	[Player2DaysSinceLastLoss] [int] NULL,
	[Player1DaysSinceLastLossS1] [int] NULL,
	[Player2DaysSinceLastLossS1] [int] NULL,
	[Player1DaysSinceLastLossS2] [int] NULL,
	[Player2DaysSinceLastLossS2] [int] NULL,
	[Player1DaysSinceLastLossS3] [int] NULL,
	[Player2DaysSinceLastLossS3] [int] NULL,
	[Player1DaysSinceLastLossS4] [int] NULL,
	[Player2DaysSinceLastLossS4] [int] NULL,
	[Player1TotalWinsAsFavourite] [int] NULL,
	[Player2TotalWinsAsFavourite] [int] NULL,
	[Player1TotalWinsAsUnderdog] [int] NULL,
	[Player2TotalWinsAsUnderdog] [int] NULL,
	[Player1TotalLossesAsFavourite] [int] NULL,
	[Player2TotalLossesAsFavourite] [int] NULL,
	[Player1TotalLossesAsUnderdog] [int] NULL,
	[Player2TotalLossesAsUnderdog] [int] NULL,
	[Player1AverageWinningProbabilityAtWonAsFavourite] [float] NULL,
	[Player2AverageWinningProbabilityAtWonAsFavourite] [float] NULL,
	[Player1AverageWinningProbabilityAtWonAsUnderdog] [float] NULL,
	[Player2AverageWinningProbabilityAtWonAsUnderdog] [float] NULL,
	[Player1AverageWinningProbabilityAtLossAsFavourite] [float] NULL,
	[Player2AverageWinningProbabilityAtLossAsFavourite] [float] NULL,
	[Player1AverageWinningProbabilityAtLossAsUnderdog] [float] NULL,
	[Player2AverageWinningProbabilityAtLossAsUnderdog] [float] NULL,
	[Player1TotalWinsAsFavouriteLastYear] [int] NULL,
	[Player2TotalWinsAsFavouriteLastYear] [int] NULL,
	[Player1TotalWinsAsUnderdogLastYear] [int] NULL,
	[Player2TotalWinsAsUnderdogLastYear] [int] NULL,
	[Player1TotalLossesAsFavouriteLastYear] [int] NULL,
	[Player2TotalLossesAsFavouriteLastYear] [int] NULL,
	[Player1TotalLossesAsUnderdogLastYear] [int] NULL,
	[Player2TotalLossesAsUnderdogLastYear] [int] NULL,
	[Player1AverageWinningProbabilityAtWonAsFavouriteLastYear] [float] NULL,
	[Player2AverageWinningProbabilityAtWonAsFavouriteLastYear] [float] NULL,
	[Player1AverageWinningProbabilityAtWonAsUnderdogLastYear] [float] NULL,
	[Player2AverageWinningProbabilityAtWonAsUnderdogLastYear] [float] NULL,
	[Player1AverageWinningProbabilityAtLossAsFavouriteLastYear] [float] NULL,
	[Player2AverageWinningProbabilityAtLossAsFavouriteLastYear] [float] NULL,
	[Player1AverageWinningProbabilityAtLossAsUnderdogLastYear] [float] NULL,
	[Player2AverageWinningProbabilityAtLossAsUnderdogLastYear] [float] NULL,
	[Player1TotalWinsAsFavouriteLastMonth] [int] NULL,
	[Player2TotalWinsAsFavouriteLastMonth] [int] NULL,
	[Player1TotalWinsAsUnderdogLastMonth] [int] NULL,
	[Player2TotalWinsAsUnderdogLastMonth] [int] NULL,
	[Player1TotalLossesAsFavouriteLastMonth] [int] NULL,
	[Player2TotalLossesAsFavouriteLastMonth] [int] NULL,
	[Player1TotalLossesAsUnderdogLastMonth] [int] NULL,
	[Player2TotalLossesAsUnderdogLastMonth] [int] NULL,
	[Player1AverageWinningProbabilityAtWonAsFavouriteLastMonth] [float] NULL,
	[Player2AverageWinningProbabilityAtWonAsFavouriteLastMonth] [float] NULL,
	[Player1AverageWinningProbabilityAtWonAsUnderdogLastMonth] [float] NULL,
	[Player2AverageWinningProbabilityAtWonAsUnderdogLastMonth] [float] NULL,
	[Player1AverageWinningProbabilityAtLossAsFavouriteLastMonth] [float] NULL,
	[Player2AverageWinningProbabilityAtLossAsFavouriteLastMonth] [float] NULL,
	[Player1AverageWinningProbabilityAtLossAsUnderdogLastMonth] [float] NULL,
	[Player2AverageWinningProbabilityAtLossAsUnderdogLastMonth] [float] NULL,
	[Player1TotalWinsAsFavouriteLastWeek] [int] NULL,
	[Player2TotalWinsAsFavouriteLastWeek] [int] NULL,
	[Player1TotalWinsAsUnderdogLastWeek] [int] NULL,
	[Player2TotalWinsAsUnderdogLastWeek] [int] NULL,
	[Player1TotalLossesAsFavouriteLastWeek] [int] NULL,
	[Player2TotalLossesAsFavouriteLastWeek] [int] NULL,
	[Player1TotalLossesAsUnderdogLastWeek] [int] NULL,
	[Player2TotalLossesAsUnderdogLastWeek] [int] NULL,
	[Player1AverageWinningProbabilityAtWonAsFavouriteLastWeek] [float] NULL,
	[Player2AverageWinningProbabilityAtWonAsFavouriteLastWeek] [float] NULL,
	[Player1AverageWinningProbabilityAtWonAsUnderdogLastWeek] [float] NULL,
	[Player2AverageWinningProbabilityAtWonAsUnderdogLastWeek] [float] NULL,
	[Player1AverageWinningProbabilityAtLossAsFavouriteLastWeek] [float] NULL,
	[Player2AverageWinningProbabilityAtLossAsFavouriteLastWeek] [float] NULL,
	[Player1AverageWinningProbabilityAtLossAsUnderdogLastWeek] [float] NULL,
	[Player2AverageWinningProbabilityAtLossAsUnderdogLastWeek] [float] NULL,
	[Player1H2H] [int] NULL,
	[Player2H2H] [int] NULL,
	[Player1H2HS1] [int] NULL,
	[Player2H2HS1] [int] NULL,
	[Player1H2HS2] [int] NULL,
	[Player2H2HS2] [int] NULL,
	[Player1H2HS3] [int] NULL,
	[Player2H2HS3] [int] NULL,
	[Player1H2HS4] [int] NULL,
	[Player2H2HS4] [int] NULL,
	[Player1H2HTrueSkillMeanM] [float] NULL,
	[Player1H2HTrueSkillStandardDeviationM] [float] NULL,
	[Player2H2HTrueSkillMeanM] [float] NULL,
	[Player2H2HTrueSkillStandardDeviationM] [float] NULL,
	[Player1H2HTrueSkillMeanOldM] [float] NULL,
	[Player1H2HTrueSkillStandardDeviationOldM] [float] NULL,
	[Player2H2HTrueSkillMeanOldM] [float] NULL,
	[Player2H2HTrueSkillStandardDeviationOldM] [float] NULL,
	[WinProbabilityPlayer1H2HM] [float] NULL,
	[Player1H2HTrueSkillMeanSM] [float] NULL,
	[Player1H2HTrueSkillStandardDeviationSM] [float] NULL,
	[Player2H2HTrueSkillMeanSM] [float] NULL,
	[Player2H2HTrueSkillStandardDeviationSM] [float] NULL,
	[Player1H2HTrueSkillMeanOldSM] [float] NULL,
	[Player1H2HTrueSkillStandardDeviationOldSM] [float] NULL,
	[Player2H2HTrueSkillMeanOldSM] [float] NULL,
	[Player2H2HTrueSkillStandardDeviationOldSM] [float] NULL,
	[WinProbabilityPlayer1H2HSM] [float] NULL,
	[Player1H2HTrueSkillMeanGSM] [float] NULL,
	[Player1H2HTrueSkillStandardDeviationGSM] [float] NULL,
	[Player2H2HTrueSkillMeanGSM] [float] NULL,
	[Player2H2HTrueSkillStandardDeviationGSM] [float] NULL,
	[Player1H2HTrueSkillMeanOldGSM] [float] NULL,
	[Player1H2HTrueSkillStandardDeviationOldGSM] [float] NULL,
	[Player2H2HTrueSkillMeanOldGSM] [float] NULL,
	[Player2H2HTrueSkillStandardDeviationOldGSM] [float] NULL,
	[WinProbabilityPlayer1H2HGSM] [float] NULL,
	[Player1H2HTrueSkillMeanMS1] [float] NULL,
	[Player1H2HTrueSkillStandardDeviationMS1] [float] NULL,
	[Player2H2HTrueSkillMeanMS1] [float] NULL,
	[Player2H2HTrueSkillStandardDeviationMS1] [float] NULL,
	[Player1H2HTrueSkillMeanOldMS1] [float] NULL,
	[Player1H2HTrueSkillStandardDeviationOldMS1] [float] NULL,
	[Player2H2HTrueSkillMeanOldMS1] [float] NULL,
	[Player2H2HTrueSkillStandardDeviationOldMS1] [float] NULL,
	[WinProbabilityPlayer1H2HMS1] [float] NULL,
	[Player1H2HTrueSkillMeanSMS1] [float] NULL,
	[Player1H2HTrueSkillStandardDeviationSMS1] [float] NULL,
	[Player2H2HTrueSkillMeanSMS1] [float] NULL,
	[Player2H2HTrueSkillStandardDeviationSMS1] [float] NULL,
	[Player1H2HTrueSkillMeanOldSMS1] [float] NULL,
	[Player1H2HTrueSkillStandardDeviationOldSMS1] [float] NULL,
	[Player2H2HTrueSkillMeanOldSMS1] [float] NULL,
	[Player2H2HTrueSkillStandardDeviationOldSMS1] [float] NULL,
	[WinProbabilityPlayer1H2HSMS1] [float] NULL,
	[Player1H2HTrueSkillMeanGSMS1] [float] NULL,
	[Player1H2HTrueSkillStandardDeviationGSMS1] [float] NULL,
	[Player2H2HTrueSkillMeanGSMS1] [float] NULL,
	[Player2H2HTrueSkillStandardDeviationGSMS1] [float] NULL,
	[Player1H2HTrueSkillMeanOldGSMS1] [float] NULL,
	[Player1H2HTrueSkillStandardDeviationOldGSMS1] [float] NULL,
	[Player2H2HTrueSkillMeanOldGSMS1] [float] NULL,
	[Player2H2HTrueSkillStandardDeviationOldGSMS1] [float] NULL,
	[WinProbabilityPlayer1H2HGSMS1] [float] NULL,
	[Player1H2HTrueSkillMeanMS2] [float] NULL,
	[Player1H2HTrueSkillStandardDeviationMS2] [float] NULL,
	[Player2H2HTrueSkillMeanMS2] [float] NULL,
	[Player2H2HTrueSkillStandardDeviationMS2] [float] NULL,
	[Player1H2HTrueSkillMeanOldMS2] [float] NULL,
	[Player1H2HTrueSkillStandardDeviationOldMS2] [float] NULL,
	[Player2H2HTrueSkillMeanOldMS2] [float] NULL,
	[Player2H2HTrueSkillStandardDeviationOldMS2] [float] NULL,
	[WinProbabilityPlayer1H2HMS2] [float] NULL,
	[Player1H2HTrueSkillMeanSMS2] [float] NULL,
	[Player1H2HTrueSkillStandardDeviationSMS2] [float] NULL,
	[Player2H2HTrueSkillMeanSMS2] [float] NULL,
	[Player2H2HTrueSkillStandardDeviationSMS2] [float] NULL,
	[Player1H2HTrueSkillMeanOldSMS2] [float] NULL,
	[Player1H2HTrueSkillStandardDeviationOldSMS2] [float] NULL,
	[Player2H2HTrueSkillMeanOldSMS2] [float] NULL,
	[Player2H2HTrueSkillStandardDeviationOldSMS2] [float] NULL,
	[WinProbabilityPlayer1H2HSMS2] [float] NULL,
	[Player1H2HTrueSkillMeanGSMS2] [float] NULL,
	[Player1H2HTrueSkillStandardDeviationGSMS2] [float] NULL,
	[Player2H2HTrueSkillMeanGSMS2] [float] NULL,
	[Player2H2HTrueSkillStandardDeviationGSMS2] [float] NULL,
	[Player1H2HTrueSkillMeanOldGSMS2] [float] NULL,
	[Player1H2HTrueSkillStandardDeviationOldGSMS2] [float] NULL,
	[Player2H2HTrueSkillMeanOldGSMS2] [float] NULL,
	[Player2H2HTrueSkillStandardDeviationOldGSMS2] [float] NULL,
	[WinProbabilityPlayer1H2HGSMS2] [float] NULL,
	[Player1H2HTrueSkillMeanMS3] [float] NULL,
	[Player1H2HTrueSkillStandardDeviationMS3] [float] NULL,
	[Player2H2HTrueSkillMeanMS3] [float] NULL,
	[Player2H2HTrueSkillStandardDeviationMS3] [float] NULL,
	[Player1H2HTrueSkillMeanOldMS3] [float] NULL,
	[Player1H2HTrueSkillStandardDeviationOldMS3] [float] NULL,
	[Player2H2HTrueSkillMeanOldMS3] [float] NULL,
	[Player2H2HTrueSkillStandardDeviationOldMS3] [float] NULL,
	[WinProbabilityPlayer1H2HMS3] [float] NULL,
	[Player1H2HTrueSkillMeanSMS3] [float] NULL,
	[Player1H2HTrueSkillStandardDeviationSMS3] [float] NULL,
	[Player2H2HTrueSkillMeanSMS3] [float] NULL,
	[Player2H2HTrueSkillStandardDeviationSMS3] [float] NULL,
	[Player1H2HTrueSkillMeanOldSMS3] [float] NULL,
	[Player1H2HTrueSkillStandardDeviationOldSMS3] [float] NULL,
	[Player2H2HTrueSkillMeanOldSMS3] [float] NULL,
	[Player2H2HTrueSkillStandardDeviationOldSMS3] [float] NULL,
	[WinProbabilityPlayer1H2HSMS3] [float] NULL,
	[Player1H2HTrueSkillMeanGSMS3] [float] NULL,
	[Player1H2HTrueSkillStandardDeviationGSMS3] [float] NULL,
	[Player2H2HTrueSkillMeanGSMS3] [float] NULL,
	[Player2H2HTrueSkillStandardDeviationGSMS3] [float] NULL,
	[Player1H2HTrueSkillMeanOldGSMS3] [float] NULL,
	[Player1H2HTrueSkillStandardDeviationOldGSMS3] [float] NULL,
	[Player2H2HTrueSkillMeanOldGSMS3] [float] NULL,
	[Player2H2HTrueSkillStandardDeviationOldGSMS3] [float] NULL,
	[WinProbabilityPlayer1H2HGSMS3] [float] NULL,
	[Player1H2HTrueSkillMeanMS4] [float] NULL,
	[Player1H2HTrueSkillStandardDeviationMS4] [float] NULL,
	[Player2H2HTrueSkillMeanMS4] [float] NULL,
	[Player2H2HTrueSkillStandardDeviationMS4] [float] NULL,
	[Player1H2HTrueSkillMeanOldMS4] [float] NULL,
	[Player1H2HTrueSkillStandardDeviationOldMS4] [float] NULL,
	[Player2H2HTrueSkillMeanOldMS4] [float] NULL,
	[Player2H2HTrueSkillStandardDeviationOldMS4] [float] NULL,
	[WinProbabilityPlayer1H2HMS4] [float] NULL,
	[Player1H2HTrueSkillMeanSMS4] [float] NULL,
	[Player1H2HTrueSkillStandardDeviationSMS4] [float] NULL,
	[Player2H2HTrueSkillMeanSMS4] [float] NULL,
	[Player2H2HTrueSkillStandardDeviationSMS4] [float] NULL,
	[Player1H2HTrueSkillMeanOldSMS4] [float] NULL,
	[Player1H2HTrueSkillStandardDeviationOldSMS4] [float] NULL,
	[Player2H2HTrueSkillMeanOldSMS4] [float] NULL,
	[Player2H2HTrueSkillStandardDeviationOldSMS4] [float] NULL,
	[WinProbabilityPlayer1H2HSMS4] [float] NULL,
	[Player1H2HTrueSkillMeanGSMS4] [float] NULL,
	[Player1H2HTrueSkillStandardDeviationGSMS4] [float] NULL,
	[Player2H2HTrueSkillMeanGSMS4] [float] NULL,
	[Player2H2HTrueSkillStandardDeviationGSMS4] [float] NULL,
	[Player1H2HTrueSkillMeanOldGSMS4] [float] NULL,
	[Player1H2HTrueSkillStandardDeviationOldGSMS4] [float] NULL,
	[Player2H2HTrueSkillMeanOldGSMS4] [float] NULL,
	[Player2H2HTrueSkillStandardDeviationOldGSMS4] [float] NULL,
	[WinProbabilityPlayer1H2HGSMS4] [float] NULL,
	[Player1Streak] [int] NULL,
	[Player2Streak] [int] NULL,
	[Player1StreakS1] [int] NULL,
	[Player2StreakS1] [int] NULL,
	[Player1StreakS2] [int] NULL,
	[Player2StreakS2] [int] NULL,
	[Player1StreakS3] [int] NULL,
	[Player2StreakS3] [int] NULL,
	[Player1StreakS4] [int] NULL,
	[Player2StreakS4] [int] NULL,
	[P1SetsWon] [int] NULL,
	[P2SetsWon] [int] NULL,
	[P1GamesWon] [int] NULL,
	[P2GamesWon] [int] NULL,
	[P1SetsLoss] [int] NULL,
	[P2SetsLoss] [int] NULL,
	[P1GamesLoss] [int] NULL,
	[P2GamesLoss] [int] NULL,
	[WinProbabilityNN] [float] NULL,
 CONSTRAINT [PK_Match] PRIMARY KEY CLUSTERED 
(
	[MatchTPId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MatchActive]    Script Date: 19.7.2025. 11:43:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MatchActive](
	[MatchTPId] [int] NOT NULL,
	[TournamentEventTPId] [int] NULL,
	[DateTime] [datetime] NULL,
	[Player1TPID] [int] NULL,
	[Player2TPID] [int] NULL,
	[Player1Seed] [varchar](10) NULL,
	[Player2Seed] [varchar](10) NULL,
	[Player1Odds] [decimal](5, 2) NULL,
	[Player2Odds] [decimal](5, 2) NULL,
	[Player1Percentage] [decimal](5, 2) NULL,
	[Player2Percentage] [decimal](5, 2) NULL,
	[SurfaceId] [tinyint] NULL,
	[RoundId] [tinyint] NULL,
	[Player1TrueSkillMeanOldM] [float] NULL,
	[Player1TrueSkillStandardDeviationOldM] [float] NULL,
	[Player2TrueSkillMeanOldM] [float] NULL,
	[Player2TrueSkillStandardDeviationOldM] [float] NULL,
	[WinProbabilityPlayer1M] [float] NULL,
	[Player1TrueSkillMeanOldSM] [float] NULL,
	[Player1TrueSkillStandardDeviationOldSM] [float] NULL,
	[Player2TrueSkillMeanOldSM] [float] NULL,
	[Player2TrueSkillStandardDeviationOldSM] [float] NULL,
	[WinProbabilityPlayer1SM] [float] NULL,
	[Player1TrueSkillMeanOldGSM] [float] NULL,
	[Player1TrueSkillStandardDeviationOldGSM] [float] NULL,
	[Player2TrueSkillMeanOldGSM] [float] NULL,
	[Player2TrueSkillStandardDeviationOldGSM] [float] NULL,
	[WinProbabilityPlayer1GSM] [float] NULL,
	[Player1TrueSkillMeanOldMS1] [float] NULL,
	[Player1TrueSkillStandardDeviationOldMS1] [float] NULL,
	[Player2TrueSkillMeanOldMS1] [float] NULL,
	[Player2TrueSkillStandardDeviationOldMS1] [float] NULL,
	[WinProbabilityPlayer1MS1] [float] NULL,
	[Player1TrueSkillMeanOldSMS1] [float] NULL,
	[Player1TrueSkillStandardDeviationOldSMS1] [float] NULL,
	[Player2TrueSkillMeanOldSMS1] [float] NULL,
	[Player2TrueSkillStandardDeviationOldSMS1] [float] NULL,
	[WinProbabilityPlayer1SMS1] [float] NULL,
	[Player1TrueSkillMeanOldGSMS1] [float] NULL,
	[Player1TrueSkillStandardDeviationOldGSMS1] [float] NULL,
	[Player2TrueSkillMeanOldGSMS1] [float] NULL,
	[Player2TrueSkillStandardDeviationOldGSMS1] [float] NULL,
	[WinProbabilityPlayer1GSMS1] [float] NULL,
	[Player1TrueSkillMeanOldMS2] [float] NULL,
	[Player1TrueSkillStandardDeviationOldMS2] [float] NULL,
	[Player2TrueSkillMeanOldMS2] [float] NULL,
	[Player2TrueSkillStandardDeviationOldMS2] [float] NULL,
	[WinProbabilityPlayer1MS2] [float] NULL,
	[Player1TrueSkillMeanOldSMS2] [float] NULL,
	[Player1TrueSkillStandardDeviationOldSMS2] [float] NULL,
	[Player2TrueSkillMeanOldSMS2] [float] NULL,
	[Player2TrueSkillStandardDeviationOldSMS2] [float] NULL,
	[WinProbabilityPlayer1SMS2] [float] NULL,
	[Player1TrueSkillMeanOldGSMS2] [float] NULL,
	[Player1TrueSkillStandardDeviationOldGSMS2] [float] NULL,
	[Player2TrueSkillMeanOldGSMS2] [float] NULL,
	[Player2TrueSkillStandardDeviationOldGSMS2] [float] NULL,
	[WinProbabilityPlayer1GSMS2] [float] NULL,
	[Player1TrueSkillMeanOldMS3] [float] NULL,
	[Player1TrueSkillStandardDeviationOldMS3] [float] NULL,
	[Player2TrueSkillMeanOldMS3] [float] NULL,
	[Player2TrueSkillStandardDeviationOldMS3] [float] NULL,
	[WinProbabilityPlayer1MS3] [float] NULL,
	[Player1TrueSkillMeanOldSMS3] [float] NULL,
	[Player1TrueSkillStandardDeviationOldSMS3] [float] NULL,
	[Player2TrueSkillMeanOldSMS3] [float] NULL,
	[Player2TrueSkillStandardDeviationOldSMS3] [float] NULL,
	[WinProbabilityPlayer1SMS3] [float] NULL,
	[Player1TrueSkillMeanOldGSMS3] [float] NULL,
	[Player1TrueSkillStandardDeviationOldGSMS3] [float] NULL,
	[Player2TrueSkillMeanOldGSMS3] [float] NULL,
	[Player2TrueSkillStandardDeviationOldGSMS3] [float] NULL,
	[WinProbabilityPlayer1GSMS3] [float] NULL,
	[Player1TrueSkillMeanOldMS4] [float] NULL,
	[Player1TrueSkillStandardDeviationOldMS4] [float] NULL,
	[Player2TrueSkillMeanOldMS4] [float] NULL,
	[Player2TrueSkillStandardDeviationOldMS4] [float] NULL,
	[WinProbabilityPlayer1MS4] [float] NULL,
	[Player1TrueSkillMeanOldSMS4] [float] NULL,
	[Player1TrueSkillStandardDeviationOldSMS4] [float] NULL,
	[Player2TrueSkillMeanOldSMS4] [float] NULL,
	[Player2TrueSkillStandardDeviationOldSMS4] [float] NULL,
	[WinProbabilityPlayer1SMS4] [float] NULL,
	[Player1TrueSkillMeanOldGSMS4] [float] NULL,
	[Player1TrueSkillStandardDeviationOldGSMS4] [float] NULL,
	[Player2TrueSkillMeanOldGSMS4] [float] NULL,
	[Player2TrueSkillStandardDeviationOldGSMS4] [float] NULL,
	[WinProbabilityPlayer1GSMS4] [float] NULL,
	[Player1WinsTotal] [int] NULL,
	[Player1LossesTotal] [int] NULL,
	[Player1WinsLastYear] [int] NULL,
	[Player1LossesLastYear] [int] NULL,
	[Player1WinsLastMonth] [int] NULL,
	[Player1LossesLastMonth] [int] NULL,
	[Player1WinsLastWeek] [int] NULL,
	[Player1LossesLastWeek] [int] NULL,
	[Player2WinsTotal] [int] NULL,
	[Player2LossesTotal] [int] NULL,
	[Player2WinsLastYear] [int] NULL,
	[Player2LossesLastYear] [int] NULL,
	[Player2WinsLastMonth] [int] NULL,
	[Player2LossesLastMonth] [int] NULL,
	[Player2WinsLastWeek] [int] NULL,
	[Player2LossesLastWeek] [int] NULL,
	[Player1WinsTotalS1] [int] NULL,
	[Player1LossesTotalS1] [int] NULL,
	[Player1WinsLastYearS1] [int] NULL,
	[Player1LossesLastYearS1] [int] NULL,
	[Player1WinsLastMonthS1] [int] NULL,
	[Player1LossesLastMonthS1] [int] NULL,
	[Player1WinsLastWeekS1] [int] NULL,
	[Player1LossesLastWeekS1] [int] NULL,
	[Player2WinsTotalS1] [int] NULL,
	[Player2LossesTotalS1] [int] NULL,
	[Player2WinsLastYearS1] [int] NULL,
	[Player2LossesLastYearS1] [int] NULL,
	[Player2WinsLastMonthS1] [int] NULL,
	[Player2LossesLastMonthS1] [int] NULL,
	[Player2WinsLastWeekS1] [int] NULL,
	[Player2LossesLastWeekS1] [int] NULL,
	[Player1WinsTotalS2] [int] NULL,
	[Player1LossesTotalS2] [int] NULL,
	[Player1WinsLastYearS2] [int] NULL,
	[Player1LossesLastYearS2] [int] NULL,
	[Player1WinsLastMonthS2] [int] NULL,
	[Player1LossesLastMonthS2] [int] NULL,
	[Player1WinsLastWeekS2] [int] NULL,
	[Player1LossesLastWeekS2] [int] NULL,
	[Player2WinsTotalS2] [int] NULL,
	[Player2LossesTotalS2] [int] NULL,
	[Player2WinsLastYearS2] [int] NULL,
	[Player2LossesLastYearS2] [int] NULL,
	[Player2WinsLastMonthS2] [int] NULL,
	[Player2LossesLastMonthS2] [int] NULL,
	[Player2WinsLastWeekS2] [int] NULL,
	[Player2LossesLastWeekS2] [int] NULL,
	[Player1WinsTotalS3] [int] NULL,
	[Player1LossesTotalS3] [int] NULL,
	[Player1WinsLastYearS3] [int] NULL,
	[Player1LossesLastYearS3] [int] NULL,
	[Player1WinsLastMonthS3] [int] NULL,
	[Player1LossesLastMonthS3] [int] NULL,
	[Player1WinsLastWeekS3] [int] NULL,
	[Player1LossesLastWeekS3] [int] NULL,
	[Player2WinsTotalS3] [int] NULL,
	[Player2LossesTotalS3] [int] NULL,
	[Player2WinsLastYearS3] [int] NULL,
	[Player2LossesLastYearS3] [int] NULL,
	[Player2WinsLastMonthS3] [int] NULL,
	[Player2LossesLastMonthS3] [int] NULL,
	[Player2WinsLastWeekS3] [int] NULL,
	[Player2LossesLastWeekS3] [int] NULL,
	[Player1WinsTotalS4] [int] NULL,
	[Player1LossesTotalS4] [int] NULL,
	[Player1WinsLastYearS4] [int] NULL,
	[Player1LossesLastYearS4] [int] NULL,
	[Player1WinsLastMonthS4] [int] NULL,
	[Player1LossesLastMonthS4] [int] NULL,
	[Player1WinsLastWeekS4] [int] NULL,
	[Player1LossesLastWeekS4] [int] NULL,
	[Player2WinsTotalS4] [int] NULL,
	[Player2LossesTotalS4] [int] NULL,
	[Player2WinsLastYearS4] [int] NULL,
	[Player2LossesLastYearS4] [int] NULL,
	[Player2WinsLastMonthS4] [int] NULL,
	[Player2LossesLastMonthS4] [int] NULL,
	[Player2WinsLastWeekS4] [int] NULL,
	[Player2LossesLastWeekS4] [int] NULL,
	[Player1WinsSetsTotal] [int] NULL,
	[Player1LossesSetsTotal] [int] NULL,
	[Player1WinsSetsLastYear] [int] NULL,
	[Player1LossesSetsLastYear] [int] NULL,
	[Player1WinsSetsLastMonth] [int] NULL,
	[Player1LossesSetsLastMonth] [int] NULL,
	[Player1WinsSetsLastWeek] [int] NULL,
	[Player1LossesSetsLastWeek] [int] NULL,
	[Player2WinsSetsTotal] [int] NULL,
	[Player2LossesSetsTotal] [int] NULL,
	[Player2WinsSetsLastYear] [int] NULL,
	[Player2LossesSetsLastYear] [int] NULL,
	[Player2WinsSetsLastMonth] [int] NULL,
	[Player2LossesSetsLastMonth] [int] NULL,
	[Player2WinsSetsLastWeek] [int] NULL,
	[Player2LossesSetsLastWeek] [int] NULL,
	[Player1WinsSetsTotalS1] [int] NULL,
	[Player1LossesSetsTotalS1] [int] NULL,
	[Player1WinsSetsLastYearS1] [int] NULL,
	[Player1LossesSetsLastYearS1] [int] NULL,
	[Player1WinsSetsLastMonthS1] [int] NULL,
	[Player1LossesSetsLastMonthS1] [int] NULL,
	[Player1WinsSetsLastWeekS1] [int] NULL,
	[Player1LossesSetsLastWeekS1] [int] NULL,
	[Player2WinsSetsTotalS1] [int] NULL,
	[Player2LossesSetsTotalS1] [int] NULL,
	[Player2WinsSetsLastYearS1] [int] NULL,
	[Player2LossesSetsLastYearS1] [int] NULL,
	[Player2WinsSetsLastMonthS1] [int] NULL,
	[Player2LossesSetsLastMonthS1] [int] NULL,
	[Player2WinsSetsLastWeekS1] [int] NULL,
	[Player2LossesSetsLastWeekS1] [int] NULL,
	[Player1WinsSetsTotalS2] [int] NULL,
	[Player1LossesSetsTotalS2] [int] NULL,
	[Player1WinsSetsLastYearS2] [int] NULL,
	[Player1LossesSetsLastYearS2] [int] NULL,
	[Player1WinsSetsLastMonthS2] [int] NULL,
	[Player1LossesSetsLastMonthS2] [int] NULL,
	[Player1WinsSetsLastWeekS2] [int] NULL,
	[Player1LossesSetsLastWeekS2] [int] NULL,
	[Player2WinsSetsTotalS2] [int] NULL,
	[Player2LossesSetsTotalS2] [int] NULL,
	[Player2WinsSetsLastYearS2] [int] NULL,
	[Player2LossesSetsLastYearS2] [int] NULL,
	[Player2WinsSetsLastMonthS2] [int] NULL,
	[Player2LossesSetsLastMonthS2] [int] NULL,
	[Player2WinsSetsLastWeekS2] [int] NULL,
	[Player2LossesSetsLastWeekS2] [int] NULL,
	[Player1WinsSetsTotalS3] [int] NULL,
	[Player1LossesSetsTotalS3] [int] NULL,
	[Player1WinsSetsLastYearS3] [int] NULL,
	[Player1LossesSetsLastYearS3] [int] NULL,
	[Player1WinsSetsLastMonthS3] [int] NULL,
	[Player1LossesSetsLastMonthS3] [int] NULL,
	[Player1WinsSetsLastWeekS3] [int] NULL,
	[Player1LossesSetsLastWeekS3] [int] NULL,
	[Player2WinsSetsTotalS3] [int] NULL,
	[Player2LossesSetsTotalS3] [int] NULL,
	[Player2WinsSetsLastYearS3] [int] NULL,
	[Player2LossesSetsLastYearS3] [int] NULL,
	[Player2WinsSetsLastMonthS3] [int] NULL,
	[Player2LossesSetsLastMonthS3] [int] NULL,
	[Player2WinsSetsLastWeekS3] [int] NULL,
	[Player2LossesSetsLastWeekS3] [int] NULL,
	[Player1WinsSetsTotalS4] [int] NULL,
	[Player1LossesSetsTotalS4] [int] NULL,
	[Player1WinsSetsLastYearS4] [int] NULL,
	[Player1LossesSetsLastYearS4] [int] NULL,
	[Player1WinsSetsLastMonthS4] [int] NULL,
	[Player1LossesSetsLastMonthS4] [int] NULL,
	[Player1WinsSetsLastWeekS4] [int] NULL,
	[Player1LossesSetsLastWeekS4] [int] NULL,
	[Player2WinsSetsTotalS4] [int] NULL,
	[Player2LossesSetsTotalS4] [int] NULL,
	[Player2WinsSetsLastYearS4] [int] NULL,
	[Player2LossesSetsLastYearS4] [int] NULL,
	[Player2WinsSetsLastMonthS4] [int] NULL,
	[Player2LossesSetsLastMonthS4] [int] NULL,
	[Player2WinsSetsLastWeekS4] [int] NULL,
	[Player2LossesSetsLastWeekS4] [int] NULL,
	[Player1WinsGamesTotal] [int] NULL,
	[Player1LossesGamesTotal] [int] NULL,
	[Player1WinsGamesLastYear] [int] NULL,
	[Player1LossesGamesLastYear] [int] NULL,
	[Player1WinsGamesLastMonth] [int] NULL,
	[Player1LossesGamesLastMonth] [int] NULL,
	[Player1WinsGamesLastWeek] [int] NULL,
	[Player1LossesGamesLastWeek] [int] NULL,
	[Player2WinsGamesTotal] [int] NULL,
	[Player2LossesGamesTotal] [int] NULL,
	[Player2WinsGamesLastYear] [int] NULL,
	[Player2LossesGamesLastYear] [int] NULL,
	[Player2WinsGamesLastMonth] [int] NULL,
	[Player2LossesGamesLastMonth] [int] NULL,
	[Player2WinsGamesLastWeek] [int] NULL,
	[Player2LossesGamesLastWeek] [int] NULL,
	[Player1WinsGamesTotalS1] [int] NULL,
	[Player1LossesGamesTotalS1] [int] NULL,
	[Player1WinsGamesLastYearS1] [int] NULL,
	[Player1LossesGamesLastYearS1] [int] NULL,
	[Player1WinsGamesLastMonthS1] [int] NULL,
	[Player1LossesGamesLastMonthS1] [int] NULL,
	[Player1WinsGamesLastWeekS1] [int] NULL,
	[Player1LossesGamesLastWeekS1] [int] NULL,
	[Player2WinsGamesTotalS1] [int] NULL,
	[Player2LossesGamesTotalS1] [int] NULL,
	[Player2WinsGamesLastYearS1] [int] NULL,
	[Player2LossesGamesLastYearS1] [int] NULL,
	[Player2WinsGamesLastMonthS1] [int] NULL,
	[Player2LossesGamesLastMonthS1] [int] NULL,
	[Player2WinsGamesLastWeekS1] [int] NULL,
	[Player2LossesGamesLastWeekS1] [int] NULL,
	[Player1WinsGamesTotalS2] [int] NULL,
	[Player1LossesGamesTotalS2] [int] NULL,
	[Player1WinsGamesLastYearS2] [int] NULL,
	[Player1LossesGamesLastYearS2] [int] NULL,
	[Player1WinsGamesLastMonthS2] [int] NULL,
	[Player1LossesGamesLastMonthS2] [int] NULL,
	[Player1WinsGamesLastWeekS2] [int] NULL,
	[Player1LossesGamesLastWeekS2] [int] NULL,
	[Player2WinsGamesTotalS2] [int] NULL,
	[Player2LossesGamesTotalS2] [int] NULL,
	[Player2WinsGamesLastYearS2] [int] NULL,
	[Player2LossesGamesLastYearS2] [int] NULL,
	[Player2WinsGamesLastMonthS2] [int] NULL,
	[Player2LossesGamesLastMonthS2] [int] NULL,
	[Player2WinsGamesLastWeekS2] [int] NULL,
	[Player2LossesGamesLastWeekS2] [int] NULL,
	[Player1WinsGamesTotalS3] [int] NULL,
	[Player1LossesGamesTotalS3] [int] NULL,
	[Player1WinsGamesLastYearS3] [int] NULL,
	[Player1LossesGamesLastYearS3] [int] NULL,
	[Player1WinsGamesLastMonthS3] [int] NULL,
	[Player1LossesGamesLastMonthS3] [int] NULL,
	[Player1WinsGamesLastWeekS3] [int] NULL,
	[Player1LossesGamesLastWeekS3] [int] NULL,
	[Player2WinsGamesTotalS3] [int] NULL,
	[Player2LossesGamesTotalS3] [int] NULL,
	[Player2WinsGamesLastYearS3] [int] NULL,
	[Player2LossesGamesLastYearS3] [int] NULL,
	[Player2WinsGamesLastMonthS3] [int] NULL,
	[Player2LossesGamesLastMonthS3] [int] NULL,
	[Player2WinsGamesLastWeekS3] [int] NULL,
	[Player2LossesGamesLastWeekS3] [int] NULL,
	[Player1WinsGamesTotalS4] [int] NULL,
	[Player1LossesGamesTotalS4] [int] NULL,
	[Player1WinsGamesLastYearS4] [int] NULL,
	[Player1LossesGamesLastYearS4] [int] NULL,
	[Player1WinsGamesLastMonthS4] [int] NULL,
	[Player1LossesGamesLastMonthS4] [int] NULL,
	[Player1WinsGamesLastWeekS4] [int] NULL,
	[Player1LossesGamesLastWeekS4] [int] NULL,
	[Player2WinsGamesTotalS4] [int] NULL,
	[Player2LossesGamesTotalS4] [int] NULL,
	[Player2WinsGamesLastYearS4] [int] NULL,
	[Player2LossesGamesLastYearS4] [int] NULL,
	[Player2WinsGamesLastMonthS4] [int] NULL,
	[Player2LossesGamesLastMonthS4] [int] NULL,
	[Player2WinsGamesLastWeekS4] [int] NULL,
	[Player2LossesGamesLastWeekS4] [int] NULL,
	[Player1DaysSinceLastWin] [int] NULL,
	[Player2DaysSinceLastWin] [int] NULL,
	[Player1DaysSinceLastWinS1] [int] NULL,
	[Player2DaysSinceLastWinS1] [int] NULL,
	[Player1DaysSinceLastWinS2] [int] NULL,
	[Player2DaysSinceLastWinS2] [int] NULL,
	[Player1DaysSinceLastWinS3] [int] NULL,
	[Player2DaysSinceLastWinS3] [int] NULL,
	[Player1DaysSinceLastWinS4] [int] NULL,
	[Player2DaysSinceLastWinS4] [int] NULL,
	[Player1DaysSinceLastLoss] [int] NULL,
	[Player2DaysSinceLastLoss] [int] NULL,
	[Player1DaysSinceLastLossS1] [int] NULL,
	[Player2DaysSinceLastLossS1] [int] NULL,
	[Player1DaysSinceLastLossS2] [int] NULL,
	[Player2DaysSinceLastLossS2] [int] NULL,
	[Player1DaysSinceLastLossS3] [int] NULL,
	[Player2DaysSinceLastLossS3] [int] NULL,
	[Player1DaysSinceLastLossS4] [int] NULL,
	[Player2DaysSinceLastLossS4] [int] NULL,
	[Player1TotalWinsAsFavourite] [int] NULL,
	[Player2TotalWinsAsFavourite] [int] NULL,
	[Player1TotalWinsAsUnderdog] [int] NULL,
	[Player2TotalWinsAsUnderdog] [int] NULL,
	[Player1TotalLossesAsFavourite] [int] NULL,
	[Player2TotalLossesAsFavourite] [int] NULL,
	[Player1TotalLossesAsUnderdog] [int] NULL,
	[Player2TotalLossesAsUnderdog] [int] NULL,
	[Player1AverageWinningProbabilityAtWonAsFavourite] [float] NULL,
	[Player2AverageWinningProbabilityAtWonAsFavourite] [float] NULL,
	[Player1AverageWinningProbabilityAtWonAsUnderdog] [float] NULL,
	[Player2AverageWinningProbabilityAtWonAsUnderdog] [float] NULL,
	[Player1AverageWinningProbabilityAtLossAsFavourite] [float] NULL,
	[Player2AverageWinningProbabilityAtLossAsFavourite] [float] NULL,
	[Player1AverageWinningProbabilityAtLossAsUnderdog] [float] NULL,
	[Player2AverageWinningProbabilityAtLossAsUnderdog] [float] NULL,
	[Player1TotalWinsAsFavouriteLastYear] [int] NULL,
	[Player2TotalWinsAsFavouriteLastYear] [int] NULL,
	[Player1TotalWinsAsUnderdogLastYear] [int] NULL,
	[Player2TotalWinsAsUnderdogLastYear] [int] NULL,
	[Player1TotalLossesAsFavouriteLastYear] [int] NULL,
	[Player2TotalLossesAsFavouriteLastYear] [int] NULL,
	[Player1TotalLossesAsUnderdogLastYear] [int] NULL,
	[Player2TotalLossesAsUnderdogLastYear] [int] NULL,
	[Player1AverageWinningProbabilityAtWonAsFavouriteLastYear] [float] NULL,
	[Player2AverageWinningProbabilityAtWonAsFavouriteLastYear] [float] NULL,
	[Player1AverageWinningProbabilityAtWonAsUnderdogLastYear] [float] NULL,
	[Player2AverageWinningProbabilityAtWonAsUnderdogLastYear] [float] NULL,
	[Player1AverageWinningProbabilityAtLossAsFavouriteLastYear] [float] NULL,
	[Player2AverageWinningProbabilityAtLossAsFavouriteLastYear] [float] NULL,
	[Player1AverageWinningProbabilityAtLossAsUnderdogLastYear] [float] NULL,
	[Player2AverageWinningProbabilityAtLossAsUnderdogLastYear] [float] NULL,
	[Player1TotalWinsAsFavouriteLastMonth] [int] NULL,
	[Player2TotalWinsAsFavouriteLastMonth] [int] NULL,
	[Player1TotalWinsAsUnderdogLastMonth] [int] NULL,
	[Player2TotalWinsAsUnderdogLastMonth] [int] NULL,
	[Player1TotalLossesAsFavouriteLastMonth] [int] NULL,
	[Player2TotalLossesAsFavouriteLastMonth] [int] NULL,
	[Player1TotalLossesAsUnderdogLastMonth] [int] NULL,
	[Player2TotalLossesAsUnderdogLastMonth] [int] NULL,
	[Player1AverageWinningProbabilityAtWonAsFavouriteLastMonth] [float] NULL,
	[Player2AverageWinningProbabilityAtWonAsFavouriteLastMonth] [float] NULL,
	[Player1AverageWinningProbabilityAtWonAsUnderdogLastMonth] [float] NULL,
	[Player2AverageWinningProbabilityAtWonAsUnderdogLastMonth] [float] NULL,
	[Player1AverageWinningProbabilityAtLossAsFavouriteLastMonth] [float] NULL,
	[Player2AverageWinningProbabilityAtLossAsFavouriteLastMonth] [float] NULL,
	[Player1AverageWinningProbabilityAtLossAsUnderdogLastMonth] [float] NULL,
	[Player2AverageWinningProbabilityAtLossAsUnderdogLastMonth] [float] NULL,
	[Player1TotalWinsAsFavouriteLastWeek] [int] NULL,
	[Player2TotalWinsAsFavouriteLastWeek] [int] NULL,
	[Player1TotalWinsAsUnderdogLastWeek] [int] NULL,
	[Player2TotalWinsAsUnderdogLastWeek] [int] NULL,
	[Player1TotalLossesAsFavouriteLastWeek] [int] NULL,
	[Player2TotalLossesAsFavouriteLastWeek] [int] NULL,
	[Player1TotalLossesAsUnderdogLastWeek] [int] NULL,
	[Player2TotalLossesAsUnderdogLastWeek] [int] NULL,
	[Player1AverageWinningProbabilityAtWonAsFavouriteLastWeek] [float] NULL,
	[Player2AverageWinningProbabilityAtWonAsFavouriteLastWeek] [float] NULL,
	[Player1AverageWinningProbabilityAtWonAsUnderdogLastWeek] [float] NULL,
	[Player2AverageWinningProbabilityAtWonAsUnderdogLastWeek] [float] NULL,
	[Player1AverageWinningProbabilityAtLossAsFavouriteLastWeek] [float] NULL,
	[Player2AverageWinningProbabilityAtLossAsFavouriteLastWeek] [float] NULL,
	[Player1AverageWinningProbabilityAtLossAsUnderdogLastWeek] [float] NULL,
	[Player2AverageWinningProbabilityAtLossAsUnderdogLastWeek] [float] NULL,
	[Player1H2H] [int] NULL,
	[Player2H2H] [int] NULL,
	[Player1H2HS1] [int] NULL,
	[Player2H2HS1] [int] NULL,
	[Player1H2HS2] [int] NULL,
	[Player2H2HS2] [int] NULL,
	[Player1H2HS3] [int] NULL,
	[Player2H2HS3] [int] NULL,
	[Player1H2HS4] [int] NULL,
	[Player2H2HS4] [int] NULL,
	[Player1H2HTrueSkillMeanOldM] [float] NULL,
	[Player1H2HTrueSkillStandardDeviationOldM] [float] NULL,
	[Player2H2HTrueSkillMeanOldM] [float] NULL,
	[Player2H2HTrueSkillStandardDeviationOldM] [float] NULL,
	[WinProbabilityPlayer1H2HM] [float] NULL,
	[Player1H2HTrueSkillMeanOldSM] [float] NULL,
	[Player1H2HTrueSkillStandardDeviationOldSM] [float] NULL,
	[Player2H2HTrueSkillMeanOldSM] [float] NULL,
	[Player2H2HTrueSkillStandardDeviationOldSM] [float] NULL,
	[WinProbabilityPlayer1H2HSM] [float] NULL,
	[Player1H2HTrueSkillMeanOldGSM] [float] NULL,
	[Player1H2HTrueSkillStandardDeviationOldGSM] [float] NULL,
	[Player2H2HTrueSkillMeanOldGSM] [float] NULL,
	[Player2H2HTrueSkillStandardDeviationOldGSM] [float] NULL,
	[WinProbabilityPlayer1H2HGSM] [float] NULL,
	[Player1H2HTrueSkillMeanOldMS1] [float] NULL,
	[Player1H2HTrueSkillStandardDeviationOldMS1] [float] NULL,
	[Player2H2HTrueSkillMeanOldMS1] [float] NULL,
	[Player2H2HTrueSkillStandardDeviationOldMS1] [float] NULL,
	[WinProbabilityPlayer1H2HMS1] [float] NULL,
	[Player1H2HTrueSkillMeanOldSMS1] [float] NULL,
	[Player1H2HTrueSkillStandardDeviationOldSMS1] [float] NULL,
	[Player2H2HTrueSkillMeanOldSMS1] [float] NULL,
	[Player2H2HTrueSkillStandardDeviationOldSMS1] [float] NULL,
	[WinProbabilityPlayer1H2HSMS1] [float] NULL,
	[Player1H2HTrueSkillMeanOldGSMS1] [float] NULL,
	[Player1H2HTrueSkillStandardDeviationOldGSMS1] [float] NULL,
	[Player2H2HTrueSkillMeanOldGSMS1] [float] NULL,
	[Player2H2HTrueSkillStandardDeviationOldGSMS1] [float] NULL,
	[WinProbabilityPlayer1H2HGSMS1] [float] NULL,
	[Player1H2HTrueSkillMeanOldMS2] [float] NULL,
	[Player1H2HTrueSkillStandardDeviationOldMS2] [float] NULL,
	[Player2H2HTrueSkillMeanOldMS2] [float] NULL,
	[Player2H2HTrueSkillStandardDeviationOldMS2] [float] NULL,
	[WinProbabilityPlayer1H2HMS2] [float] NULL,
	[Player1H2HTrueSkillMeanOldSMS2] [float] NULL,
	[Player1H2HTrueSkillStandardDeviationOldSMS2] [float] NULL,
	[Player2H2HTrueSkillMeanOldSMS2] [float] NULL,
	[Player2H2HTrueSkillStandardDeviationOldSMS2] [float] NULL,
	[WinProbabilityPlayer1H2HSMS2] [float] NULL,
	[Player1H2HTrueSkillMeanOldGSMS2] [float] NULL,
	[Player1H2HTrueSkillStandardDeviationOldGSMS2] [float] NULL,
	[Player2H2HTrueSkillMeanOldGSMS2] [float] NULL,
	[Player2H2HTrueSkillStandardDeviationOldGSMS2] [float] NULL,
	[WinProbabilityPlayer1H2HGSMS2] [float] NULL,
	[Player1H2HTrueSkillMeanOldMS3] [float] NULL,
	[Player1H2HTrueSkillStandardDeviationOldMS3] [float] NULL,
	[Player2H2HTrueSkillMeanOldMS3] [float] NULL,
	[Player2H2HTrueSkillStandardDeviationOldMS3] [float] NULL,
	[WinProbabilityPlayer1H2HMS3] [float] NULL,
	[Player1H2HTrueSkillMeanOldSMS3] [float] NULL,
	[Player1H2HTrueSkillStandardDeviationOldSMS3] [float] NULL,
	[Player2H2HTrueSkillMeanOldSMS3] [float] NULL,
	[Player2H2HTrueSkillStandardDeviationOldSMS3] [float] NULL,
	[WinProbabilityPlayer1H2HSMS3] [float] NULL,
	[Player1H2HTrueSkillMeanOldGSMS3] [float] NULL,
	[Player1H2HTrueSkillStandardDeviationOldGSMS3] [float] NULL,
	[Player2H2HTrueSkillMeanOldGSMS3] [float] NULL,
	[Player2H2HTrueSkillStandardDeviationOldGSMS3] [float] NULL,
	[WinProbabilityPlayer1H2HGSMS3] [float] NULL,
	[Player1H2HTrueSkillMeanOldMS4] [float] NULL,
	[Player1H2HTrueSkillStandardDeviationOldMS4] [float] NULL,
	[Player2H2HTrueSkillMeanOldMS4] [float] NULL,
	[Player2H2HTrueSkillStandardDeviationOldMS4] [float] NULL,
	[WinProbabilityPlayer1H2HMS4] [float] NULL,
	[Player1H2HTrueSkillMeanOldSMS4] [float] NULL,
	[Player1H2HTrueSkillStandardDeviationOldSMS4] [float] NULL,
	[Player2H2HTrueSkillMeanOldSMS4] [float] NULL,
	[Player2H2HTrueSkillStandardDeviationOldSMS4] [float] NULL,
	[WinProbabilityPlayer1H2HSMS4] [float] NULL,
	[Player1H2HTrueSkillMeanOldGSMS4] [float] NULL,
	[Player1H2HTrueSkillStandardDeviationOldGSMS4] [float] NULL,
	[Player2H2HTrueSkillMeanOldGSMS4] [float] NULL,
	[Player2H2HTrueSkillStandardDeviationOldGSMS4] [float] NULL,
	[WinProbabilityPlayer1H2HGSMS4] [float] NULL,
	[Player1Streak] [int] NULL,
	[Player2Streak] [int] NULL,
	[Player1StreakS1] [int] NULL,
	[Player2StreakS1] [int] NULL,
	[Player1StreakS2] [int] NULL,
	[Player2StreakS2] [int] NULL,
	[Player1StreakS3] [int] NULL,
	[Player2StreakS3] [int] NULL,
	[Player1StreakS4] [int] NULL,
	[Player2StreakS4] [int] NULL,
	[P1SetsWon] [int] NULL,
	[P2SetsWon] [int] NULL,
	[P1GamesWon] [int] NULL,
	[P2GamesWon] [int] NULL,
	[P1SetsLoss] [int] NULL,
	[P2SetsLoss] [int] NULL,
	[P1GamesLoss] [int] NULL,
	[P2GamesLoss] [int] NULL,
	[WinProbabilityNN] [float] NULL,
 CONSTRAINT [PK_MatchActive] PRIMARY KEY CLUSTERED 
(
	[MatchTPId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MatchActiveOdds]    Script Date: 19.7.2025. 11:43:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MatchActiveOdds](
	[MatchTPId] [int] NOT NULL,
	[BookieId] [tinyint] NULL,
	[DateTime] [datetime] NULL,
	[Player1Odds] [float] NULL,
	[Player2Odds] [float] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MatchOdds]    Script Date: 19.7.2025. 11:43:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MatchOdds](
	[MatchTPId] [int] NOT NULL,
	[BookieId] [tinyint] NULL,
	[DateTime] [datetime] NULL,
	[Player1Odds] [float] NULL,
	[Player2Odds] [float] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Player]    Script Date: 19.7.2025. 11:43:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Player](
	[PlayerTPId] [int] NOT NULL,
	[PlayerName] [nvarchar](50) NOT NULL,
	[CountryTPId] [tinyint] NULL,
	[PlayerBirthDate] [date] NULL,
	[PlayerHeight] [tinyint] NULL,
	[PlayerWeight] [tinyint] NULL,
	[PlayerTurnedPro] [int] NULL,
	[PlaysId] [tinyint] NULL,
	[TrueSkillMeanM] [float] NOT NULL,
	[TrueSkillStandardDeviationM] [float] NOT NULL,
	[TrueSkillMeanSM] [float] NOT NULL,
	[TrueSkillStandardDeviationSM] [float] NOT NULL,
	[TrueSkillMeanGSM] [float] NOT NULL,
	[TrueSkillStandardDeviationGSM] [float] NOT NULL,
	[TrueSkillMeanMS1] [float] NOT NULL,
	[TrueSkillStandardDeviationMS1] [float] NOT NULL,
	[TrueSkillMeanSMS1] [float] NOT NULL,
	[TrueSkillStandardDeviationSMS1] [float] NOT NULL,
	[TrueSkillMeanGSMS1] [float] NOT NULL,
	[TrueSkillStandardDeviationGSMS1] [float] NOT NULL,
	[TrueSkillMeanMS2] [float] NOT NULL,
	[TrueSkillStandardDeviationMS2] [float] NOT NULL,
	[TrueSkillMeanSMS2] [float] NOT NULL,
	[TrueSkillStandardDeviationSMS2] [float] NOT NULL,
	[TrueSkillMeanGSMS2] [float] NOT NULL,
	[TrueSkillStandardDeviationGSMS2] [float] NOT NULL,
	[TrueSkillMeanMS3] [float] NOT NULL,
	[TrueSkillStandardDeviationMS3] [float] NOT NULL,
	[TrueSkillMeanSMS3] [float] NOT NULL,
	[TrueSkillStandardDeviationSMS3] [float] NOT NULL,
	[TrueSkillMeanGSMS3] [float] NOT NULL,
	[TrueSkillStandardDeviationGSMS3] [float] NOT NULL,
	[TrueSkillMeanMS4] [float] NOT NULL,
	[TrueSkillStandardDeviationMS4] [float] NOT NULL,
	[TrueSkillMeanSMS4] [float] NOT NULL,
	[TrueSkillStandardDeviationSMS4] [float] NOT NULL,
	[TrueSkillMeanGSMS4] [float] NOT NULL,
	[TrueSkillStandardDeviationGSMS4] [float] NOT NULL,
	[WinsTotal] [int] NOT NULL,
	[LossesTotal] [int] NOT NULL,
	[WinsLastYear] [int] NOT NULL,
	[LossesLastYear] [int] NOT NULL,
	[WinsLastMonth] [int] NOT NULL,
	[LossesLastMonth] [int] NOT NULL,
	[WinsLastWeek] [int] NOT NULL,
	[LossesLastWeek] [int] NOT NULL,
	[WinsTotalS1] [int] NOT NULL,
	[LossesTotalS1] [int] NOT NULL,
	[WinsLastYearS1] [int] NOT NULL,
	[LossesLastYearS1] [int] NOT NULL,
	[WinsLastMonthS1] [int] NOT NULL,
	[LossesLastMonthS1] [int] NOT NULL,
	[WinsLastWeekS1] [int] NOT NULL,
	[LossesLastWeekS1] [int] NOT NULL,
	[WinsTotalS2] [int] NOT NULL,
	[LossesTotalS2] [int] NOT NULL,
	[WinsLastYearS2] [int] NOT NULL,
	[LossesLastYearS2] [int] NOT NULL,
	[WinsLastMonthS2] [int] NOT NULL,
	[LossesLastMonthS2] [int] NOT NULL,
	[WinsLastWeekS2] [int] NOT NULL,
	[LossesLastWeekS2] [int] NOT NULL,
	[WinsTotalS3] [int] NOT NULL,
	[LossesTotalS3] [int] NOT NULL,
	[WinsLastYearS3] [int] NOT NULL,
	[LossesLastYearS3] [int] NOT NULL,
	[WinsLastMonthS3] [int] NOT NULL,
	[LossesLastMonthS3] [int] NOT NULL,
	[WinsLastWeekS3] [int] NOT NULL,
	[LossesLastWeekS3] [int] NOT NULL,
	[WinsTotalS4] [int] NOT NULL,
	[LossesTotalS4] [int] NOT NULL,
	[WinsLastYearS4] [int] NOT NULL,
	[LossesLastYearS4] [int] NOT NULL,
	[WinsLastMonthS4] [int] NOT NULL,
	[LossesLastMonthS4] [int] NOT NULL,
	[WinsLastWeekS4] [int] NOT NULL,
	[LossesLastWeekS4] [int] NOT NULL,
	[WinsSetsTotal] [int] NOT NULL,
	[LossesSetsTotal] [int] NOT NULL,
	[WinsSetsLastYear] [int] NOT NULL,
	[LossesSetsLastYear] [int] NOT NULL,
	[WinsSetsLastMonth] [int] NOT NULL,
	[LossesSetsLastMonth] [int] NOT NULL,
	[WinsSetsLastWeek] [int] NOT NULL,
	[LossesSetsLastWeek] [int] NOT NULL,
	[WinsSetsTotalS1] [int] NOT NULL,
	[LossesSetsTotalS1] [int] NOT NULL,
	[WinsSetsLastYearS1] [int] NOT NULL,
	[LossesSetsLastYearS1] [int] NOT NULL,
	[WinsSetsLastMonthS1] [int] NOT NULL,
	[LossesSetsLastMonthS1] [int] NOT NULL,
	[WinsSetsLastWeekS1] [int] NOT NULL,
	[LossesSetsLastWeekS1] [int] NOT NULL,
	[WinsSetsTotalS2] [int] NOT NULL,
	[LossesSetsTotalS2] [int] NOT NULL,
	[WinsSetsLastYearS2] [int] NOT NULL,
	[LossesSetsLastYearS2] [int] NOT NULL,
	[WinsSetsLastMonthS2] [int] NOT NULL,
	[LossesSetsLastMonthS2] [int] NOT NULL,
	[WinsSetsLastWeekS2] [int] NOT NULL,
	[LossesSetsLastWeekS2] [int] NOT NULL,
	[WinsSetsTotalS3] [int] NOT NULL,
	[LossesSetsTotalS3] [int] NOT NULL,
	[WinsSetsLastYearS3] [int] NOT NULL,
	[LossesSetsLastYearS3] [int] NOT NULL,
	[WinsSetsLastMonthS3] [int] NOT NULL,
	[LossesSetsLastMonthS3] [int] NOT NULL,
	[WinsSetsLastWeekS3] [int] NOT NULL,
	[LossesSetsLastWeekS3] [int] NOT NULL,
	[WinsSetsTotalS4] [int] NOT NULL,
	[LossesSetsTotalS4] [int] NOT NULL,
	[WinsSetsLastYearS4] [int] NOT NULL,
	[LossesSetsLastYearS4] [int] NOT NULL,
	[WinsSetsLastMonthS4] [int] NOT NULL,
	[LossesSetsLastMonthS4] [int] NOT NULL,
	[WinsSetsLastWeekS4] [int] NOT NULL,
	[LossesSetsLastWeekS4] [int] NOT NULL,
	[WinsGamesTotal] [int] NOT NULL,
	[LossesGamesTotal] [int] NOT NULL,
	[WinsGamesLastYear] [int] NOT NULL,
	[LossesGamesLastYear] [int] NOT NULL,
	[WinsGamesLastMonth] [int] NOT NULL,
	[LossesGamesLastMonth] [int] NOT NULL,
	[WinsGamesLastWeek] [int] NOT NULL,
	[LossesGamesLastWeek] [int] NOT NULL,
	[WinsGamesTotalS1] [int] NOT NULL,
	[LossesGamesTotalS1] [int] NOT NULL,
	[WinsGamesLastYearS1] [int] NOT NULL,
	[LossesGamesLastYearS1] [int] NOT NULL,
	[WinsGamesLastMonthS1] [int] NOT NULL,
	[LossesGamesLastMonthS1] [int] NOT NULL,
	[WinsGamesLastWeekS1] [int] NOT NULL,
	[LossesGamesLastWeekS1] [int] NOT NULL,
	[WinsGamesTotalS2] [int] NOT NULL,
	[LossesGamesTotalS2] [int] NOT NULL,
	[WinsGamesLastYearS2] [int] NOT NULL,
	[LossesGamesLastYearS2] [int] NOT NULL,
	[WinsGamesLastMonthS2] [int] NOT NULL,
	[LossesGamesLastMonthS2] [int] NOT NULL,
	[WinsGamesLastWeekS2] [int] NOT NULL,
	[LossesGamesLastWeekS2] [int] NOT NULL,
	[WinsGamesTotalS3] [int] NOT NULL,
	[LossesGamesTotalS3] [int] NOT NULL,
	[WinsGamesLastYearS3] [int] NOT NULL,
	[LossesGamesLastYearS3] [int] NOT NULL,
	[WinsGamesLastMonthS3] [int] NOT NULL,
	[LossesGamesLastMonthS3] [int] NOT NULL,
	[WinsGamesLastWeekS3] [int] NOT NULL,
	[LossesGamesLastWeekS3] [int] NOT NULL,
	[WinsGamesTotalS4] [int] NOT NULL,
	[LossesGamesTotalS4] [int] NOT NULL,
	[WinsGamesLastYearS4] [int] NOT NULL,
	[LossesGamesLastYearS4] [int] NOT NULL,
	[WinsGamesLastMonthS4] [int] NOT NULL,
	[LossesGamesLastMonthS4] [int] NOT NULL,
	[WinsGamesLastWeekS4] [int] NOT NULL,
	[LossesGamesLastWeekS4] [int] NOT NULL,
	[DateSinceLastWin] [datetime] NULL,
	[DateSinceLastWinS1] [datetime] NULL,
	[DateSinceLastWinS2] [datetime] NULL,
	[DateSinceLastWinS3] [datetime] NULL,
	[DateSinceLastWinS4] [datetime] NULL,
	[DateSinceLastLoss] [datetime] NULL,
	[DateSinceLastLossS1] [datetime] NULL,
	[DateSinceLastLossS2] [datetime] NULL,
	[DateSinceLastLossS3] [datetime] NULL,
	[DateSinceLastLossS4] [datetime] NULL,
	[TotalWinsAsFavourite] [int] NULL,
	[TotalWinsAsUnderdog] [int] NULL,
	[TotalLossesAsFavourite] [int] NULL,
	[TotalLossesAsUnderdog] [int] NULL,
	[WinsAsFavouriteRatio] [float] NULL,
	[LossesAsFavouriteRatio] [float] NULL,
	[WinsAsUnderdogRatio] [float] NULL,
	[LossesAsUnderdogRatio] [float] NULL,
	[AverageWinningProbabilityAtWonAsFavourite] [float] NULL,
	[AverageWinningProbabilityAtWonAsUnderdog] [float] NULL,
	[AverageWinningProbabilityAtLossAsFavourite] [float] NULL,
	[AverageWinningProbabilityAtLossAsUnderdog] [float] NULL,
	[TotalWinsAsFavouriteLastYear] [int] NULL,
	[TotalWinsAsUnderdogLastYear] [int] NULL,
	[TotalLossesAsFavouriteLastYear] [int] NULL,
	[TotalLossesAsUnderdogLastYear] [int] NULL,
	[WinsAsFavouriteLastYearRatio] [float] NULL,
	[LossesAsFavouriteLastYearRatio] [float] NULL,
	[WinsAsUnderdogLastYearRatio] [float] NULL,
	[LossesAsUnderdogLastYearRatio] [float] NULL,
	[AverageWinningProbabilityAtWonAsFavouriteLastYear] [float] NULL,
	[AverageWinningProbabilityAtWonAsUnderdogLastYear] [float] NULL,
	[AverageWinningProbabilityAtLossAsFavouriteLastYear] [float] NULL,
	[AverageWinningProbabilityAtLossAsUnderdogLastYear] [float] NULL,
	[TotalWinsAsFavouriteLastMonth] [int] NULL,
	[TotalWinsAsUnderdogLastMonth] [int] NULL,
	[TotalLossesAsFavouriteLastMonth] [int] NULL,
	[TotalLossesAsUnderdogLastMonth] [int] NULL,
	[WinsAsFavouriteLastMonthRatio] [float] NULL,
	[LossesAsFavouriteLastMonthRatio] [float] NULL,
	[WinsAsUnderdogLastMonthRatio] [float] NULL,
	[LossesAsUnderdogLastMonthRatio] [float] NULL,
	[AverageWinningProbabilityAtWonAsFavouriteLastMonth] [float] NULL,
	[AverageWinningProbabilityAtWonAsUnderdogLastMonth] [float] NULL,
	[AverageWinningProbabilityAtLossAsFavouriteLastMonth] [float] NULL,
	[AverageWinningProbabilityAtLossAsUnderdogLastMonth] [float] NULL,
	[TotalWinsAsFavouriteLastWeek] [int] NULL,
	[TotalWinsAsUnderdogLastWeek] [int] NULL,
	[TotalLossesAsFavouriteLastWeek] [int] NULL,
	[TotalLossesAsUnderdogLastWeek] [int] NULL,
	[WinsAsFavouriteLastWeekRatio] [float] NULL,
	[LossesAsFavouriteLastWeekRatio] [float] NULL,
	[WinsAsUnderdogLastWeekRatio] [float] NULL,
	[LossesAsUnderdogLastWeekRatio] [float] NULL,
	[AverageWinningProbabilityAtWonAsFavouriteLastWeek] [float] NULL,
	[AverageWinningProbabilityAtWonAsUnderdogLastWeek] [float] NULL,
	[AverageWinningProbabilityAtLossAsFavouriteLastWeek] [float] NULL,
	[AverageWinningProbabilityAtLossAsUnderdogLastWeek] [float] NULL,
	[Streak] [int] NULL,
	[StreakS1] [int] NULL,
	[StreakS2] [int] NULL,
	[StreakS3] [int] NULL,
	[StreakS4] [int] NULL,
 CONSTRAINT [PK_Player] PRIMARY KEY CLUSTERED 
(
	[PlayerTPId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Plays]    Script Date: 19.7.2025. 11:43:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Plays](
	[PlaysId] [tinyint] NOT NULL,
	[Plays] [varchar](13) NOT NULL,
 CONSTRAINT [PK_Plays] PRIMARY KEY CLUSTERED 
(
	[PlaysId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Round]    Script Date: 19.7.2025. 11:43:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Round](
	[RoundId] [tinyint] NOT NULL,
	[RoundName] [varchar](20) NOT NULL,
 CONSTRAINT [PK_Round] PRIMARY KEY CLUSTERED 
(
	[RoundId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Surface]    Script Date: 19.7.2025. 11:43:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Surface](
	[SurfaceId] [tinyint] NOT NULL,
	[Surface] [varchar](20) NOT NULL,
 CONSTRAINT [PK_Surface] PRIMARY KEY CLUSTERED 
(
	[SurfaceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TournamentEvent]    Script Date: 19.7.2025. 11:43:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TournamentEvent](
	[TournamentEventTPId] [int] NOT NULL,
	[TournamentEventName] [varchar](max) NULL,
	[CountryTPId] [tinyint] NULL,
	[TournamentEventDate] [date] NULL,
	[TournamentLevelId] [tinyint] NULL,
	[TournamentTypeId] [tinyint] NULL,
	[Prize] [int] NULL,
	[SurfaceId] [tinyint] NULL,
 CONSTRAINT [PK_TournamentEvent] PRIMARY KEY CLUSTERED 
(
	[TournamentEventTPId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TournamentLevel]    Script Date: 19.7.2025. 11:43:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TournamentLevel](
	[TournamentLevelId] [tinyint] NOT NULL,
	[TournamentLevelName] [nchar](20) NOT NULL,
 CONSTRAINT [PK_TournamentLevel] PRIMARY KEY CLUSTERED 
(
	[TournamentLevelId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TournamentType]    Script Date: 19.7.2025. 11:43:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TournamentType](
	[TournamentTypeId] [tinyint] NOT NULL,
	[TournamentTypeName] [varchar](11) NOT NULL,
 CONSTRAINT [PK_TournamentType] PRIMARY KEY CLUSTERED 
(
	[TournamentTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User]    Script Date: 19.7.2025. 11:43:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[UserFirstName] [varchar](100) NOT NULL,
	[UserLastName] [varchar](100) NOT NULL,
	[UserMail] [varchar](100) NOT NULL,
	[UserPassword] [varchar](100) NOT NULL,
	[UserTypeId] [tinyint] NOT NULL,
	[Registered] [bit] NOT NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserType]    Script Date: 19.7.2025. 11:43:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserType](
	[UserTypeId] [tinyint] NOT NULL,
	[UserTypeName] [varchar](50) NOT NULL,
 CONSTRAINT [PK_UserType] PRIMARY KEY CLUSTERED 
(
	[UserTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Index [IX_Match_DateTime]    Script Date: 19.7.2025. 11:43:54 ******/
CREATE NONCLUSTERED INDEX [IX_Match_DateTime] ON [dbo].[Match]
(
	[DateTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Match_Player2TPId]    Script Date: 19.7.2025. 11:43:54 ******/
CREATE NONCLUSTERED INDEX [IX_Match_Player2TPId] ON [dbo].[Match]
(
	[Player2TPId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Match_RoundId]    Script Date: 19.7.2025. 11:43:54 ******/
CREATE NONCLUSTERED INDEX [IX_Match_RoundId] ON [dbo].[Match]
(
	[RoundId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Match_SurfaceId]    Script Date: 19.7.2025. 11:43:54 ******/
CREATE NONCLUSTERED INDEX [IX_Match_SurfaceId] ON [dbo].[Match]
(
	[SurfaceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Match_TournamentEventTPId]    Script Date: 19.7.2025. 11:43:54 ******/
CREATE NONCLUSTERED INDEX [IX_Match_TournamentEventTPId] ON [dbo].[Match]
(
	[TournamentEventTPId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_MatchActive_DateTime]    Script Date: 19.7.2025. 11:43:54 ******/
CREATE NONCLUSTERED INDEX [IX_MatchActive_DateTime] ON [dbo].[MatchActive]
(
	[DateTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_MatchActive_Player1TPId]    Script Date: 19.7.2025. 11:43:54 ******/
CREATE NONCLUSTERED INDEX [IX_MatchActive_Player1TPId] ON [dbo].[MatchActive]
(
	[Player1TPID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_MatchActive_Player2TPId]    Script Date: 19.7.2025. 11:43:54 ******/
CREATE NONCLUSTERED INDEX [IX_MatchActive_Player2TPId] ON [dbo].[MatchActive]
(
	[Player2TPID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_MatchActive_RoundId]    Script Date: 19.7.2025. 11:43:54 ******/
CREATE NONCLUSTERED INDEX [IX_MatchActive_RoundId] ON [dbo].[MatchActive]
(
	[RoundId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_MatchActive_SurfaceId]    Script Date: 19.7.2025. 11:43:54 ******/
CREATE NONCLUSTERED INDEX [IX_MatchActive_SurfaceId] ON [dbo].[MatchActive]
(
	[SurfaceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_MatchActive_TournamentEventTPId]    Script Date: 19.7.2025. 11:43:54 ******/
CREATE NONCLUSTERED INDEX [IX_MatchActive_TournamentEventTPId] ON [dbo].[MatchActive]
(
	[TournamentEventTPId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_MatchActiveOdds_BookieId]    Script Date: 19.7.2025. 11:43:54 ******/
CREATE NONCLUSTERED INDEX [IX_MatchActiveOdds_BookieId] ON [dbo].[MatchActiveOdds]
(
	[BookieId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_MatchActiveOdds_MatchTPId]    Script Date: 19.7.2025. 11:43:54 ******/
CREATE NONCLUSTERED INDEX [IX_MatchActiveOdds_MatchTPId] ON [dbo].[MatchActiveOdds]
(
	[MatchTPId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_MatchOdds_BookieId]    Script Date: 19.7.2025. 11:43:54 ******/
CREATE NONCLUSTERED INDEX [IX_MatchOdds_BookieId] ON [dbo].[MatchOdds]
(
	[BookieId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_MatchOdds_MatchTPId]    Script Date: 19.7.2025. 11:43:54 ******/
CREATE NONCLUSTERED INDEX [IX_MatchOdds_MatchTPId] ON [dbo].[MatchOdds]
(
	[MatchTPId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Player_CountryTPId]    Script Date: 19.7.2025. 11:43:54 ******/
CREATE NONCLUSTERED INDEX [IX_Player_CountryTPId] ON [dbo].[Player]
(
	[CountryTPId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Player_PlaysId]    Script Date: 19.7.2025. 11:43:54 ******/
CREATE NONCLUSTERED INDEX [IX_Player_PlaysId] ON [dbo].[Player]
(
	[PlaysId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TournamentEvent_CountryTPId]    Script Date: 19.7.2025. 11:43:54 ******/
CREATE NONCLUSTERED INDEX [IX_TournamentEvent_CountryTPId] ON [dbo].[TournamentEvent]
(
	[CountryTPId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TournamentEvent_SurfaceId]    Script Date: 19.7.2025. 11:43:54 ******/
CREATE NONCLUSTERED INDEX [IX_TournamentEvent_SurfaceId] ON [dbo].[TournamentEvent]
(
	[SurfaceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TournamentEvent_TournamentLevelId]    Script Date: 19.7.2025. 11:43:54 ******/
CREATE NONCLUSTERED INDEX [IX_TournamentEvent_TournamentLevelId] ON [dbo].[TournamentEvent]
(
	[TournamentLevelId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TournamentEvent_TournamentTypeId]    Script Date: 19.7.2025. 11:43:54 ******/
CREATE NONCLUSTERED INDEX [IX_TournamentEvent_TournamentTypeId] ON [dbo].[TournamentEvent]
(
	[TournamentTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_User_UserTypeId]    Script Date: 19.7.2025. 11:43:54 ******/
CREATE NONCLUSTERED INDEX [IX_User_UserTypeId] ON [dbo].[User]
(
	[UserTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Match]  WITH CHECK ADD  CONSTRAINT [FK_Match_Player1] FOREIGN KEY([Player1TPId])
REFERENCES [dbo].[Player] ([PlayerTPId])
GO
ALTER TABLE [dbo].[Match] CHECK CONSTRAINT [FK_Match_Player1]
GO
ALTER TABLE [dbo].[Match]  WITH CHECK ADD  CONSTRAINT [FK_Match_Player2] FOREIGN KEY([Player2TPId])
REFERENCES [dbo].[Player] ([PlayerTPId])
GO
ALTER TABLE [dbo].[Match] CHECK CONSTRAINT [FK_Match_Player2]
GO
ALTER TABLE [dbo].[Match]  WITH CHECK ADD  CONSTRAINT [FK_Match_Round] FOREIGN KEY([RoundId])
REFERENCES [dbo].[Round] ([RoundId])
GO
ALTER TABLE [dbo].[Match] CHECK CONSTRAINT [FK_Match_Round]
GO
ALTER TABLE [dbo].[Match]  WITH CHECK ADD  CONSTRAINT [FK_Match_Surface] FOREIGN KEY([SurfaceId])
REFERENCES [dbo].[Surface] ([SurfaceId])
GO
ALTER TABLE [dbo].[Match] CHECK CONSTRAINT [FK_Match_Surface]
GO
ALTER TABLE [dbo].[Match]  WITH CHECK ADD  CONSTRAINT [FK_Match_TournamentEvent] FOREIGN KEY([TournamentEventTPId])
REFERENCES [dbo].[TournamentEvent] ([TournamentEventTPId])
GO
ALTER TABLE [dbo].[Match] CHECK CONSTRAINT [FK_Match_TournamentEvent]
GO
ALTER TABLE [dbo].[MatchActive]  WITH CHECK ADD  CONSTRAINT [FK_MatchActive_Player1] FOREIGN KEY([Player1TPID])
REFERENCES [dbo].[Player] ([PlayerTPId])
GO
ALTER TABLE [dbo].[MatchActive] CHECK CONSTRAINT [FK_MatchActive_Player1]
GO
ALTER TABLE [dbo].[MatchActive]  WITH CHECK ADD  CONSTRAINT [FK_MatchActive_Player2] FOREIGN KEY([Player2TPID])
REFERENCES [dbo].[Player] ([PlayerTPId])
GO
ALTER TABLE [dbo].[MatchActive] CHECK CONSTRAINT [FK_MatchActive_Player2]
GO
ALTER TABLE [dbo].[MatchActive]  WITH CHECK ADD  CONSTRAINT [FK_MatchActive_Round] FOREIGN KEY([RoundId])
REFERENCES [dbo].[Round] ([RoundId])
GO
ALTER TABLE [dbo].[MatchActive] CHECK CONSTRAINT [FK_MatchActive_Round]
GO
ALTER TABLE [dbo].[MatchActive]  WITH CHECK ADD  CONSTRAINT [FK_MatchActive_Surface] FOREIGN KEY([SurfaceId])
REFERENCES [dbo].[Surface] ([SurfaceId])
GO
ALTER TABLE [dbo].[MatchActive] CHECK CONSTRAINT [FK_MatchActive_Surface]
GO
ALTER TABLE [dbo].[MatchActive]  WITH CHECK ADD  CONSTRAINT [FK_MatchActive_TournamentEvent] FOREIGN KEY([TournamentEventTPId])
REFERENCES [dbo].[TournamentEvent] ([TournamentEventTPId])
GO
ALTER TABLE [dbo].[MatchActive] CHECK CONSTRAINT [FK_MatchActive_TournamentEvent]
GO
ALTER TABLE [dbo].[MatchActiveOdds]  WITH CHECK ADD  CONSTRAINT [FK_MatchActiveOdds_Bookie] FOREIGN KEY([BookieId])
REFERENCES [dbo].[Bookie] ([BookieId])
GO
ALTER TABLE [dbo].[MatchActiveOdds] CHECK CONSTRAINT [FK_MatchActiveOdds_Bookie]
GO
ALTER TABLE [dbo].[MatchActiveOdds]  WITH CHECK ADD  CONSTRAINT [FK_MatchActiveOdds_Match] FOREIGN KEY([MatchTPId])
REFERENCES [dbo].[Match] ([MatchTPId])
GO
ALTER TABLE [dbo].[MatchActiveOdds] CHECK CONSTRAINT [FK_MatchActiveOdds_Match]
GO
ALTER TABLE [dbo].[MatchOdds]  WITH CHECK ADD  CONSTRAINT [FK_MatchOdds_Bookie] FOREIGN KEY([BookieId])
REFERENCES [dbo].[Bookie] ([BookieId])
GO
ALTER TABLE [dbo].[MatchOdds] CHECK CONSTRAINT [FK_MatchOdds_Bookie]
GO
ALTER TABLE [dbo].[MatchOdds]  WITH CHECK ADD  CONSTRAINT [FK_MatchOdds_Match] FOREIGN KEY([MatchTPId])
REFERENCES [dbo].[Match] ([MatchTPId])
GO
ALTER TABLE [dbo].[MatchOdds] CHECK CONSTRAINT [FK_MatchOdds_Match]
GO
ALTER TABLE [dbo].[Player]  WITH CHECK ADD  CONSTRAINT [FK_Player_Country] FOREIGN KEY([CountryTPId])
REFERENCES [dbo].[Country] ([CountryTPId])
GO
ALTER TABLE [dbo].[Player] CHECK CONSTRAINT [FK_Player_Country]
GO
ALTER TABLE [dbo].[Player]  WITH CHECK ADD  CONSTRAINT [FK_Player_Plays] FOREIGN KEY([PlaysId])
REFERENCES [dbo].[Plays] ([PlaysId])
GO
ALTER TABLE [dbo].[Player] CHECK CONSTRAINT [FK_Player_Plays]
GO
ALTER TABLE [dbo].[TournamentEvent]  WITH CHECK ADD  CONSTRAINT [FK_TournamentEvent_Country] FOREIGN KEY([CountryTPId])
REFERENCES [dbo].[Country] ([CountryTPId])
GO
ALTER TABLE [dbo].[TournamentEvent] CHECK CONSTRAINT [FK_TournamentEvent_Country]
GO
ALTER TABLE [dbo].[TournamentEvent]  WITH CHECK ADD  CONSTRAINT [FK_TournamentEvent_Surface] FOREIGN KEY([SurfaceId])
REFERENCES [dbo].[Surface] ([SurfaceId])
GO
ALTER TABLE [dbo].[TournamentEvent] CHECK CONSTRAINT [FK_TournamentEvent_Surface]
GO
ALTER TABLE [dbo].[TournamentEvent]  WITH CHECK ADD  CONSTRAINT [FK_TournamentEvent_TournamentLevel] FOREIGN KEY([TournamentLevelId])
REFERENCES [dbo].[TournamentLevel] ([TournamentLevelId])
GO
ALTER TABLE [dbo].[TournamentEvent] CHECK CONSTRAINT [FK_TournamentEvent_TournamentLevel]
GO
ALTER TABLE [dbo].[TournamentEvent]  WITH CHECK ADD  CONSTRAINT [FK_TournamentEvent_TournamentType] FOREIGN KEY([TournamentTypeId])
REFERENCES [dbo].[TournamentType] ([TournamentTypeId])
GO
ALTER TABLE [dbo].[TournamentEvent] CHECK CONSTRAINT [FK_TournamentEvent_TournamentType]
GO
ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FK_User_UserType] FOREIGN KEY([UserTypeId])
REFERENCES [dbo].[UserType] ([UserTypeId])
GO
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_User_UserType]
GO
/****** Object:  StoredProcedure [dbo].[GetLatestMatchDate]    Script Date: 19.7.2025. 11:43:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetLatestMatchDate]
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @LatestDate DATETIME;

    SELECT @LatestDate = MAX([DateTime])
    FROM [dbo].[Match];

    IF @LatestDate IS NULL
        SET @LatestDate = '19900101';

    SELECT @LatestDate AS LatestMatchDate;
END
GO
USE [master]
GO
ALTER DATABASE [BonzoByte] SET  READ_WRITE 
GO
