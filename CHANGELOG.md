# Changelog

## [1.12.0]

### Added
- `StateMachineStateChangeType.Canceled` — terminal `HandleStateChanged` notification when the run is canceled through the token passed to `Execute`
- `LimitedStack<T>.MaxSize` and `LimitedStack<T>.Clear()`

### Changed
- **BREAKING**: Only an `OperationCanceledException` observed while the token passed to `Execute` is canceled stops the state machine; any other `OperationCanceledException` (internal timeouts, linked token sources, third-party APIs) is routed to `HandleError` and followed by the recovery transition — see "Upgrading from Versions < 1.12.0" in the README
- **BREAKING**: An exception thrown from an overridden `HandleError` stops the machine and propagates to the `Execute` caller with its original stack trace; it is no longer reported back into `HandleError`
- Composite states wait for every sub-state to finish each phase
  - `Execute` returns the transition of the first sub-state to complete, cancels the remaining ones and awaits them, so a sub-state's `Exit`/`Dispose` never overlaps its own `Execute`
  - A failure in any sub-state's `Initialize`, `Execute` or `Exit` is rethrown once the phase completes (multiple failures as an `AggregateException`) and takes priority over a transition returned by another sub-state
- State machine history is reused between runs of the same instance instead of being reallocated
- `NoSubStatesException` message explains that sub-states are matched by the exact state type used in the transition

### Fixed
- A throwing entry in `StateBase.Disposables` no longer prevents the remaining disposables from being disposed; the failure reaches `HandleError` as `StateMachineErrorType.StateDisposing` (multiple failures as an `AggregateException`)
- `StateBase.Dispose` stays safe to call again after a disposable threw
- `CompositeStateBase.Dispose` disposes sub-states even when the base disposal throws
- `Execute` on a state machine without a resolver throws a descriptive `InvalidOperationException` instead of a `NullReferenceException`
- `[StateBehaviour]` data cache is thread-safe
- Stray `");` characters removed from DI registration error messages


## [1.11.0]

### Added
- `StateMachine.HandleStateChanged(StateMachineStateChangedData)` virtual hook, notified when the machine starts, changes state and exits
  - `StateMachineStateChangedData` carries previous and current state types, previous and current transitions, and the requested transition
  - `StateMachineStateChangeType` enum with `Started`, `Changed` and `Exited`


## [1.10.0]

### Added
- `StateMachineErrorType.StateDisposing` — exceptions thrown from a state's `Dispose` are reported to `HandleError` instead of escaping the state machine
- `EmptyPayload.Instance`

### Changed
- **BREAKING**: Minimum supported Unity version raised to 2022.3 (was 2021.3)
- Reduced per-transition allocations: `[StateBehaviour]` data is cached per state type, payload-less states share `EmptyPayload.Instance`, and composite states no longer use LINQ or copy the sub-state list

### Fixed
- An exception thrown while disposing one sub-state no longer prevents the remaining sub-states from being disposed; failures are rethrown afterwards (multiple failures as an `AggregateException`)
- Both the active and the pending state are disposed during shutdown even if disposing one of them fails


## [1.9.1]

### Added
- Reflex `RegisterStateMachine` and `RegisterState` extensions, with overloads accepting a `Lifetime`

### Changed
- **BREAKING**: Reflex integration targets the Reflex 14.3.0 API
- Reflex `AddStateMachine`, `AddSingletonStateMachine`, `AddState` and `AddSingletonState` marked `[Obsolete]` in favor of the `Register*` extensions

### Fixed
- Compatibility with Reflex 14.3.0


## [1.9.0]

### Changed
- Reflex integration is no longer experimental
- Reflex singleton state machine registrations return a single instance, and state machines implementing `IDisposable` are disposed together with the container
- Reflex `AddState(Type)` and `AddSingletonState(Type)` register the state under all of its implemented interfaces as well as its own type, so sub-states resolve correctly


## [1.8.0]

### Added
- `Disposables` extensions on `List<IDisposable>`: `Add(params IDisposable[])` and chainable `ThenAdd(IDisposable)` / `ThenAdd(Action)`

### Changed
- Default `StateMachine.HandleError` logs the exception through `Debug.LogError` instead of ignoring it


## [1.7.0]

### Changed
- **BREAKING**: Reflex registration API reworked to `Type`-based extensions: `AddStateMachine(Type, Type)`, `AddSingletonStateMachine(Type, Type)`, `AddState(Type)`, `AddState(Type, Type)`, `AddSingletonState(Type)` and `AddSingletonState(Type, Type)`; the generic overloads are gone
- **BREAKING**: VContainer `RegisterState` and Zenject `BindState` / `BindStateAsSingle` require the state type to implement `IExecutableState`


## [1.6.0]

### Added
- Experimental Reflex integration, compiled under `UNISTATE_REFLEX_SUPPORT` (defined automatically when Reflex is installed via UPM)
  - `ContainerBuilder` extensions `AddStateMachine`, `AddSingletonStateMachine`, `AddState` and `AddSingletonState`
  - `Container.ToTypeResolver()`


## [1.5.0]

### Changed
- **BREAKING**: Reworked state machine creation and launching — see "Upgrading from Versions < 1.5.0" in the README
  - `IExecutableStateMachine` and `IInitializableStateMachine` merged into `IStateMachine`
  - State machines are registered with an interface and an implementation — VContainer `RegisterStateMachine<TInterface, TStateMachine>()`, Zenject `BindStateMachine<TInterface, TStateMachine>()` / `BindStateMachineAsSingle<TInterface, TStateMachine>()` — and resolving the interface returns a machine with its resolver already set
  - VContainer `RegisterAbstractState` and Zenject `BindAbstractState` / `BindAbstractStateAsSingle` renamed to `RegisterState<TInterface, TState>` and `BindState<TInterface, TState>` / `BindStateAsSingle<TInterface, TState>`
  - VContainer registration extensions return `void` instead of `RegistrationBuilder`
  - `IStateMachineFactorySetter` renamed to `ISetTransitionFacadeSetter`

### Removed
- **BREAKING**: `IStateMachineFactory`, `StateMachineFactory`, `StateMachineHelper` and `StateBase.StateMachineFactory` — nested state machines are resolved from the DI container
- **BREAKING**: `ITransitionFacadeSetter`, VContainer `RegisterStateMachine<TStateMachine>` / `RegisterAbstractStateMachine`, and Zenject `BindStateMachine<TStateMachine>` / `BindAbstractStateMachine` / `BindAbstractStateMachineAsSingle`


## [1.4.0]

### Added
- `IsExecuting` property on state machines
- `AlreadyExecutingException`, thrown when `Execute` is called on a state machine that is already running
- `NoSubStatesException`, thrown by a composite state without sub-states (derives from `InvalidOperationException`, which was thrown before)

### Changed
- **BREAKING**: `IInitializableStateMachine.Initialize(ITypeResolver)` renamed to `SetResolver(ITypeResolver)`
- State machine history is created at the start of each `Execute` run, so an instance can be executed again with a fresh history


## [1.3.0]

### Added
- `Transition.GoBackTo<TState>()` — returns to the most recent `TState` in the history, discarding newer entries, or exits the state machine when `TState` is not found

### Changed
- **BREAKING**: `IStateCreator.StateType`, `IStateTransitionFacade.GoBackTo<TState>()` and `IStateTransitionFactory.CreateBackToTransition<TState>()` added, breaking custom implementations of these interfaces
- States marked with `ProhibitReturnToState` are no longer stored in the history


## [1.2.0]

### Added
- Lifetime-aware DI registrations
  - VContainer `RegisterStateMachine`, `RegisterAbstractStateMachine`, `RegisterState` and `RegisterAbstractState` overloads accepting a `Lifetime`
  - Zenject `BindStateMachineAsSingle`, `BindAbstractStateMachineAsSingle`, `BindStateAsSingle` and `BindAbstractStateAsSingle`

### Changed
- Fewer allocations when setting up composite state sub-states


## [1.1.0]

### Changed
- Package description repositioned UniState as a state machine package for Unity


## [1.0.0]

### Added
- `StateMachine.MaxHistorySize` virtual property to configure the history size (default 15, 0 disables the history)
- `StateMachine.BuildRecoveryTransition(IStateTransitionFactory)` virtual method choosing the transition after a state's `Execute` fails (default `GoBack`)

### Changed
- Package Manager category changed to `Scripting` (was `Task`)

### Fixed
- Composite states dispose their sub-states


## [0.9.0]

### Added
- DI registration extensions: VContainer `RegisterStateMachine`, `RegisterAbstractStateMachine`, `RegisterState` and `RegisterAbstractState`; Zenject `BindStateMachine`, `BindAbstractStateMachine`, `BindState` and `BindAbstractState`
- `IStateMachineFactory.Create<TStateMachine, TReturn>()` overloads returning a custom state machine interface
- `StateMachineErrorData` carrying the exception, error type and failing state

### Changed
- **BREAKING**: `StateMachine.OnError(Exception, StateMachineErrorType)` replaced by `HandleError(StateMachineErrorData)`
- `StateMachine.Initialize` and both `Execute` overloads are `virtual`
- Executing a composite state without sub-states throws `InvalidOperationException`


## [0.8.0]

### Added
- `StateMachine.OnError(Exception, StateMachineErrorType)` is `protected virtual`, allowing custom error handling

### Fixed
- `GoBack()` returns to the previous state instead of re-entering the current one


## [0.7.2]

### Changed
- Release tags use the `*.*.*` format without the `v` prefix


## [v0.7.1]

### Added
- `List<IDisposable>.Dispose()` extension disposing entries in reverse order


## [v0.7.0]

### Added
- `StateBase.Disposables` — `IDisposable` instances and delegates (via `Add(Action)`) disposed in reverse order together with the state
- `DisposableAction`


## [v0.6.1]

### Added
- `LimitedStack<T>.Count()` and `LimitedStack<T>.ToArray()`

### Changed
- **BREAKING**: `LimitedStack<T>.Pick()` renamed to `Peek()`
- `LimitedStack<T>` is backed by a ring buffer, so pushing past capacity no longer shifts the stored items


## [v0.6.0]

### Added
- Zenject (Extenject) integration compiled under `UNISTATE_ZENJECT_SUPPORT`, with `ZenjectTypeResolver` and `DiContainer.ToTypeResolver()`
- VContainer `IObjectResolver.ToTypeResolver()` extension

### Changed
- VContainer integration is optional and compiled only under `UNISTATE_VCONTAINER_SUPPORT` (defined automatically when VContainer is installed via UPM)
- UniTask is no longer declared as a package dependency and has to be installed separately

### Removed
- **BREAKING**: `IStateMachineFactory.Create<TStateMachine>(IObjectResolver)` — pass `ToTypeResolver()` to the `ITypeResolver` overload instead
- `UniStateLifetimeScope` placeholder


## [v0.5.0]

First public version:
- `StateMachine` running async UniTask-based states with `GoTo`, `GoBack` and `GoToExit` transitions and a bounded history
- `StateBase` / `StateBase<TPayload>` with the `Initialize`, `Execute`, `Exit` and `Dispose` lifecycle and typed payloads
- Composite states (`CompositeStateBase`, `DefaultCompositeState`, `SubStateBase`) executing sub-states in parallel
- `[StateBehaviour]` attribute with `InitializeOnStateTransition` and `ProhibitReturnToState`
- `ITypeResolver` abstraction with a VContainer resolver
- `IStateMachineFactory` for nested state machines
