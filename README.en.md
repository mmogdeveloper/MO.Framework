# MO.Framework

#### Introduction
A distributed game framework capable of supporting millions of concurrent online users (supports WebSocket, Socket, and HTTP simultaneously).

#### Architecture
A cross-platform distributed game framework based on .NET 9, Orleans, and DotNetty.

Modules:
MO.Login: Login server
MO.Api: HTTP server
MO.Gate: Supports Socket and WebSocket
MO.Silo, MO.Interfaces, MO.Grains: Distributed services based on Orleans

MO.Model: Module based on Entity Framework Core
MO.Protocol: Protocol based on proto3 (shared code with client)
MO.Common: Basic utility module (shared code with client)
MO.Algorithm: Basic logic module (shared code with client)

Technologies used:
Orleans
https://github.com/dotnet/orleans
Documentation: https://dotnet.github.io/orleans/
DotNetty
https://github.com/Azure/DotNetty
Entity Framework Core
https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql
CSRedisCore
https://github.com/2881099/csredis
Google.Protobuf
https://github.com/protocolbuffers/protobuf/tree/master/csharp
NLog
https://github.com/NLog/NLog

#### Installation


#### Usage
Running the framework requires MySQL.
Create a new database named "Orleans" in MySQL.
Execute the SQL files in the Database folder on the MySQL database (required by Orleans).

(Define database table structures in Model as needed by your business logic)
Two databases are created in the Model module (Code First):
MOData and MORecord.
MOData is the primary business database.
MORecord is the logging database.

(Redis is optional, use as needed)
Redis is used as a data cache.
Redis is used by default to store basic business data (custom storage format).

Startup order:
1. Silo
2. Gate
3. Api
4. Login
Check that the JSON configuration files are correct before starting.

Production environment NLog configuration:
Comment out the Console output in Share/NLog.Config.
Keep only the required log levels in Share/NLog.Config.
Logging configuration can significantly impact runtime performance.

#### Contributing


#### Tips

