Modules.NEventStore
===================

OrigoDB storage module adapter for NEventStore.

OrigoDB is the in-memory database toolkit for NET/Mono. See http://devrexlabs.github.io

From the readme at http://github.com/NEventStore
	"NEventStore is a persistence library used to abstract different storage implementations when using event sourcing as storage mechanism"

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
