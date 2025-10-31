Below is an improved `README.md` based on your requirements 👇
Feel free to tweak wording!

# Grey Hack Audio Mod

This mod adds computer-like audio feedback to **Grey Hack**, enhancing immersion by simulating real system sounds.  
You will hear things such as boot-up beeps, hard-drive access, system error beeps, OS alerts, and more.

## Features / Included Sounds

The mod currently includes the following audio effects:

| Audio File | Description |
|-----------|-------------|
| `bootup-beep.wav` | Startup BIOS-style beep |
| `cpu-fan-loop.wav` | Looping CPU fan noise |
| `game-over.wav` | Failure/game-over sound |
| `harddrive-loop.wav` | Hard-disk idle/working loop |
| `harddrive-spindown.wav` | Hard-disk powering down |
| `harddrive-spinup.wav` | Hard-disk powering up |
| `os-action-fail.wav` | Action failed sound |
| `os-boot.wav` | OS boot chime |
| `os-error.wav` | OS error alert |
| `os-notification.wav` | Notification ping |
| `system-failure-loop.wav` | Critical failure loop |
| `trace-beep.wav` | Network trace alert beep |

## Installation

### ✅ **BepInEx 5 Manual Install (recommended)**

1. Install **BepInEx 5** for Grey Hack  
   *(This mod is designed for BepInEx 5)*  
2. Place the mod `.dll` file into:

```

Grey Hack/BepInEx/plugins/

```

3. Place the **Audio** folder from this mod into:

```

Grey Hack/BepInEx/plugins/Audio/

```

4. Launch the game — the sounds will play automatically.

### ⚠️ BepInEx 6 Support

If you require the **BepInEx 6 version**, download it here:

🔗 https://github.com/Patrick-van-Halm/greyhack.audio-mod/releases/latest

---

## Custom Audio

All sounds are completely user-replaceable.

To customize audio:
1. Ensure the game is closed.
2. Go to the specific BepInEx plugin folder
   - **Thunderstore installation**
     1. Go to the Settings > Locations > Browse profile folder. (This opens a new explorer folder)
     ![img.png](img/profile-location.png)
     2. Go to the installed mod folder:
     ```
     BepInEx/plugins/AudioMod/
     ```
   - **Manual Installation**
     1. Go to the installed mod folder:
      ```
      <Grey Hack Installation Folder>/BepInEx/plugins/AudioMod/
      ```
3. Go into the Audio folder (here are the audio clips stored)
   ```
   Audio/
   ```
4. Replace any `.wav` file with your own version **using the same file name**.
   > Example: replace `os-error.wav` with your own error sound.

5. Restart the game.

---

## About

This mod aims to make Grey Hack feel more like interacting with a physical machine, adding ambient and reactive system audio to deepen immersion and atmosphere.

---

Enjoy the enhanced hacking experience! 🎧💻