FROM mcr.microsoft.com/dotnet/core/sdk:3.1.101-nanoserver-1909
EXPOSE 443
WORKDIR "/sourceCode"
COPY . .
WORKDIR "/sourceCode/NewsWebsite"
RUN dotnet restore NewsWebsite.csproj

# We need to use publish, because in .NET Core, the NuGets, don't get added in the output. On the local machine it works without publishing, since the NuGet packages are cached (%userprofile%\.nuget\packages).
RUN dotnet publish NewsWebsite.csproj -c Debug -o "/publishedCode/Debug"
WORKDIR "/publishedCode/Debug"
ENTRYPOINT ["dotnet", "NewsWebsite.dll"]