# Social Models

[![Publish to GitHub Packages](https://github.com/lostinmyworld/packages.SocialModels/actions/workflows/publish.yml/badge.svg)](https://github.com/lostinmyworld/packages.SocialModels/actions/workflows/publish.yml)

[![NuGet Package](https://img.shields.io/badge/Access-Packages-blue?logo=nuget)](https://github.com/lostinmyworld/packages.SocialModels/packages)

Shared models for projects that use code logic for Social Media.

## Social.Models
This project has all the models, enumerations, extensions and base URIs needed to communicate with social APIs.

## Social.OverThinkers
This project has all the models & service classes needed that include parsing logic for the date of the Social Media API responses.

## Social.OverSharers
This project has all the models & service classes to consume APIs per Social Media platforms.

## Test
Create `.env.local` with some environment variables. Be sure to not commit & push them. _They are considered sensitive data._
Check `Program.cs` on what environment variables to add.
Run this project just to see the magic happen! 🪄✨

# How to use
First add this in your bootstrapping project:
```
var services = new ServiceCollection();

services.AddSocialOverThinkers();
services.AddSocialOverSharers();
```
