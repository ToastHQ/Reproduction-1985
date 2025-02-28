<div align="center">
  <img width="400" src="/Branding/pd-dark.svg" alt="Material Bread logo">
  <p align="center">
    Animatronic Show Simulation Software<br/>
    <a href="https://discord.gg/U6Cs7njpFJ">Discord Server</a>
  </p>
</div>

---

### Contents
1. [About](#about)
2. [Getting Started](#getting-started)
    - [Gamejolt](#gamejolt)
    - [Source Code](#source-code)
3. [Contributing](#contributing)
4. [Roadmap](#roadmap)
5. [Copyright](#copyright)
6. [Acknowledgments](#acknowledgments)


## About
Project Duofur is an animatronic simulation software based in Unity. Most of its code currently derives from [RR Engine Vanilla Enhanced](https://github.com/EnderSkippy/RR-Engine-Vanilla-Enhanced), a fork of the long-abandoned RR Engine (aka Reel to Real). This project aims to continue it and add needed features.

## Getting Started
> [!NOTE]
> Most maps are not ready at this time. The 3-Stage map is the most stable and ideally should be used as a base.
This is instructions on how to install and setup the software. Only the source code method can be used if you intend on [contributing to the software.](#contributing)

> [!IMPORTANT]
> Read the [Legal Disclaimer](#legal-disclaimer) before you use this software.

### Gamejolt
*Recommended for general users of the software. Quickest method.*

No releases have been published at this time. Please check back later.

### Source Code
*Recommended for experienced users & people intending on contributing to the software.*

> [!NOTE]
> It's recommended to have good knowledge of both Github and Unity before proceeding, else you are more likely to run into issues.

1. Setup Prerequisites
   - Make sure you are on a stable internet connection
   - Install Unity 6 Long Term Support (6000.0.37f1). This can be done via the [Unity Hub](https://docs.unity3d.com/hub/manual/InstallHub.html)
   - Install a suitable git client, we recommend [Github Desktop](https://desktop.github.com/download/)
   - [Fork the repository](https://docs.github.com/en/pull-requests/collaborating-with-pull-requests/working-with-forks/fork-a-repo#forking-a-repository) and then [clone your fork](https://docs.github.com/en/pull-requests/collaborating-with-pull-requests/working-with-forks/fork-a-repo#cloning-your-forked-repository) onto a drive with suitable space (25GB+)
2. Open the project
   - Switch to the relevant branch based on what you intend on doing. See [Branches](#branches) for more info.
   - Open the Unity Hub and then click `Open > Add Project From Disk`
   - Wait for the project to open, it will take a bit as it has to download & install all the packages.
4. Get editing!
   - As of right now, map scenes are stored in `Assets > Scenes > Maps`. Once you're in one *(we recommend the 3-Stage map as it is the most used)*, hit the play button and you'll be shortly loaded in!
  
Make sure you fetch changes regularly via your git client (e.g Github Desktop). Fetching changes will make sure you always have the latest version of the software.

## Contributing

> [!WARNING]
> Be advised that maintaining your own independent "mod" of the software will be a challenging task if you intend on staying up to date with the latest features. It's easier to contribute to the main repository instead.
> If you do choose to maintain your own mod, please make sure you keep it up to date with the base to ensure proper compatibility.

### Branches
There are two branches currently
1. **main**: This is the stable version of the software, usually in line with the latest release. May not contain the latest features. Should only really be used if you are using the software via the source code method.
2. **dev**: This is the bleeding-edge version of the software, this will contain all the latest changes. It's recommended to work on new features here.

Once you have made the necessary changes in your fork, [open up a pull request](https://docs.github.com/en/pull-requests/collaborating-with-pull-requests/proposing-changes-to-your-work-with-pull-requests/about-pull-requests) with relevant information.

## Roadmap
- [x] Switch from HDRP to URP
- [ ] Prop system for stages, allowing for more customisation on stages (e.g organ fronts, etc)
- [ ] Launcher-style main menu
- [ ] New flow management system
- [ ] Improve the existing .*shw format's security & performance
- [x] Valve Sounds
    
- [ ] **Maps**   
  - [ ] **3-Stage**  
    - [x] All materials converted to URP (no pink missing shaders)
    - [x] General Props implemented fully
    - [ ] Spotlights prop
    - [ ] **Characters**
      - [ ] CathodePlayer's Helen (unknown location, was publically released in [Animatronic Game Hub](https://discord.gg/WRQvaw6EM9))
      - [ ] Munch Jr.
  - [ ] **Cyberamics (Pizza Time Players)**
    - [ ] All materials converted to URP (no pink missing shaders)
    - [ ] Band Board prop
    - [ ] Spotlights prop
    - [ ] **Characters**
      - [ ] CathodePlayer's PTT Cyberamics (unknown location, was publically released in [Animatronic Game Hub](https://discord.gg/WRQvaw6EM9))
      - [ ] ReminaProd's extra Warblettes variants (from [Reel to Real Ultimate Mod](https://github.com/ReminaProd/RR-Engine-Ultimate-Mod))
  - [ ] **Cyberamics (Munch's Make Believe Band)**  
      - [ ] All materials converted to URP (no pink missing shaders)
      - [ ] **Characters**
        - [ ] CathodePlayer's MMBB Cyberamics (unknown location, was publically released in [Animatronic Game Hub](https://discord.gg/WRQvaw6EM9))
        - [ ] ReminaProd's MMBB Stages (from [Reel to Real Ultimate Mod](https://github.com/ReminaProd/RR-Engine-Ultimate-Mod))
  - [ ] **Winchester**  
      - [ ] All materials converted to URP (no pink missing shaders)
  - [ ] **Kooser**  
      - [ ] All materials converted to URP (no pink missing shaders)
  - [ ] **Awesome Adventure Machine**  
      - [ ] All materials converted to URP (no pink missing shaders)
  - [ ] **Tully**  
      - [ ] All materials converted to URP (no pink missing shaders)
  - [ ] **Studio C**  
      - [ ] All materials converted to URP (no pink missing shaders)

## Copyright
This project is licensed under the GNU General Public License v3.0 - see the [LICENSE](LICENSE) file for details.

### Legal Disclaimer

> This project is an unofficial, fan-made continuation of the “Reel to Real” game, originally developed by The 64th Gamer. It is not affiliated with, endorsed by, or associated with Chuck E. Cheese, CEC Entertainment Inc, Creative Engineering Inc, or any related entities. All trademarks, characters, and other copyrighted materials are the property of their respective owners. 
>
> This project is intended solely for educational and entertainment purposes, and is non-commercial in nature. No financial gain is made from this project, and all content remains free to the public. Any use of copyrighted materials is for the purpose of preservation, tribute, and transformation, and is considered fair use under copyright law.
> 
> This project is provided "as is," without any warranty of any kind, either express or implied, including but not limited to the warranties of merchantability, fitness for a particular purpose, or non-infringement. In no event shall the authors or copyright holders be liable for any claim, damages, or other liability, whether in an action of contract, tort, or otherwise, arising from, out of, or in connection with the software or the use or other dealings in the software.

</details>

## Acknowledgments
- [The64thGamer](https://github.com/The64thGamer) &middot; RR Engine
- Himitsu &middot; RR Engine Codebase Improvements
- [EnderSkippy](https://github.com/EnderSkippy) &middot; RR Engine Vanilla Enhanced
