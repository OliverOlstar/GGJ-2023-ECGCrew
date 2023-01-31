using OliverLoescher.Debug2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OliverLoescher
{
	[RequireComponent(typeof(CharacterController))]
    public class GizmoCharacterController : GizmoBase
	{
		[SerializeField]
		private bool wireframe = false;

		private CharacterController character = null;

		protected override void DrawGizmos()
		{
			base.DrawGizmos();
			if (character == null && !TryGetComponent(out character))
			{
				return;
			}
			Gizmos.matrix = transform.localToWorldMatrix;
			if (wireframe)
			{
				Util.GizmoCapsule(character.center, character.radius, character.height);
			}
			else
			{
				Gizmos.DrawMesh(Util.GetPrimitiveMesh(PrimitiveType.Capsule), character.center, Quaternion.identity, 
					new Vector3(character.radius * 2.0f, character.height * 0.5f, character.radius * 2.0f));
			}
			Gizmos.matrix = Matrix4x4.identity;
		}
	}
}
