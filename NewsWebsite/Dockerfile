FROM mcr.microsoft.com/dotnet/core/sdk:3.1.101-nanoserver-1909
#Install NodeJS, so we can have NPM

USER ContainerAdministrator
SHELL ["pwsh", "-Command", "$ErrorActionPreference = 'Stop';$ProgressPreference='silentlyContinue';"] # We need to switch to powershell just to download NodeJS 
RUN Invoke-WebRequest -OutFile "nodejs.zip" "https://nodejs.org/dist/v13.9.0/node-v13.9.0-win-x64.zip"
RUN Expand-Archive "nodejs.zip" -DestinationPath "C:/"
RUN Rename-Item "C:/node-v13.9.0-win-x64" "C:/nodejs"
SHELL ["cmd", "/S", "/C"] # We need to change back to our normal windows shell (cmd) because powershell doesn't have the path variable and who knows what more
RUN setx /M PATH "%PATH%;C:/nodejs"
USER ContainerUser

EXPOSE 443
WORKDIR "/sourceCode"
COPY . .
WORKDIR "/sourceCode/NewsWebsite"
RUN dotnet restore NewsWebsite.csproj
RUN npm install

# We need to use publish, because in .NET Core, the NuGets, don't get added in the output. On the local machine it works without publishing, since the NuGet packages are cached (%userprofile%\.nuget\packages).
RUN dotnet publish NewsWebsite.csproj -c Debug -o "/publishedCode/Debug"
WORKDIR "/publishedCode/Debug"
ENTRYPOINT ["dotnet", "NewsWebsite.dll"]