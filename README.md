Modules.NEventStore
===================

OrigoDB storage module adapter for NEventStore, see http://github.com/NEventStore

OrigoDB is the in-memory database toolkit for NET/Mono. See http://devrexlabs.github.io

NEventStore supports mssql, mysql, postgresql, oracle and sqlite, ravendb and mongodb.


### Example setup
```csharp
	var wireup = Wireup.Init()
		.LogToOutputWindow()
		.UsingSqlPersistence("NEventStore") // Connection string is in app.config
		.WithDialect(new MsSqlDialect())
		.InitializeStorageEngine()
		.UsingJsonSerialization();
	
	var config = new EngineConfiguration();
	config.SetCommandStoreFactory(cfg => new NESCommandStore(cfg, wireup));
	
	var db = Db.For<MyModel>(config);
```
