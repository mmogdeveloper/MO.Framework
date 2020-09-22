@echo off
for %%i in (*.proto) do (
   echo gen %%~nxi...
   tool\protoc.exe -I=. -I=.. --csharp_out=message --csharp_opt=file_extension=.cs --grpc_out=message --plugin=protoc-gen-grpc=tool\grpc_csharp_plugin.exe  %%~nxi)

echo finish... 
pause