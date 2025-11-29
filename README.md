# NadekoBot

NadekoBot is an open source Discord bot. It is written in C# and is built on .NET 8.

If you have any questions, please visit our Discord: https://discord.nadeko.bot

## Installation

### Hosting on a linux server

If you want your bot to be online 24/7, you should host it on a linux vps.

### Docker

There is an official Docker image for a simple setup
Short version:
  ```sh
    docker run -d --name nadeko ghcr.io/nadeko-bot/nadekobot:v6 -e bot_token=YOUR_TOKEN_HERE -v "./data:/app/data" && docker logs -f --tail 500 nadeko
  ```

## Contributing to NadekoBot

We love your input! We want to make contributing to this project as easy as possible, whether it's:

- Reporting a bug
- Discussing the current state of the code
- Submitting a fix
- Proposing new features
- Becoming a maintainer

### Streaming Tools & Resources
- [Self-Hosting Guides](https://docs.nadeko.bot) - Complete setup documentation
- [Streamer Resources](https://kwoth.github.io/NadekoBot/streamer-resources) - Analytics tools and growth guides
- [Stream Notifications](https://kwoth.github.io/NadekoBot/stream-notifications) - Twitch & Kick alert setup
- [Stream Tools](https://www.theviewbot.com/tools) - Analytic and metric tools for Twitch and Kick

### Contribution

By submitting code, content, or materials via pull request or similar means ("Contribution"), you irrevocably assign all
intellectual property rights (including copyright and patents) to NadekoBot Repository Owner and affirm you either:

- (a) own the Contribution outright, or
- (b) it is licensed under compatible terms permitting unrestricted relicensing.

You grant the NadekoBot Repository Owner perpetual, worldwide rights to use, modify, distribute, and sublicense the
Contribution under AGPLv3, a commercial license, or any other terms without compensation.

These terms survive termination of this agreement.
