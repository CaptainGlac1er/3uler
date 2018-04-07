namespace Bot3ulerLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Channels",
                c => new
                    {
                        ChannelId = c.Long(nullable: false, identity: true),
                        ChannelGuild_GuildId = c.Long(),
                    })
                .PrimaryKey(t => t.ChannelId)
                .ForeignKey("dbo.Guilds", t => t.ChannelGuild_GuildId)
                .Index(t => t.ChannelGuild_GuildId);
            
            CreateTable(
                "dbo.Commands",
                c => new
                    {
                        CommandId = c.Long(nullable: false),
                        CommandString = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.CommandId);
            
            CreateTable(
                "dbo.Guilds",
                c => new
                    {
                        GuildId = c.Long(nullable: false, identity: true),
                        GuildName = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.GuildId);
            
            CreateTable(
                "dbo.ChannelCommands",
                c => new
                    {
                        ChannelId = c.Long(nullable: false),
                        CommandId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.ChannelId, t.CommandId })
                .ForeignKey("dbo.Channels", t => t.ChannelId, cascadeDelete: true)
                .ForeignKey("dbo.Commands", t => t.CommandId, cascadeDelete: true)
                .Index(t => t.ChannelId)
                .Index(t => t.CommandId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Channels", "ChannelGuild_GuildId", "dbo.Guilds");
            DropForeignKey("dbo.ChannelCommands", "CommandId", "dbo.Commands");
            DropForeignKey("dbo.ChannelCommands", "ChannelId", "dbo.Channels");
            DropIndex("dbo.ChannelCommands", new[] { "CommandId" });
            DropIndex("dbo.ChannelCommands", new[] { "ChannelId" });
            DropIndex("dbo.Channels", new[] { "ChannelGuild_GuildId" });
            DropTable("dbo.ChannelCommands");
            DropTable("dbo.Guilds");
            DropTable("dbo.Commands");
            DropTable("dbo.Channels");
        }
    }
}
