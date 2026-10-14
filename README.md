# VR Biometric Simulation & AI Report System

A Virtual Reality application developed in Unity for Meta Quest headsets. The simulation combines real-time biometric and motion tracking via UDP (heart rate and accelerometer-based locomotion), environmental gaze tracking (`GazeTracker`), interactive scenarios (`HorseController`), cloud telemetry (Ubidots/Firebase), and automated end-of-session analysis powered by the Google Gemini API (`LLMReport`).

---

## 1. Prerequisites

Before opening the project, install the following on your PC:
* **Unity Hub**: Download from [unity.com/download](https://unity.com/download).
* **Meta Quest Link App**: Download from [meta.com/quest/setup](https://www.meta.com/quest/setup/).
* **Hardware**: A Meta Quest headset (Quest 2, 3, or Pro) connected via a USB 3.0 Link cable (or a 5GHz Wi-Fi network for Air Link).

---

## 2. How to Open the Project in Unity

Even if you have never used Unity before, follow these exact steps:

1. Download and extract the project folder to your PC (or clone the repository via Git).
2. Open **Unity Hub** and click the **Add** (or **Open**) button in the top-right corner.
3. Select the extracted project folder.
4. Unity Hub will automatically detect the required Unity Editor version. If a warning appears, click **Install** to download that exact version.
   * **Important**: When selecting installation modules, check **Android Build Support** (including OpenJDK and Android SDK & NDK Tools).
5. Once installed, click the project name in Unity Hub to launch the editor.
   * *Note: The first launch will take several minutes while Unity generates the local `Library` cache.*

---

## 3. Setting Up the Gemini API Key (Required)

To generate the AI session summary at the end of the simulation, you must insert a valid Google Gemini API key into the code:

1. Inside the Unity Editor, go to the **Project** panel at the bottom of the screen.
2. Type `LLMReport` in the search bar of the Project panel.
3. Double-click the **LLMReport** C# script to open it in your code editor (such as Visual Studio).
4. Locate the `apiKey` variable at the top of the class:
   ```csharp
   [Header("Impostazioni API Gemini")]
   private readonly string apiKey = "INSERT_YOUR_API_KEY_HERE";
   private readonly string primaryModel = "gemini-3.1-flash-lite";
   private readonly string fallbackModel = "gemma-4-31b-it";
   ```
5. Replace the string inside the quotes with your personal Gemini API key.
6. Save the file (`Ctrl + S` on Windows, `Cmd + S` on Mac) and return to Unity so it recompiles automatically.
   * *(Optional)* If you are using cloud telemetry, open the Ubidots script in the same way and insert your personal token.

---

## 4. Running the Simulation

1. Turn on your Meta Quest headset and connect it to the PC using the Link cable.
2. Put on the headset and launch **Quest Link** from the VR Quick Settings menu.
3. In the Unity Editor on your PC, open the starting scene from the `Assets/Scenes` folder (if not already open).
4. Press the **Play (▶)** button at the top-center of the Unity Editor.
5. Put on the headset to interact with the simulation:
   * **Movement & Biometrics**: Physical steps and heart rate data received via UDP dynamically control movement speed (`SpeedCalculator`) and environment responsiveness (`HorseController`).
   * **Gaze Tracking**: Head orientation and looked-at objects are logged automatically at 10Hz into a local `.csv` file (`SimulationReport_*.csv`).
   * **AI Report**: Trigger the report popup in the VR UI at the end of a session to send the CSV data to Gemini (with automatic fallback to `gemma-4-31b-it` in case of high server traffic).

---

## 5. Troubleshooting (Laptops & Performance)

If you experience frame drops or lag while testing on a laptop:
* **Connect to Power**: Always keep the laptop plugged into the wall charger.
* **Force Dedicated GPU**: Open the *NVIDIA Control Panel* -> *Manage 3D Settings*, and set both `Unity.exe` and `OVRServer_x64.exe` to use the **High-performance NVIDIA processor**.
* **Hide Scene View**: While in Play Mode, close or minimize the **Scene** tab in Unity so your PC does not render the 3D environment twice.
