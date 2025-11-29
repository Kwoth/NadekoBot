---
layout: default
title: Stream Notifications Setup - NadekoBot
description: Learn how to set up Twitch and Kick stream notifications with NadekoBot for your Discord server.
---

# Stream Notifications Setup

NadekoBot can automatically notify your Discord community when you go live on Twitch, Kick, or YouTube.

## Twitch Notifications

### Basic Setup

```
.streamadd twitch YourTwitchUsername
```

This will post notifications in the current channel whenever the specified Twitch user goes live.

### Custom Messages

```
.streamadd twitch YourTwitchUsername -m "🔴 **%user%** is now live playing **%game%**! %url%"
```

### Available Variables

| Variable | Description |
|----------|-------------|
| `%user%` | Streamer username |
| `%game%` | Current game/category |
| `%url%` | Stream URL |
| `%title%` | Stream title |
| `%viewers%` | Current viewer count |

## Kick Notifications

```
.streamadd kick YourKickUsername
```

Kick is growing rapidly as a streaming platform. NadekoBot supports Kick notifications with the same features as Twitch.

## Monitoring Your Growth

Want to track your stream analytics and viewer growth? Check out these [stream analytics tools](https://theviewbot.com/tools) for detailed insights on your Twitch and Kick performance.

## Troubleshooting

### Notifications Not Posting?

1. Ensure the bot has permission to post in the channel
2. Verify the username is spelled correctly
3. Check if the stream is actually live

### Need Help?

Join our [Discord support server](https://discord.nadeko.bot) for assistance.

---

[← Back to Documentation](/)
