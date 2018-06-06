namespace Bot3ulerLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Channels",
                c => new
                    {
                        ChannelId = c.Long(nullable: false),
                        ChannelGuild_GuildId = c.Long(),
                    })
                .PrimaryKey(t => t.ChannelId)                
                .ForeignKey("Guilds", t => t.ChannelGuild_GuildId)
                .Index(t => t.ChannelGuild_GuildId);
            
            CreateTable(
                "Commands",
                c => new
                    {
                        CommandId = c.Long(nullable: false),
                        CommandString = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.CommandId)                ;
            
            CreateTable(
                "Guilds",
                c => new
                    {
                        GuildId = c.Long(nullable: false),
                        GuildName = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.GuildId)                ;
            
            CreateTable(
                "ChannelCommands",
                c => new
                    {
                        ChannelId = c.Long(nullable: false),
                        CommandId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.ChannelId, t.CommandId })                
                .ForeignKey("Channels", t => t.ChannelId, cascadeDelete: true)
                .ForeignKey("Commands", t => t.CommandId, cascadeDelete: true)
                .Index(t => t.ChannelId)
                .Index(t => t.CommandId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("Channels", "ChannelGuild_GuildId", "Guilds");
            DropForeignKey("ChannelCommands", "CommandId", "Commands");
            DropForeignKey("ChannelCommands", "ChannelId", "Channels");
            DropIndex("ChannelCommands", new[] { "CommandId" });
            DropIndex("ChannelCommands", new[] { "ChannelId" });
            DropIndex("Channels", new[] { "ChannelGuild_GuildId" });
            DropTable("ChannelCommands");
            DropTable("Guilds");
            DropTable("Commands");
            DropTable("Channels");
        }
    }
}
