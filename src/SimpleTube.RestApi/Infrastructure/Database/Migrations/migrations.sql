CREATE TABLE IF NOT EXISTS [Subscriptions]
(
    [CreatedAt]        INT          NOT NULL,
    [ChannelHandle]    VARCHAR(250) NOT NULL,
    [ChannelId]        VARCHAR(250) NOT NULL PRIMARY KEY,
    [ChannelName]      VARCHAR(250) NOT NULL,
    [ChannelThumbnail] VARCHAR      NOT NULL,
    [LastModifiedAt]   INT          NOT NULL
)
WITHOUT ROWID;
