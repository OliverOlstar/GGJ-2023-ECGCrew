using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

namespace OliverLoescher.Camera
{
    public class FirstPersonCamera : MonoBehaviour
    {
        [SerializeField] 
		private Transform cameraTransform = null;
		[SerializeField]
		private MonoUtil.Updateable updateable = new MonoUtil.Updateable(MonoUtil.UpdateType.Late, MonoUtil.Priorities.Camera);

        [Space, SerializeField, MinMaxSlider(-90, 90, true)] 
		private Vector2 cameraYClamp = new Vector2(-40, 50);

		private Vector2 input = Vector2.zero;

		void OnEnable()
		{
			updateable.Register(Tick);
		}
		void OnDisable()
		{
			updateable.Deregister();
		}

		public void SetCameraMove(Vector2 pInput) => input = pInput;
        public void OnCameraMove(Vector2 pInput) => RotateCamera(pInput);
		
		private void Tick(float pDeltaTime)
		{
			RotateCamera(input * pDeltaTime);
		}

        private void RotateCamera(Vector2 pInput)
        {
            Vector3 euler = cameraTransform.eulerAngles;
            euler.x = Mathf.Clamp(Util.SafeAngle(euler.x - pInput.y), cameraYClamp.x, cameraYClamp.y);
            euler.y += pInput.x;
            euler.z = 0.0f;
            cameraTransform.rotation = Quaternion.Euler(euler);
        }
    }
}