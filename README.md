# aplause-mr-communication-tasks

This repository contains Unity implementations corresponding to the following conversation tasks:

1. Floor Plan Negotiation
	* TODO description
	* This task was used in the publication: Felix Immohr, Gareth Rendle, Annika Neidhardt, Steve Göring, Rakesh Rao Ramachandra Rao, Stephanie Arevalo Arboleda, Bernd Froehlich, and Alexander Raake. 2023. _Proof-of-Concept Study to Evaluate the Impact of Spatial Audio on Social Presence and User Behavior in Multi-Modal VR Communication_. In Proceedings of the 2023 ACM International Conference on Interactive Media Experiences (IMX '23). Association for Computing Machinery, New York, NY, USA, 209–215. DOI: [10.1145/3573381.3596458](https://doi.org/10.1145/3573381.3596458).

2. Spot-the-difference collaborative task
	* TODO description
	* This task was used in the publication: Felix Immohr, Gareth Rendle, Anton Lammert, Annika Neidhardt, Victoria Meyer Zur Heyde, Bernd Froehlich, and Alexander Raake. 2024. _Evaluating the Effect of Binaural Auralization on Audiovisual Plausibility and Communication Behavior in Virtual Reality_. In 2024 IEEE Conference Virtual Reality and 3D User Interfaces (VR), Orlando, FL, USA, 2024, pp. 849-858. DOI: [10.1109/VR58804.2024.00104](https://doi.org/10.1109/VR58804.2024.00104). 

3. Survival Task (with interactable objects)
	* TODO description
	* This task was used in the publication: Felix Immohr, Gareth Rendle, Christian Kehling, Anton Lammert, Steve Göring, Bernd Froehlich, and Alexander Raake. 2024. _Subjective Evaluation of the Impact of Spatial Audio on Triadic Communication in Virtual Reality_. In 2024 16th International Conference on Quality of Multimedia Experience (QoMEX), Karlshamn, Sweden, 2024, pp. 262-265. DOI: [10.1109/QoMEX61742.2024.10598292](https://doi.org/10.1109/QoMEX61742.2024.10598292).

4. Survival Task (without interactable objects)
	* In this version of the task, all materials (i.e. scenario descriptions and survival items) are distributed to participants on paper. Therefore, the only changes in the scene are the appearance of signs to notify participants about the progression of the experiment. 
	* This task was used in the publication: Gareth Rendle, Felix Immohr, Christian Kehling, Anton Lammert, Adrian Kreskowski, Karlheinz Brnadenburg, Alexander Raake, and Bernd Froehlich. 2025. _Influence of Audiovisual Realism on Communication Behaviour in Group-to-Group Telepresence_. In 2025 IEEE Conference Virtual Reality and 3D User Interfaces (VR), Saint Malo, France, 2025, pp. 569-579, DOI: [10.1109/VR59515.2025.00080](https://doi.org/10.1109/VR59515.2025.00080).




# Usage Notes

## Scene Types

The repository includes two sets of scenes (in `Assets/aplause-mr/Resources/Scenes`):

* **Basic** Scenes: these scenes show the different scene states or scenarios available for each task, and allow the user to advance through the experiment, but do not include any interactivity. They can serve as building blocks, if you wish to use the tasks in your own distribution system.

* **Networked** Scenes: these scenes allow multiple participants to join the scene as avatars, communicate via voice, and interact with scene objects where relevant. For more information on networking, see below.


## Networked Implementations

To allow multiple participants to interact in the same scene, the networked scenes corresponding to each task handle distribution of scene state (i.e. the 6DOF pose of moving objects), and allow voice communication between users.

This is made possible by the [VRSYS-Core](https://github.com/vrsys/vrsys-core/) framework (note: the version of VRSYS-Core used in this project is, at the time of writing, found on the [feature/update-unity6](https://github.com/vrsys/vrsys-core/tree/feature/update-unity6) branch). 

Please note that in order to run the project using Unity Netcode, the Unity project must be linked to a project ID. The project ID can be selected under `Edit > Project Settings > Services`.

### Networked Scene Settings

**Auto Start**: the scenes are set to 'auto start' by default, meaning that when play is pressed, users will be automatically added to a default lobby. This is practical if only small numbers of users will join the scene at once. If you need more control over who joins which lobby, disable Auto Start in the Lobby Settings (accessed either at path `Assets/VRSYS/Core/ScriptableObjects/Instances/Lobby Settings` or in the scenes in `__NETWORKING__/VRSYS-Networking > Connection Manager component > Lobby Setting`). 

**User Type/Role**: In Auto Start mode, a user type will automatically be selected, e.g. as Desktop user or HMD user. The default role can be set in the Network User Spawn Info settings (accessed either at path `Assets/VRSYS/Core/ScriptableObjects/Instances/Network User Spawn Info` or in the scenes in `__NETWORKING__/VRSYS-Networking > Connection Manager component > User Spawn Info`). If you prefer to select the user type dynamically when starting the scene, disable Auto Start mode (see above).

