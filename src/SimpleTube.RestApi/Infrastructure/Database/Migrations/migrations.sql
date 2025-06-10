CREATE TABLE IF NOT EXISTS [Channels]
(
    [Banner]         VARCHAR      NULL,
    [CreatedAt]      INT          NOT NULL,
    [Favorite]       INT          NOT NULL DEFAULT 0,
    [Handle]         VARCHAR(250) NOT NULL,
    [Id]             VARCHAR(250) NOT NULL PRIMARY KEY,
    [Name]           VARCHAR(250) NOT NULL,
    [Thumbnail]      VARCHAR      NOT NULL,
    [LastModifiedAt] INT          NOT NULL
)
WITHOUT ROWID;

ALTER TABLE [Channels] ADD COLUMN [Favorite] INT NOT NULL DEFAULT 0;
ALTER TABLE [Channels] ADD COLUMN [Banner] VARCHAR;
ALTER TABLE [Channels] RENAME [ChannelHandle] TO [Handle];
ALTER TABLE [Channels] RENAME [ChannelId] TO [Id];
ALTER TABLE [Channels] RENAME [ChannelName] TO [Name];
ALTER TABLE [Channels] RENAME [ChannelThumbnail] TO [Thumbnail];

CREATE TABLE IF NOT EXISTS [Videos]
(
    [ChannelId]   VARCHAR(250) NOT NULL,
    [Description] VARCHAR      NOT NULL,
    [Id]          VARCHAR(250) NOT NULL PRIMARY KEY,
    [Ignored]     INT          NOT NULL DEFAULT 0,
    [PublishedAt] INT          NOT NULL,
    [Thumbnail]   VARCHAR      NOT NULL,
    [Title]       VARCHAR(250) NOT NULL,
    [Url]         VARCHAR      NULL,
    [Watched]     INT          NOT NULL DEFAULT 0
)
WITHOUT ROWID;

ALTER TABLE [Videos]
    ADD COLUMN [Watched] INT NOT NULL DEFAULT 0;
ALTER TABLE [Videos]
    ADD COLUMN [Watched] INT NOT NULL DEFAULT 0;
ALTER TABLE [Videos]
    ADD COLUMN [Ignored] INT NOT NULL DEFAULT 0;
