# 构建阶段
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 复制项目文件
COPY ["src/Backend/UnifiedPlatform.WebApi/UnifiedPlatform.WebApi.csproj", "src/Backend/UnifiedPlatform.WebApi/"]
COPY ["src/Backend/UnifiedPlatform.DbService/UnifiedPlatform.DbService.csproj", "src/Backend/UnifiedPlatform.DbService/"]
COPY ["src/Backend/UnifiedPlatform.Shared/UnifiedPlatform.Shared.csproj", "src/Backend/UnifiedPlatform.Shared/"]
COPY ["src/Libraries/HFastKit/HFastKit/HFastKit.csproj", "src/Libraries/HFastKit/HFastKit/"]
COPY ["src/Libraries/HFastKit/HFastKit.AspNetCore/HFastKit.AspNetCore.csproj", "src/Libraries/HFastKit/HFastKit.AspNetCore/"]
COPY ["src/Libraries/HFastKit/HFastKit.AspNetCore.Shared/HFastKit.AspNetCore.Shared.csproj", "src/Libraries/HFastKit/HFastKit.AspNetCore.Shared/"]
COPY ["src/Libraries/Nblockchain/Nblockchain.Signer/Nblockchain.Signer.csproj", "src/Libraries/Nblockchain/Nblockchain.Signer/"]
COPY ["src/Libraries/Nblockchain/Nblockchain.Tron/Nblockchain.Tron.csproj", "src/Libraries/Nblockchain/Nblockchain.Tron/"]
COPY ["src/Libraries/Nblockchain/Nblockchain.Tron.Protocol/Nblockchain.Tron.Protocol.csproj", "src/Libraries/Nblockchain/Nblockchain.Tron.Protocol/"]

# 恢复依赖
RUN dotnet restore "src/Backend/UnifiedPlatform.WebApi/UnifiedPlatform.WebApi.csproj"

# 复制所有文件
COPY . .

# 构建项目
WORKDIR "/src/src/Backend/UnifiedPlatform.WebApi"
RUN dotnet build "UnifiedPlatform.WebApi.csproj" -c Release -o /app/build

# 发布阶段
FROM build AS publish
RUN dotnet publish "UnifiedPlatform.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 运行阶段
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443

# 复制发布文件
COPY --from=publish /app/publish .

# 设置入口点
ENTRYPOINT ["dotnet", "UnifiedPlatform.WebApi.dll"]

