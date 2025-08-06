# Live Streaming Server

A .NET 8 based live streaming server that provides RTMP ingestion and FLV playback capabilities. This server allows you to receive RTMP streams and serve them as FLV streams over HTTP/WebSocket for web-based playback.

## Overview

This project implements a complete live streaming solution using:
- **RTMP Server**: Receives live streams from streaming software (OBS, FFmpeg, etc.)
- **FLV Distribution**: Serves streams as FLV format over HTTP and WebSocket
- **Web Player**: HTML5-based player using flv.js for browser playback
- **Docker Support**: Containerized deployment with Docker Compose

## Architecture

```
┌─────────────────┐    RTMP (Port 1935)    ┌─────────────────────┐
│   Streaming     │───────────────────────→│  Live Streaming     │
│   Software      │                        │      Server         │
│ (OBS, FFmpeg)   │                        │   (.NET 8 App)      │
└─────────────────┘                        └─────────────────────┘
                                                      │
                                           HTTP/WebSocket (Port 8080)
                                                      │
                                                      ▼
                                           ┌─────────────────────┐
                                           │    Web Browser      │
                                           │   (FLV.js Player)   │
                                           └─────────────────────┘
```

## Key Components

### 1. RTMP Server (Program.cs)
The main application built with ASP.NET Core that:
- Listens on port 1935 for RTMP streams
- Provides HTTP/WebSocket endpoints on port 8080
- Serves static files (web player)
- Handles FLV stream distribution

### 2. Web Player (wwwroot/player/flv.html)
A browser-based player that:
- Uses flv.js library for FLV playback in browsers
- Supports both URL and MediaDataSource input modes
- Provides playback controls (play, pause, seek, destroy)
- Saves user preferences in localStorage
- Displays real-time logs from the flv.js player

### 3. Docker Configuration
- **Dockerfile**: Multi-stage build for optimized container size
- **docker-compose.yml**: Service orchestration with port mapping

## Dependencies

This project uses the LiveStreamingServerNet library:
- `LiveStreamingServerNet` (v0.31.1): Core streaming server functionality
- `LiveStreamingServerNet.Flv` (v0.31.1): FLV format support

## Ports

- **1935**: RTMP streaming input port
- **8080**: HTTP/WebSocket server for web interface and FLV output

## Getting Started

### Method 1: Docker Compose (Recommended)

1. Clone the repository
2. Run with Docker Compose:
   ```bash
   docker-compose up --build
   ```

### Method 2: Local Development

1. Ensure you have .NET 8 SDK installed
2. Restore dependencies:
   ```bash
   dotnet restore
   ```
3. Run the application:
   ```bash
   dotnet run
   ```

## Usage

### Publishing a Stream

Use any RTMP-capable streaming software:

#### OBS Studio Setup (Detailed)

**Step 1: Install OBS Studio**
- Download from [obsproject.com](https://obsproject.com/)
- Install and launch OBS Studio

**Step 2: Configure Stream Settings**
1. Open OBS Studio
2. Go to **File** → **Settings** (or click the Settings button)
3. Navigate to the **Stream** tab
4. Configure the following:
   - **Service**: Select "Custom..."
   - **Server**: Enter `rtmp://your-server-ip:1935/live`
   - **Stream Key**: Enter your desired stream name (e.g., `mystream`, `demo`, `test`)
   - Leave **Use authentication** unchecked

**Step 3: Configure Output Settings (Optional but Recommended)**
1. Go to the **Output** tab in Settings
2. Set **Output Mode** to "Advanced" for more control
3. **Streaming** tab settings:
   - **Encoder**: x264 (for CPU) or NVENC/AMD (for GPU encoding)
   - **Bitrate**: 2500-5000 Kbps (adjust based on your upload speed)
   - **Keyframe Interval**: 2 seconds
   - **Profile**: main
   - **Tune**: zerolatency (for live streaming)

**Step 4: Configure Video Settings**
1. Go to the **Video** tab in Settings
2. Recommended settings:
   - **Base Resolution**: 1920x1080 (or your desired resolution)
   - **Output Resolution**: 1280x720 (good balance of quality/bandwidth)
   - **FPS**: 30 (or 60 for smooth motion)

**Step 5: Add Sources**
1. In the main OBS window, click the **+** in the Sources box
2. Add sources like:
   - **Display Capture**: Capture your entire screen
   - **Window Capture**: Capture a specific application window
   - **Video Capture Device**: Use a webcam
   - **Media Source**: Play video files
   - **Browser Source**: Display web pages
   - **Text (GDI+)**: Add text overlays

**Step 6: Start Streaming**
1. Click **Start Streaming** in the main OBS window
2. OBS will connect to your RTMP server
3. Check the bottom status bar for connection status
4. If successful, you'll see "Live" indicator and streaming stats

**Step 7: View Your Stream**
1. Open your browser to `http://your-server-ip:8080/player/flv.html`
2. Enter the stream URL: `/live/your-stream-key.flv`
3. Click "Load" then "Start" to view your stream

**OBS Studio Tips:**
- **Test your setup**: Use a simple scene first (like Display Capture)
- **Monitor bandwidth**: Keep an eye on the bitrate and dropped frames
- **Audio sources**: Add Desktop Audio and/or Mic/Aux for sound
- **Scene collections**: Save different setups for different streaming scenarios
- **Filters**: Add filters to sources for effects (color correction, noise suppression, etc.)

**Quick Setup Summary:**
- Service: Custom
- Server: `rtmp://localhost:1935/live` (for local testing)
- Stream Key: `demo`
- Bitrate: 2500 Kbps
- Resolution: 1280x720@30fps

**FFmpeg (Alternative):**
```bash
ffmpeg -re -i input.mp4 -c copy -f flv rtmp://your-server-ip:1935/live/your-stream-name
```

### Watching a Stream

1. Open your browser and navigate to: `http://your-server-ip:8080/player/flv.html`
2. Enter the stream URL: `/live/your-stream-name.flv`
3. Click "Load" then "Start" to begin playback

## Web Player Features

### Input Modes
- **Stream URL Mode**: Direct FLV stream URL input
- **MediaDataSource Mode**: JSON-based configuration for advanced options

### Playback Options
- **isLive**: Enables live streaming mode
- **withCredentials**: Sends credentials with requests
- **hasAudio/hasVideo**: Audio and video track configuration

### Controls
- **Load**: Initialize the player with the stream
- **Start**: Begin playback
- **Pause**: Pause the stream
- **Destroy**: Clean up player resources
- **SeekTo**: Jump to specific time position (for non-live content)

## Configuration

### Environment Variables
- `ASPNETCORE_ENVIRONMENT`: Set to "Production" or "Development"
- `ASPNETCORE_URLS`: HTTP listening URLs (default: http://+:8080)

### CORS Policy
The server is configured with `AllowAnyOrigin` policy for development purposes. For production, consider restricting origins.

## File Structure

```
live-streaming-server/
├── Program.cs                    # Main application entry point
├── live-streaming-server.csproj  # Project configuration
├── Dockerfile                    # Container build instructions
├── docker-compose.yml           # Service orchestration
├── appsettings.json             # Application configuration
├── wwwroot/                     # Static web files
│   └── player/
│       ├── flv.html            # Web player interface
│       ├── flv.css             # Player styling
│       └── flv.js              # FLV.js library
└── README.md                   # This documentation
```

## Development Notes

### Building the Application
```bash
dotnet build
```

### Publishing for Release
```bash
dotnet publish -c Release -o ./publish
```

### Docker Commands
```bash
# Build image
docker build -t live-streaming-server .

# Run container
docker run -p 1935:1935 -p 8080:8080 live-streaming-server
```

## Troubleshooting

### Common Issues

1. **Stream not connecting**: Check firewall settings for ports 1935 and 8080
2. **Player not loading**: Ensure the stream is publishing before attempting playback
3. **CORS errors**: Verify the server is running and accessible from your browser
4. **Container issues**: Check Docker logs with `docker-compose logs`

### Testing Stream Connectivity

Test if the RTMP server is accepting connections:
```bash
ffmpeg -f lavfi -i testsrc=duration=10:size=320x240:rate=1 -f flv rtmp://localhost:1935/live/test
```

Then access the stream at: `http://localhost:8080/player/flv.html` with URL `/live/test.flv`

## References

- [LiveStreamingServerNet GitHub](https://github.com/josephnhtam/live-streaming-server-net/tree/develop)
- [FLV.js Demo](https://bilibili.github.io/flv.js/demo/)
- [RTMP Specification](https://www.adobe.com/devnet/rtmp.html)

## License

This project uses the LiveStreamingServerNet library. Please refer to their licensing terms for usage restrictions.
