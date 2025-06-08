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
  [NSubstitutes](https://nsubstitute.github.io/) or any other framework. States can be run separately for testing
  purposes.
* **DI friendly**: has [integration](#integrations) with most popular DI containers
* **Continuous Testing**: fully covered by tests. All tests run [automatically](https://github.com/bazyleu/UniState/actions) to verify each change.

## Table of Contents

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
            _stateMachine.Execute<StartGameState>(CancellationToken.None).Forget();
        }
    }
```
More details on running the state machine can be found [here](#running-a-state-machine).

That is it! Your first project with UniState is set up. In [tutorials](#tutorials) section more detailed tutorial can be
found.

## Installation

### Requirements

* Requires Unity version 2022.3 or higher.
* Requires UniTask package installed. Guide regarding UniTask installation can be found
  on [Cysharp/UniTask README](https://github.com/Cysharp/UniTask/blob/master/README.md#upm-package).

### Option 1: Add package from git URL

You can add `https://github.com/bazyleu/UniState.git?path=Assets/UniState` to Package Manager.

It is a good practice to specify target version, UniState uses the `*.*.*` release tag so you can specify a version
like `#1.3.0`. For example `https://github.com/bazyleu/UniState.git?path=Assets/UniState#1.3.0`.
You can find latest version number [here](https://github.com/bazyleu/UniState/releases).

![image](https://github.com/user-attachments/assets/120e6750-1f33-44f7-99c8-a3e7fa166d21)
![image](https://github.com/user-attachments/assets/3fed7201-b748-4261-b4f8-7bdffdac072d)

### Option 2: Add via manifest.json

You can add `"com.bazyleu.unistate": "https://github.com/bazyleu/UniState.git?path=Assets/UniState"` (or with version
tag `https://github.com/bazyleu/UniState.git?path=Assets/UniState#1.3.0`) to `Packages/manifest.json`.

## Performance

UniState is the fastest and most efficient asynchronous state machine available for Unity. When compared to state
machine implementations based on MonoBehaviour, UniState delivers a performance boost of over 5000x in execution speed
and up to a 10x reduction in allocations.

For typical scenarios involving small to medium state chains - the most common use case - UniState can reduce memory
allocations by a factor ranging between 2x and 10x. In cases where state chains exceed 200 states, the benefits in
memory allocation become less pronounced but execution speed remain consistent with 5000x+ boost.

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
creating [state](#state), [state machine](#state-machine).

### What is a State?

A state is an abstraction that represents a specific condition or phase of the game, often corresponding to a "screen" that the user interacts
with. For example, the main menu is a state, a settings popup is another state, and gameplay itself may take place in a
separate `GameplayState`. When the user opens a shop popup, they may transition into a `ShopState`. However, states are
not always tied to visual elements. Some states, like `GameLoadingState`, may handle background processes such as
loading resources.

State class contains all logic related to that state including loading and unloading resources. UniState does not restrict the use of other
frameworks or patterns, meaning you can freely use whatever suits your needs. You could, for example, run controllers
and follow an MVC approach, follow MVVM approach, or even execute ECS code within a state.

The key concept of the framework is that once a state is exited, all resources it allocated should be released. For
details on how to do this see [Disposables](#disposables).

It is not recommended to use Unity GameObjects directly inside states, as it reduces testability and increases code
coupling. A better approach is to load GameObjects through an abstraction and use them as an interface (essentially as a
View in UniState). Add a handler for unloading to the Disposables of the state that loaded it. All approaches / patterns
which were mentioned above support this, and you can choose any based on your preferences, as this functionality is
outside the scope of UniState.

```csharp
    //Popup prefab (Monobehaviour, view)
    public class SimplePopupView : ISimplePopupView, Monobehaviour
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
            Disposables.Add(UnloadShopView);
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
        private ISimplePopupView LoadShopView(CancellationToken token)
        {
             // Loading logic
        }
        
        private void UnloadShopView()
        {
             // Unloading logic
        }
    }
```

If the popup is complex with multiple features, it could be represented as its own state machine. 
In cases where you have a complex popup with its own state machine, it’s important to allocate resources specific to the popup before launching the separate
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
            await stateMachine.Execute<ShopPopupIdleState>(cts.Token);

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
lifetime, guaranteeing disposal and delegate execution on state's `Dispose`, without overriding the method

```csharp
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
        catch (OperationCancelledException) when (!token.IsCancellationRequested)
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

    public GameplayState(IStateMachine uiMachine,
                         IStateMachine logicMachine)
    {
        _uiMachine = uiMachine;
        _logicMachine = _logicMachine;
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
size of this history can be customized through the `StateMachineLongHistory.MaxHistorySize` property (default value is
15). If more transitions occur than the history size, only the most recent transitions will be retained, with no
overhead or errors resulting from the limit.

Setting `MaxHistorySize = 0` disables the history, causing `Transition.GoBack()` to exit the state machine directly.

```csharp
public class StateMachineWithDisabledHistory : StateMachine
{
    protected override int MaxHistorySize => 0;
}
```

#### State Machine Error Handling

##### General Error-Handling Principles

In UniState, state machine error handling can be customized to control how exceptions within states are processed. The
primary mechanism for this is the `HandleError()` method, which you can override in your custom state machine. This
method is called whenever an exception occurs, allowing you to define specific logic to handle errors.

Exceptions are caught and processed internally without propagating further, except for `OperationCanceledException`,
which will stop the state machine. `StateMachineErrorData` provides metadata related to exceptions, and
`StateMachineErrorData.State` may be `null` if `StateMachineErrorType` is set to `StateMachineFail`.

```csharp
public class BarStateMachine : StateMachine
{
    protected override void HandleError(StateMachineErrorData errorData)
    {
        // Custom logic here
    }
}
```

To halt state machine execution after an exception, include a `throw` statement in `HandleError()`:
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
            // Stop state mahine execution and throw an exception out
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

##### State Machine Specific Exceptions

During the lifetime of UniState state machine may raise state-machine-specific exceptions:

* **`AlreadyExecutingException`** — derived from `InvalidOperationException`. Thrown when `Execute()` is called while the
  state machine is already executing, preventing a second concurrent run and indicating an incorrect lifecycle invocation.

* **`NoSubStatesException`** — derived from `InvalidOperationException`. Thrown by `DefaultCompositeState` if its
  `Execute()` method starts without any SubStates being present.

#### Built-in Support for DI Scopes

UniState natively supports sub-containers and sub-contexts available in modern DI frameworks.

A state machine uses the **container scope in which it was registered**:

* Registered in the root container → its context is the root.
* Registered in a child container → its context is that child.

All states created by the machine—and every dependency those states request—are resolved through this context.

To switch the context at runtime call **`SetResolver(ITypeResolver)`** with a resolver obtained from any container or sub-container:
```csharp
IObjectResolver container;
var newResolver = container.ToTypeResolver();

stateMachine.SetResolver(newResolver);
```

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
state machines (see [Working Without a DI Framework](#working-without-aa-di-framework).

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
                return FooState();
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

#### Default Composite State

A ready-to-use implementation for a composite state that propagates `Initialize`, `Execute`, and `Exit` methods to all
SubStates within it. The result of the `Execute` method will be the first completed `Execute` method among all sub
states.

If you use `DefaultCompositeState` and it is executed without any SubStates, its `Execute` method will throw
an `InvalidOperationException`.

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
workflow — from defining states to wiring everything together with **VContainer**.

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

            var dice = Random.Range(0, 7);
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
            Debug.Log("You lost. You will have a another chance in...");

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

DiceEntryPoint runs on scene start, converts IObjectResolver into an ITypeResolver, creates the state machine, and runs
StartGameState.

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
The helper extensions RegisterStateMachine and RegisterState is used for registering.
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
Press Play — all interaction happens in the Console:

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

### Upgrading from Versions < 1.5.0

The 1.5.0 release removes several helper APIs and unifies state-machine usage. The table below lists each breaking change and its direct replacement.

| Removed API | Use Instead                                                    | Notes |
|-------------|----------------------------------------------------------------|-------|
| `StateMachineHelper` | Resolve the state machine via DI and call `Execute`            | Helper no longer required. |
| `StateMachineFactory` | Inject the state machine directly via interface into the state | Helper no longer required. |
| `IExecutableStateMachine` | `IStateMachine`                                                | Single interface for all operations. |
| `RegisterAbstractState` / `BindAbstractState` and variants | `RegisterState<TBase, TImpl>` / `BindState<TBase, TImpl>`      | Same functionality without the *Abstract* prefix. |

1. **Register and inject state machines by the `IStateMachine` (or your own) interface.**
2. Replace factory/utility calls** (`StateMachineHelper`, `StateMachineFactory`) with direct DI resolution.
3. Update container bindings** to the two-parameter `RegisterState` / `BindState` overloads.
4. Remove references to `IExecutableStateMachine`;** use `IStateMachine` everywhere.

After applying these steps the project will compile and run on UniState ≥ 1.5.0.



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

## License

This library is under the MIT License. Full text is [here](LICENSE).