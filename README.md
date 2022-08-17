

<table frame="void">
    <tr>
      <td width="200px">
        <img src="https://raw.githubusercontent.com/Lachee/unity-utilities/master/Editor/Icons/logo.png" align="center" width="100%" />
      </td>
      <td>
        <h1>Lachee's Utilities</h1>
        <p>
            <a href="https://github.com/Lachee/unity-utilities/actions/workflows/release.yml"><img src="https://github.com/Lachee/unity-utilities/actions/workflows/release.yml/badge.svg" /></a>
            <a href="https://github.com/Lachee/unity-utilities/tags"><img alt="GitHub package.json version" src="https://img.shields.io/github/package-json/v/lachee/unity-utilities?label=github"></a>
            <a href="https://openupm.com/packages/com.lachee.utilities/"><img src="https://img.shields.io/npm/v/com.lachee.utilities?label=openupm&amp;registry_uri=https://package.openupm.com" /></a>
        </p>
        <p>
          This package contains a collection of useful classes and tools that I have personally used throughout my games. 
          Originating from Party Crashers, I have been slowly building and involving this kit, and now with Unity Package Manager being "somewhat" stable, I decided to make a github repository so I can better track the changes and synchronise the numerous versions I have.
        </p>
        <p>
          Since it is just a "collection of scripts", there isn't much in the ways of a manual or a theme for the package other than "hey thats useful". Since it is all under MIT, you are free to simply extract just the scripts you need, there is no dependencies amongst the files unless explicitly stated in the top of the file (ie: some of the custom editors).
        </p>
      </td>
    </tr>
</table>

# Usage
Since this is just a folder of scripts, you can use it how you need it. Check out the documentation for more information.

[https://lachee.github.io/unity-utilities/](https://lachee.github.io/unity-utilities/) 

# Installation
#### OpenUPM <a href="https://openupm.com/packages/com.lachee.utilities/"><img src="https://img.shields.io/npm/v/com.lachee.utilities?label=openupm&amp;registry_uri=https://package.openupm.com" /></a>
The [openupm registry](https://openupm.com)  is a open source package manager for Unity and provides the [openupm-cli](https://github.com/openupm/openupm-cli) to manage your dependencies.
```
openupm add com.lachee.unity-utilities
```

#### Manual UPM             <a href="https://github.com/Lachee/unity-utilities/tags"><img alt="GitHub package.json version" src="https://img.shields.io/github/package-json/v/lachee/unity-utilities?label=github"></a>
Use the Unity Package Manager to add a git package. Adding the git to your UPM will limit updates as Unity will not track versioning on git projects (even though they totally could with tags).
1. Open the Unity Package Manager and `Add Package by git URL...`
2. `https://github.com/Lachee/unity-utilities.git `

For local editable versions, manually clone the repo into your package folder. Note the exact spelling on destination name.
1. `git clone https://github.com/Lachee/unity-utilities.git Packages/com.lachee.utilties`

#### Unity Package
Go old school and download the Unity Package and import it into your project.
1. Download the `.unitypackage` from the [Releases](releases) or via the last run `Create Release` action.
2. Import that package into your Unity3D

# TODO
List of things I wish to implement:

  - More Surrogates for Binary Formatter
  - My PlayerPrefs from Cross-Platform as it supports Serialization / Deserialization
  - Fix to my Automatic Namespacer with better support for rules
    - I should rewrite this
  - Configuration panel for stuff? Probably not required
  - Implement https://forum.unity.com/threads/detecting-textmesh-pro.755501/ so i can add Input features
