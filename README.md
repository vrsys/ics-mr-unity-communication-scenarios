# aplause-mr-communication-tasks

This repository contains Unity implementations corresponding to the following conversation tasks:

1. Floor Plan Negotiation
	* TODO
	* Task used in study: 

Felix Immohr, Gareth Rendle, Annika Neidhardt, Steve Göring, Rakesh Rao Ramachandra Rao, Stephanie Arevalo Arboleda, Bernd Froehlich, and Alexander Raake. 2023. Proof-of-Concept Study to Evaluate the Impact of Spatial Audio on Social Presence and User Behavior in Multi-Modal VR Communication. In Proceedings of the 2023 ACM International Conference on Interactive Media Experiences (IMX '23). Association for Computing Machinery, New York, NY, USA, 209–215. [https://doi.org/10.1145/3573381.3596458](https://doi.org/10.1145/3573381.3596458)

2. Spot-the-difference collaborative task
	* TODO
	* Task used in study: TODO ref

3. Survival Task (with interactable objects)
	* TODO
	* Task used in study: TODO ref

4. Survival Task (without interactable objects)
	* In this version of the task, all materials (i.e. scenario descriptions and survival items) are distributed to participants on paper. Therefore, the only changes in the scene are the appearance of signs to notify participants about the progression of the experiment. 
	* Task used in study: TODO ref


## Usage Notes

### Distributed Implementations

To allow multiple participants to interact in the same scene, the scenes corresponding to each task handle distribution of scene state (i.e. the 6DOF pose of moving objects), and allow voice communication between users.

This is made possible by the [VRSYS-Core](https://github.com/vrsys/vrsys-core/) library (note: the version of VRSYS-Core used in this project is currently found on the [feature/update-unity6](https://github.com/vrsys/vrsys-core/tree/feature/update-unity6) branch). 

Please note that in order to run the project using Unity Netcode, the Unity project must be linked to a project ID. The project ID can be selected under `Edit > Project Settings > Services`.

