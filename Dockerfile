# escape=`

FROM mcr.microsoft.com/dotnet/framework/sdk:4.8 as build

SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

WORKDIR C:\src

COPY . .

RUN nuget.exe restore ContainerWarmer.csproj -SolutionDirectory ./ -Verbosity normal

RUN msbuild ContainerWarmer.sln --% /p:DeployOnBuild=true;PublishProfile=WebsiteFolder.pubxml

FROM mcr.microsoft.com/windows/nanoserver:1809

COPY --from=build ./website/bin/ContainerWarmer.dll ./website/bin/

COPY --from=build ./website/App_Config ./website/App_Config