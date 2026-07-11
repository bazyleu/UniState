# UniState

[![Last Releases](https://img.shields.io/github/v/release/bazyleu/UniState.svg)](https://github.com/bazyleu/UniState/releases)
[![Last Release Date](https://img.shields.io/github/release-date/bazyleu/UniState)](https://github.com/bazyleu/UniState/releases)
[![All Tests](https://github.com/bazyleu/UniState/actions/workflows/tests.yml/badge.svg?branch=main)](https://github.com/bazyleu/UniState/actions)
[![Last Commit](https://img.shields.io/github/last-commit/bazyleu/UniState)](https://github.com/bazyleu/UniState/branches)
[![License](https://img.shields.io/github/license/bazyleu/UniState)](LICENSE)

UniState is a modern, high-performance, scalable state machine package for Unity. It can serve as a core architectural
pattern or be used to address specific tasks.

* **Performance**: optimized for minimal runtime allocations, see [performance](#performance) section for details.
* **Modularity**: designed to define [states](#state), [substates](#substate), and [state machines](#state-machine) in an
  isolated way. States can be easily replaced or removed without hidden effects even in big projects.
* **Scalability**: memory allocations happen only on demand, [performance](#performance) does not degrade with the number of
  states and state machines.
* **Asynchronous**: modern asynchronous API with async-await and [UniTask](https://github.com/Cysharp/UniTask)
* **Reliability**: allows you to define [global error handling](#state-machine-error-handling) at the state machine level,
  and guarantees that all resources will be [disposed](#disposables).
* **Simplicity**: if you use [state base](#state-creating) you have to implement only one method for fast start.
* **Flexibility**: everything in framework core is an abstraction. Can be replaced with your own implementation,
  see [state creating](#state-creating) and [creating a state machine](#creating-a-state-machine) sections for details.
* **Testability**: UniState is designed to be testable. All abstractions use interfaces that can be easily mocked with
  [NSubstitute](https://nsubstitute.github.io/) or any other framework. States can be run separately for testing
  purposes.
* **AI development friendly**: explicit state classes, typed payloads and transitions, and DI registrations give
  coding assistants stable architectural boundaries to follow. The
  [mobile game architecture guide](#mobile-game-architecture-guide) provides practical patterns for implementing
  complete Unity game flows with states, database services, popups, gameplay, and meta progression.
* **DI friendly**: has [integration](#integrations) with most popular DI containers
* **Continuous Testing**: fully covered by tests. All tests run [automatically](https://github.com/bazyleu/UniState/actions) to verify each change.

## Table of Contents

<!-- TOC start (generated with https://github.com/derlin/bitdowntoc) -->

- [Getting Started](#getting-started)
- [Installation](#installation)
    * [Requirements](#requirements)
    * [Option 1: Add package from git URL](#option-1-add-package-from-git-url)
    * [Option 2: Add via manifest.json](#option-2-add-via-manifestjson)
- [Performance](#performance)
- [Framework Philosophy](#framework-philosophy)
    * [Dependency Injection](#dependency-injection)
    * [What is a State?](#what-is-a-state)
    * [Mobile Game Architecture Guide](#mobile-game-architecture-guide)
        + [Flow at a Glance](#flow-at-a-glance)
        + [Architecture Rules](#architecture-rules)
        + [Register the Flow in VContainer](#register-the-flow-in-vcontainer)
        + [Start With One Root State](#start-with-one-root-state)
        + [Keep Long-Lived Screens in Root States](#keep-long-lived-screens-in-root-states)
        + [Use a Distribution State to Refresh the Hub](#use-a-distribution-state-to-refresh-the-hub)
        + [Convert Hub UI Actions into Transitions](#convert-hub-ui-actions-into-transitions)
        + [Pass Short-Lived Context with Payloads](#pass-short-lived-context-with-payloads)
        + [Run Gameplay as a State, Not as the Whole App](#run-gameplay-as-a-state-not-as-the-whole-app)
        + [Insert Between-Level Meta States](#insert-between-level-meta-states)
        + [Use Nested State Machines for Modal Flows](#use-nested-state-machines-for-modal-flows)
        + [Keep Persistent Progress Outside States](#keep-persistent-progress-outside-states)
        + [Transition Choices](#transition-choices)
        + [Short Guide Takeaway](#short-guide-takeaway)
- [API Details and Usage](#api-details-and-usage)
    * [State](#state)
        + [State Creating](#state-creating)
        + [State Lifecycle](#state-lifecycle)
        + [State Transitions](#state-transitions)
        + [Disposables](#disposables)
        + [State Behavior Attribute](#state-behavior-attribute)
    * [State Machine](#state-machine)
        + [Creating a State Machine](#creating-a-state-machine)
        + [Running a State Machine](#running-a-state-machine)
        + [Launching Nested State Machines](#launching-nested-state-machines)
        + [State Machine History](#state-machine-history)
        + [State Machine State Change Hook](#state-machine-state-change-hook)
        + [State Machine Error Handling](#state-machine-error-handling)
            - [General Error-Handling Principles](#general-error-handling-principles)
            - [State Machine Specific Exceptions](#state-machine-specific-exceptions)
        + [Built-in Support for DI Scopes](#built-in-support-for-di-scopes)
        + [Custom Type Resolvers](#custom-type-resolvers)
        + [Working Without a DI Framework](#working-without-a-di-framework)
    * [Composite State](#composite-state)
        + [Creating a Composite State](#creating-a-composite-state)
        + [SubState](#substate)
        + [Default Composite State](#default-composite-state)
- [Tutorials](#tutorials)
    * [Simple Dice Game](#simple-dice-game)
        + [Overview](#overview)
        + [Step 1: Create the states](#step-1-create-the-states)
        + [Step 2: Create entry point](#step-2-create-entry-point)
        + [Step 3: Configure VContainer](#step-3-configure-vcontainer)
        + [Step 4: Set up the scene](#step-4-set-up-the-scene)
- [Upgrade Guide](#upgrade-guide)
    * [Upgrading from Versions < 1.11.0](#upgrading-from-versions--1110)
    * [Upgrading from Versions < 1.5.0](#upgrading-from-versions--150)
- [Integrations](#integrations)
    * [VContainer](#vcontainer)
        + [VContainer Preparation](#vcontainer-preparation)
        + [VContainer Usage](#vcontainer-usage)
        + [VContainer Registering](#vcontainer-registering)
    * [Zenject (Extenject)](#zenject-extenject)
        + [Zenject Preparation](#zenject-preparation)
        + [Zenject Usage](#zenject-usage)
        + [Zenject Registering](#zenject-registering)
    * [Reflex](#reflex)
        + [Reflex Preparation](#reflex-preparation)
        + [Reflex Usage](#reflex-usage)
        + [Reflex Registering](#reflex-registering)
- [License](#license)

<!-- TOC end -->

## Getting Started

**Step 1:** 
Install UniState by adding the following URL to Unity Package Manager:  
`https://github.com/bazyleu/UniState.git?path=Assets/UniState`.  
Details on installation are available [here](#installation).

**Step 2:** Create a state by defining a class that inherits from `StateBase` or `StateBase<T>`. Example transition logic:
```csharp
public class MainMenuState : StateBase
{
    public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
    {
        // Add your state logic here
        return Transition.GoTo<GameplayState>();
    }
}

public class GameplayState : StateBase
{
    public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
    {
        // Add your state logic here
        return Transition.GoBack();
    }
}
```
Detailed information about creating states is
available [here](#state-creating).

**Step 3:** Configure Dependency Injection (DI) by registering the state machine and states in the DI container. 
```csharp
builder.RegisterStateMachine<IStateMachine, StateMachine>();
builder.RegisterState<MainMenuState>();
builder.RegisterState<GameplayState>();
```
Additional information on DI configuration is available [here](#integrations).

**Step 4:** Create and run the state machine by specifying the initial state.
```csharp
    public class Game
    {
        // Note that you must resolve the interface but not the implementation
        private readonly IStateMachine _stateMachine;

        public Game(IStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void Start()
        {
            _stateMachine.Execute<MainMenuState>(CancellationToken.None).Forget();
        }
    }
```
More details on running the state machine can be found [here](#running-a-state-machine).

That's it! Your first UniState project is set up. You can find a more detailed walkthrough in the
[tutorials](#tutorials) section.

## Installation

### Requirements

* Requires Unity version 2022.3 or higher.
* Requires UniTask package installed. Guide regarding UniTask installation can be found
  on [Cysharp/UniTask README](https://github.com/Cysharp/UniTask/blob/master/README.md#upm-package).

### Option 1: Add package from git URL

You can add `https://github.com/bazyleu/UniState.git?path=Assets/UniState` to Package Manager.

It is a good practice to specify target version, UniState uses the `*.*.*` release tag so you can specify a version
like `#1.12.0`. For example `https://github.com/bazyleu/UniState.git?path=Assets/UniState#1.12.0`.
You can find latest version number [here](https://github.com/bazyleu/UniState/releases).

![image](https://github.com/user-attachments/assets/120e6750-1f33-44f7-99c8-a3e7fa166d21)
![image](https://github.com/user-attachments/assets/3fed7201-b748-4261-b4f8-7bdffdac072d)

### Option 2: Add via manifest.json

You can add `"com.bazyleu.unistate": "https://github.com/bazyleu/UniState.git?path=Assets/UniState"` (or with version
tag `https://github.com/bazyleu/UniState.git?path=Assets/UniState#1.9.0`) to `Packages/manifest.json`.

## Performance

UniState is the fastest and most efficient asynchronous state machine available for Unity. When compared to state
machine implementations based on MonoBehaviour, UniState delivers a performance boost of over 5000x in execution speed
and up to a 10x reduction in allocations.

For typical scenarios involving small to medium state chains - the most common use case - UniState can reduce memory
allocations by a factor ranging between 2x and 10x. In cases where state chains exceed 200 states, the benefits in
memory allocation become less pronounced, but execution speed remains consistent with a 5000x+ boost.

Measurements for Windows PC (with IL2CPP scripting backend):
```
Benchmark Mono 10 states: 516.4 ms, 120.83 KB
Benchmark Mono 50 states: 2520.9 ms, 150.44 KB
Benchmark Mono 200 states: 10033.6 ms, 283.83 KB

Benchmark UniState 10 states: 0.1 ms, 13.11 KB
Benchmark UniState 50 states: 0.2 ms, 68.81 KB
Benchmark UniState 200 states: 0.7 ms, 273.20 KB

Benchmark UniState with history 10 states: 0.1 ms, 14.34 KB
Benchmark UniState with history 50 states: 0.2 ms, 69.58 KB
Benchmark UniState with history 200 states: 0.7 ms, 276.95 KB
```

## Framework Philosophy

### Dependency Injection

All dependencies for states, commands, and other entities should be passed through the constructor.
UniState supports automatic integration with the most popular DI frameworks for Unity.
Refer to the [integration documentation](#integrations) for more details.
Dependencies must be registered in your DI framework, and they will automatically be resolved when
creating a [state](#state) or [state machine](#state-machine).

### What is a State?

A state is an abstraction that represents a specific condition or phase of the game, often corresponding to a "screen" that the user interacts
with. For example, the main menu is a state, a settings popup is another state, and gameplay itself may take place in a
separate `GameplayState`. When the user opens a shop popup, they may transition into a `ShopState`. However, states are
not always tied to visual elements. Some states, like `GameLoadingState`, may handle background processes such as
loading resources.

A state class contains all logic related to that state, including loading and unloading resources. UniState does not restrict the use of other
frameworks or patterns, meaning you can freely use whatever suits your needs. You could, for example, run controllers
and follow an MVC approach, follow an MVVM approach, or even execute ECS code within a state.

The key concept of the framework is that once a state is exited, all resources it allocated should be released. For
details on how to do this, see [Disposables](#disposables).

It is not recommended to use Unity GameObjects directly inside states, as it reduces testability and increases code
coupling. A better approach is to load GameObjects through an abstraction and use them as an interface (essentially as a
View in UniState). Add a handler for unloading to the Disposables of the state that loaded it. All approaches and patterns
mentioned above support this, and you can choose any based on your preferences, as this functionality is
outside the scope of UniState.

```csharp
    // Popup prefab (MonoBehaviour, view)
    public class SimplePopupView : MonoBehaviour, ISimplePopupView
    {
        //...
    }
    
    // Simple popup state example
    public class SimplePopupState : StateBase
    {
        private ISimplePopupView _view;
    
        public override async UniTask Initialize(CancellationToken token) 
        {
            _view = LoadPopupView(token);
            Disposables.Add(UnloadPopupView);
        }
    
        public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            await _view.Show(token);
            await _view.WaitForClick(token);

            return Transition.GoBack();
        }
        
        public override async UniTask Exit(CancellationToken token)
        {
            await _view.Hide(token);
        }

        // The implementation of this method depends on other frameworks/patterns used in the project.
        private ISimplePopupView LoadPopupView(CancellationToken token)
        {
             // Loading logic
        }
        
        private void UnloadPopupView()
        {
             // Unloading logic
        }
    }
```

If the popup is complex with multiple features, it could be represented as its own state machine. 
In cases where you have a complex popup with its own state machine, it is important to allocate resources specific to the popup before launching the separate
state machine, ensuring they are properly cleaned up after the state machine exits.

```csharp
    // This state loads resources, adds them to Disposables, and runs the internal state machine for ShopPopup.
    // When the StateMachine completes its execution, RootShopPopupState finishes and releases its resources.
    public class RootShopPopupState : StateBase
    {
        public override async UniTask Initialize(CancellationToken token) 
        {
            // Load ShopView (a Unity GameObject) and create an IDisposable handler that 
            // will unload the GameObject after Disposing. 
            // After that, the GameObject will be available as IShopView in internal states.
            var disposable = LoadShopView();
            Disposables.Add(disposable);
        }
    
        public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            var stateMachine = StateMachineFactory.Create<StateMachine>();
            
            // Run the internal state machine for ShopPopup.
            // In all states inside this state machine, all resources allocated in this state will be available.
            await stateMachine.Execute<ShopPopupIdleState>(token);

            return Transition.GoBack();
        }

        // The implementation of this method depends on other frameworks/patterns used in the project.
        private IDisposable LoadShopView()
        {
             // Loading logic
        }
    }
    
    public class ShopPopupIdleState : StateBase
    {
        // IShopView is a Unity GameObject loaded in RootShopPopupState (outside the current state machine). 
        // IShopView will be available as long as RootShopPopupState is running, 
        // meaning throughout the entire internal state machine's operation.
        private IShopView _view;
        
        public ShopPopupIdleState(IShopView view)
        {
             _view = view;
        }
    
        public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            var action = await _view.Show(token);
            
            // Transition logic with 'action'
        }
    }
```


### Mobile Game Architecture Guide

This guide shows how to structure a larger Unity mobile game with UniState: a boot flow, a hub screen,
gameplay levels, between-level rewards, popups, and a meta progression feature such as building a base, upgrading
a headquarters, restoring a town, unlocking an area, or decorating a room.

The examples are intentionally generic. They use ordinary project-owned types such as `GameDatabase`,
`IAssetLoader<T>`, `HubView`, and `ILevelController`. These are not required UniState APIs. Replace them with your own
data services, Addressables loaders, UI views, and gameplay services. UniState's job is to own the lifecycle and
transitions between those parts.

For the snippets below, assume that loaders own the loaded resources and release them when disposed:

```csharp
using System;
using System.Threading;
using Cysharp.Threading.Tasks;

public interface IAssetLoader<T> : IDisposable
{
    UniTask<T> InstantiateAsync(CancellationToken token);
}
```

#### Flow at a Glance

A typical mobile game flow can be modeled as a nested list of small, explicit states:

- `GameEntryPoint` starts `AppRootState`.
- `AppRootState` loads the app shell and runs `HubRootState`.
- `HubRootState` loads the hub screen and runs `HubDistributionState`.
- `HubDistributionState` refreshes the hub, then chooses:
  - `ClaimRunRewardState` if a previous run has an unclaimed reward.
  - `HubIdleState` otherwise.
- `HubIdleState` waits for one hub action, then chooses:
  - `RunSetupState -> LoadoutState -> LevelIntroState -> LevelState` for gameplay.
  - `BuildMetaState -> MetaCompleteState -> NewAreaRevealState` for long-term progression.
  - `SettingsPopupState`, `ShopPopupState`, or `AreaInfoPopupState` for modal hub actions.
- `LevelState` can route to:
  - `LevelWinPopupState -> LevelIntroState` after a completed non-final level.
  - `HubDistributionState` when the run ends.

The important idea is not the exact names. The important idea is that every state owns one clear part of the game:

| State kind | Responsibility |
|------------|----------------|
| Root state | Load long-lived app or feature resources, then run a child flow. |
| Distribution state | Refresh shared UI from persistent data and choose the next state. |
| Idle state | Wait for user actions on an already loaded screen and convert them into transitions. |
| Popup state | Load one temporary view, wait for an action, update data if needed, then return or route forward. |
| Gameplay state | Own the level lifecycle and delegate rules to gameplay services or commands. |
| Between-level state | Apply rewards, upgrades, shops, or choices between gameplay levels. |
| Meta state | Spend persistent resources and update long-term progression. |

#### Architecture Rules

Use these rules when deciding where code should live:

1. A state owns a phase of the game, not the entire game.
2. A state should decide when a phase starts, what temporary resources are loaded, what user action completes the phase,
   and what transition follows.
3. Durable progress should live in a database or services, not in state fields.
4. UI views should expose actions such as `WaitForClose`, `WaitForSelection`, or `WaitForInput`. Views should not
   decide UniState transitions directly.
5. Gameplay rules should live in services, command objects, ECS systems, or another domain layer. The gameplay state
   starts them, observes completion, and transitions.
6. Add loaded views, Addressables handles, event subscriptions, and cancellation sources to `Disposables`.
7. Use nested state machines when a parent state must keep its view alive while a child popup or child flow runs.
8. Return to a distribution state after the database changes and the hub must be refreshed.

#### Register the Flow in VContainer

The VContainer `LifetimeScope` is the composition root. Register the entry point, long-lived services, state machine,
states, and your project-specific loaders here.

```csharp
using UniState;
using VContainer;
using VContainer.Unity;

public sealed class GameLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterEntryPoint<GameEntryPoint>();

        builder.Register<GameDatabase>(Lifetime.Singleton);
        builder.Register<AudioService>(Lifetime.Singleton)
            .AsImplementedInterfaces();
        builder.Register<LevelController>(Lifetime.Transient)
            .AsImplementedInterfaces();
        builder.Register<RewardGenerator>(Lifetime.Transient)
            .AsImplementedInterfaces();

        builder.RegisterStateMachine<IStateMachine, GameStateMachine>();

        builder.RegisterState<AppRootState>();
        builder.RegisterState<HubRootState>();
        builder.RegisterState<HubDistributionState>();
        // ...

        // Project-specific loading. Use Addressables, Resources, scene references,
        // or any loader abstraction that fits your game.
        builder.Register<IAssetLoader<RootCanvasView>, RootCanvasLoader>(Lifetime.Transient);
        builder.Register<IAssetLoader<HubView>, HubLoader>(Lifetime.Transient);
        // ...
    }
}
```

You can use the built-in `StateMachine` directly, or derive from it if the game needs custom error handling.

```csharp
using UniState;
using UnityEngine;

public sealed class GameStateMachine : StateMachine
{
    protected override void HandleError(StateMachineErrorData errorData)
    {
        Debug.LogError(errorData.Exception);
    }
}
```

In general, register states and state machines as transient. A state machine can run only one execution flow at a time,
so nested flows should receive a separate machine instance. The default VContainer `RegisterStateMachine` overload uses
transient lifetime.

#### Start With One Root State

The entry point is the bridge from Unity/VContainer startup into UniState. Keep it small: resolve a state machine and
start one root state.

```csharp
using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using VContainer.Unity;

public sealed class GameEntryPoint : IStartable
{
    private readonly IStateMachine _stateMachine;

    public GameEntryPoint(IStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public void Start()
    {
        _stateMachine.Execute<AppRootState>(CancellationToken.None).Forget();
    }
}
```

`CancellationToken.None` is fine for a minimal sample. In a production project, you can pass an application-level
token that is cancelled on scene unload, logout, or app shutdown.

#### Keep Long-Lived Screens in Root States

Root states load resources that should survive many smaller states. For example, the app root can load the root canvas,
initialize audio, and then run the hub flow as a nested state machine.

```csharp
using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;

public sealed class AppRootState : StateBase
{
    private readonly IAssetLoader<RootCanvasView> _rootCanvasLoader;
    private readonly IStateMachine _hubMachine;

    public AppRootState(
        IAssetLoader<RootCanvasView> rootCanvasLoader,
        IStateMachine hubMachine)
    {
        _rootCanvasLoader = rootCanvasLoader;
        _hubMachine = hubMachine;
    }

    public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
    {
        Disposables.Add(_rootCanvasLoader);

        await _rootCanvasLoader.InstantiateAsync(token);
        await _hubMachine.Execute<HubRootState>(token);

        return Transition.GoToExit();
    }
}
```

The hub root can load the hub screen and environment once, store them in holders, subscribe to shared events, and then
run hub substates.

```csharp
public sealed class HubRootState : StateBase
{
    private readonly IAssetLoader<HubView> _hubLoader;
    private readonly HubViewHolder _hubViewHolder;
    private readonly IStateMachine _hubMachine;

    public HubRootState(
        IAssetLoader<HubView> hubLoader,
        HubViewHolder hubViewHolder,
        IStateMachine hubMachine)
    {
        _hubLoader = hubLoader;
        _hubViewHolder = hubViewHolder;
        _hubMachine = hubMachine;
    }

    public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
    {
        Disposables.Add(_hubLoader);

        var hubView = await _hubLoader.InstantiateAsync(token);
        _hubViewHolder.Initialize(hubView);

        await _hubMachine.Execute<HubDistributionState>(token);

        return Transition.GoToExit();
    }
}
```

This pattern prevents every popup or menu state from loading the hub again. The smaller states can work with
`HubViewHolder` and persistent data services.

#### Use a Distribution State to Refresh the Hub

A distribution state is a lightweight routing state. It refreshes shared UI from durable data, then immediately returns
the next transition.

```csharp
using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;

public sealed class HubDistributionState : StateBase
{
    private readonly GameDatabase _database;
    private readonly HubViewHolder _hub;

    public HubDistributionState(GameDatabase database, HubViewHolder hub)
    {
        _database = database;
        _hub = hub;
    }

    public override UniTask<StateTransitionInfo> Execute(CancellationToken token)
    {
        _hub.SetVisible(true);

        var transition = _database.HasPendingReward
            ? Transition.GoTo<ClaimRunRewardState>()
            : Transition.GoTo<HubIdleState>();

        return UniTask.FromResult(transition);
    }
}
```

Many states can return to this one place after they change persistent data:

- A shop state returns after buying currency.
- A settings state returns after changing options.
- A meta build state returns after spending resources.
- A gameplay state returns after ending a run.
- A reward claim state returns after clearing pending rewards.

This keeps "refresh hub UI" logic out of every feature state.

#### Convert Hub UI Actions into Transitions

An idle state waits for possible player actions on an already visible screen. It turns the first completed action into a
transition.

```csharp
using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;

public enum HubInput
{
    Play,
    Build,
    Shop,
    Settings,
    AreaInfo
}

public sealed class HubIdleState : StateBase
{
    private readonly HubViewHolder _hub;

    public HubIdleState(HubViewHolder hub)
    {
        _hub = hub;
    }

    public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
    {
        var input = await _hub.View.WaitForInput(token);

        switch (input)
        {
            case HubInput.Play:
                return Transition.GoTo<RunSetupState>();

            case HubInput.Build:
                return Transition.GoTo<BuildMetaState>();

            case HubInput.Shop:
                return Transition.GoTo<ShopPopupState>();

            case HubInput.Settings:
                return Transition.GoTo<SettingsPopupState>();

            case HubInput.AreaInfo:
                return Transition.GoTo<AreaInfoPopupState>();

            default:
                return Transition.GoTo<HubDistributionState>();
        }
    }
}
```

The view can race buttons, gestures, or platform input internally and return one `HubInput`. The state owns the mapping
from input to transitions.

After the player presses Play, a setup state usually writes the selected run configuration into the database and moves to a
loadout, deck-building, or loading state.

#### Pass Short-Lived Context with Payloads

Use `StateBase<TPayload>` for data that belongs to one transition. For example, a level intro state can decide whether
the next level should run in the current game mode.

```csharp
public enum GameMode
{
    Normal,
    Assisted
}

public sealed class LevelPayload
{
    public GameMode Mode { get; }

    public LevelPayload(GameMode mode)
    {
        Mode = mode;
    }
}
```

```csharp
using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;

public sealed class LevelIntroState : StateBase
{
    private readonly GameDatabase _database;

    public LevelIntroState(GameDatabase database)
    {
        _database = database;
    }

    public override UniTask<StateTransitionInfo> Execute(CancellationToken token)
    {
        _database.AdvanceLevel();
        var payload = new LevelPayload(_database.Mode);

        return UniTask.FromResult(
            Transition.GoTo<LevelState, LevelPayload>(payload));
    }
}
```

Good payloads are one-time parameters: selected difficulty, game mode, popup reason, selected item id. Persistent data
such as currencies, inventory, current run deck, and meta progress should live in the database.

#### Run Gameplay as a State, Not as the Whole App

The gameplay state owns the level lifecycle. It should not contain every rule in the game. Delegate gameplay rules to
a domain service, command engine, ECS world, or another testable layer.

```csharp
using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;

public enum LevelResult
{
    Completed,
    Exited
}

public sealed class LevelState : StateBase<LevelPayload>
{
    private readonly GameDatabase _database;
    private readonly ILevelController _levelController;

    public LevelState(
        GameDatabase database,
        ILevelController levelController)
    {
        _database = database;
        _levelController = levelController;
    }

    public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
    {
        var result = await _levelController.RunAsync(_database, Payload, token);

        if (result == LevelResult.Exited)
        {
            _database.MarkRunRewardPending();
            return Transition.GoTo<HubDistributionState>();
        }

        _database.SaveLevelResult(result);

        if (_database.IsFinalLevel)
        {
            _database.MarkRunRewardPending();
            return Transition.GoTo<HubDistributionState>();
        }

        return Transition.GoTo<NextLevelState>();
    }
}
```

This state decides the flow around gameplay:

- Start the level.
- Run the level.
- Save the result.
- Open a nested win popup for a completed non-final level.
- Go to a reward draft between levels.
- Return to the hub when the run ends.

The card rules, enemy behavior, physics, or puzzle logic stay in `ILevelController` or another domain layer.

#### Insert Between-Level Meta States

Between-level states are where many mobile games grow their depth: reward drafts, shops, healing screens, upgrade
choices, narrative events, or ad offers. The previous level should not know the details of these systems. It only
transitions to the next state.

The next level reads updated run data from the database. This makes the loop easy to extend:

```text
LevelState -> RewardDraftState -> LevelIntroState -> LevelState
LevelState -> ShopBetweenLevelsState -> LevelIntroState -> LevelState
LevelState -> HealOrUpgradeState -> LevelIntroState -> LevelState
```

#### Use Nested State Machines for Modal Flows

Nested state machines are useful when a parent state must remain active while a child flow runs. For example, a "new
area unlocked" popup can keep its reveal view alive while it opens a reusable area-info popup.

The reusable popup state stays simple:

```csharp
public sealed class AreaInfoPopupState : StateBase
{
    private readonly IAssetLoader<AreaInfoView> _areaInfoLoader;
    private readonly GameDatabase _database;

    public AreaInfoPopupState(
        IAssetLoader<AreaInfoView> areaInfoLoader,
        GameDatabase database)
    {
        _areaInfoLoader = areaInfoLoader;
        _database = database;
    }

    public override UniTask Initialize(CancellationToken token)
    {
        Disposables.Add(_areaInfoLoader);
        return UniTask.CompletedTask;
    }

    public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
    {
        var view = await _areaInfoLoader.InstantiateAsync(token);
        view.Initialize(_database.CurrentArea);

        await view.WaitForClose(token);

        return Transition.GoBack();
    }
}
```

`AreaInfoPopupState` can be opened from the hub, from the reveal popup, or from another flow. The parent decides where
the flow continues.

If the parent view must stay alive, prefer this nested-machine pattern over relying on `GoBack()` to preserve the same
C# state instance. `GoBack()` returns through state-machine history, but the previous state is resolved again through
the configured container.

#### Keep Persistent Progress Outside States

States are transient. They can be disposed, recreated, skipped by history rules, or run inside nested flows. Store
progress that must survive the state in a database or a service.

Good state fields:

- Current view instance.
- Completion source.
- Temporary selected option.
- Local cancellation source.
- Animation handles.

Good database data:

- Player currencies.
- Inventory.
- Current run deck or loadout.
- Current level number.
- Pending rewards.
- Meta progression.
- Settings.

#### Transition Choices

Use these transitions for the common routing cases:

| Situation | Transition |
|-----------|------------|
| Move to the next known phase | `Transition.GoTo<NextState>()` |
| Move to a state with one-time data | `Transition.GoTo<NextState, Payload>(payload)` |
| Close a temporary modal and return to history | `Transition.GoBack()` |
| Return to a specific previous state | `Transition.GoBackTo<TState>()` |
| Finish a nested flow | `Transition.GoToExit()` |
| Refresh hub after data changes | `Transition.GoTo<HubDistributionState>()` |

#### Short Guide Takeaway

If a reader follows only one rule from this guide, make it this one: keep state classes responsible for flow and
lifetime, keep durable data in a database or services, and keep reusable game rules outside the state.

## API Details and Usage

### State

A state is a fundamental unit of logic in an application, often representing different screens or states, such as an
idle scene, main menu, popup, or a specific state of a popup.

#### State Creating

To create your custom state, you can inherit from `StateBase` or `StateBase<T>`. Use `StateBase<T>` if you need to pass
parameters to the state.

For highly customized states, you can manually implement the `IState<TPayload>` interface. However, in most
cases, `StateBase` will suffice.

```csharp

// Simple State Inheritance
public class FooState : StateBase
{
    public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
    {
        // State logic here
    }
}

// State with Parameters
public class FooStateWithPayload : StateBase<FooPayload>
{
    public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
    {
        // Get payload
        FooPayload payload = Payload; 
        
        // State logic with payload here
    }
}

//Custom State Implementation
public class CustomFooState : IState<MyParams>
{
    public async UniTask Initialize(CancellationToken token) 
    {
        // Initialization logic
    }

    public async UniTask<StateTransitionInfo> Execute(MyParams payload, CancellationToken token) 
    {
        // Execution logic with payload
    }

    public async UniTask Exit(CancellationToken token)
    {
        // Exit logic
    }

    public void Dispose()
    {
        // Cleanup logic
    }
}

```

#### State Lifecycle

The lifecycle of a state consists of four stages, represented by the following methods:

1. **Initialize**
    - Used for initializing resources, such as loading prefabs, subscribing to events, etc.

2. **Execute**
    - The only method that must be overridden in `StateBase`. It contains the main logic of the state and remains active
      until it returns a result with a transition to another state. For example, a state displaying a popup might wait
      for button presses and handle the result here. See the [State Transitions](#state-transitions) section for more
      details.

3. **Exit**
    - Completes the state's work, such as unsubscribing from buttons and closing the popup (e.g., playing a closing
      animation).

4. **Dispose**
    - Cleans up resources. If you inherit from `StateBase`, this method does not need implementation.
    - **Note:** If you inherit state from StateBase, do not override the Dispose method. Use [Disposables](#disposables)
      instead.
    - If you manually implement `IState` or override `Dispose`, make it safe to call more than once. Some DI containers
      may dispose resolved state instances again when their scope is disposed.

#### State Transitions

The `Execute` method of a state should return a `StateTransitionInfo` object, which dictates the next actions of the
state machine. To simplify its generation, you can use the `Transition` property in `StateBase`. The possible transition
options are:

1. **GoTo**
    - Used to transition to another state. If the state contains a payload, it should be passed to `GoTo`.

2. **GoBack**
    - Returns to the previous state. If there is no previous state (the current state is the first), it will exit the
      state machine. See the [State Machine](#state-machine) section for more details.

3. **GoBackTo**
   - Returns to specified previous state, dropping all intermediate states from the [State Machine's History](#state-machine-history).
   - If specified state isn't found in the history, it will exit the state machine.
   - If multiple states with specified type are present in the history, the latest state will be selected.

4. **GoToExit**
    - Exits the current state machine. See the [State Machine](#state-machine) section for more details.

```csharp
public class ExampleState : StateBase
{
    public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
    {
        var transition = await DoSomeAsyncLogic(token);

        switch (transition)
        {
            case TransitionExample.GoTo:
                return Transition.GoTo<FooState>();

            case TransitionExample.GoToWithPayload:
                var payload = 42;
                return Transition.GoTo<BarState, int>(payload);

            case TransitionExample.GoToAbstract:
                return Transition.GoTo<IFooState>();

            case TransitionExample.GoBack:
                return Transition.GoBack();

            case TransitionExample.GoBackTo:
                return Transition.GoBackTo<BarState>();

            case TransitionExample.GoToExit:
                return Transition.GoToExit();

            default:
                return Transition.GoToExit();
        }
    }

    private UniTask<TransitionExample> DoSomeAsyncLogic(CancellationToken token)
    {
        // Some logic here
        return UniTask.FromResult(TransitionExample.GoTo);
    }
}
```

#### Disposables

Disposables are a part of `StateBase` that allow users to tie `IDisposable` references and delegates to state's
lifetime, guaranteeing disposal and delegate execution on state's `Dispose`, without overriding the method.
This is the recommended way to clean up resources because `StateBase` keeps disposal safe if the same state instance is
disposed more than once by UniState and a DI container.

Disposal is exception-safe: if one of the registered disposables throws, the remaining ones are still disposed (in
reverse registration order), and the failure is reported to the state machine's `HandleError()` as
`StateMachineErrorType.StateDisposing` (multiple failures are combined into an `AggregateException`).

```csharp
// Available API
Disposables.Add(fooDisposable);
Disposables.Add(() => Unsubscribe());
Disposables.Add(fooDisposable, barDisposable);
Disposables.ThenAdd(fooDisposable).ThenAdd(barDisposable).ThenAdd(() => Unsubscribe());

// Example Usage
public class LoadingState : StateBase<ILoadingScreenView>
{
    private CancellationTokenSource _loadingCts;

    public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
    {
        // State's disposable references
        _loadingCts = CancellationTokenSource.CreateLinkedTokenSource(token);
        Disposables.Add(_loadingCts);

        // Handling of subscriptions with locality of behaviour
        Payload.CancelClicked += OnCancelLoadingClicked;        
        Disposables.Add(() => Payload.CancelClicked -= OnCancelLoadingClicked);

        try
        {
            await Payload.PretendToWork(_loadingCts.Token);
        }
        catch (OperationCanceledException) when (!token.IsCancellationRequested)
        {
            return Transition.GoBack();
        }

        return Transition.GoTo<NextState>();
    }
    
    private void OnCancelLoadingClicked()
    {
        _loadingCts.Cancel();
    }
}
```

#### State Behavior Attribute

It is possible to customize the behavior of a specific state using the `StateBehaviour` attribute.

This attribute has the following parameters:

- **ProhibitReturnToState** (default value: false): When enabled, this state cannot be returned to
  via `Transition.GoBack()`. The state with this attribute will be skipped, and control will return to the state before
  it. This behavior can be useful for states that represent 'loading', there is no point of returning to loading.

- **InitializeOnStateTransition** (default value: false): When enabled, the initialization of the state will begin
  before exiting the previous state. Technically, this means `Initialize()` of the state will be called before `Exit()`
  of the previous state. This behavior can be useful for seamless transitions in complex animations, where the state
  represents only part of the animation.

```csharp
[StateBehaviour(ProhibitReturnToState = true)]
public class FooState: StateBase
{
    //...
}

[StateBehaviour(InitializeOnStateTransition = true)]
public class BarState: StateBase
{
    //...
}

[StateBehaviour(InitializeOnStateTransition = true, ProhibitReturnToState = true)]
public class BazState: StateBase
{
    //...
}
```

### State Machine

The state machine is the entry point into the framework, responsible for running states.

#### Creating a State Machine

You can work with the built-in `StateMachine` class or supply a custom implementation by either deriving from
`StateMachine` or implementing `IStateMachine`.
Custom interfaces that extend `IStateMachine` are fully supported and can be registered side-by-side.

```csharp
    public class StateMachineWithoutHistory : StateMachine
    {
        protected override int MaxHistorySize => 0;
    }
    
    public interface IBarMachine : IStateMachine
    {
        public void Bar();
    }
    
    public class BarMachine : StateMachine, IBarMachine
    {
       public void Bar()
       {
            Debug.Log("Bar");
       }
    }
```

#### Running a State Machine

To use a state machine, resolve it through its interface and invoke `Execute<TInitialState>(cancellationToken)` with the
desired entry state.

```csharp
await stateMachine.Execute<FooState>(cts.Token);

var payload = new BarPayload();
await stateMachine.Execute<BarState>(payload, cts.Token);
```

A state machine supports only one active execution flow.  
Calling `Execute()` again while the current run has not finished raises **`AlreadyExecutingException`** to prevent
concurrent execution.

You can determine whether the machine is already running by checking property **`IsExecuting`**.


#### Launching Nested State Machines

Any state can launch any number of nested state machines.  
Simply inject the machines through the state’s constructor, no additional action required.

```csharp
public class RootGameplayState : StateBase
{
    private readonly IStateMachine _uiMachine;
    private readonly IStateMachine _logicMachine;

    public RootGameplayState(IStateMachine uiMachine,
                             IStateMachine logicMachine)
    {
        _uiMachine = uiMachine;
        _logicMachine = logicMachine;
    }

    public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
    {
        // Run UI-related flow in parallel
        _uiMachine.Execute<UiRootState>(token).Forget();

        // Run logic and await completion
        await _logicMachine.Execute<LogicRootState>(token);

        return Transition.GoBack();
    }
}
```


#### State Machine History

The state machine maintains a history of transitions between states, allowing for the use of `Transition.GoBack()`. The
size of this history can be customized by overriding the `StateMachine.MaxHistorySize` property (default value is
15). If more transitions occur than the history size, only the most recent transitions will be retained, with no
overhead or errors resulting from the limit.

Setting `MaxHistorySize = 0` disables the history, causing `Transition.GoBack()` to exit the state machine directly.

```csharp
public class StateMachineWithDisabledHistory : StateMachine
{
    protected override int MaxHistorySize => 0;
}
```

#### State Machine State Change Hook

Override `HandleStateChanged()` when a custom state machine needs to observe high-level state machine movement, for
example to report analytics, update debug UI, or build a lightweight execution trace.

```csharp
public class ObservedStateMachine : StateMachine
{
    protected override void HandleStateChanged(StateMachineStateChangedData changeData)
    {
        Debug.Log(
            $"{changeData.ChangeType}: " +
            $"{changeData.PreviousStateType?.Name ?? "None"} -> " +
            $"{changeData.CurrentStateType?.Name ?? "None"}");
    }
}
```

The hook receives `StateMachineStateChangedData` with the following data:

* `ChangeType` - `Started`, `Changed`, `Exited`, or `Canceled`.
* `PreviousStateType` and `CurrentStateType` - state types before and after the change. One side is `null` when the state
  machine starts or exits.
* `PreviousTransition` and `CurrentTransition` - transition metadata for the previous and current state.
* `RequestedTransition` - the transition returned by the state that caused this change.

`Started` is reported after the initial state's `Initialize()` completes and before its `Execute()` starts. `Changed` is
reported after the previous state has exited and disposed and after the current state has initialized. `Exited` is
reported after the last state has exited and disposed. `Canceled` is reported as the terminal event when execution is
canceled through the cancellation token passed to `Execute()`; in this case `CurrentStateType` is `null`,
`PreviousStateType` is the state that was active when cancellation was observed, and `RequestedTransition` is the last
transition returned by a state, or `null` if cancellation happened before the first `Execute()` completed (`Canceled`
may even be the only reported event if cancellation happens before `Started`).

`StateMachineStateChangedData` is a readonly value type and contains state types rather than state instances. This keeps
the hook lightweight and prevents observers from accidentally keeping disposed states alive. If the hook throws an
exception, the state machine reports it through `HandleError()` as `StateMachineErrorType.StateMachineFail` and still
performs cleanup.

#### State Machine Error Handling

##### General Error-Handling Principles

In UniState, state machine error handling can be customized to control how exceptions within states are processed. The
primary mechanism for this is the `HandleError()` method, which you can override in your custom state machine. This
method is called whenever an exception occurs, allowing you to define specific logic to handle errors.

By default, `HandleError()` writes every captured exception to the Unity Console via `UnityEngine.Debug.LogError`.
Override this method if you need custom logic for exceptions.

```csharp
public class BarStateMachine : StateMachine
{
    protected override void HandleError(StateMachineErrorData errorData)
    {
        // Custom logic here
    }
}
```

Exceptions are processed internally without propagating further. By default this means logging to the Unity Console
through `Debug.LogError`. `StateMachineErrorData` provides metadata related to exceptions, and
`StateMachineErrorData.State` may be `null` if `StateMachineErrorType` is set to `StateMachineFail`.
If a state or substate throws during `Dispose()`, the error is reported as `StateMachineErrorType.StateDisposing`.
Multiple substate dispose failures are reported as an `AggregateException`.

The only exception that is not routed to `HandleError()` is a genuine cancellation: an `OperationCanceledException`
observed while the token passed to `Execute()` is canceled. See
[Cancellation Behavior](#cancellation-behavior) for details. An `OperationCanceledException` raised while the machine's
token is *not* canceled (for example, one that leaked from a state's internal linked token source or a third-party
timeout) is treated as a regular error: it goes through `HandleError()` and the standard recovery flow instead of
silently stopping the machine.

To halt state machine execution after an exception, include a `throw` statement in `HandleError()`. The machine stops,
all created states are disposed, and the thrown exception propagates to the `await stateMachine.Execute()` caller, so
an aborted run is distinguishable from a normal exit. `HandleError()` is invoked once per error; an exception thrown
from `HandleError()` itself is not reported back into `HandleError()` again.
In the example provided, the state machine will terminate after encountering a second exception within the same state in a row.

```csharp
public class FooStateMachine : StateMachine
{
    private Type _lastErrorState;

    protected override void HandleError(StateMachineErrorData errorData)
    {
        var stateType = errorData.State?.GetType();

        if (stateType != null && _lastErrorState == stateType)
        {
            // Stop state machine execution and throw an exception out
            throw new Exception($"Second exception in same state.", errorData.Exception);
        }

        _lastErrorState = stateType;
    }
}
```

If an exception is encountered in a state’s `Initialize()` or `Exit()` methods, the state machine will continue working.
However, if an exception occurs in the state’s `Execute()` method, the state machine defaults to a
`GoBack()` operation, as though `Transition.GoBack()` were returned. You can override this behavior by customizing
`BuildRecoveryTransition`, which receives an `IStateTransitionFactory` to specify any desired transition for error
recovery.

When an exception occurs in `Execute()`, `HandleError` will be invoked first, followed by `BuildRecoveryTransition`.

```csharp
public class BarStateMachine : StateMachine
{
       // If exception occurs in the state in the Execute() method, the state machine will go to the ErrorPopupState.
       protected override StateTransitionInfo BuildRecoveryTransition(IStateTransitionFactory transitionFactory)
            => transitionFactory.CreateStateTransition<ErrorPopupState>();
}
```

##### Cancellation Behavior

Canceling the token passed to `Execute()` aborts the run:

* The active state's `Execute()` receives the cancellation through its token as usual.
* `Exit()` of the active state is **not** called - cancellation is an abortive shutdown, only `Dispose()` is guaranteed
  for every created state.
* The `HandleStateChanged()` hook receives a terminal `Canceled` event.
* The `OperationCanceledException` propagates out of `Execute()` to the caller; it is not passed to `HandleError()`.

Keep in mind that if the machine was started with `Execute(...).Forget()`, UniTask silently swallows
`OperationCanceledException` by default, so nothing is logged when the flow is canceled - this is regular UniTask
behavior for canceled forgotten tasks.

##### State Machine Specific Exceptions

During the lifetime of UniState state machine may raise state-machine-specific exceptions:

* **`AlreadyExecutingException`** - derived from `InvalidOperationException`. Thrown when `Execute()` is called while the
  state machine is already executing, preventing a second concurrent run and indicating an incorrect lifecycle invocation.

* **`NoSubStatesException`** - derived from `InvalidOperationException`. Thrown by `DefaultCompositeState` if its
  `Execute()` method starts without any SubStates being present.

#### Built-in Support for DI Scopes

UniState natively supports sub-containers and sub-contexts available in modern DI frameworks.

A state machine uses the **container scope in which it was registered**:

* Registered in the root container → its context is the root.
* Registered in a child container → its context is that child.

All states created by the machine - and every dependency those states request - are resolved through this context.

To switch the context at runtime call **`SetResolver(ITypeResolver)`** with a resolver obtained from any container or sub-container:
```csharp
IObjectResolver container;
var newResolver = container.ToTypeResolver();

stateMachine.SetResolver(newResolver);
```

`SetResolver()` can be called at any time, including while the machine is executing: states created after the call are
resolved through the new context, while states that already exist keep the context they were created with. If a state
machine is created manually (without a DI integration), `SetResolver()` must be called before `Execute()` - otherwise
`Execute()` throws an `InvalidOperationException`.

#### Custom Type Resolvers

While UniState provides `ITypeResolver` implementations for modern DI frameworks out of the box, you can create custom implementations, tailored to your needs

An example of `ITypeResolver` with automatic state bindings for Zenject/Extenject:
```csharp
public class ZenjectAutoBindTypeResolver : ITypeResolver
{
    ...

    public object Resolve(Type type)
    {
        if (!type.IsAbstract && !type.IsInterface && !_container.HasBinding(type))
        {
            _container.BindState(type);
        }

        return _container.Resolve(type);
    }
}
```

If you do not have DI framework you have to implement ITypeResolver by your own by manually creating requested states and
state machines (see [Working Without a DI Framework](#working-without-a-di-framework)).

#### Working Without a DI Framework

UniState is engineered to integrate seamlessly with modern DI containers.  
However, if your project does not use a DI framework you can still adopt UniState by **supplying a manual implementation of `ITypeResolver`**.

An example of `ITypeResolver` without DI framework and state machine running:
```csharp
    public class CustomResolver : ITypeResolver
    {
        public object Resolve(Type type)
        {
            if (typeof(BarState) == type)
            {
                return new BarState();
            }

            if (typeof(FooState) == type)
            {
                return new FooState();
            }

            if (typeof(StateMachine) == type)
            {
                return new StateMachine();
            }

            throw new NotImplementedException();
        }
    }

    public class EntryPoint : MonoBehaviour
    {
        public async UniTask Run()
        {
            var resolver = new CustomResolver();
            var stateMachine = resolver.Resolve<StateMachine>();

            stateMachine.SetResolver(resolver);

            await stateMachine.Execute<FooState>(CancellationToken.None);
        }
    }
}

```

### Composite State

Composite State is essential for complex areas of an application likely to be worked on by multiple people
simultaneously. They consist of various independent sub states, each with its own logic.

#### Creating a Composite State

To create a composite state, inherit from `CompositeStateBase` (or implement the `ICompositeState` interface for more
detailed control). You can also use the ready-made implementation `DefaultCompositeState` (see
the [DefaultCompositeState](#defaultcompositestate) section). No additional actions are needed.

#### SubState

SubStates are states tied to a composite state, created and run simultaneously with it. To create a SubState, inherit
from `SubStateBase` or implement the `ISubState` interface for greater customization. When creating a sub state, specify
the parent composite state as a generic parameter, e.g., `FooSubState : SubStateBase<BarCompositeState>`. In all other
aspects, it functions like a regular state.

Sub states are linked to their composite state by the type used in the transition. `Transition.GoTo<TState>()` resolves
`ISubState<TState, TPayload>` from the container, so the generic parameter of `SubStateBase<T>` must match the type used
in `GoTo`. If you open a composite state through an interface (e.g. `Transition.GoTo<IFooState>()`), declare its sub
states with the same interface (`SubStateBase<IFooState>`); sub states declared with the concrete type
(`SubStateBase<FooCompositeState>`) will not be found in that case, and `DefaultCompositeState` will throw
`NoSubStatesException`.

#### Default Composite State

A ready-to-use implementation for a composite state that propagates `Initialize`, `Execute`, and `Exit` methods to all
SubStates within it. The result of the `Execute` method will be the transition returned by the first sub state to
complete its `Execute` method.

Once the first sub state completes, the remaining sub states are canceled through the token passed to their `Execute()`
and the composite state waits for all of them to finish before returning. This guarantees that `Exit()`/`Dispose()` of a
sub state never runs in parallel with its still-executing `Execute()`, even for sub states that do not react to
cancellation promptly.

Errors never disappear silently: if any sub state fails in `Initialize`, `Execute`, or `Exit`, the composite state still
waits for all remaining sub states to finish that phase, then rethrows the failure (multiple failures are combined into
an `AggregateException`) so it reaches the state machine's `HandleError()`. If one sub state completes `Execute`
successfully while another one fails, the error takes priority: the returned transition is discarded and the exception
is delivered to `HandleError()`, followed by the standard recovery flow.

If you use `DefaultCompositeState` and it is executed without any SubStates, its `Execute` method will throw
a `NoSubStatesException`.

To use `DefaultCompositeState`, simply inherit your composite state from it. Here's an example:
```csharp
internal class FooCompositeState : DefaultCompositeState
{
}

internal class BazSubState : SubStateBase<DefaultCompositeState>
{
}

internal class BarSubState : SubStateBase<DefaultCompositeState>
{
}
```

## Tutorials

### Simple Dice Game

#### Overview

In this hands‑on tutorial you will create a tiny, self‑playing **dice game** that demonstrates the simple UniState
workflow - from defining states to wiring everything together with **VContainer**.

> **Goal**  
> Roll a six‑sided die until the value is 5 or 6.\
>`StartGameState` → `RollDiceState`\
>5,6 → `WinState` → Exit\
>1,2,3,4 → `LostState` → `RollDiceState`

You can find code [here](https://github.com/bazyleu/UniState/tree/main/Assets/Examples).

#### Step 1: Create the states

Each state inherits from **`StateBase`** and returns a transition that drives the flow.

```csharp
    internal class StartGameState : StateBase
    {
        public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            Debug.Log("Welcome to the game! Your game will be loaded in 2 seconds!");
            await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: token);
            
            return Transition.GoTo<RollDiceState>();
        }
    }
```
```csharp
    public class RollDiceState : StateBase
    {
        public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            Debug.Log("Need to roll 5+. Rolling the dice...");
            await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: token);

            var dice = Random.Range(1, 7);
            Debug.Log($"Dice is {dice}");

            if (dice > 4)
                return Transition.GoTo<WinState>();

            return Transition.GoTo<LostState>();
        }
    }
```
```csharp
    public class LostState : StateBase
    {
        public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            Debug.Log("You lost. You will have another chance in...");

            Debug.Log("3 seconds");
            await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token);

            Debug.Log("2 seconds");
            await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token);

            Debug.Log("1 second");
            await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token);

            return Transition.GoBack();
        }
    }
```
```csharp
    public class WinState : StateBase
    {
        public override UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            Debug.Log("Congratulations! You won this game!");
            
            return UniTask.FromResult(Transition.GoToExit());
        }
    }
```

#### Step 2: Create entry point

DiceEntryPoint runs on scene start, resolves the state machine, and runs StartGameState.

```csharp
    public class DiceEntryPoint : IStartable
    {
        private readonly IStateMachine _stateMachine;

        public DiceEntryPoint(IStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void Start()
        {
            _stateMachine.Execute<StartGameState>(CancellationToken.None).Forget();
        }
    }
```

#### Step 3: Configure VContainer

DiceScope is a LifetimeScope that registers the state machine and all states.
The helper extensions RegisterStateMachine and RegisterState are used for registering.
Note that for a state machine you must register an interface (or abstract class) and an implementation, and resolve the
interface, not the implementation.

```csharp
    public class DiceScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<DiceEntryPoint>();

            builder.RegisterStateMachine<IStateMachine, StateMachine>();

            builder.RegisterState<StartGameState>();
            builder.RegisterState<RollDiceState>();
            builder.RegisterState<LostState>();
            builder.RegisterState<WinState>();
        }
    }
```

#### Step 4: Set up the scene

Create a new Unity scene (e.g., DiceGameScene).
Add an empty GameObject and attach the DiceScope component.
Press Play - all interaction happens in the Console:

```csharp
Welcome to the game! Your game will be loaded in 2 seconds!
Need to roll 5+. Rolling the dice...
Dice is 2
You lost. You will have another chance in...
3 seconds
2 seconds
1 second
Need to roll 5+. Rolling the dice...
Dice is 6
Congratulations! You won this game!
```

## Upgrade Guide

### Upgrading from Versions < 1.12.0

Version 1.12.0 changes how the state machine treats `OperationCanceledException`.

Previously, *any* `OperationCanceledException` thrown from a state stopped the state machine as if it had been
canceled - even when the exception came from an unrelated token (an internal timeout, a linked token source, a
third-party API). `HandleError()` was not called, so such shutdowns were silent and easy to miss.

Starting with 1.12.0, only the state machine's own token (the one passed to `Execute()`) counts as cancellation:

* **Machine token canceled**: the run stops, the `HandleStateChanged()` hook receives a terminal `Canceled`
  notification, and `OperationCanceledException` propagates to the `Execute()` caller. See
  [Cancellation Behavior](#cancellation-behavior).
* **Any other `OperationCanceledException`** is handled like a regular state error: it is routed to `HandleError()`
  and the machine continues with the recovery transition (`GoBack()` by default, see
  [State Machine Error Handling](#state-machine-error-handling)).

Consider a state that guards a long operation with its own timeout:

```csharp
public class LoadingState : StateBase
{
    public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
    {
        // Internal timeout, linked to the state machine token
        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(token);
        timeoutCts.CancelAfter(TimeSpan.FromSeconds(10));

        await LoadDataAsync(timeoutCts.Token);

        return Transition.GoTo<MenuState>();
    }
}
```

When the timeout fires, `LoadDataAsync` throws `OperationCanceledException` while the state machine token is still
alive:

* **Before 1.12.0**: the state machine silently stopped, exactly as if it had been canceled from outside.
* **1.12.0 and later**: `HandleError()` is called with `StateMachineErrorType.StateExecuting`, then the machine
  recovers via `BuildRecoveryTransition()` and keeps running.

If you relied on throwing `OperationCanceledException` from inside a state to stop the whole machine, cancel the
state machine token instead, or return `Transition.GoToExit()`.

Other behavioral changes in 1.12.0:

* Exceptions thrown from `Disposables` no longer prevent the remaining items from being disposed: everything is
  disposed, then the first error is rethrown (or an `AggregateException` if there were several).
* A failing substate no longer interrupts its siblings: all substates of a composite state complete `Initialize()`,
  `Execute()` and `Exit()`, and every collected error is reported instead of being lost.
* An exception thrown from `HandleError()` stops the machine and reaches the `Execute()` caller exactly once.

### Upgrading from Versions < 1.5.0

The 1.5.0 release removes several helper APIs and unifies state-machine usage. The table below lists each breaking
change and its direct replacement.

| Removed API                                                | Use Instead                                                                       | Notes                                             |
|------------------------------------------------------------|-----------------------------------------------------------------------------------|---------------------------------------------------|
| `StateMachineHelper`                                       | Inject the state machine directly via interface into the state and call `Execute` | Helper no longer required.                        |
| `StateMachineFactory`                                      | Inject the state machine directly via interface into the state and call `Execute` | Helper no longer required.                        |
| `IExecutableStateMachine`                                  | `IStateMachine`                                                                   | Single interface for all operations.              |
| `RegisterAbstractState` / `BindAbstractState` and variants | `RegisterState<TBase, TImpl>` / `BindState<TBase, TImpl>`                         | Same functionality without the *Abstract* prefix. |

1. **Register and inject state machines by the `IStateMachine` (or your own) interface.**
2. Replace factory/utility calls (`StateMachineHelper`, `StateMachineFactory`) with state machine interface injection.
3. Update container bindings to the two-parameter `RegisterState` / `BindState` overloads.
4. Remove references to `IExecutableStateMachine`, use `IStateMachine` everywhere.

## Integrations

UniState supports integrations with the most popular DI containers. If these frameworks are installed via UPM,
everything will work out of the box, and no additional actions are required.

### VContainer

GitHub: [VContainer](https://github.com/hadashiA/VContainer)

#### VContainer Preparation

If the VContainer is installed via UPM, you can skip this step and proceed to the [VContainer Usage](#vcontainer-usage)
section.
If the package is not installed via UPM, you need to manually add the `UNISTATE_VCONTAINER_SUPPORT` define symbol in
Scripting Define Symbols (Player Settings -> Player -> Scripting Define Symbols).

#### VContainer Usage

No extra setup is required - simply resolve the state machine from the DI container and invoke its Execute method.

```csharp
    public class GameEntryPoint : IStartable
    {
        private readonly IStateMachine _stateMachine;

        public GameEntryPoint(IStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void Start()
        {
            _stateMachine.Execute<StartGameState>(CancellationToken.None).Forget();
        }
    }
```

#### VContainer Registering

All state machines, states and their dependencies should be registered in DI container.
For convenient registering of states and state machines, special extension methods are available.

Here's an example code:
```csharp
private void RegisterStates(IContainerBuilder builder)
{
      // Use these registering in general use
    
      builder.RegisterStateMachine<IStateMachine, BarStateMachine>();
      builder.RegisterState<BarState>();
      builder.RegisterState<IBarState, BarState>();
    
      // Singleton version of registering, not recommended in general use
      
      builder.RegisterStateMachine<IStateMachine, BarStateMachine>(Lifetime.Singleton);
      builder.RegisterState<BarState>(Lifetime.Singleton);
      builder.RegisterState<IBarState, BarState>(Lifetime.Singleton);
}
```
You can always skip the extensions and register directly if you need custom behavior.

### Zenject (Extenject)

GitHub: [Extenject](https://github.com/Mathijs-Bakker/Extenject) or [Zenject](https://github.com/modesttree/Zenject)

#### Zenject Preparation

If the Zenject / Extenject is installed via UPM, you can skip this step and proceed to
the [Zenject Usage](#zenject-usage) section.
If the package is not installed via UPM, you need to manually add the `UNISTATE_ZENJECT_SUPPORT` define symbol in
Scripting Define Symbols (Player Settings -> Player -> Scripting Define Symbols).

#### Zenject Usage

No extra setup is required - simply resolve the state machine from the DI container and invoke its Execute method.

```csharp
    public class GameEntryPoint : IStartable
    {
        private readonly IStateMachine _stateMachine;

        public GameEntryPoint(IStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void Start()
        {
            _stateMachine.Execute<StartGameState>(CancellationToken.None).Forget();
        }
    }
```

#### Zenject Registering

All state machines, states and their dependencies should be registered in DI container.
For convenient registering of states and state machines, special extension methods are available.

Here's an example code:
```csharp
private void BindStates(DiContainer container)
{
     // Use these bindings in general use
     
    container.BindStateMachine<IStateMachine, BarStateMachine>();
    container.BindState<BarState>();
    container.BindState<IBarState, BarState>();
    
    // Singleton version of bindings, not recommended in general use
    
    container.BindStateMachineAsSingle<IStateMachine, BarStateMachine>();
    container.BindStateAsSingle<BarState>();
    container.BindStateAsSingle<IBarState, BarState>();
}
```

### Reflex

GitHub: [Reflex](https://github.com/gustavopsantos/Reflex)

#### Reflex Preparation

If Reflex is installed via UPM, you can skip this step and proceed directly to the [Reflex Usage](#reflex-usage)
section.

If Reflex is not installed via UPM, manually add the `UNISTATE_REFLEX_SUPPORT` define symbol to your Scripting Define
Symbols (Player Settings -> Player -> Scripting Define Symbols).

#### Reflex Usage

No additional setup is required after registering. Simply resolve the state machine from the Reflex DI container and
invoke its `Execute` method.

Here's an example:

```csharp
using System.Threading;
using Cysharp.Threading.Tasks;
using Examples.States;
using UniState;

namespace Examples.Infrastructure.Reflex
{
    public class DiceEntryPoint
    {
        private readonly IStateMachine _stateMachine;

        public DiceEntryPoint(IStateMachine stateMachine) =>  _stateMachine = stateMachine;

        public void Start()
        {
            _stateMachine.Execute<StartGameState>(CancellationToken.None).Forget();
        }
    }
}
```

#### Reflex Registering

All state machines, states, and their dependencies should be registered in the DI container using Reflex's
`ContainerBuilder`. Special extension methods have been provided for convenient registration.

`RegisterState(typeof(MyState))` registers the state as itself and as all implemented interfaces.
Use the overload with `Lifetime.Singleton` when you need a singleton lifetime.
Use the two-parameter overload when you need to expose an additional abstract/base contract explicitly.

Here's example code demonstrating the available extension methods:

```csharp
using Reflex.Core;
using Reflex.Enums;
using UniState;

private void RegisterStates(ContainerBuilder builder)
{
    // Recommended usage for general cases

    builder.RegisterStateMachine(typeof(StateMachine), typeof(IStateMachine));
    builder.RegisterState(typeof(BarState));
    builder.RegisterState(typeof(BarState), typeof(BarBaseState));
    
    // Singleton version (use cautiously, not recommended in most cases)

    builder.RegisterStateMachine(typeof(StateMachine), typeof(IStateMachine), Lifetime.Singleton);
    builder.RegisterState(typeof(BarState), Lifetime.Singleton);
    builder.RegisterState(typeof(BarState), typeof(BarBaseState), Lifetime.Singleton);
}
```

## License

This library is under the MIT License. Full text is [here](LICENSE).