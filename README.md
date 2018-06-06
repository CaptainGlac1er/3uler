# 3uler bot

## Made by George W. Colgrove IV

Discord bot that can access weather underground, cleverbot, and imgur

## DB setup

Make file `ConnectionStrings.config` in `3ulerBot\Config` containing a filled out connectionString

```xml
<connectionStrings>
  <add name="{connection string name}" connectionString="server= ;port= ;uid= ;password= ;database= " providerName="MySql.Data.MySqlClient" />
</connectionStrings>
```

Run `Update-Database` to init the database on the server
