UniState
===

## Table of Contents
- [Getting started](#getting-started)
- [Install via git URL](#install-via-git-url)
- [License](#license)

Getting started
---
Install via [Install via git URL](#install-via-git-url) with git reference.

Install via git URL
---
Requires a version of unity that supports path query parameter for git packages (Unity >= 2020.1a21). You can add `https://github.com/bazyleu/UniState.git?path=Assets/UniState#` to Package Manager

![image](https://github.com/user-attachments/assets/120e6750-1f33-44f7-99c8-a3e7fa166d21)
![image](https://github.com/user-attachments/assets/3fed7201-b748-4261-b4f8-7bdffdac072d)

or add `"com.bazyleu.unistate": "https://github.com/bazyleu/UniState.git?path=Assets/UniState#"` to `Packages/manifest.json`.

If you want to set a target version, UniState uses the `*.*.*` release tag so you can specify a version like `#1.1.0`. For example `https://github.com/bazyleu/UniState.git?path=Assets/UniState#1.1.0`.

> [!IMPORTANT]
> You need to install UniTask package before UniState usage. Guide regarding UniTask installation can be found [here](https://github.com/Cysharp/UniTask/blob/master/README.md#upm-package).

License
---
This library is under the MIT License. Full text is [here](LICENSE).