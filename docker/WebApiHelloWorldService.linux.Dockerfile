#Dockerfile does not build anything.  It simply copies already generated/built files from a local host publish folder.

FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal
EXPOSE 80
EXPOSE 443

ARG branch=""
ARG commit=""
ARG version=""
ARG build=""
WORKDIR /app
COPY ["/src/Supply.WebApiHelloWorldService/bin/Release/net6.0/publish/", "/app"]
LABEL com.amsoftware.build.branch=$branch
LABEL com.amsoftware.build.commit=$commit
LABEL com.amsoftware.build.version=$version
LABEL com.amsoftware.build.buildmetadata=$build
ENV SOURCE_COMMIT=$commit
ENV SOURCE_BRANCH=$branch
ENV SOURCE_VERSION=$version
ENV SOURCE_BUILD=$build
ENTRYPOINT ["dotnet", "Amsoftware.E2E.Supply.WebApiHelloWorldService.dll"]