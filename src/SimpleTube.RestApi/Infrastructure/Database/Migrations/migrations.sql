CREATE TABLE IF NOT EXISTS [Channels]
(
    [Banner]         VARCHAR      NULL,
    [CreatedAt]      INT          NOT NULL,
    [Favorite]       INT          NOT NULL DEFAULT 0,
    [Handle]         VARCHAR(250) NOT NULL,
    [Id]             VARCHAR(250) NOT NULL PRIMARY KEY,
    [Name]           VARCHAR(250) NOT NULL,
    [Subscribed]     INT          NOT NULL DEFAULT 0,
    [Thumbnail]      VARCHAR      NOT NULL,
    [LastModifiedAt] INT          NOT NULL
)
WITHOUT ROWID;

CREATE TABLE IF NOT EXISTS [Videos]
(
    [ChannelId]   VARCHAR(250) NOT NULL,
    [Description] VARCHAR      NOT NULL,
    [Duration]    VARCHAR      NULL,
    [ETag]        VARCHAR      NULL,
    [Id]          VARCHAR(250) NOT NULL PRIMARY KEY,
    [Ignored]     INT          NOT NULL DEFAULT 0,
    [PublishedAt] INT          NOT NULL,
    [Thumbnail]   VARCHAR      NOT NULL,
    [Title]       VARCHAR(250) NOT NULL,
    [Watched]     INT          NOT NULL DEFAULT 0
)
WITHOUT ROWID;
