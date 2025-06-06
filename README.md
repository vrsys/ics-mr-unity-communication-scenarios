# ICS-MR: Communication Scenario Implementations

This repository contains Unity implementations of the communication tasks contained in the ICS-MR (Interactive Conversation Scenarios for Assessment of Mixed Reality Communication) dataset.    

## Installation

The Unity implementations depend on the scenario materials that are contained in a [separate repository](https://github.com/vrsys/ics-mr-communication-scenario-materials).

1. **Clone Repository.** The scenario materials are included as a git submodule, so this repository should be cloned as follows to simultaneously clone the scenario material repository:

```
git clone --recurse-submodules https://github.com/vrsys/ics-mr-unity-communication-scenarios.git
```

2. **Open the Unity project.** 

3. **Create Unity Materials.** To create Unity _Material_ files from images that are contained in the scenario materials repository, execute the Material creation script by selecting `Tools > Create Materials From Textures` and clicking `Generate Materials`.

The repository should now be ready to use.


## Scenarios

This repository contains Unity implementations corresponding to the following conversation scenarios:

1. Floor Plan Negotiation (S1)
2. Spot the Difference (S2)
3. Survival Game (S3)

Please the associated paper (link above) for full descriptions of the tasks.


## Usage Notes

### Scene Types

The repository includes two sets of scenes (in `Assets/ics-mr/Resources/Scenes`):

* **Basic** Scenes: these scenes show the different scene states or scenarios available for each task, and allow the user to advance through the experiment, but do not include any interactivity. They can serve as building blocks, if you wish to use the tasks in your own distribution system.

* **Networked** Scenes: these scenes allow multiple participants to join the scene as avatars, communicate via voice, and interact with scene objects where relevant. For more information on networking, see below.


### Networked Implementations

To allow multiple participants to interact in the same scene, the networked scenes corresponding to each task handle distribution of scene state (i.e. the 6DOF pose of moving objects), and allow voice communication between users.

This is made possible by the [VRSYS-Core](https://github.com/vrsys/vrsys-core/) framework (note: the version of VRSYS-Core used in this project is, at the time of writing, found on the [feature/update-unity6](https://github.com/vrsys/vrsys-core/tree/feature/update-unity6) branch). 

Please note that in order to run the project using Unity Netcode, the Unity project must be linked to a project ID. The project ID can be selected under `Edit > Project Settings > Services`.

### Networked Scene Settings

**Auto Start**: the scenes are set to 'auto start' by default, meaning that when play is pressed, users will be automatically added to a default lobby. This is practical if only small numbers of users will join the scene at once. If you need more control over who joins which lobby, disable Auto Start in the Lobby Settings (accessed either at path `Assets/VRSYS/Core/ScriptableObjects/Instances/Lobby Settings` or in the scenes in `__NETWORKING__/VRSYS-Networking > Connection Manager component > Lobby Setting`). 

**User Type/Role**: In Auto Start mode, a user type will automatically be selected, e.g. as Desktop user or HMD user. The default role can be set in the Network User Spawn Info settings (accessed either at path `Assets/VRSYS/Core/ScriptableObjects/Instances/Network User Spawn Info` or in the scenes in `__NETWORKING__/VRSYS-Networking > Connection Manager component > User Spawn Info`). If you prefer to select the user type dynamically when starting the scene, disable Auto Start mode (see above).


### Running Experiments

**Experiment Data:** The `__CONTROL__/ExperimentRunner` Game Object, found in all experiment scenes, contains an `*Experiment Runner` component that allows experiments to be configured. For example, the interlocutor (participant) ID and group ID can be set, the output data directory can be specified, and the order of conditions (e.g. combinations of indpendent variables) can be edited.

**Experiment Control:** After a scene has started running, the experiment can be started by pressing the Enter key. Subsequently, the experiment can be advanced using the Space key. Note that in networked mode, starting and advancing the experiment on one PC causes the experiment to start/advance for all participants. 


## License

This dataset is licensed under the Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International (CC BY-NC-SA 4.0) license.

See the [LICENSE](LICENSE.txt) file for details.
