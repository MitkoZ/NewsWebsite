FROM mcr.microsoft.com/dotnet/core/sdk:3.1.101-nanoserver-1909
# When writing a comment, preferably write the comments on new lines, since the comment can be treated as a parameter of some docker file command (like the USER command)

#We need to install NodeJS, so we can have NPM
SHELL ["pwsh", "-Command", "$ErrorActionPreference='Stop';"] # We need to switch to powershell just to download NodeJS 
RUN Invoke-WebRequest -OutFile "nodejs.zip" "https://nodejs.org/dist/v13.9.0/node-v13.9.0-win-x64.zip"
RUN Expand-Archive "nodejs.zip" -DestinationPath "C:/"
RUN Rename-Item "C:/node-v13.9.0-win-x64" "C:/nodejs"
SHELL ["cmd", "/S", "/C"] # We need to change back to our normal windows shell (cmd) because powershell doesn't have the path variable and who knows what more
# We need to switch to the ContainerAdministrator user or we will get an "Access to the registry path is denied." error when trying to set the path variable for the machine
USER ContainerAdministrator
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