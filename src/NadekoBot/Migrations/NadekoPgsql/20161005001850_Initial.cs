using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NadekoBot.Migrations.NadekoPgsql
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BotConfig",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    CurrencyGenerationChance = table.Column<float>(nullable: false),
                    CurrencyGenerationCooldown = table.Column<int>(nullable: false),
                    CurrencyName = table.Column<string>(nullable: true),
                    CurrencyPluralName = table.Column<string>(nullable: true),
                    CurrencySign = table.Column<string>(nullable: true),
                    DontJoinServers = table.Column<bool>(nullable: false),
                    ForwardMessages = table.Column<bool>(nullable: false),
                    ForwardToAllOwners = table.Column<bool>(nullable: false),
                    RemindMessageFormat = table.Column<string>(nullable: true),
                    RotatingStatuses = table.Column<bool>(nullable: false),
                    BufferSize = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClashOfClans",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    EnemyClan = table.Column<string>(nullable: true),
                    Size = table.Column<int>(nullable: false),
                    StartedAt = table.Column<DateTime>(nullable: false),
                    WarState = table.Column<int>(nullable: false),
                    ChannelId = table.Column<long>(nullable: false),
                    GuildId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClashOfClans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConversionUnits",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    InternalTrigger = table.Column<string>(nullable: true),
                    Modifier = table.Column<decimal>(nullable: false),
                    UnitType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversionUnits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Currency",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    Amount = table.Column<long>(nullable: false),
                    UserId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currency", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Donators",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    Amount = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    UserId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Donators", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LogSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    ChannelCreated = table.Column<bool>(nullable: false),
                    ChannelDestroyed = table.Column<bool>(nullable: false),
                    ChannelUpdated = table.Column<bool>(nullable: false),
                    IsLogging = table.Column<bool>(nullable: false),
                    LogUserPresence = table.Column<bool>(nullable: false),
                    LogVoicePresence = table.Column<bool>(nullable: false),
                    MessageDeleted = table.Column<bool>(nullable: false),
                    MessageUpdated = table.Column<bool>(nullable: false),
                    UserBanned = table.Column<bool>(nullable: false),
                    UserJoined = table.Column<bool>(nullable: false),
                    UserLeft = table.Column<bool>(nullable: false),
                    UserUnbanned = table.Column<bool>(nullable: false),
                    UserUpdated = table.Column<bool>(nullable: false),
                    ChannelId = table.Column<long>(nullable: false),
                    UserPresenceChannelId = table.Column<long>(nullable: false),
                    VoicePresenceChannelId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MusicPlaylists",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    Author = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    AuthorId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MusicPlaylists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permission",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    NextId = table.Column<int>(nullable: true),
                    Order = table.Column<int>(nullable: false),
                    PrimaryTarget = table.Column<int>(nullable: false),
                    SecondaryTarget = table.Column<int>(nullable: false),
                    SecondaryTargetName = table.Column<string>(nullable: true),
                    State = table.Column<bool>(nullable: false),
                    PrimaryTargetId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Permission_Permission_NextId",
                        column: x => x.NextId,
                        principalTable: "Permission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Quotes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    AuthorName = table.Column<string>(nullable: false),
                    Keyword = table.Column<string>(nullable: false),
                    Text = table.Column<string>(nullable: false),
                    AuthorId = table.Column<long>(nullable: false),
                    GuildId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quotes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reminders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    IsPrivate = table.Column<bool>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    When = table.Column<DateTime>(nullable: false),
                    ChannelId = table.Column<long>(nullable: false),
                    GuildId = table.Column<long>(nullable: false),
                    UserId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reminders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Repeaters",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    Interval = table.Column<TimeSpan>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    ChannelId = table.Column<long>(nullable: false),
                    GuildId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Repeaters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SelfAssignableRoles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    GuildId = table.Column<long>(nullable: false),
                    RoleId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SelfAssignableRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TypingArticles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    Author = table.Column<string>(nullable: true),
                    Text = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypingArticles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BlacklistItem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    BotConfigId = table.Column<int>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    ItemId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlacklistItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlacklistItem_BotConfig_BotConfigId",
                        column: x => x.BotConfigId,
                        principalTable: "BotConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EightBallResponses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    BotConfigId = table.Column<int>(nullable: true),
                    Text = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EightBallResponses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EightBallResponses_BotConfig_BotConfigId",
                        column: x => x.BotConfigId,
                        principalTable: "BotConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ModulePrefixes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    BotConfigId = table.Column<int>(nullable: false),
                    ModuleName = table.Column<string>(nullable: true),
                    Prefix = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModulePrefixes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModulePrefixes_BotConfig_BotConfigId",
                        column: x => x.BotConfigId,
                        principalTable: "BotConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayingStatus",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    BotConfigId = table.Column<int>(nullable: true),
                    Status = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayingStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayingStatus_BotConfig_BotConfigId",
                        column: x => x.BotConfigId,
                        principalTable: "BotConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RaceAnimals",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    BotConfigId = table.Column<int>(nullable: true),
                    Icon = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RaceAnimals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RaceAnimals_BotConfig_BotConfigId",
                        column: x => x.BotConfigId,
                        principalTable: "BotConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ClashCallers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    BaseDestroyed = table.Column<bool>(nullable: false),
                    CallUser = table.Column<string>(nullable: true),
                    ClashWarId = table.Column<int>(nullable: false),
                    Stars = table.Column<int>(nullable: false),
                    TimeAdded = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClashCallers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClashCallers_ClashOfClans_ClashWarId",
                        column: x => x.ClashWarId,
                        principalTable: "ClashOfClans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IgnoredLogChannels",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    LogSettingId = table.Column<int>(nullable: true),
                    ChannelId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IgnoredLogChannels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IgnoredLogChannels_LogSettings_LogSettingId",
                        column: x => x.LogSettingId,
                        principalTable: "LogSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IgnoredVoicePresenceCHannels",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    LogSettingId = table.Column<int>(nullable: true),
                    ChannelId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IgnoredVoicePresenceCHannels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IgnoredVoicePresenceCHannels_LogSettings_LogSettingId",
                        column: x => x.LogSettingId,
                        principalTable: "LogSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlaylistSong",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    MusicPlaylistId = table.Column<int>(nullable: true),
                    Provider = table.Column<string>(nullable: true),
                    ProviderType = table.Column<int>(nullable: false),
                    Query = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Uri = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaylistSong", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlaylistSong_MusicPlaylists_MusicPlaylistId",
                        column: x => x.MusicPlaylistId,
                        principalTable: "MusicPlaylists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GuildConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    AutoDeleteByeMessages = table.Column<bool>(nullable: false),
                    AutoDeleteGreetMessages = table.Column<bool>(nullable: false),
                    AutoDeleteGreetMessagesTimer = table.Column<int>(nullable: false),
                    AutoDeleteSelfAssignedRoleMessages = table.Column<bool>(nullable: false),
                    ChannelByeMessageText = table.Column<string>(nullable: true),
                    ChannelGreetMessageText = table.Column<string>(nullable: true),
                    DefaultMusicVolume = table.Column<float>(nullable: false),
                    DeleteMessageOnCommand = table.Column<bool>(nullable: false),
                    DmGreetMessageText = table.Column<string>(nullable: true),
                    ExclusiveSelfAssignedRoles = table.Column<bool>(nullable: false),
                    FilterInvites = table.Column<bool>(nullable: false),
                    FilterWords = table.Column<bool>(nullable: false),
                    LogSettingId = table.Column<int>(nullable: true),
                    PermissionRole = table.Column<string>(nullable: true),
                    RootPermissionId = table.Column<int>(nullable: true),
                    SendChannelByeMessage = table.Column<bool>(nullable: false),
                    SendChannelGreetMessage = table.Column<bool>(nullable: false),
                    SendDmGreetMessage = table.Column<bool>(nullable: false),
                    VerbosePermissions = table.Column<bool>(nullable: false),
                    VoicePlusTextEnabled = table.Column<bool>(nullable: false),
                    AutoAssignRoleId = table.Column<long>(nullable: false),
                    ByeMessageChannelId = table.Column<long>(nullable: false),
                    GenerateCurrencyChannelId = table.Column<long>(nullable: true),
                    GreetMessageChannelId = table.Column<long>(nullable: false),
                    GuildId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GuildConfigs_LogSettings_LogSettingId",
                        column: x => x.LogSettingId,
                        principalTable: "LogSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GuildConfigs_Permission_RootPermissionId",
                        column: x => x.RootPermissionId,
                        principalTable: "Permission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FilterChannelId",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    GuildConfigId = table.Column<int>(nullable: true),
                    GuildConfigId1 = table.Column<int>(nullable: true),
                    ChannelId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilterChannelId", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FilterChannelId_GuildConfigs_GuildConfigId",
                        column: x => x.GuildConfigId,
                        principalTable: "GuildConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FilterChannelId_GuildConfigs_GuildConfigId1",
                        column: x => x.GuildConfigId1,
                        principalTable: "GuildConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FilteredWord",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    GuildConfigId = table.Column<int>(nullable: true),
                    Word = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilteredWord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FilteredWord_GuildConfigs_GuildConfigId",
                        column: x => x.GuildConfigId,
                        principalTable: "GuildConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FollowedStream",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    GuildConfigId = table.Column<int>(nullable: true),
                    LastStatus = table.Column<bool>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Username = table.Column<string>(nullable: true),
                    ChannelId = table.Column<long>(nullable: false),
                    GuildId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FollowedStream", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FollowedStream_GuildConfigs_GuildConfigId",
                        column: x => x.GuildConfigId,
                        principalTable: "GuildConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlacklistItem_BotConfigId",
                table: "BlacklistItem",
                column: "BotConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_ClashCallers_ClashWarId",
                table: "ClashCallers",
                column: "ClashWarId");

            migrationBuilder.CreateIndex(
                name: "IX_Currency_UserId",
                table: "Currency",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Donators_UserId",
                table: "Donators",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EightBallResponses_BotConfigId",
                table: "EightBallResponses",
                column: "BotConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_FilterChannelId_GuildConfigId",
                table: "FilterChannelId",
                column: "GuildConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_FilterChannelId_GuildConfigId1",
                table: "FilterChannelId",
                column: "GuildConfigId1");

            migrationBuilder.CreateIndex(
                name: "IX_FilteredWord_GuildConfigId",
                table: "FilteredWord",
                column: "GuildConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_FollowedStream_GuildConfigId",
                table: "FollowedStream",
                column: "GuildConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildConfigs_LogSettingId",
                table: "GuildConfigs",
                column: "LogSettingId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildConfigs_RootPermissionId",
                table: "GuildConfigs",
                column: "RootPermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildConfigs_GuildId",
                table: "GuildConfigs",
                column: "GuildId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IgnoredLogChannels_LogSettingId",
                table: "IgnoredLogChannels",
                column: "LogSettingId");

            migrationBuilder.CreateIndex(
                name: "IX_IgnoredVoicePresenceCHannels_LogSettingId",
                table: "IgnoredVoicePresenceCHannels",
                column: "LogSettingId");

            migrationBuilder.CreateIndex(
                name: "IX_ModulePrefixes_BotConfigId",
                table: "ModulePrefixes",
                column: "BotConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_Permission_NextId",
                table: "Permission",
                column: "NextId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayingStatus_BotConfigId",
                table: "PlayingStatus",
                column: "BotConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaylistSong_MusicPlaylistId",
                table: "PlaylistSong",
                column: "MusicPlaylistId");

            migrationBuilder.CreateIndex(
                name: "IX_RaceAnimals_BotConfigId",
                table: "RaceAnimals",
                column: "BotConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_Repeaters_ChannelId",
                table: "Repeaters",
                column: "ChannelId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SelfAssignableRoles_GuildId_RoleId",
                table: "SelfAssignableRoles",
                columns: new[] { "GuildId", "RoleId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlacklistItem");

            migrationBuilder.DropTable(
                name: "ClashCallers");

            migrationBuilder.DropTable(
                name: "ConversionUnits");

            migrationBuilder.DropTable(
                name: "Currency");

            migrationBuilder.DropTable(
                name: "Donators");

            migrationBuilder.DropTable(
                name: "EightBallResponses");

            migrationBuilder.DropTable(
                name: "FilterChannelId");

            migrationBuilder.DropTable(
                name: "FilteredWord");

            migrationBuilder.DropTable(
                name: "FollowedStream");

            migrationBuilder.DropTable(
                name: "IgnoredLogChannels");

            migrationBuilder.DropTable(
                name: "IgnoredVoicePresenceCHannels");

            migrationBuilder.DropTable(
                name: "ModulePrefixes");

            migrationBuilder.DropTable(
                name: "PlayingStatus");

            migrationBuilder.DropTable(
                name: "PlaylistSong");

            migrationBuilder.DropTable(
                name: "Quotes");

            migrationBuilder.DropTable(
                name: "RaceAnimals");

            migrationBuilder.DropTable(
                name: "Reminders");

            migrationBuilder.DropTable(
                name: "Repeaters");

            migrationBuilder.DropTable(
                name: "SelfAssignableRoles");

            migrationBuilder.DropTable(
                name: "TypingArticles");

            migrationBuilder.DropTable(
                name: "ClashOfClans");

            migrationBuilder.DropTable(
                name: "GuildConfigs");

            migrationBuilder.DropTable(
                name: "MusicPlaylists");

            migrationBuilder.DropTable(
                name: "BotConfig");

            migrationBuilder.DropTable(
                name: "LogSettings");

            migrationBuilder.DropTable(
                name: "Permission");
        }
    }
}
