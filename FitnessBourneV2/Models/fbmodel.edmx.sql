
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 10/07/2018 07:05:49
-- Generated from EDMX file: \\ad.monash.edu\home\User046\sjay0010\Desktop\VS-2018\FitnessBourneV2\Models\fbmodel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_MemberTableFitnessClub]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MemberTables] DROP CONSTRAINT [FK_MemberTableFitnessClub];
GO
IF OBJECT_ID(N'[dbo].[FK_LocationTableEventTable_LocationTable]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[LocationTableEventTable] DROP CONSTRAINT [FK_LocationTableEventTable_LocationTable];
GO
IF OBJECT_ID(N'[dbo].[FK_LocationTableEventTable_EventTable]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[LocationTableEventTable] DROP CONSTRAINT [FK_LocationTableEventTable_EventTable];
GO
IF OBJECT_ID(N'[dbo].[FK_NotificationTableEventEdit]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[EventEdits] DROP CONSTRAINT [FK_NotificationTableEventEdit];
GO
IF OBJECT_ID(N'[dbo].[FK_EventEditMemberTable]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[EventEdits] DROP CONSTRAINT [FK_EventEditMemberTable];
GO
IF OBJECT_ID(N'[dbo].[FK_EventEditEventTable]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[EventEdits] DROP CONSTRAINT [FK_EventEditEventTable];
GO
IF OBJECT_ID(N'[dbo].[FK_AddressTableFitnessClub]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[FitnessClubs] DROP CONSTRAINT [FK_AddressTableFitnessClub];
GO
IF OBJECT_ID(N'[dbo].[FK_AddressTableMemberTable]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MemberTables] DROP CONSTRAINT [FK_AddressTableMemberTable];
GO
IF OBJECT_ID(N'[dbo].[FK_AddressTableLocationTable]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[LocationTables] DROP CONSTRAINT [FK_AddressTableLocationTable];
GO
IF OBJECT_ID(N'[dbo].[FK_EventTypeEventTable]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[EventTables] DROP CONSTRAINT [FK_EventTypeEventTable];
GO
IF OBJECT_ID(N'[dbo].[FK_EventTableFitnessClub]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[FitnessClubs] DROP CONSTRAINT [FK_EventTableFitnessClub];
GO
IF OBJECT_ID(N'[dbo].[FK_EventMembersEventTable]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[EventMembers] DROP CONSTRAINT [FK_EventMembersEventTable];
GO
IF OBJECT_ID(N'[dbo].[FK_EventMembersMemberTable]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[EventMembers] DROP CONSTRAINT [FK_EventMembersMemberTable];
GO
IF OBJECT_ID(N'[dbo].[FK_EventTableMemberTable]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[EventTables] DROP CONSTRAINT [FK_EventTableMemberTable];
GO
IF OBJECT_ID(N'[dbo].[FK_NotificationActionTableMemberTable]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[NotificationActionTables] DROP CONSTRAINT [FK_NotificationActionTableMemberTable];
GO
IF OBJECT_ID(N'[dbo].[FK_NotificationTableNotificationActionTable]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[NotificationActionTables] DROP CONSTRAINT [FK_NotificationTableNotificationActionTable];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FitnessClubs]', 'U') IS NOT NULL
    DROP TABLE [dbo].[FitnessClubs];
GO
IF OBJECT_ID(N'[dbo].[AddressTables]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AddressTables];
GO
IF OBJECT_ID(N'[dbo].[MemberTables]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MemberTables];
GO
IF OBJECT_ID(N'[dbo].[EventTables]', 'U') IS NOT NULL
    DROP TABLE [dbo].[EventTables];
GO
IF OBJECT_ID(N'[dbo].[NotificationActionTables]', 'U') IS NOT NULL
    DROP TABLE [dbo].[NotificationActionTables];
GO
IF OBJECT_ID(N'[dbo].[NotificationTables]', 'U') IS NOT NULL
    DROP TABLE [dbo].[NotificationTables];
GO
IF OBJECT_ID(N'[dbo].[EventEdits]', 'U') IS NOT NULL
    DROP TABLE [dbo].[EventEdits];
GO
IF OBJECT_ID(N'[dbo].[LocationTables]', 'U') IS NOT NULL
    DROP TABLE [dbo].[LocationTables];
GO
IF OBJECT_ID(N'[dbo].[EventTypes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[EventTypes];
GO
IF OBJECT_ID(N'[dbo].[EventMembers]', 'U') IS NOT NULL
    DROP TABLE [dbo].[EventMembers];
GO
IF OBJECT_ID(N'[dbo].[LocationTableEventTable]', 'U') IS NOT NULL
    DROP TABLE [dbo].[LocationTableEventTable];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'FitnessClubs'
CREATE TABLE [dbo].[FitnessClubs] (
    [FC_Id] int IDENTITY(1,1) NOT NULL,
    [FC_Ref_Name] nvarchar(max)  NOT NULL,
    [AddressTable_Adr_Id] int  NULL,
    [EventTable_Evnt_Id] int  NULL
);
GO

-- Creating table 'AddressTables'
CREATE TABLE [dbo].[AddressTables] (
    [Adr_Id] int IDENTITY(1,1) NOT NULL,
    [Adr_Lat] float  NOT NULL,
    [Adr_Long] float  NOT NULL,
    [Adr_Unit_No] nvarchar(max)  NOT NULL,
    [Adr_House_No] nvarchar(max)  NOT NULL,
    [Adr_Street_Name] nvarchar(max)  NOT NULL,
    [Adr_Suburb_Name] nvarchar(max)  NOT NULL,
    [Adr_City_Name] nvarchar(max)  NOT NULL,
    [Adr_Zipcode] nvarchar(max)  NOT NULL,
    [Adr_State_Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'MemberTables'
CREATE TABLE [dbo].[MemberTables] (
    [Mem_Id] int IDENTITY(1,1) NOT NULL,
    [Mem_Contact_No] nvarchar(max)  NOT NULL,
    [Mem_Email_Id] nvarchar(max)  NOT NULL,
    [FitnessClubFC_Id] int  NOT NULL,
    [Mem_FirstName] nvarchar(max)  NOT NULL,
    [Mem_GivenName] nvarchar(max)  NOT NULL,
    [AddressTable_Adr_Id] int  NULL
);
GO

-- Creating table 'EventTables'
CREATE TABLE [dbo].[EventTables] (
    [Evnt_Id] int IDENTITY(1,1) NOT NULL,
    [Evnt_Is_Private] tinyint  NOT NULL,
    [Evnt_Start_DateTime] datetime  NOT NULL,
    [Evnt_End_DateTime] datetime  NOT NULL,
    [Evnt_Is_Checkd_In] tinyint  NOT NULL,
    [Evnt_Capacity] int  NOT NULL,
    [EventTypeET_Id] int  NOT NULL,
    [Evnt_NavigDetails] nvarchar(max)  NOT NULL,
    [Evnt_IsEdit] bit  NOT NULL,
    [MemberTable_Mem_Id] int  NOT NULL
);
GO

-- Creating table 'NotificationActionTables'
CREATE TABLE [dbo].[NotificationActionTables] (
    [NA_Id] int IDENTITY(1,1) NOT NULL,
    [NA_Decision] nvarchar(max)  NOT NULL,
    [MemberTableMem_Id] int  NULL,
    [NotificationTableNotif_Id] int  NULL,
    [MemberTable_Mem_Id] int  NULL,
    [EventTable_Evnt_Id] int  NULL
);
GO

-- Creating table 'NotificationTables'
CREATE TABLE [dbo].[NotificationTables] (
    [Notif_Id] int IDENTITY(1,1) NOT NULL,
    [Notif_Type] nvarchar(max)  NOT NULL,
    [Notif_Message] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'EventEdits'
CREATE TABLE [dbo].[EventEdits] (
    [EE_Id] int IDENTITY(1,1) NOT NULL,
    [EE_DateTime] datetime  NOT NULL,
    [EE_EventIdToEdit] int  NOT NULL,
    [NotificationTable_Notif_Id] int  NULL,
    [Creator_Mem_Id] int  NOT NULL,
    [EventTable_Evnt_Id] int  NOT NULL
);
GO

-- Creating table 'LocationTables'
CREATE TABLE [dbo].[LocationTables] (
    [Loc_Id] int IDENTITY(1,1) NOT NULL,
    [Loc_Ref_Name] nvarchar(max)  NOT NULL,
    [AddressTable_Adr_Id] int  NULL
);
GO

-- Creating table 'EventTypes'
CREATE TABLE [dbo].[EventTypes] (
    [ET_Id] int IDENTITY(1,1) NOT NULL,
    [ET_Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'EventMembers'
CREATE TABLE [dbo].[EventMembers] (
    [EvMem_Id] int IDENTITY(1,1) NOT NULL,
    [EventTable_Evnt_Id] int  NULL,
    [MemberTable_Mem_Id] int  NULL
);
GO

-- Creating table 'LocationTableEventTable'
CREATE TABLE [dbo].[LocationTableEventTable] (
    [LocationTables_Loc_Id] int  NOT NULL,
    [EventTables_Evnt_Id] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [FC_Id] in table 'FitnessClubs'
ALTER TABLE [dbo].[FitnessClubs]
ADD CONSTRAINT [PK_FitnessClubs]
    PRIMARY KEY CLUSTERED ([FC_Id] ASC);
GO

-- Creating primary key on [Adr_Id] in table 'AddressTables'
ALTER TABLE [dbo].[AddressTables]
ADD CONSTRAINT [PK_AddressTables]
    PRIMARY KEY CLUSTERED ([Adr_Id] ASC);
GO

-- Creating primary key on [Mem_Id] in table 'MemberTables'
ALTER TABLE [dbo].[MemberTables]
ADD CONSTRAINT [PK_MemberTables]
    PRIMARY KEY CLUSTERED ([Mem_Id] ASC);
GO

-- Creating primary key on [Evnt_Id] in table 'EventTables'
ALTER TABLE [dbo].[EventTables]
ADD CONSTRAINT [PK_EventTables]
    PRIMARY KEY CLUSTERED ([Evnt_Id] ASC);
GO

-- Creating primary key on [NA_Id] in table 'NotificationActionTables'
ALTER TABLE [dbo].[NotificationActionTables]
ADD CONSTRAINT [PK_NotificationActionTables]
    PRIMARY KEY CLUSTERED ([NA_Id] ASC);
GO

-- Creating primary key on [Notif_Id] in table 'NotificationTables'
ALTER TABLE [dbo].[NotificationTables]
ADD CONSTRAINT [PK_NotificationTables]
    PRIMARY KEY CLUSTERED ([Notif_Id] ASC);
GO

-- Creating primary key on [EE_Id] in table 'EventEdits'
ALTER TABLE [dbo].[EventEdits]
ADD CONSTRAINT [PK_EventEdits]
    PRIMARY KEY CLUSTERED ([EE_Id] ASC);
GO

-- Creating primary key on [Loc_Id] in table 'LocationTables'
ALTER TABLE [dbo].[LocationTables]
ADD CONSTRAINT [PK_LocationTables]
    PRIMARY KEY CLUSTERED ([Loc_Id] ASC);
GO

-- Creating primary key on [ET_Id] in table 'EventTypes'
ALTER TABLE [dbo].[EventTypes]
ADD CONSTRAINT [PK_EventTypes]
    PRIMARY KEY CLUSTERED ([ET_Id] ASC);
GO

-- Creating primary key on [EvMem_Id] in table 'EventMembers'
ALTER TABLE [dbo].[EventMembers]
ADD CONSTRAINT [PK_EventMembers]
    PRIMARY KEY CLUSTERED ([EvMem_Id] ASC);
GO

-- Creating primary key on [LocationTables_Loc_Id], [EventTables_Evnt_Id] in table 'LocationTableEventTable'
ALTER TABLE [dbo].[LocationTableEventTable]
ADD CONSTRAINT [PK_LocationTableEventTable]
    PRIMARY KEY CLUSTERED ([LocationTables_Loc_Id], [EventTables_Evnt_Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [FitnessClubFC_Id] in table 'MemberTables'
ALTER TABLE [dbo].[MemberTables]
ADD CONSTRAINT [FK_MemberTableFitnessClub]
    FOREIGN KEY ([FitnessClubFC_Id])
    REFERENCES [dbo].[FitnessClubs]
        ([FC_Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_MemberTableFitnessClub'
CREATE INDEX [IX_FK_MemberTableFitnessClub]
ON [dbo].[MemberTables]
    ([FitnessClubFC_Id]);
GO

-- Creating foreign key on [LocationTables_Loc_Id] in table 'LocationTableEventTable'
ALTER TABLE [dbo].[LocationTableEventTable]
ADD CONSTRAINT [FK_LocationTableEventTable_LocationTable]
    FOREIGN KEY ([LocationTables_Loc_Id])
    REFERENCES [dbo].[LocationTables]
        ([Loc_Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [EventTables_Evnt_Id] in table 'LocationTableEventTable'
ALTER TABLE [dbo].[LocationTableEventTable]
ADD CONSTRAINT [FK_LocationTableEventTable_EventTable]
    FOREIGN KEY ([EventTables_Evnt_Id])
    REFERENCES [dbo].[EventTables]
        ([Evnt_Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_LocationTableEventTable_EventTable'
CREATE INDEX [IX_FK_LocationTableEventTable_EventTable]
ON [dbo].[LocationTableEventTable]
    ([EventTables_Evnt_Id]);
GO

-- Creating foreign key on [NotificationTable_Notif_Id] in table 'EventEdits'
ALTER TABLE [dbo].[EventEdits]
ADD CONSTRAINT [FK_NotificationTableEventEdit]
    FOREIGN KEY ([NotificationTable_Notif_Id])
    REFERENCES [dbo].[NotificationTables]
        ([Notif_Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_NotificationTableEventEdit'
CREATE INDEX [IX_FK_NotificationTableEventEdit]
ON [dbo].[EventEdits]
    ([NotificationTable_Notif_Id]);
GO

-- Creating foreign key on [Creator_Mem_Id] in table 'EventEdits'
ALTER TABLE [dbo].[EventEdits]
ADD CONSTRAINT [FK_EventEditMemberTable]
    FOREIGN KEY ([Creator_Mem_Id])
    REFERENCES [dbo].[MemberTables]
        ([Mem_Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_EventEditMemberTable'
CREATE INDEX [IX_FK_EventEditMemberTable]
ON [dbo].[EventEdits]
    ([Creator_Mem_Id]);
GO

-- Creating foreign key on [EventTable_Evnt_Id] in table 'EventEdits'
ALTER TABLE [dbo].[EventEdits]
ADD CONSTRAINT [FK_EventEditEventTable]
    FOREIGN KEY ([EventTable_Evnt_Id])
    REFERENCES [dbo].[EventTables]
        ([Evnt_Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_EventEditEventTable'
CREATE INDEX [IX_FK_EventEditEventTable]
ON [dbo].[EventEdits]
    ([EventTable_Evnt_Id]);
GO

-- Creating foreign key on [AddressTable_Adr_Id] in table 'FitnessClubs'
ALTER TABLE [dbo].[FitnessClubs]
ADD CONSTRAINT [FK_AddressTableFitnessClub]
    FOREIGN KEY ([AddressTable_Adr_Id])
    REFERENCES [dbo].[AddressTables]
        ([Adr_Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AddressTableFitnessClub'
CREATE INDEX [IX_FK_AddressTableFitnessClub]
ON [dbo].[FitnessClubs]
    ([AddressTable_Adr_Id]);
GO

-- Creating foreign key on [AddressTable_Adr_Id] in table 'MemberTables'
ALTER TABLE [dbo].[MemberTables]
ADD CONSTRAINT [FK_AddressTableMemberTable]
    FOREIGN KEY ([AddressTable_Adr_Id])
    REFERENCES [dbo].[AddressTables]
        ([Adr_Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AddressTableMemberTable'
CREATE INDEX [IX_FK_AddressTableMemberTable]
ON [dbo].[MemberTables]
    ([AddressTable_Adr_Id]);
GO

-- Creating foreign key on [AddressTable_Adr_Id] in table 'LocationTables'
ALTER TABLE [dbo].[LocationTables]
ADD CONSTRAINT [FK_AddressTableLocationTable]
    FOREIGN KEY ([AddressTable_Adr_Id])
    REFERENCES [dbo].[AddressTables]
        ([Adr_Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AddressTableLocationTable'
CREATE INDEX [IX_FK_AddressTableLocationTable]
ON [dbo].[LocationTables]
    ([AddressTable_Adr_Id]);
GO

-- Creating foreign key on [EventTypeET_Id] in table 'EventTables'
ALTER TABLE [dbo].[EventTables]
ADD CONSTRAINT [FK_EventTypeEventTable]
    FOREIGN KEY ([EventTypeET_Id])
    REFERENCES [dbo].[EventTypes]
        ([ET_Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_EventTypeEventTable'
CREATE INDEX [IX_FK_EventTypeEventTable]
ON [dbo].[EventTables]
    ([EventTypeET_Id]);
GO

-- Creating foreign key on [EventTable_Evnt_Id] in table 'FitnessClubs'
ALTER TABLE [dbo].[FitnessClubs]
ADD CONSTRAINT [FK_EventTableFitnessClub]
    FOREIGN KEY ([EventTable_Evnt_Id])
    REFERENCES [dbo].[EventTables]
        ([Evnt_Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_EventTableFitnessClub'
CREATE INDEX [IX_FK_EventTableFitnessClub]
ON [dbo].[FitnessClubs]
    ([EventTable_Evnt_Id]);
GO

-- Creating foreign key on [EventTable_Evnt_Id] in table 'EventMembers'
ALTER TABLE [dbo].[EventMembers]
ADD CONSTRAINT [FK_EventMembersEventTable]
    FOREIGN KEY ([EventTable_Evnt_Id])
    REFERENCES [dbo].[EventTables]
        ([Evnt_Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_EventMembersEventTable'
CREATE INDEX [IX_FK_EventMembersEventTable]
ON [dbo].[EventMembers]
    ([EventTable_Evnt_Id]);
GO

-- Creating foreign key on [MemberTable_Mem_Id] in table 'EventMembers'
ALTER TABLE [dbo].[EventMembers]
ADD CONSTRAINT [FK_EventMembersMemberTable]
    FOREIGN KEY ([MemberTable_Mem_Id])
    REFERENCES [dbo].[MemberTables]
        ([Mem_Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_EventMembersMemberTable'
CREATE INDEX [IX_FK_EventMembersMemberTable]
ON [dbo].[EventMembers]
    ([MemberTable_Mem_Id]);
GO

-- Creating foreign key on [MemberTable_Mem_Id] in table 'EventTables'
ALTER TABLE [dbo].[EventTables]
ADD CONSTRAINT [FK_EventTableMemberTable]
    FOREIGN KEY ([MemberTable_Mem_Id])
    REFERENCES [dbo].[MemberTables]
        ([Mem_Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_EventTableMemberTable'
CREATE INDEX [IX_FK_EventTableMemberTable]
ON [dbo].[EventTables]
    ([MemberTable_Mem_Id]);
GO

-- Creating foreign key on [MemberTable_Mem_Id] in table 'NotificationActionTables'
ALTER TABLE [dbo].[NotificationActionTables]
ADD CONSTRAINT [FK_NotificationActionTableMemberTable]
    FOREIGN KEY ([MemberTable_Mem_Id])
    REFERENCES [dbo].[MemberTables]
        ([Mem_Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_NotificationActionTableMemberTable'
CREATE INDEX [IX_FK_NotificationActionTableMemberTable]
ON [dbo].[NotificationActionTables]
    ([MemberTable_Mem_Id]);
GO

-- Creating foreign key on [NotificationTableNotif_Id] in table 'NotificationActionTables'
ALTER TABLE [dbo].[NotificationActionTables]
ADD CONSTRAINT [FK_NotificationTableNotificationActionTable]
    FOREIGN KEY ([NotificationTableNotif_Id])
    REFERENCES [dbo].[NotificationTables]
        ([Notif_Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_NotificationTableNotificationActionTable'
CREATE INDEX [IX_FK_NotificationTableNotificationActionTable]
ON [dbo].[NotificationActionTables]
    ([NotificationTableNotif_Id]);
GO

-- Creating foreign key on [EventTable_Evnt_Id] in table 'NotificationActionTables'
ALTER TABLE [dbo].[NotificationActionTables]
ADD CONSTRAINT [FK_NotificationActionTableEventTable]
    FOREIGN KEY ([EventTable_Evnt_Id])
    REFERENCES [dbo].[EventTables]
        ([Evnt_Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_NotificationActionTableEventTable'
CREATE INDEX [IX_FK_NotificationActionTableEventTable]
ON [dbo].[NotificationActionTables]
    ([EventTable_Evnt_Id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------