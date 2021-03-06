﻿/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary and  confidential.  Not for distribution.            *
* Use subject to the terms of the Leap Motion SDK Agreement available at       *
* https://developer.leapmotion.com/sdk_agreement, or another agreement between *
* Leap Motion and you, your company or other organization.                     *
* Author: Matt Tytel
\******************************************************************************/

using UnityEngine;
using System.Collections;
using Leap;

// The finger model for our geometric hand made out of various polyhedra.
public class PhysicsFinger : SkeletalFinger {

  public float easing = 0.5f;

  void Start() {
    for (int i = 0; i < NUM_BONES; ++i)
      bones[i].rigidbody.maxAngularVelocity = Mathf.Infinity;
  }

  public override void UpdateFinger (Finger finger, Transform deviceTransform,
                                     Vector3 palm_normal, Vector3 palm_direction) {
    Vector3 last_bone_normal = palm_normal;
    Vector3 last_bone_direction = palm_direction;

    for (int i = 0; i < NUM_BONES; ++i) {
      // Set velocity.
      Vector3 next_bone_position = deviceTransform.TransformPoint(GetBonePosition(finger, i));
      bones[i].rigidbody.velocity = (next_bone_position - bones[i].transform.position) *
                                    ((1 - easing) / Time.fixedDeltaTime);

      // Set angular velocity.
      Vector3 bone_direction = deviceTransform.rotation * GetBoneDirection(finger, i);
      Vector3 bone_normal = Quaternion.FromToRotation(last_bone_direction, bone_direction) * last_bone_normal;

      Quaternion delta_rotation = Quaternion.LookRotation(bone_direction, -bone_normal) *
                                  Quaternion.Inverse(bones[i].transform.rotation);
      float angle = 0.0f;
      Vector3 axis = Vector3.zero;
      delta_rotation.ToAngleAxis(out angle, out axis);

      if (angle >= 180) {
        angle = 360 - angle;
        axis  = -axis;
      }
      if (angle != 0)
        bones[i].rigidbody.angularVelocity = (1 - easing) * angle * axis;

      last_bone_direction = bone_direction;
      last_bone_normal = bone_normal;
    }
  }
}
