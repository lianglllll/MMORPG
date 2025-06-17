using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    public class EditorAnimDisplay : EditorWindow
    {
        #region init
        private static EditorAnimDisplay instance;
        [MenuItem("Tools/EditorAnimDisplayWindow")]
        static void ShowWindow()
        {
            instance = GetWindow<EditorAnimDisplay>();
            instance.Show();
        }
        #endregion

        public AnimationClip[] clips;
        public GameObject player;
        public string Fitter = "";

        private AnimationClip curAnimClip;
        private float Timer = 0;
        private int playCount = 0;
        private Vector2 pos = Vector2.zero;

        private void OnEnable()
        {
            EditorApplication.update += OnEditorUpdate;
        }

        private void OnDisable()
        {
            EditorApplication.update -= OnEditorUpdate;
        }

        private void OnEditorUpdate()
        {
            UpdateAnim(Time.deltaTime);
            Repaint();
        }

        void OnGUI()
        {
            player = EditorGUILayout.ObjectField("Player", player, typeof(GameObject), true) as GameObject;
            Fitter = EditorGUILayout.TextField("Name Filter", Fitter);

            if (player == null)
            {
                GUILayout.Label("Assign a Player Object");
                return;
            }

            if (!player.TryGetComponent<Animator>(out var anim) || anim.runtimeAnimatorController == null)
            {
                GUILayout.Label("Missing Animator or Animation Controller");
                return;
            }

            clips = anim.runtimeAnimatorController.animationClips;
            pos = GUILayout.BeginScrollView(pos, false, false);
            foreach (var item in clips)
            {
                if (IsShow(item.name) && GUILayout.Button(item.name))
                {
                    PlayAnim(item);
                }
            }
            GUILayout.EndScrollView();
        }

        private bool IsShow(string clipName)
        {
            return string.IsNullOrEmpty(Fitter) ||
                   clipName.ToLower().Contains(Fitter.ToLower());
        }

        private void PlayAnim(AnimationClip clip)
        {
            Timer = 0;
            playCount = 0;
            curAnimClip = clip;
            Selection.activeObject = clip;
        }

        private void UpdateAnim(float delta)
        {
            if (player == null || curAnimClip == null) return;

            Timer += delta;
            if (Timer > curAnimClip.length && playCount < 2)
            {
                playCount++;
                Timer = 0;
            }
            else
            {
                if (curAnimClip.length > 0)
                {
                    // 安全采样（处理边界）
                    float sampleTime = Mathf.Clamp(Timer, 0, curAnimClip.length);
                    curAnimClip.SampleAnimation(player, sampleTime);
                }
            }
        }
    }
}
