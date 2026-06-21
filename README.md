# HimawariBackground 🌍

![Himawari 8](https://img.shields.io/badge/Satellite-Himawari%208-blue)
![Platform](https://img.shields.io/badge/Platform-Windows-lightgrey)
![.NET](https://img.shields.io/badge/.NET-3.5-purple)
![License](https://img.shields.io/badge/License-MIT-green)

**HimawariBackground** is a lightweight Windows background application that automatically fetches real-time images of Earth from the Japanese Himawari 8 weather satellite and sets them as your desktop wallpaper. 

Instead of dealing with PowerShell scripts, Task Scheduler configurations, and execution policies, this standalone C# application handles everything for you. It runs silently in the background, updates your wallpaper every 10 minutes, and automatically adds itself to Windows startup.

## 🌌 How it works

The Himawari 8 satellite captures full-disk images of the Earth, which are updated every 10 minutes. On the server, each image is split into a 4x4 matrix (16 tiles). 

When the application runs, it:
1. Downloads the 16 individual image tiles from the satellite server.
2. Seamlessly stitches them together into a single high-resolution image.
3. Sets the newly created image as your desktop wallpaper.
4. Repeats this process every 10 minutes to keep your background up-to-date.

## ✨ Features

- 🛰️ **Real-time Earth View**: Get a live look at the Earth, including weather patterns, typhoons, and day/night cycles.
- ⏱️ **Auto-Updates**: Automatically refreshes the wallpaper every 10 minutes.
- 🤫 **Silent Operation**: Runs completely in the background without any pop-ups, notifications, or message boxes.
- 🚀 **Auto-Start**: Automatically adds itself to Windows startup so your wallpaper is always up-to-date when you boot your PC.
- 🛠️ **Zero Configuration**: No need to mess with PowerShell execution policies or set up Windows Task Scheduler tasks.

## 📥 Installation & Usage

1. Download the latest `HimawariBackground.exe` from the repository.
2. Run the executable. 
3. That's it! The application will immediately fetch the latest satellite image, set it as your wallpaper, and start running in the background.

*Note: The application requires an active internet connection to download the image tiles.*

## 🛠️ Technologies

- **Language**: C#
- **Framework**: .NET Framework 3.5
- **Platform**: Windows

## 📜 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Credits & References

- **Original Concept**: The idea originated from a Windows PowerShell script written by [MichaelPote](https://gist.github.com/MichaelPote/92fa6e65eacf26219022) that scrapes the latest image from the Himawari-8 satellite and recombines the tiled image [[10]].
- **Inspiration**: This C# application was created as a standalone, user-friendly alternative to the script, solving the issues of Task Scheduler setup and permissions, as featured in the [tproger.ru article](https://tproger.ru/articles/himawari-8-downloader) [[7]].
- **Satellite Data**: Imagery provided by the [Himawari 8 Weather Satellite](https://himawari8.nict.go.jp/).

---
*Enjoy your live view of Earth from space! 🌎*

## About the author

**Ivan Koroteev** is a software engineer, AI creator and public speaker from Moscow, working at the intersection of backend engineering, generative AI, media production and education.

He has 10+ years of experience in software development, including .NET backend systems, integrations, microservices, document recognition, AI-assisted workflows and automation. He has also spoken at TEDx, taught programming and AI, and built practical tools for creators, educators and independent media.

Ivan also publishes creative and musical experiments under the pseudonym **Aivan Karade** — a name used for AI-assisted music, storytelling, visual media and poetic technology projects.

This project reflects his larger mission: to make AI tools more open, practical and empowering for independent creators — especially in multilingual contexts such as Hindi, English, Arabic, Chinese, French and Spanish.
