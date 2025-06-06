CREATE TABLE IF NOT EXISTS [Subscriptions]
(
    [CreatedAt] INT NOT NULL,
    [ChannelHandle] VARCHAR(250) NOT NULL,
    [ChannelId] VARCHAR(250) PRIMARY KEY NOT NULL
    [ChannelName] VARCHAR(250) NOT NULL,
    [LastModifiedAt] INT NOT NULL
)
WITHOUT ROWID;
