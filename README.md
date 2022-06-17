# ASP.NET Core Web API

## Introduction

Minimal web api example builded with .net core v6.

### Commands

Listing the .net SDK's 
```powershell
dotnet --list-sdks
```

project creation
```powershell
dotnet new webapi -f net6.0
```

restoring & running the application
```powershel
dotnet restore
```
```powershel
dotnet run
```

### Install .Net Http REPL

command line to make http requests from the terminal
```powershell
dotnet tool install -g Microsoft.dotnet-httprepl
```

Connect to `httprepl`. Make sure that you have running the application with `dotnet run` command then go and open a new terminal a run the following command.
```powershell
httprepl https://localhost:{PORT}
```
Where the `{PORT}` is the current port where your application is running like `https://localhost:7240`.

Commands example inside of `httprepl` cli.
```
ls
cd WeatherForecast
get
exit
```


send an http post request with json body
```powershell
post -c "{"name":"Hawaii", "isGlutenFree":false}"
```