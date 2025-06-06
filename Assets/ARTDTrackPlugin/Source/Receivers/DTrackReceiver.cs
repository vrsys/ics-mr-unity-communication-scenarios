/* ART DTRACK Plugin for Unity Game Engine: DTrackReceiver.cs
 *
 * Abstract class to provide DTRACK tracking data to a game object.
 *
 * Copyright (c) 2020-2023 Advanced Realtime Tracking GmbH & Co. KG
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 * 
 * 1. Redistributions of source code must retain the above copyright
 *    notice, this list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright
 *    notice, this list of conditions and the following disclaimer in the
 *    documentation and/or other materials provided with the distribution.
 * 3. Neither the name of copyright holder nor the names of its contributors
 *    may be used to endorse or promote products derived from this software
 *    without specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
 * FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
 * DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
 * CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
 * OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using UnityEngine;

using DTrackSDK;

namespace DTrack
{


public abstract class DTrackReceiver : MonoBehaviour, IDTrackReceiver
{
	// DTrack object this receiver is associated to
	private DTrack dtrackSource = null;

	private DTrackSDK.Frame nextFrame = null;
	private long currentFrameCounter = 0;


	// Register oneself at DTrack object to receive DTRACK tracking data.

	protected void Register()
	{
#if UNITY_2023_1_OR_NEWER
		DTrack dtr = FindFirstObjectByType< DTrack >();
#else
		DTrack dtr = FindObjectOfType< DTrack >();
#endif

		if ( dtr != null )
		{
			dtr.RegisterTarget( this.gameObject );
			this.dtrackSource = dtr;
		}
		else
		{
			Debug.Log( "Cannot find DTrack object" );
		}
	}

	// Unregister oneself at DTrack object.

	protected void Unregister()
	{
		if ( this.dtrackSource != null )
		{
			this.dtrackSource.UnregisterTarget( this.gameObject );
		}
	}


	// Receive new frame of DTRACK tracking data from DTrack class.

	public void ReceiveDTrackFrame( DTrackSDK.Frame frame )  // gives a new DTrackSDK.Frame object for every frame
	{
		this.nextFrame = frame;
	}

	// Get next frame of DTRACK tracking data.

	protected DTrackSDK.Frame GetDTrackFrame()
	{
		DTrackSDK.Frame frame = this.nextFrame;  // ensures data integrity against DTrack class
		if ( frame == null )  return null;

		if ( this.currentFrameCounter > 0 && ( this.currentFrameCounter == frame.FrameCounter ) )  // no new tracking data
			return null;

		this.currentFrameCounter = frame.FrameCounter;

		return frame;
	}
}


}  // namespace DTrack

