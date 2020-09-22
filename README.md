# MO.Framework

#### 介绍
可承载百万人同时在线的分布式游戏框架  

#### 软件架构
基于Orleans,Dotnetty跨平台的分布式游戏框架  
模块分为:  
Login:登录服务器  
Api:http服务器  
Gate:socket网关  
Silo:基于Orleans的分布式服务  
Model:基于entityframeworkcore模块  
Protocol:基于proto3的协议  
Common:基本工具模块  
Algorithm:基本逻辑模块  

框架使用了
Orleans 分布式基础  
Dotnetty 网关socket通信  
Entityframeworkcore 操作mysql  
CSRedisCore 操作redis  
Google.Protobuf 通信协议  
NLog 系统日志  

#### 安装教程


#### 使用说明
运行框架需要安装Mysql和Redis  
在mysql数据库执行Database文件夹中的sql文件(Orleans需要)  
Model模块中创建了两个数据库(Code First)  
分别是MOData和MORecord  
MOData是基本的业务数据库表  
MORecord是记录日志的数据库表  
Redis作为数据缓存使用  
Redis 默认 DB10 用来存储Token 过期时间为30秒  
Redis 默认 DB0 用来存储基本业务数据  (自定义存储格式)

启动顺序  
1.Silo  
2.Gate  
3.Login  
4.Api  
先检查json文件配置是否正确  

#### 参与贡献



#### 特技

