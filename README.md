CREATE TABLE [dbo].[tblTask](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](50) NULL,
	[Description] [nvarchar](max) NULL,
	[Assignee] [nvarchar](50) NULL,
	[DueDate] [datetime] NULL,
	[EmailId] [nvarchar](200) NULL,
	[StatusId] [nvarchar](20) NULL,
 CONSTRAINT [PK__Task__3214EC07FECCEE0C] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
