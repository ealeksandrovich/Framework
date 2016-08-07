--https://docs.mongodb.com/manual/tutorial/install-mongodb-on-windows/

mkdir C:\mongodb\db

mkdir C:\mongodb\log

--"C:\Program Files\MongoDB\Server\3.2\bin\mongod.exe" --config "C:\Program Files\MongoDB\Server\3.2\bin\mongod.cfg" --serviceName "MongoDB" --serviceDisplayName "MongoDB Service" --install 
	
sc.exe create MongoDB binPath="\"C:\Program Files\MongoDB\Server\3.2\bin\mongod.exe\" --service --config \"C:\Program Files\MongoDB\Server\3.2\bin\mongod.cfg\"" DisplayName="MongoDB Service" start="auto"

sc.exe description MongoDB  "This service runs the MongoDB server"

net start MongoDB