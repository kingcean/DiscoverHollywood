
USE [DiscoverHollywood]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Movies](
	[id] [int] NOT NULL,
	[name] [nchar](200) NOT NULL,
	[year] [int] NULL,
	[genres] [nchar](250) NULL,
	[thumbnail] [nchar](250) NULL,
	[rating] [float] NULL,
	[intro] [nchar](250) NULL,
	[region] [nchar](10) NULL,
	[imdb] [int] NULL,
	[tmdb] [int] NULL,
 CONSTRAINT [PK_Movies] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[RatingSummary](
	[id] [nchar](36) NOT NULL,
	[movie] [int] NOT NULL,
	[rating] [float] NOT NULL,
	[count] [int] NOT NULL,
	[year] [int] NOT NULL,
 CONSTRAINT [PK_RatingSummary] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Tags](
	[id] [nchar](36) NOT NULL,
	[movie] [int] NOT NULL,
	[user] [int] NOT NULL,
	[tag] [nchar](250) NOT NULL,
	[created] [datetime] NOT NULL,
 CONSTRAINT [PK_Tags] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


GO
