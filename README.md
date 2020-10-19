# MO.Framework

#### 介绍
可承载百万人同时在线的分布式游戏框架(同时支持websocket,socket和http)  

#### 软件架构
基于dotnetcore3.1,Orleans,Dotnetty跨平台的分布式游戏框架  
模块分为:  
MO.Login:登录服务器  
MO.Api:http服务器  
MO.Gate:支持socket和websocket  
MO.Silo,MO.Interfaces,MO.Grains:基于Orleans的分布式服务  
  
MO.Model:基于entityframeworkcore模块  
MO.Protocol:基于proto3的协议  (与客户端共享代码)   
MO.Common:基本工具模块  (与客户端共享代码)  
MO.Algorithm:基本逻辑模块  (与客户端共享代码)  

框架使用了  
Orleans  
https://github.com/dotnet/orleans  
英文文档:https://dotnet.github.io/orleans/  
中文文档:https://orleans.azurewebsites.net/  
Dotnetty  
https://github.com/Azure/DotNetty  
Entityframeworkcore  
https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql  
CSRedisCore  
https://github.com/2881099/csredis  
Google.Protobuf  
https://github.com/protocolbuffers/protobuf/tree/master/csharp  
NLog  
https://github.com/NLog/NLog  
#### 安装教程


#### 使用说明
运行框架需要安装Mysql  
在mysql中新建数据库命名为Orleans  
在mysql数据库执行Database文件夹中的sql文件(Orleans需要)  

(Model根据业务需要,定义数据库表结构)  
Model模块中创建了两个数据库(Code First)  
分别是MOData和MORecord  
MOData是基本的业务数据库  
MORecord是记录日志的数据库  

(Redis根据业务需要,非必须模块)  
Redis作为数据缓存使用  
Redis 默认 用来存储基本业务数据  (自定义存储格式)


启动顺序  
1.Silo  
2.Gate  
3.Api  
4.Login  
先检查json文件配置是否正确  

正式环境配置NLog  
将Share/NLog.Config中的Console打印注释  
只保留Share/NLog.Config中的需要的打印等级  
日志配置会影响严重影响运行效率  

#### 参与贡献



#### 特技

