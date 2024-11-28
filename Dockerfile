# syntax=docker/dockerfile:1
# --------------------------------------- Building stage -----------------------------------------
# Create a stage for building the application.
# Note that alpine mean LINUX based system (Linux build is usually faster)
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build

# Copy all content of the current folder to Docker folder named source
COPY . /source

# Change our current workdir location
WORKDIR /source/Dynamics

# This is the architecture you’re building for, which is passed in by the builder.
# Placing it here allows the previous steps to be cached across architectures.
ARG TARGETARCH

# Build the application.
# Install Node.js and npm and build the frontend (You needed this for MVC)
RUN apk add nodejs npm && npm install

# Leverage a cache mount to /root/.nuget/packages so that subsequent builds don't have to re-download packages.
# If TARGETARCH is "amd64", replace it with "x64" - "x64" is .NET's canonical name for this and "amd64" doesn't
#   work in .NET 6.0.
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet publish -a ${TARGETARCH/amd64/x64} --use-current-runtime --self-contained false -o /app

# --------------------------------------- Development stage -------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS development
COPY . /source
WORKDIR /source/src
CMD dotnet run --no-launch-profile

# --------------------------------------- Final stage (Run) -----------------------------------------
# Note that alpine is Linux while normal is Window
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
WORKDIR /app

# Install the icu for globalization stuff
RUN apk add icu-libs
# This is needed to make sure no globalization error
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Copy everything needed to run the app from the "build" stage.
COPY --from=build /app .

USER $APP_UID

ENTRYPOINT ["dotnet", "Dynamics.dll"]
